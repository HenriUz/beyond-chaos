using System;
using UnityEngine;

public class CharacterBattle : MonoBehaviour {
    public Health _healthSystem;
    private HealthBar _healthBar;

    private CharacterBase _characterBase;
    private RuntimeAnimatorController _enemyAnimatorOverride;
    private EnemyStatsData _enemyStats;

    private State _state;
    private Vector3 _originalPosition;
    private Vector3 _targetPosition;

    private Action _onMoveComplete;
    private Action _onDefenseComplete;

    private GameObject _selectionIndicatorGo;
    private SpriteRenderer _spriteRenderer;
    private bool _isPlayerTeam;
    private Transform _visualTransform;


    private bool _isInvincible;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float parryWindow = 0.22f;
    [SerializeField] private float dodgeWindow = 0.28f;
    [SerializeField] private float speedDodge = 10f;
    private bool _isReturningFromDodge;
    private float _dodgeWaitTimer;
    private float _invincibleTimer;

    private enum State {
        Idle,
        Moving,
        Attacking,
        Defending,
        Dodging,
        Dead
    }

    private void Awake() {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _characterBase = GetComponent<CharacterBase>();
        _enemyAnimatorOverride = WorldManager.Instance.LastEnemyAnimatorController;
        
        _selectionIndicatorGo = transform.Find("SelectionIndicator").gameObject;
        HideSelectionIndicator();
        
        _originalPosition = GetPosition();    
        _state = State.Idle;


        if (_spriteRenderer != null) {
            _visualTransform = _spriteRenderer.transform;
        }
    }
    
    private void Update() {
        UpdateHealthBarPosition();

        switch (_state) {
            case State.Idle:
                // Handle idle behavior.
                break;
            case State.Moving:
                float step = speed * Time.deltaTime;
                if (_enemyStats != null) {
                    step *= _enemyStats.attackSpeedMultiplier;
                }

                transform.position = Vector3.MoveTowards(GetPosition(), _targetPosition, step);

                const float distanceAllowed = 1.5f;
                if (Vector3.Distance(GetPosition(), _targetPosition) < distanceAllowed) {
                    transform.position = _targetPosition;
                    _onMoveComplete();
                }

                break;
            case State.Defending:
                // Handle defending behavior.
                _invincibleTimer -= Time.deltaTime;
                if (_invincibleTimer <= 0f) {
                    StopDefending();
                }
                break;
            case State.Dodging:
                // Handle dodging behavior.
                // float speedDodge = 10f;
                transform.position = Vector3.MoveTowards(GetPosition(), _targetPosition, Time.deltaTime * speedDodge);

                if (Vector3.Distance(GetPosition(), _targetPosition) < 0.1f) { 
                    if (!_isReturningFromDodge) {
                        // It's reached the point of evasion — wait a moment before returning.
                        _invincibleTimer -= Time.deltaTime;

                        if (_invincibleTimer <= 0f) {
                            // Begins the return.
                            _targetPosition = _originalPosition; // Return to original position.
                            _isReturningFromDodge = true;
                        }
                    } else {
                        // Finished the return.
                        _isInvincible = false;
                        _invincibleTimer = 0f;
                        StopDefendingVisual();

                        _onDefenseComplete?.Invoke();
                        _onDefenseComplete = null;
                        _state = State.Idle;
                    }
                }
                break;
            case State.Attacking:
                // Handle attacking behavior.
            case State.Dead:
                // Handle dead behavior.
                break;
        }
    }
    
    public int GetLife() {
        return _healthSystem.currentHealth;
    }
    
    public Vector3 GetPosition() {
        return transform.position;
    }

    public EnemyStatsData GetEnemyStats() {
        return _enemyStats;
    }

    /* Setup functions. */
    
