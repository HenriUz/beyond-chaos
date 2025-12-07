using UnityEngine;
using UnityEngine.InputSystem;

public class WorldPlayer : MonoBehaviour {
    private Rigidbody2D _rigidbody;
    
    private Animator _animator;
    private readonly int isWalking = Animator.StringToHash("isRunning");
    
    private Vector2 _direction;
    [SerializeField] private float speed;
    private PlayerStats _playerStats;
    
    private void Awake() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _playerStats = GetComponent<PlayerStats>();
    }
    

    private void Start() {
        transform.position = WorldManager.Instance.PlayerPosition;
        
        // Initialize player stats in WorldManager
        if (_playerStats != null) {
            WorldManager.Instance.InitializePlayerStats(_playerStats);
        }
    }
    
    private void FixedUpdate() {
        Move();
    }

    private void OnMove(InputValue inputValue) {
        _direction = inputValue.Get<Vector2>();
    }
    
    private void Move() {
        if (PauseManager.Instance.IsGamePaused) {
            _rigidbody.linearVelocity = Vector2.zero;
            _animator.SetBool(isWalking, false);
        } else {
            _rigidbody.linearVelocity = _direction * (speed * Time.deltaTime);
            WorldManager.Instance.UpdatePlayerPosition(transform.position);
            
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

    private void OnPause(InputValue inputValue) {
        if (PauseManager.Instance.IsMenuActive) {
            PauseManager.Instance.Resume();
        } else if (!PauseManager.Instance.IsGamePaused) {
            PauseManager.Instance.ShowMenu();
        }
    }
}
