using System;
using UnityEngine;

public class CharacterBase : MonoBehaviour {

    private Animator animator;
    private Action onAttackCompleteEvent;
    private Action onAttackHitEvent;
    private SpriteRenderer spriteRenderer;
    private int defaultSortingOrder;

    private void Awake() {
        if (animator == null) {
            animator = GetComponent<Animator>();
        }
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        defaultSortingOrder = spriteRenderer.sortingOrder;
    }

    public void SetAnimatorOverride(RuntimeAnimatorController animatorOverride) {
        if (animator == null) return;
        if (animatorOverride == null) return;

        animator.runtimeAnimatorController = animatorOverride;
    }

    public void PlayIdleAnimation(Vector3 direction) {
        if (animator == null) return;

        if (direction.x < 0) {
            Vector3 localScale = transform.localScale;
            localScale.x = -Mathf.Abs(localScale.x);
            transform.localScale = localScale;
        } else if (direction.x > 0) {
            Vector3 localScale = transform.localScale;
            localScale.x = Mathf.Abs(localScale.x);
            transform.localScale = localScale;
        }
        
        animator.SetBool("isRunning", false);
    }

    public void PlayMoveAnimation(Vector3 direction) {
        if (animator == null) return;
        
        if (direction.x < 0) {
            Vector3 localScale = transform.localScale;
            localScale.x = -Mathf.Abs(localScale.x);
            transform.localScale = localScale;
        } else if (direction.x > 0) {
            Vector3 localScale = transform.localScale;
            localScale.x = Mathf.Abs(localScale.x);
            transform.localScale = localScale;
        }

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
        spriteRenderer.sortingOrder = order;
    }

    private void ResetSortingOrder() {
        spriteRenderer.sortingOrder = defaultSortingOrder;
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
