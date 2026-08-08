using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Rigidbody2D _rb;

    public float CurrentHealth { get; private set; }

    private void Awake()
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody2D>();

        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0f);

        Debug.Log($"{name} took {damage} damage.");

        //if (CurrentHealth <= 0f)
        //    Destroy(gameObject);
    }

    // TODO: update knockback logic later for smoother knockback
    public void ApplyKnockback(Vector2 hitOrigin, float distance)
    {
        Vector2 direction = (_rb.position - hitOrigin).normalized;
        _rb.position += direction * distance;
    }
}
