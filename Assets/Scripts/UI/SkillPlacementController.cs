using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// 카드 선택 후, 카루셀을 확대하고 프리뷰 카드를 마우스 근처 칸에 끼워 보여준다.
// 클릭하면 그 위치로 삽입을 확정한다. (기존 SkillSlotPanel의 ＋버튼 방식을 대체)
public class SkillPlacementController : MonoBehaviour
{
    [SerializeField] private LevelUpController levelUpController;
    [SerializeField] private PlayerSkillController skillController;
    [SerializeField] private CardCarousel carousel;
    [SerializeField] private RectTransform carouselContainer;
    [SerializeField] private Canvas canvas; // 좌표 변환용 (Overlay면 카메라 null)

    private Card _preview;
    private CarouselItem _previewItem;
    private readonly List<CarouselItem> _baseItems = new();
    private bool _active;
    private int _startFrame;

    private void Start()
    {
        if (levelUpController != null)
            levelUpController.OnSlotSelectionStarted += BeginPlacement;
    }

    private void OnDestroy()
    {
        if (levelUpController != null)
            levelUpController.OnSlotSelectionStarted -= BeginPlacement;
    }

    private void BeginPlacement(Skill selected)
    {
        Card prefab = levelUpController.GetCardPrefab(selected);
        if (prefab == null || carousel == null || carouselContainer == null)
            return;

        // 기존 카드들을 논리 순서대로 캡처.
        _baseItems.Clear();
        foreach (Transform child in carouselContainer)
        {
            CarouselItem it = child.GetComponent<CarouselItem>();
            if (it != null)
                _baseItems.Add(it);
        }
        _baseItems.Sort((a, b) => a.LogicalIndex.CompareTo(b.LogicalIndex));

        // 프리뷰 카드 생성 (표시 전용).
        _preview = Instantiate(prefab, carouselContainer);
        Button b = _preview.GetComponentInChildren<Button>();
        if (b != null)
            b.enabled = false; // Disabled 틴트로 반투명해지지 않게 컴포넌트를 끔

        _preview.SetHighlighted(true); // 배치 중인 카드만 테두리로 강조

        _previewItem = _preview.GetComponent<CarouselItem>();
        if (_previewItem == null)
            _previewItem = _preview.gameObject.AddComponent<CarouselItem>();

        _active = true;
        _startFrame = Time.frameCount;
        carousel.SetPlacementMode(true);
    }

    private void Update()
    {
        if (!_active || Mouse.current == null)
            return;

        Vector2 mouse = Mouse.current.position.ReadValue();
        int insertIndex = carousel.GetNearestSlotIndex(mouse, GetCanvasCamera());

        ReflowWithPreview(insertIndex);

        // 활성화된 첫 프레임의 클릭(카드 선택 클릭)은 무시.
        if (Time.frameCount > _startFrame && Mouse.current.leftButton.wasPressedThisFrame)
            Confirm(insertIndex);
    }

    // 프리뷰를 p 위치에 두고, 기존 카드들을 그 앞뒤로 재배치.
    private void ReflowWithPreview(int p)
    {
        for (int j = 0; j < _baseItems.Count; j++)
            _baseItems[j].LogicalIndex = (j < p) ? j : j + 1;

        if (_previewItem != null)
            _previewItem.LogicalIndex = p;
    }

    private void Confirm(int insertIndex)
    {
        _active = false;
        carousel.SetPlacementMode(false);

        if (_preview != null)
            Destroy(_preview.gameObject);
        _preview = null;
        _previewItem = null;
        _baseItems.Clear();

        // 삽입 확정 → InsertSkill → OnSkillsChanged → 바인더가 카루셀 재구성.
        levelUpController.SelectInsertPosition(insertIndex);
    }

    private Camera GetCanvasCamera()
    {
        if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return null;

        return canvas.worldCamera;
    }
}
