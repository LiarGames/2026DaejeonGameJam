using UnityEngine;

public class EnemyVisualBob : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.1f;
    [SerializeField] private float frequency = 3f;

    private Vector3 _startLocalPosition;

    private float _phase;

    private void Awake()
    {
        _startLocalPosition = transform.localPosition;
        _phase = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        float offset =
            Mathf.Sin(Time.time * frequency + _phase)
            * amplitude;

        transform.localPosition =
            _startLocalPosition + Vector3.up * offset;
    }
}