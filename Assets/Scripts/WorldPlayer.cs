using UnityEngine;
using UnityEngine.InputSystem;

public class WorldPlayer : MonoBehaviour {
    private Rigidbody2D _rigidbody;
    
    private Animator _animator;
    private readonly int isWalking = Animator.StringToHash("isWalking");
    
    private Vector2 _direction;
    [SerializeField] private float speed;
    
    private void Awake() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start() {
        transform.position = WorldManager.Instance.PlayerPosition;
    }
    
    private void FixedUpdate() {
        Move();
    }

    private void OnMove(InputValue inputValue) {
        _direction = inputValue.Get<Vector2>();
    }
    
    private void Move() {
        if (PauseManager.IsGamePaused) {
            _rigidbody.linearVelocity = Vector2.zero;
            _animator.SetBool(isWalking, false);
        } else {
            _rigidbody.linearVelocity = _direction * (speed * Time.deltaTime);
            
            var flip = Mathf.Abs(_rigidbody.linearVelocityX) > Mathf.Epsilon;
            if (flip) {
                FlipObject();
            }
            
            var isWalkingCond = flip || Mathf.Abs(_rigidbody.linearVelocityY) > Mathf.Epsilon;
            _animator.SetBool(isWalking, isWalkingCond);
        }
    }

    private void FlipObject() {
        transform.localScale = new Vector3(Mathf.Sign(_rigidbody.linearVelocityX), transform.localScale.y, transform.localScale.z);
    }
}
