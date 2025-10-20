using System;
using UnityEngine;

public class CharacterBattle : MonoBehaviour {

    private int characterHealth = 100;
    private CharacterBase characterBase;
    private State state;
    private Vector3 targetPosition;
    private Action onMoveComplete;
    private GameObject selectionIndicatorGO;
    private SpriteRenderer spriteRenderer;

    private bool isInvincible;
    [SerializeField] private float parryWindow = 0.22f;
    [SerializeField] private float dodgeWindow = 0.28f;
    [SerializeField] private float speedDodge = 10f;
    private bool isReturningFromDodge = false;
    private float dodgeWaitTimer = 0f;
    private float invencibleTimer = 0f;

    private enum State {
        IDLE,
        MOVING,
        ATTACKING,
        DEFENDING,
        DODGING,
        DEAD
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        characterBase = GetComponent<CharacterBase>();        
        selectionIndicatorGO = transform.Find("SelectionIndicator").gameObject;
        HideSelectionIndicator();
        state = State.IDLE;
    }

    private void Update() {
        switch (state) {
            case State.IDLE:
                // Handle idle behavior
                break;
            case State.MOVING:
                float speed = 20f;
                transform.position = Vector3.MoveTowards(GetPosition(), targetPosition, Time.deltaTime * speed);

                float distanceAllowed = 1f;
                if (Vector3.Distance(GetPosition(), targetPosition) < distanceAllowed) {
                    transform.position = targetPosition;
                    onMoveComplete();
                }

                break;
            case State.DEFENDING:
                // Handle defending behavior
                invencibleTimer -= Time.deltaTime;
                if (invencibleTimer <= 0f) {
                    StopDefending();
                }
                break;
            case State.DODGING:
                // Handle dodging behavior
                // float speedDodge = 10f;
                transform.position = Vector3.MoveTowards(GetPosition(), targetPosition, Time.deltaTime * speedDodge);

                if (Vector3.Distance(GetPosition(), targetPosition) < 0.1f) {
                if (!isReturningFromDodge) {
                    // Chegou ao ponto da esquiva — espera um pouco antes de voltar
                    invencibleTimer -= Time.deltaTime;

                    if (invencibleTimer <= 0f) {
                        // Começa o retorno
                        targetPosition = GetPosition() - new Vector3(-1f, 0); // volta à posição original
                        isReturningFromDodge = true;
                    }
                } else {
                    // Terminou o retorno
                    state = State.IDLE;
                    isInvincible = false;
                    StopDefendingVisual();
                    Debug.Log("CharacterBattle finished dodging!");
                }
            }
                break;
            case State.ATTACKING:
                // Handle attacking behavior
            case State.DEAD:
                // Handle dead behavior
                break;
        }
    }


    public void Setup(bool isPlayerTeam) {
        RuntimeAnimatorController animatorOverride = isPlayerTeam ? BattleHandler.GetInstance().playerAnimatorOverride : BattleHandler.GetInstance().enemyAnimatorOverride;

        characterBase.SetAnimatorOverride(animatorOverride);

        if (!isPlayerTeam) {
            Vector3 localScale = transform.localScale;
            localScale.x = -Mathf.Abs(localScale.x);
            transform.localScale = localScale;
        }
    }

    private void MoveToPosition(Vector3 targetPosition, Action onMoveComplete) {
        this.targetPosition = targetPosition;
        this.onMoveComplete = onMoveComplete;
        state = State.MOVING;

        characterBase.PlayMoveAnimation(targetPosition);
    }

    private void DodgeToPosition(Vector3 targetPosition, Action onMoveComplete) {
        this.targetPosition = targetPosition;
        this.onMoveComplete = onMoveComplete;
        state = State.DODGING;

        // characterBase.PlayDodgeAnimation(targetPosition);
    }

    public void Attack(CharacterBattle targetCharacter, Action onAttackComplete) {
        Vector3 originalPosition = GetPosition();
        Vector3 targetPosition = targetCharacter.GetPosition();
        Vector3 directionToTarget = targetPosition + (originalPosition - targetPosition).normalized * 1.5f;

        // Move towards the target
        MoveToPosition(directionToTarget, () => {
            // Arrived, attacking
            state = State.ATTACKING;
            characterBase.PlayAttackAnimation(() => {
                // Attack hit event
                targetCharacter.Damage(25);
            }, () => {
                // Attack finished, moving back to original position
                MoveToPosition(originalPosition, () => {
                    // Moving back finished
                    state = State.IDLE;
                    characterBase.PlayIdleAnimation(directionToTarget);
                    onAttackComplete();
                });
            });
        });
    }

    private void StartDefendingVisual() {
        spriteRenderer.color = Color.cyan;
    }

    private void StopDefendingVisual() {
        spriteRenderer.color = Color.white;
    }

    public void Dodge() {
        if (state == State.DODGING) return; // evita múltiplos dashes sobrepostos
        
        Debug.Log("CharacterBattle is dodging!");
        isInvincible = true;
        invencibleTimer = dodgeWindow;

        StartDefendingVisual();
        
        Vector3 dodgeDirection = new Vector3(-1f, 0); // pode ajustar conforme o lado do player
        Vector3 dodgeTarget = GetPosition() + dodgeDirection;

        targetPosition = dodgeTarget;
        isReturningFromDodge = false;
        dodgeWaitTimer = 0f;
        state = State.DODGING;
        // characterBase.PlayDodgeAnimation();
    }

    public void StartDefending() {
        Debug.Log("CharacterBattle is defending!");
        state = State.DEFENDING;
        isInvincible = true;
        invencibleTimer = parryWindow;
        
        StartDefendingVisual();
        // characterBase.PlayDefendAnimation();
    }

    public void StopDefending() {
        state = State.IDLE;
        isInvincible = false;

        StopDefendingVisual();
        // characterBase.PlayIdleAnimation();
        Debug.Log("CharacterBattle stopped defending!");
    }

    private void Damage(int damageAmount) {
        if (isInvincible) {
            // Successful parry
            Debug.Log("CharacterBattle parried the attack!");
            isInvincible = false;
            return;
        }

        // Handle taking damage
        characterBase.PlayDamageAnimation();
        characterHealth -= damageAmount;
        Debug.Log("CharacterBattle took " + damageAmount + " damage!");

        if (characterHealth <= 0) {
            characterHealth = 0;
            state = State.DEAD;
            // characterBase.PlayDeathAnimation();
        }
    }

    public bool IsDead() {
        // Verify if character is dead
        // return state == State.DEAD;
        return false;
    }

    public void ShowSelectionIndicator() {
        selectionIndicatorGO.SetActive(true);
    }

    public void HideSelectionIndicator() {
        selectionIndicatorGO.SetActive(false);
    }
}
