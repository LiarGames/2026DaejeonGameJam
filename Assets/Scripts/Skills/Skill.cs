using UnityEngine;

public abstract class Skill : ScriptableObject
{
    [Header("Visual")]
    [SerializeField] private GameObject vfxPrefab;
    [Min(0f)]
    [SerializeField] private float vfxLifetime = 0.5f;

    public void PlayCasterVFX(SkillContext context)
    {
        SpawnVFX(
            GetSkillOriginPosition(context),
            context.Direction,
            context.Caster.transform
        );
    }

    protected Vector2 GetSkillOriginPosition(SkillContext context)
    {
        Transform caster = context.Caster.transform;

        if (context.CastOrigin == null)
            return caster.position;

        Vector3 localPosition = caster.InverseTransformPoint(
            context.CastOrigin.position
        );

        Vector2 facing = context.FacingDirection.sqrMagnitude > 0.001f
            ? context.FacingDirection
            : context.Direction;

        if (Mathf.Abs(facing.x) > 0.01f)
        {
            localPosition.x =
                Mathf.Abs(localPosition.x) * Mathf.Sign(facing.x);
        }

        return caster.TransformPoint(localPosition);
    }

    protected GameObject SpawnVFX(
        Vector2 position,
        Vector2 direction,
        Transform parent = null,
        Vector3? scaleMultiplier = null)
    {
        if (vfxPrefab == null)
            return null;

        float rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject vfx = Instantiate(
            vfxPrefab,
            position,
            Quaternion.Euler(0f, 0f, rotation)
        );

        if (parent != null)
            vfx.transform.SetParent(parent, true);

        if (scaleMultiplier.HasValue)
        {
            vfx.transform.localScale = Vector3.Scale(
                vfx.transform.localScale,
                scaleMultiplier.Value
            );
        }

        if (vfxLifetime > 0f)
            Destroy(vfx, vfxLifetime);

        return vfx;
    }
}
