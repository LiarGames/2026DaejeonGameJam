using UnityEngine;


public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D player;
    [SerializeField] private Rigidbody2D enemyself;
    [SerializeField] private float attackRange = 5f;


    private bool hasTarget;

    private void FixedUpdate()
    {
        Vector2 playerPosition = player.position;
        Vector2 currentPosition = enemyself.position;
        Vector2 difference = playerPosition - currentPosition;

        if (difference.sqrMagnitude < attackRange)
        {
            hasTarget = false;
            enemyself.linearVelocity = Vector2.zero;
            return;
        }

        enemyself.linearVelocity = difference.normalized * moveSpeed;
    }
}
