using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private Rigidbody2D _rb;

    private Vector2 _targetPosition;
    private bool _hasTarget;

    public void SetMoveTarget(Vector2 position)
    {
        _targetPosition = position;
        _hasTarget = true;
    }

    private void FixedUpdate()
    {
        if (!_hasTarget)
            return;

        Vector2 currentPosition = _rb.position;
        Vector2 difference = _targetPosition - currentPosition;

        if (difference.sqrMagnitude < 0.01f)
        {
            _hasTarget = false;
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        _rb.linearVelocity = difference.normalized * _playerStats.MoveSpeed;
    }
}