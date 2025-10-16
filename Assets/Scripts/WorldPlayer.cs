using UnityEngine;
using UnityEngine.InputSystem;

public class WorldPlayer : MonoBehaviour {
    private Rigidbody2D _rigidbody;

    private Vector2 _direction;
    [SerializeField] private float speed;
    
    private void Awake() {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        Move();
    }

    private void OnMove(InputValue inputValue) {
        _direction = inputValue.Get<Vector2>();
    }
    
    private void Move() {
        _rigidbody.linearVelocity = _direction * (speed * Time.deltaTime);
    }
}
