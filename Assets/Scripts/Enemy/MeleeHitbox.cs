using UnityEngine;


public class MeleeHitbox : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.2f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
