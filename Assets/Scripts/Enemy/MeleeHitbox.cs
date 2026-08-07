using UnityEngine;


public class MeleeHitbox : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.2f;

    private float damage;
    private bool hasHit;

    public void Initialize(float damage)
    {
        this.damage = damage;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
            return;

        PlayerStats stats = other.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.TakeDamage(damage);
            hasHit = true;
        }
    }
}
