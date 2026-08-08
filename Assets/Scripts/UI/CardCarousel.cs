using System.Collections.Generic;
using UnityEngine;

// 자식 카드들을 원(타원)으로 배치하고 계속 회전시킨다.
// 카드 수는 매 프레임 자식 개수로 읽으므로, 장수가 바뀌어도 자동으로 균등 배치된다.
public class CardCarousel : MonoBehaviour
{
    [SerializeField] private RectTransform cardContainer; // 카드들의 부모 (비우면 자기 자신)

    [Header("Shape")]
    [SerializeField] private float radiusX = 300f;   // 가로 반지름
    [SerializeField] private float radiusY = 60f;    // 세로 반지름 (작게 하면 납작한 카루셀)

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 30f; // 초당 회전 각도(도). 음수면 반대 방향

    [Header("Depth Effect")]
    [SerializeField] private float minScale = 0.6f;  // 맨 뒤 카드 크기
    [SerializeField] private float maxScale = 1f;    // 맨 앞 카드 크기

    private float currentAngle;
    private readonly List<(RectTransform card, float depth)> order = new();

    private void Reset()
    {
        cardContainer = transform as RectTransform;
    }

    private void Awake()
    {
        if (cardContainer == null)
            cardContainer = transform as RectTransform;
    }

    private void Update()
    {
        currentAngle += rotationSpeed * Time.deltaTime;
        LayoutCards();
    }

    private void LayoutCards()
    {
        int count = cardContainer.childCount;
        if (count == 0)
            return;

        float step = 360f / count; // 카드 수에 맞춰 균등 분할
        order.Clear();

        for (int i = 0; i < count; i++)
        {
            RectTransform card = cardContainer.GetChild(i) as RectTransform;
            if (card == null)
                continue;

            float angle = (currentAngle + i * step) * Mathf.Deg2Rad;
            float depth = Mathf.Cos(angle); // 1 = 맨 앞, -1 = 맨 뒤

            // 앞(크게 보이는) 카드가 아래로 가도록 depth에 마이너스.
            card.anchoredPosition = new Vector2(Mathf.Sin(angle) * radiusX, -depth * radiusY);

            float scale = Mathf.Lerp(minScale, maxScale, (depth + 1f) * 0.5f);
            card.localScale = new Vector3(scale, scale, 1f);

            order.Add((card, depth));
        }

        // 앞쪽(depth 큰) 카드가 위에 그려지도록 정렬
        order.Sort((a, b) => a.depth.CompareTo(b.depth));
        foreach (var item in order)
            item.card.SetAsLastSibling();
    }
}
