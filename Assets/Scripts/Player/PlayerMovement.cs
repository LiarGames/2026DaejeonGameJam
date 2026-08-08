using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerStateController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private PlayerStateController _stateController;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private Collider2D _movementBounds;

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
        ClampToBounds();
    }

    private void Move()
    {
        if (_stateController.CurrentState == PlayerState.Dashing)
            return;

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

   private void ClampToBounds()
    {
        Bounds bounds = _movementBounds.bounds;
        Vector2 halfSize = _collider.bounds.extents;

        Vector2 position = _rb.position;

        position.x = Mathf.Clamp(
            position.x,
            bounds.min.x + halfSize.x,
            bounds.max.x - halfSize.x
        );

        position.y = Mathf.Clamp(
            position.y,
            bounds.min.y + halfSize.y,
            bounds.max.y - halfSize.y
        );

        _rb.position = position;
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
