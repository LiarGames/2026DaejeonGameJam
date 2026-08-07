using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;

    private float _damage;
    private LayerMask _enemyLayer;

    public void Initialize(float damage, LayerMask enemyLayer)
    {
        _damage = damage;
        _enemyLayer = enemyLayer;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((_enemyLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        ApplyDamage(other);
        Destroy(gameObject);
    }

    private void ApplyDamage(Collider2D target)
    {
        Debug.Log(
            $"Projectile hit {target.name} for {_damage} damage!"
        );

        // Eventually:
        // target.GetComponent<EnemyHealth>()?.TakeDamage(_damage);
    }
}
