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
            context.Caster.transform.position,
            context.Direction,
            context.Caster.transform
        );
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
