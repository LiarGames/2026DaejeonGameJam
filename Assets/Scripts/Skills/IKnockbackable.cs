using UnityEngine;

public interface IKnockbackable
{
    void ApplyKnockback(Vector2 hitOrigin, float distance);
}
