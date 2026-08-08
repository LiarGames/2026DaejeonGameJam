using System.Collections;
using UnityEngine;

public class AoEZone : MonoBehaviour
{
    private float _radius;
    private float _duration;
    private float _tickInterval;
    private float _damagePerTick;
    private LayerMask _targetLayer;
    private SkillModifiers _modifiers;

    public void Initialize(
        float radius,
        float duration,
        float tickInterval,
        float damagePerTick,
        LayerMask targetLayer,
        SkillModifiers modifiers)
    {
        _radius = Mathf.Max(0f, radius);
        _duration = Mathf.Max(0f, duration);
        _tickInterval = Mathf.Max(0.01f, tickInterval);
        _damagePerTick = damagePerTick;
        _targetLayer = targetLayer;
        _modifiers = modifiers;

        StartCoroutine(RunZone());
    }

    private IEnumerator RunZone()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            ApplyDamage();

            yield return new WaitForSeconds(_tickInterval);
            elapsedTime += _tickInterval;
        }

        Destroy(gameObject);
    }

    private void ApplyDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            _radius,
            _targetLayer
        );

        foreach (Collider2D hit in hits)
        {
            SkillHitProcessor.ApplyHit(
                hit,
                _damagePerTick,
                transform.position,
                _modifiers
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
