using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;

    private float _damage;
    private LayerMask _enemyLayer;
    private SkillModifiers _modifiers;

    public void Initialize(
        float damage,
        LayerMask enemyLayer,
        SkillModifiers modifiers)
    {
        _damage = damage;
        _enemyLayer = enemyLayer;
        _modifiers = modifiers;
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
        SkillHitProcessor.ApplyHit(
            target.GetComponentInParent<EnemyHealth>(),
            _damage,
            transform.position,
            _modifiers
        );
    }
}
