using UnityEngine;

public abstract class Skill : ScriptableObject
{
    [SerializeField] protected float cooldown = 1f;

    public float Cooldown { get; private set; }

    public abstract void Activate(GameObject player, Vector2 direction);
}