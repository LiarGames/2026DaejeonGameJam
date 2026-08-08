using UnityEngine;
using UnityEngine.UI;

// 카드 프리팹에 붙인다. 카드 비주얼(그림/이름/아이콘)은 이 프리팹에서 자유롭게 구성하고,
// 이 카드가 부여하는 스킬만 여기에 연결해 둔다.
public class Card : MonoBehaviour
{
    [SerializeField] private Skill skill;

    [Header("Highlight (선택)")]
    [SerializeField] private GameObject highlight;              // 전용 테두리 오브젝트. 없으면 아래 값으로 대체
    [SerializeField] private Color outlineColor = Color.yellow;
    [SerializeField] private float outlineSize = 6f;

    public Skill Skill => skill;

    private Outline _runtimeOutline;

    private void Awake()
    {
        if (highlight != null)
            highlight.SetActive(false);
    }

    // 배치 중인 프리뷰 카드를 강조할 때 사용.
    public void SetHighlighted(bool on)
    {
        Debug.Log($"[Card] SetHighlighted({on}) on '{name}' — highlight={(highlight != null ? highlight.name : "null(미연결)")}");

        if (highlight != null)
        {
            highlight.SetActive(on);
            return;
        }

        // 전용 테두리가 없으면 런타임 Outline으로 대체.
        if (_runtimeOutline == null)
        {
            if (!on)
                return;

            Graphic graphic = GetComponentInChildren<Graphic>();
            if (graphic == null)
                return;

            _runtimeOutline = graphic.gameObject.AddComponent<Outline>();
            _runtimeOutline.effectColor = outlineColor;
            _runtimeOutline.effectDistance = new Vector2(outlineSize, outlineSize);
        }

        _runtimeOutline.enabled = on;
    }
}
