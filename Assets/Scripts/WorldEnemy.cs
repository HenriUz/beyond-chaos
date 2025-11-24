using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldEnemy : MonoBehaviour {
    [SerializeField] private int index;

    private Rigidbody2D _rigidbody;
    private Vector3 _position;
    private const float Range = 4.5f;
    private const float Speed = 1.2f;
    
    private enum State {
        Chasing,
        Idle,
        Returning,
    };

    private State CheckState() {
        if (IsPlayerInRange()) {
            return State.Chasing;
        } 
        
        return _position != transform.position ? State.Returning : State.Idle;
    }

    private bool IsPlayerInRange() {
        var playerPosition = WorldManager.Instance.PlayerPosition;
        var xDiff = transform.position.x - playerPosition.x;
        var yDiff = transform.position.y - playerPosition.y;

        return Mathf.Sqrt(xDiff * xDiff + yDiff * yDiff) < Range;
    }

    private void Awake() {
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
                    var playerPosition = WorldManager.Instance.PlayerPosition;
                    _rigidbody.linearVelocityX = (playerPosition.x - transform.position.x) * Speed;
                    _rigidbody.linearVelocityY = (playerPosition.y - transform.position.y) * Speed;
                    break;
                case State.Returning:
                    _rigidbody.linearVelocityX = (_position.x - transform.position.x) * Speed;
                    _rigidbody.linearVelocityY = (_position.y - transform.position.y) * Speed;
                    break;
                case State.Idle:
                default:
                    // idle
                    _rigidbody.linearVelocity = Vector2.zero;
                    break;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!other.gameObject.CompareTag("Player")) return;
        WorldManager.Instance.SetEnemyDead(index);
        SceneManager.LoadScene("Scenes/CombatScene");
    }
}
