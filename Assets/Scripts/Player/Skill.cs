using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public float cooldown;

    public abstract void Activate(GameObject player);
}