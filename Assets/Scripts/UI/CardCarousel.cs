using System.Collections.Generic;
using UnityEngine;

// 자식 카드들을 원(타원)으로 배치하고 회전시킨다.
// 각도는 각 카드의 CarouselItem.LogicalIndex로 매겨 순서가 안정적이며,
// 형제 순서는 그리기(depth)용으로만 재정렬한다.
public class CardCarousel : MonoBehaviour
{
    [SerializeField] private RectTransform cardContainer; // 카드들의 부모 (비우면 자기 자신)

    [Header("Shape")]
    [SerializeField] private float radiusX = 300f;
    [SerializeField] private float radiusY = 60f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 30f; // 초당 회전 각도(도)

    [Header("Depth Effect")]
    [SerializeField] private float minScale = 0.6f;
    [SerializeField] private float maxScale = 1f;

    [Header("Placement Mode")]
    [SerializeField] private float placementScale = 1.5f;          // 배치 모드 확대 배율
    [SerializeField] private Vector2 placementCenter = Vector2.zero; // 배치 모드에서 이동할 위치(부모 기준)
    [SerializeField] private float transitionSpeed = 10f;           // 이동/확대 전환 속도
    [SerializeField] private float indexSlideSpeed = 12f;           // 순서 변경 시 카드가 미끄러지는 속도

    private float currentAngle;
    private bool _placement;
    private Vector2 _basePos;   // 평소 위치 (배치 모드 해제 시 복귀)
    private float _mul = 1f;    // 현재 확대 배율 (부드럽게 전환)
    private bool _externalAngle; // 외부에서 각도를 주입 중인지
    private readonly List<(RectTransform card, float depth)> order = new();

    public bool IsPlacement => _placement;

    private void Reset()
    {
        cardContainer = transform as RectTransform;
    }

    private void Awake()
    {
        if (cardContainer == null)
            cardContainer = transform as RectTransform;

        _basePos = cardContainer.anchoredPosition;
    }

    private void Update()
    {
        // 배치 모드에선 회전을 멈춰 슬롯 위치를 고정한다. (레벨업 중 timeScale 0 대비 unscaled)
        // 외부에서 각도를 주입(스킬 루프 동기화)하는 경우에도 자체 회전은 하지 않는다.
        if (!_placement && !_externalAngle)
            currentAngle += rotationSpeed * Time.unscaledDeltaTime;

        // 위치·확대를 목표값으로 부드럽게 전환.
        float t = 1f - Mathf.Exp(-transitionSpeed * Time.unscaledDeltaTime);
        _mul = Mathf.Lerp(_mul, _placement ? placementScale : 1f, t);
        cardContainer.anchoredPosition = Vector2.Lerp(
            cardContainer.anchoredPosition,
            _placement ? placementCenter : _basePos,
            t
        );

        LayoutCards();
    }

    private float Mul => _mul;

    // 배치에 쓸 순서를 구한다. LogicalIndex가 바뀌면 링을 따라 최단 경로로 미끄러지듯 이동.
    private float GetDisplayIndex(Transform child, int fallback, float step)
    {
        CarouselItem item = child.GetComponent<CarouselItem>();
        if (item == null)
            return fallback;

        if (!item.DisplayInitialized)
        {
            item.DisplayIndex = item.LogicalIndex;
            item.DisplayInitialized = true;
            return item.DisplayIndex;
        }

        // 각도 도메인에서 최단 경로를 구하면 0↔마지막 순환도 자연스럽게 처리된다.
        float delta = Mathf.DeltaAngle(item.DisplayIndex * step, item.LogicalIndex * step) / step;
        float t = 1f - Mathf.Exp(-indexSlideSpeed * Time.unscaledDeltaTime);
        item.DisplayIndex += delta * t;

        return item.DisplayIndex;
    }

    private void LayoutCards()
    {
        int count = cardContainer.childCount;
        if (count == 0)
            return;

        float step = 360f / count;
        float mul = Mul;
        order.Clear();

        for (int i = 0; i < count; i++)
        {
            RectTransform card = cardContainer.GetChild(i) as RectTransform;
            if (card == null)
                continue;

            float logical = GetDisplayIndex(card, i, step);
            float angle = (currentAngle + logical * step) * Mathf.Deg2Rad;
            float depth = Mathf.Cos(angle); // 1 = 맨 앞, -1 = 맨 뒤

            card.anchoredPosition = new Vector2(
                Mathf.Sin(angle) * radiusX * mul,
                -depth * radiusY * mul
            );

            float scale = Mathf.Lerp(minScale, maxScale, (depth + 1f) * 0.5f) * mul;
            card.localScale = new Vector3(scale, scale, 1f);

            order.Add((card, depth));
        }

        // 앞쪽(depth 큰) 카드가 위에 그려지도록 형제 순서만 재정렬. (논리 순서엔 영향 없음)
        order.Sort((a, b) => a.depth.CompareTo(b.depth));
        foreach (var item in order)
            item.card.SetAsLastSibling();
    }

    public void SetPlacementMode(bool on)
    {
        _placement = on;
    }

    public int CardCount => cardContainer != null ? cardContainer.childCount : 0;

    // 스킬 루프에 맞춰 외부에서 회전 각도를 지정한다. (자체 회전은 중단)
    public void SetAngle(float degrees)
    {
        _externalAngle = true;
        currentAngle = degrees;
    }

    // 마우스 화면좌표에 가장 가까운 슬롯 인덱스(0 ~ count-1)를 반환.
    // 배치 모드에서 프리뷰 카드를 어디에 끼울지 결정하는 데 쓴다.
    public int GetNearestSlotIndex(Vector2 screenPoint, Camera cam)
    {
        int count = cardContainer.childCount;
        if (count == 0)
            return 0;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cardContainer, screenPoint, cam, out Vector2 local);

        float step = 360f / count;
        float mul = Mul;
        int best = 0;
        float bestDist = float.MaxValue;

        for (int m = 0; m < count; m++)
        {
            float a = (currentAngle + m * step) * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(
                Mathf.Sin(a) * radiusX * mul,
                -Mathf.Cos(a) * radiusY * mul
            );

            float d = (pos - local).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = m;
            }
        }

        return best;
    }
}
