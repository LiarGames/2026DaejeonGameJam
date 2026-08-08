using UnityEngine;
using UnityEngine.UI;

// 플레이어의 장착 스킬 리스트를 회전 카루셀에 동기화한다.
// 각 칸은 카드 풀의 "카드 프리팹"을 그대로 생성해 동일한 비주얼을 쓴다.
// 배치/회전은 같은 컨테이너의 CardCarousel이 담당한다.
public class SkillCarouselBinder : MonoBehaviour
{
    [SerializeField] private PlayerSkillController skillController;
    [SerializeField] private LevelUpController levelUpController; // 스킬→카드 프리팹 매핑 제공
    [SerializeField] private Transform carouselContainer;        // CardCarousel이 배치하는 컨테이너
    [SerializeField] private CardCarousel carousel;              // 회전 동기화 대상
    [SerializeField] private bool syncRotationWithSkillLoop = true;

    // 카드가 정면에 오는 타이밍 미세조정. 0 = 시전 시작 시점에 정면.
    [Range(-0.5f, 0.5f)]
    [SerializeField] private float alignmentOffset;

    private void Start()
    {
        if (skillController != null)
            skillController.OnSkillsChanged += Rebuild;

        Rebuild(); // 시작 시 현재 장착 스킬로 초기 구성
    }

    private void OnDestroy()
    {
        if (skillController != null)
            skillController.OnSkillsChanged -= Rebuild;
    }

    // 카루셀 각도를 스킬 루프 진행도에서 직접 계산한다.
    // 발동 순간(진행도 1)에 해당 카드가 정면(depth=1, 최대 스케일)에 오도록 맞춘다.
    private void LateUpdate()
    {
        if (!syncRotationWithSkillLoop || carousel == null || skillController == null)
            return;

        // 배치 모드에선 프리뷰 카드 때문에 구성이 달라지므로 동기화를 쉰다.
        if (carousel.IsPlacement)
            return;

        int count = carousel.CardCount;
        if (count == 0)
            return;

        float step = 360f / count;

        // 발동 시 인덱스가 먼저 증가하므로 -1을 보정하면 각도가 연속적으로 이어진다.
        float position = skillController.CurrentSkillIndex - 1
            + skillController.TurnProgress + alignmentOffset;
        carousel.SetAngle(-position * step);
    }

    private void Rebuild()
    {
        if (carouselContainer == null || skillController == null || levelUpController == null)
            return;

        // 기존 슬롯 제거 (부모에서 즉시 떼내 childCount 오류 방지 후 파괴).
        for (int i = carouselContainer.childCount - 1; i >= 0; i--)
        {
            Transform child = carouselContainer.GetChild(i);
            child.SetParent(null);
            Destroy(child.gameObject);
        }

        // 스킬 순서대로, 해당 스킬의 카드 프리팹을 생성.
        for (int i = 0; i < skillController.SkillCount; i++)
        {
            Skill skill = skillController.GetSkill(i);
            Card prefab = levelUpController.GetCardPrefab(skill);
            if (prefab == null)
            {
                Debug.LogWarning($"[Binder] {i}번 스킬 '{(skill != null ? skill.name : "null")}'에 대응하는 카드 프리팹이 Card Pool에 없음");
                continue;
            }

            Card card = Instantiate(prefab, carouselContainer);

            // 안정적인 배치를 위해 논리 순서를 부여.
            CarouselItem item = card.GetComponent<CarouselItem>();
            if (item == null)
                item = card.gameObject.AddComponent<CarouselItem>();
            item.LogicalIndex = i;

            // 표시 전용. interactable=false는 Disabled 틴트(기본 반투명)가 먹으므로
            // 컴포넌트 자체를 꺼서 색 변화 없이 클릭만 막는다.
            Button button = card.GetComponentInChildren<Button>();
            if (button != null)
                button.enabled = false;
        }
    }
}
