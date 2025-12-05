using System;
using UnityEngine;

public class CharacterBase : MonoBehaviour {

    private Animator animator;
    private Action onAttackCompleteEvent;
    private Action onAttackHitEvent;
    private SpriteRenderer _spriteRenderer;
    private int defaultSortingOrder;
    private Transform _visualTransform;

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

    public void PlayAttackAnimation(Action onAttackHit = null, Action onAttackComplete = null) {
        if (animator == null) return;

        SetSortingOrderTemporarily(defaultSortingOrder + 10);
        animator.SetTrigger("Attack");
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
        onAttackHitEvent = null;
    }

    private void OnAttackComplete() {
        ResetSortingOrder();
        onAttackCompleteEvent?.Invoke();
        onAttackCompleteEvent = null;
    }
}
