using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;

    private float _damage;
    private LayerMask _targetLayer;
    private SkillModifiers _modifiers;

    public void Initialize(
        float damage,
        LayerMask targetLayer,
        SkillModifiers modifiers)
    {
        _damage = damage;
        _targetLayer = targetLayer;
        _modifiers = modifiers;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((_targetLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        ApplyDamage(other);
        Destroy(gameObject);
    }

    private void ApplyDamage(Collider2D target)
    {
        SkillHitProcessor.ApplyHit(
            target,
            _damage,
            transform.position,
            _modifiers
        );
    }
}
