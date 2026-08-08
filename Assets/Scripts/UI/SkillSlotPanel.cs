using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 선택한 카드를 어디에 끼워넣을지 고르는 패널.
// 현재 스킬들을 순서대로 나열하고, 사이사이/양끝에 "여기 삽입" 버튼을 배치한다.
// [삽입0] 스킬0 [삽입1] 스킬1 ... [삽입N]
public class SkillSlotPanel : MonoBehaviour
{
    [SerializeField] private LevelUpController controller;
    [SerializeField] private PlayerSkillController skillController;
    [SerializeField] private GameObject panelRoot;        // 켜고 끌 슬롯 패널
    [SerializeField] private Transform container;         // 항목들의 부모 (Horizontal Layout Group 권장)
    [SerializeField] private Button insertButtonPrefab;   // "여기 삽입" 버튼 (자식에 TMP_Text)
    [SerializeField] private GameObject skillLabelPrefab; // 기존 스킬 표시 (자식에 TMP_Text)

    private void Start()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        if (controller != null)
            controller.OnSlotSelectionStarted += ShowSlots;
    }

    private void OnDestroy()
    {
        if (controller != null)
            controller.OnSlotSelectionStarted -= ShowSlots;
    }

    private void ShowSlots(Skill selectedCard)
    {
        if (panelRoot != null)
            panelRoot.SetActive(true);

        ClearContainer();

        int count = skillController != null ? skillController.SkillCount : 0;

        for (int i = 0; i <= count; i++)
        {
            CreateInsertButton(i);

            if (i < count)
                CreateSkillLabel(skillController.GetSkill(i));
        }
    }

    private void CreateInsertButton(int index)
    {
        int captured = index; // 클로저 캡처 안전용
        Button button = Instantiate(insertButtonPrefab, container);

        TMP_Text label = button.GetComponentInChildren<TMP_Text>();
        if (label != null)
            label.text = "＋"; // 삽입 지점 표시

        button.onClick.AddListener(() =>
        {
            controller.SelectInsertPosition(captured);
            if (panelRoot != null)
                panelRoot.SetActive(false);
        });
    }

    private void CreateSkillLabel(Skill skill)
    {
        if (skillLabelPrefab == null)
            return;

        GameObject label = Instantiate(skillLabelPrefab, container);
        TMP_Text text = label.GetComponentInChildren<TMP_Text>();
        if (text != null)
            text.text = skill != null ? skill.name : "(empty)";
    }

    private void ClearContainer()
    {
        if (container == null)
            return;

        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);
    }
}
