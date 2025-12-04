using System;
using UnityEngine;

public class CharacterBattle : MonoBehaviour {
    private const int MAXHEALTH = 100;
    public Health _healthSystem;
    private HealthBar _healthBar;

    private CharacterBase _characterBase;
    private State _state;
    private Vector3 _targetPosition;
    private Action _onMoveComplete;
    private GameObject _selectionIndicatorGo;
    private SpriteRenderer _spriteRenderer;
    private bool _isPlayerTeam;

    private bool _isInvincible;
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
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _characterBase = GetComponent<CharacterBase>();        
        
        _selectionIndicatorGo = transform.Find("SelectionIndicator").gameObject;
        HideSelectionIndicator();
        _state = State.Idle;
    }
    
    private void Update() {
        UpdateHealthBarPosition();

        switch (_state) {
            case State.Idle:
                // Handle idle behavior.
                break;
            case State.Moving:
                const float speed = 20f;
                transform.position = Vector3.MoveTowards(GetPosition(), _targetPosition, Time.deltaTime * speed);

                const float distanceAllowed = 1f;
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
                            _targetPosition = GetPosition() - new Vector3(-1f, 0); // Return to original position.
                            _isReturningFromDodge = true;
                        }
                    } else {
                        // Finished the return.
                        _state = State.Idle;
                        _isInvincible = false;
                        StopDefendingVisual();
                        Debug.Log("CharacterBattle finished dodging!");
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

    /* Setup functions. */
    
    public void Setup(bool isPlayerTeam, int life) {
        _isPlayerTeam = isPlayerTeam;
        var animatorOverride = isPlayerTeam ? BattleHandler.GetInstance().playerAnimatorOverride : BattleHandler.GetInstance().enemyAnimatorOverride;

        _characterBase.SetAnimatorOverride(animatorOverride);


        if (isPlayerTeam) {
            _healthSystem = new Health(MAXHEALTH);
            _healthSystem.currentHealth = life;
        } else {
            _healthSystem = new Health(MAXHEALTH);
            
            var localScale = transform.localScale;
            localScale.x = -Mathf.Abs(localScale.x);
            transform.localScale = localScale;
        }

        GameObject healthBarPrefab = BattleHandler.GetInstance().HealthBarPrefab;
        Canvas uiParent = BattleHandler.GetInstance().WorldCanvas;

        _healthBar = GameObject.Instantiate(healthBarPrefab, uiParent.transform).GetComponent<HealthBar>();

        UpdateHealthBarPosition();
        _healthSystem.OnHealthChanged += OnHealthChanged;
        _healthBar.SetValue(_healthSystem.GetHealthNormalized());
    }

    private void OnHealthChanged(float normalizedHealth) {
        _healthBar.SetValue(_healthSystem.GetHealthNormalized());
    }

    private void UpdateHealthBarPosition() {
        if (_healthBar == null) return;

        // altura acima do personagem
        Vector3 worldPos = transform.position + new Vector3(0, 1.5f, 0);

        // converte para tela
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // aplica no canvas
        _healthBar.transform.position = screenPos;
    }

    private void ShowMessagePopup(string message, Color color) {
        var damagePopupPrefab = BattleHandler.GetInstance().DamagePopupPrefab;
        var uiParent = BattleHandler.GetInstance().WorldCanvas;

        DamagePopup damagePopup = Instantiate(damagePopupPrefab, uiParent.transform).GetComponent<DamagePopup>();
        
        float xOffset = _isPlayerTeam ? 1f : -1f;
        Vector3 worldPos = transform.position + new Vector3(xOffset, 1f, 0);
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
    
    public void Attack(CharacterBattle targetCharacter, Action onAttackComplete) {
        var originalPosition = GetPosition();
        var targetPosition = targetCharacter.GetPosition();
        var directionToTarget = targetPosition + (originalPosition - targetPosition).normalized * 1.5f;

        // Move towards the target.
        MoveToPosition(directionToTarget, () => {
            // Arrived, attacking.
            _state = State.Attacking;
            _characterBase.PlayAttackAnimation(() => {
                // Attack hit event.
                targetCharacter.Damage(25);
            }, () => {
                // Attack finished, moving back to original position.
                MoveToPosition(originalPosition, () => {
                    // Moving back finished.
                    _state = State.Idle;
                    _characterBase.PlayIdleAnimation(directionToTarget);
                    onAttackComplete();
                });
            });
        });
    }

    public void Dodge() {
        if (_state == State.Dodging) return; // Avoid multiple overlapping dashes.
        
        Debug.Log("CharacterBattle is dodging!");
        _isInvincible = true;
        _invincibleTimer = dodgeWindow;

        StartDefendingVisual();
        
        var dodgeDirection = new Vector3(-1f, 0); // Can be adjusted according to the player's side.
        var dodgeTarget = GetPosition() + dodgeDirection;

        _targetPosition = dodgeTarget;
        _isReturningFromDodge = false;
        _dodgeWaitTimer = 0f;
        _state = State.Dodging;
        // characterBase.PlayDodgeAnimation();
    }

    public void StartDefending() {
        Debug.Log("CharacterBattle is defending!");
        _state = State.Defending;
        _isInvincible = true;
        _invincibleTimer = parryWindow;
        
        StartDefendingVisual();
        // characterBase.PlayDefendAnimation();
    }

    public void StopDefending() {
        _state = State.Idle;
        _isInvincible = false;

        StopDefendingVisual();
        // characterBase.PlayIdleAnimation();
        Debug.Log("CharacterBattle stopped defending!");
    }

    private void Damage(int damageAmount) {
        if (_isInvincible) {
            // Successful parry.
            ShowMessagePopup("Parry!", Color.yellow);
            Debug.Log("CharacterBattle parried the attack!");
            _isInvincible = false;
            return;
        }

        // Handle taking damage.
        _characterBase.PlayDamageAnimation();
        _healthSystem.TakeDamage(damageAmount);
        ShowMessagePopup(damageAmount.ToString(), Color.red);
        Debug.Log("CharacterBattle took " + damageAmount + " damage!");

        if (_healthSystem.currentHealth > 0) return;

        _state = State.Dead;
        // characterBase.PlayDeathAnimation();
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
