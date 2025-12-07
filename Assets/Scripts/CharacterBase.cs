using System;
using UnityEngine;


public class CharacterBase : MonoBehaviour {

    private Animator animator;
    private Action onAttackCompleteEvent;
    private Action onAttackHitEvent;
    private SpriteRenderer _spriteRenderer;
    private int defaultSortingOrder;
    private Transform _visualTransform;

    public enum AttackType {
        Attack1,
        Attack2,
        Attack3  // Apenas para boss
    }

    private void Awake() {
        if (animator == null) {
            animator = GetComponent<Animator>();
        }
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        defaultSortingOrder = _spriteRenderer.sortingOrder;

        if (_spriteRenderer != null) {
            _visualTransform = _spriteRenderer.transform;
        }
    }

    public void SetAnimatorOverride(RuntimeAnimatorController animatorOverride) {
        if (animator == null) return;
        if (animatorOverride == null) return;

        animator.runtimeAnimatorController = animatorOverride;
    }

    private void SetDirection(Vector3 direction) {
        if (direction.x < 0) {
            Vector3 localScale = _visualTransform.localScale;
            localScale.x = -Mathf.Abs(localScale.x);
            _visualTransform.localScale = localScale;
        } else if (direction.x > 0) {
            Vector3 localScale = _visualTransform.localScale;
            localScale.x = Mathf.Abs(localScale.x);
            _visualTransform.localScale = localScale;
        }
    }

    public void SetAttackSpeedMultiplier(float multiplier) {
        if (animator == null) return;

        animator.SetFloat("AttackMultiplier", multiplier);
    }

    public void PlayIdleAnimation(Vector3 direction) {
        if (animator == null) return;

        SetDirection(direction);
        
        animator.SetBool("isRunning", false);
    }

    public void PlayMoveAnimation(Vector3 direction) {
        if (animator == null) return;
        
        SetDirection(direction);    

        animator.SetBool("isRunning", true);
    }

    public void PlayDamageAnimation() {
        if (animator == null) return;

        animator.SetTrigger("Damage");
    }

    public void PlayAttackAnimation(Action onAttackHit = null, Action onAttackComplete = null, AttackType attackType = AttackType.Attack1) {
        if (animator == null) return;

        SetSortingOrderTemporarily(defaultSortingOrder + 10);
        
        switch (attackType) {
            case AttackType.Attack1:
                animator.SetTrigger("Attack 1");
                break;
            case AttackType.Attack2:
                animator.SetTrigger("Attack 2");
                break;
            case AttackType.Attack3:
                animator.SetTrigger("Attack 3");
                break;
        }
            
        onAttackHitEvent = onAttackHit;
        onAttackCompleteEvent = onAttackComplete;
    }

    private void SetSortingOrderTemporarily(int order) {
        _spriteRenderer.sortingOrder = order;
    }

    private void ResetSortingOrder() {
        _spriteRenderer.sortingOrder = defaultSortingOrder;
    }

    private void OnAttackHit() {
        onAttackHitEvent?.Invoke();
    }

    private void OnAttackComplete() {
        ResetSortingOrder();
        onAttackCompleteEvent?.Invoke();

        onAttackHitEvent = null;
        onAttackCompleteEvent = null;
    }
}
