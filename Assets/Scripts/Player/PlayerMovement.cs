using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private Rigidbody2D _rb;

    private Vector2 _movementInput;

    public Vector2 LastMoveDirection { get; private set; } = Vector2.down;

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        _rb.linearVelocity =
            _movementInput.normalized * _playerStats.MoveSpeed;
    }

    public void SetMovementInput(Vector2 input)
    {
        _movementInput = input;

        if (_movementInput.sqrMagnitude > 0.001f)
        {
            LastMoveDirection = _movementInput.normalized;
        }
    }
}