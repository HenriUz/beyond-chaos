using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldEnemy : MonoBehaviour {
    [SerializeField] private int index;

    private Rigidbody2D _rigidbody;
    private Vector3 _position;
    private const float Range = 4.5f;
    private const float Speed = 1.2f;

    private Animator _animator;
    private readonly int isWalking = Animator.StringToHash("isRunning");
    
    private enum State {
        Chasing,
        Idle,
        Returning,
    };

    private State CheckState() {
        if (IsPlayerInRange()) {
            return State.Chasing;
        } 
        
        var diffX = Mathf.Abs(transform.position.x - _position.x);
        var diffY = Mathf.Abs(transform.position.y - _position.y);
        return diffX > 0.1 || diffY > 0.1 ? State.Returning : State.Idle;
    }

    private bool IsPlayerInRange() {
        var playerPosition = WorldManager.Instance.PlayerPosition;
        var xDiff = transform.position.x - playerPosition.x;
        var yDiff = transform.position.y - playerPosition.y;

        return Mathf.Sqrt(xDiff * xDiff + yDiff * yDiff) < Range;
    }
    
    private void FlipObject() {
        transform.localScale = new Vector3(Mathf.Sign(_rigidbody.linearVelocityX), transform.localScale.y, transform.localScale.z);
    }

    private void Awake() {
        _animator = GetComponentInChildren<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _position = transform.position;
    }

    private void FixedUpdate() {
        var state = CheckState();

        if (PauseManager.IsGamePaused) {
            _rigidbody.linearVelocity = Vector2.zero;
        } else {
            switch (state) {
                case State.Chasing:
                    _animator.SetBool(isWalking, true);
                    var playerPosition = WorldManager.Instance.PlayerPosition;
                    
                    var chasingX = playerPosition.x - transform.position.x;
                    var chasingY = playerPosition.y - transform.position.y;
                    
                    _rigidbody.linearVelocityX = Mathf.Abs(chasingX) >= 1 ? chasingX * Speed : Mathf.Sign(chasingX) * Speed;
                    _rigidbody.linearVelocityY = Mathf.Abs(chasingY) >= 1 ? chasingY * Speed : Mathf.Sign(chasingY) * Speed;
                    break;
                case State.Returning:
                    _animator.SetBool(isWalking, true);

                    var returningX = _position.x - transform.position.x;
                    var returningY = _position.y - transform.position.y;
                    
                    _rigidbody.linearVelocityX = Mathf.Abs(returningX) >= 1 ? returningX * Speed : Mathf.Sign(returningX) * Speed;
                    _rigidbody.linearVelocityY = Mathf.Abs(returningY) >= 1 ? returningY * Speed : Mathf.Sign(returningY) * Speed;
                    break;
                case State.Idle:
                default:
                    // idle
                    _animator.SetBool(isWalking, false);
                    _rigidbody.linearVelocity = Vector2.zero;
                    break;
            }
        }
        
        var flip = Mathf.Abs(_rigidbody.linearVelocityX) > Mathf.Epsilon;
        if (flip) {
            FlipObject();
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!other.gameObject.CompareTag("Player")) return;
        WorldManager.Instance.SetEnemyDead(index);
        SceneManager.LoadScene("Scenes/CombatScene");
    }
}
