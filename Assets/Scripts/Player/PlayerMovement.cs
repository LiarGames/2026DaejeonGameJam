using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D rb;

    private Vector2 targetPosition;
    private bool hasTarget;

    public void SetMoveTarget(Vector2 position)
    {
        targetPosition = position;
        hasTarget = true;
    }

    private void FixedUpdate()
    {
        if (!hasTarget)
            return;

        Vector2 currentPosition = rb.position;
        Vector2 difference = targetPosition - currentPosition;

        if (difference.sqrMagnitude < 0.01f)
        {
            hasTarget = false;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = difference.normalized * moveSpeed;
    }
}