    public void Setup(bool isPlayerTeam, int life, EnemyStatsData enemyStats = null) {
        _isPlayerTeam = isPlayerTeam;
        var animatorOverride = isPlayerTeam ? BattleHandler.GetInstance().playerAnimatorOverride : _enemyAnimatorOverride;

        _characterBase.SetAnimatorOverride(animatorOverride);

        if (isPlayerTeam) {
            _healthSystem = new Health(WorldManager.Instance.PlayerStats.maxHealth);
            _healthSystem.currentHealth = WorldManager.Instance.PlayerStats.currentHealth;
        } else {
            // Usa stats do inimigo
            _enemyStats = enemyStats;
            int maxHealth = _enemyStats != null ? _enemyStats.maxHealth : 100;
            _healthSystem = new Health(maxHealth);
            
            var localScale = _visualTransform.localScale;
            localScale.x = -Mathf.Abs(localScale.x);
            _visualTransform.localScale = localScale;
        }
        
        // Register health changed event BEFORE creating health bar
        _healthSystem.OnHealthChanged += OnHealthChanged;

        // Apply initial attack speed multiplier for boss
        if (!_isPlayerTeam && WorldManager.Instance.IsLastEnemyBoss && _enemyStats != null) {
            _characterBase.SetAttackSpeedMultiplier(_enemyStats.attackSpeedMultiplier);
        }

        GameObject healthBarPrefab;
        if (!_isPlayerTeam && WorldManager.Instance.IsLastEnemyBoss) {
            healthBarPrefab = BattleHandler.GetInstance().BossHealthBarPrefab;
            _visualTransform.localPosition += new Vector3(0, 0.1f, 0);
            _selectionIndicatorGo.transform.localPosition += new Vector3(0, 0.1f, 0);
        } else {
            healthBarPrefab = BattleHandler.GetInstance().HealthBarPrefab;
        }
        Canvas uiParent = BattleHandler.GetInstance().WorldCanvas;

        _healthBar = GameObject.Instantiate(healthBarPrefab, uiParent.transform).GetComponent<HealthBar>();

        UpdateHealthBarPosition();
        _healthBar.SetValue(_healthSystem.GetHealthNormalized());
    }

    private void OnHealthChanged(float normalizedHealth) {
        _healthBar.SetValue(_healthSystem.GetHealthNormalized());

        if (!_isPlayerTeam && WorldManager.Instance.IsLastEnemyBoss) {
            // Enter frenzy mode if health below 30%
            if (normalizedHealth <= 0.3f && _enemyStats != null) {
                _characterBase.SetAttackSpeedMultiplier(_enemyStats.frenzySpeedMultiplier);
            }

        }
    }

    private void UpdateHealthBarPosition() {
        if (_healthBar == null) return;

        if (_isPlayerTeam || !WorldManager.Instance.IsLastEnemyBoss) {
            // Posição do personagem + altura acima do personagem
            Vector3 worldPos = _visualTransform.position + new Vector3(0, 1.5f, 0);
            // Converte para tela
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            // Aplica no canvas
            _healthBar.transform.position = screenPos;
        } else {
            // Posição fixa no topo da tela para boss
            Vector3 screenPos = new Vector3(Screen.width / 2f, Screen.height - 50f, 0);
            _healthBar.transform.position = screenPos;
        }
    }

    private void ShowMessagePopup(string message, Color color) {
        var damagePopupPrefab = BattleHandler.GetInstance().DamagePopupPrefab;
        var uiParent = BattleHandler.GetInstance().WorldCanvas;

        DamagePopup damagePopup = Instantiate(damagePopupPrefab, uiParent.transform).GetComponent<DamagePopup>();
        
        float xOffset = _isPlayerTeam ? 1f : -1f;
        float yOffset = 2f;
        Vector3 worldPos = transform.position + new Vector3(xOffset, yOffset, 0);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        damagePopup.transform.position = screenPos;

        damagePopup.Setup(message, color);
    }

    /* Animation functions. */
    
    private void MoveToPosition(Vector3 targetPosition, Action onMoveComplete) {
        _targetPosition = targetPosition;
        _onMoveComplete = onMoveComplete;
        _state = State.Moving;

        _characterBase.PlayMoveAnimation(_targetPosition);
    }

    private void DodgeToPosition(Vector3 targetPosition, Action onMoveComplete) {
        _targetPosition = targetPosition;
        _onMoveComplete = onMoveComplete;
        _state = State.Dodging;

        // characterBase.PlayDodgeAnimation(_targetPosition);
    }
    
    private void StartDefendingVisual() {
        _spriteRenderer.color = Color.cyan;
    }

    private void StopDefendingVisual() {
        _spriteRenderer.color = Color.white;
    }

    /* State functions. */
    
    private CharacterBase.AttackType ChooseEnemyAttack() {
        // Player sempre usa Attack1 por padrão
        if (_isPlayerTeam) {
            return CharacterBase.AttackType.Attack1;
        }

        bool isBoss = WorldManager.Instance.IsLastEnemyBoss;

        // Boss pode usar ataque 3
        if (isBoss) {
            float randomValue3 = UnityEngine.Random.Range(0f, 100f);
            if (randomValue3 < _enemyStats.attack3ChancePercent) {
                return CharacterBase.AttackType.Attack3;
            }
        }

        // Verifica se usa ataque 2
        float randomValue2 = UnityEngine.Random.Range(0f, 100f);
        if (randomValue2 < _enemyStats.attack2ChancePercent) {
            return CharacterBase.AttackType.Attack2;
        }

        // Ataque padrão
        return CharacterBase.AttackType.Attack1;
    }

