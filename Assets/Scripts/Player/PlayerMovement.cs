using UnityEngine;

[RequireComponent(typeof(PlayerStateController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private PlayerStateController _stateController;

    private Vector2 _movementInput;

    public Vector2 LastMoveDirection { get; private set; } = Vector2.down;

    private void Awake()
    {
        if (_stateController == null)
            _stateController = GetComponent<PlayerStateController>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (!_stateController.CanMove())
        {
            StopMovement();
            return;
        }

        if (_movementInput.sqrMagnitude > 0.001f)
        {
            _rb.linearVelocity =
                _movementInput.normalized * _playerStats.MoveSpeed;

            LastMoveDirection = _movementInput.normalized;
            _stateController.ChangeState(PlayerState.Moving);
            return;
        }

        StopMovement();
        _stateController.ChangeState(PlayerState.Idle);
    }

    public void SetMovementInput(Vector2 input)
    {
        _movementInput = input;

        if (_movementInput.sqrMagnitude > 0.001f)
        {
            LastMoveDirection = _movementInput.normalized;
        }
    }

    public void StopMovement()
    {
        _rb.linearVelocity = Vector2.zero;
    }
}