    public void Attack(CharacterBattle targetCharacter, Action onAttackComplete, CharacterBase.AttackType attackType = CharacterBase.AttackType.Attack1) {
        var originalPosition = GetPosition();
        var targetPosition = targetCharacter.GetPosition();
        var directionToTarget = targetPosition + (originalPosition - targetPosition).normalized * 1.5f;

        // Escolhe qual ataque usar
        if (!_isPlayerTeam) {
            attackType = ChooseEnemyAttack();
        }
        
        // Determina se é ataque especial do player
        bool isPlayerSpecialAttack = _isPlayerTeam && attackType == CharacterBase.AttackType.Attack2;

        // Move towards the target.
        MoveToPosition(directionToTarget, () => {
            // Arrived, attacking.
            _state = State.Attacking;
            _characterBase.PlayAttackAnimation(() => {
                // Attack hit event.
                int damageAmount;
                
                if (_isPlayerTeam) {
                    // Usa o sistema de stats do player
                    damageAmount = WorldManager.Instance.PlayerStats.CalculateDamage(isPlayerSpecialAttack);
                } else {
                    // Inimigo: usa stats do inimigo com variação
                    damageAmount = _enemyStats?.CalculateDamage(attackType) ?? 10;
                }
                
                targetCharacter.Damage(damageAmount);
            }, () => {
                // Attack finished, moving back to original position.
                MoveToPosition(originalPosition, () => {
                    // Moving back finished.
                    _state = State.Idle;
                    _characterBase.PlayIdleAnimation(directionToTarget);
                    onAttackComplete();
                });
            }, attackType
            );
        });
    }

    public void Dodge(Action onDodgeComplete) {
        if (_state == State.Dodging) return; // Avoid multiple overlapping dashes.
        
        // Debug.Log("CharacterBattle is dodging!");
        _isInvincible = true;
        _invincibleTimer = dodgeWindow;

        StartDefendingVisual();

        var dodgeDirection = new Vector3(-1f, 0); // Can be adjusted according to the player's side.
        var dodgeTarget = _originalPosition + dodgeDirection;

        _targetPosition = dodgeTarget;
        _isReturningFromDodge = false;
        _dodgeWaitTimer = 0f;

        _onDefenseComplete = onDodgeComplete;
        _state = State.Dodging;
        // characterBase.PlayDodgeAnimation();
    }

    public void StartDefending(Action onParryComplete) {
        // Debug.Log("CharacterBattle is defending!");
        _isInvincible = true;
        _invincibleTimer = parryWindow;
        _onDefenseComplete = onParryComplete;
        StartDefendingVisual();
        
        _state = State.Defending;
        // characterBase.PlayDefendAnimation();
    }

    public void StopDefending() {
        _isInvincible = false;
        StopDefendingVisual();
        _onDefenseComplete?.Invoke();
        _onDefenseComplete = null;

        _state = State.Idle;
        // characterBase.PlayIdleAnimation();
        // Debug.Log("CharacterBattle stopped defending!");
    }

    private void Damage(int damageAmount) {
        if (_isInvincible) {
            if (_state == State.Dodging) {
                // Successful dodge.
                ShowMessagePopup("Dodge!", Color.green);
                // Debug.Log("CharacterBattle dodged the attack!");
                _isInvincible = false;
                return;
            }

            // Successful parry.
            ShowMessagePopup("Parry!", Color.yellow);
            // Debug.Log("CharacterBattle parried the attack!");
            _isInvincible = false;
            
            // Add special charge on successful parry (only for player)
            if (_isPlayerTeam) {
                WorldManager.Instance.PlayerStats.AddSpecialCharge();
                BattleHandler.GetInstance().EnableUIKeys(WorldManager.Instance.PlayerStats.CanUseSpecial());
            }
            return;
        }

        // Handle taking damage.
        _characterBase.PlayDamageAnimation();
        _healthSystem.TakeDamage(damageAmount);
        ShowMessagePopup(damageAmount.ToString(), Color.red);
        
        Debug.Log($"{(_isPlayerTeam ? "Player" : "Enemy")} took {damageAmount} damage! Current health: {_healthSystem.currentHealth}/{_healthSystem.maxHealth}");

        if (_healthSystem.currentHealth <= 0) {
            _state = State.Dead;
            Debug.Log($"{(_isPlayerTeam ? "Player" : "Enemy")} died!");
            // characterBase.PlayDeathAnimation();
        }
    }

    public bool IsDead() {
        // Verify if character is dead.
        return _state == State.Dead;
    }

    public void ShowSelectionIndicator() {
        _selectionIndicatorGo.SetActive(true);
    }

    public void HideSelectionIndicator() {
        _selectionIndicatorGo.SetActive(false);
    }
}
