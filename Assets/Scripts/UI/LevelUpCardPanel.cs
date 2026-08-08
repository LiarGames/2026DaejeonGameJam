using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 레벨업 시 제시된 카드들을 버튼으로 표시한다.
// 카드 클릭 → LevelUpController.SelectCard → 슬롯(삽입 위치) 단계로 넘어감.
public class LevelUpCardPanel : MonoBehaviour
{
    [SerializeField] private LevelUpController controller;
    [SerializeField] private GameObject panelRoot;      // 켜고 끌 카드 패널
    [SerializeField] private Transform cardContainer;   // 카드들의 부모 (Layout Group 권장)

    private void Start()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        if (controller != null)
        {
            controller.OnCardsOffered += ShowCards;
            controller.OnSlotSelectionStarted += HandleSlotPhase;
        }
    }

    private void OnDestroy()
    {
        if (controller != null)
        {
            controller.OnCardsOffered -= ShowCards;
            controller.OnSlotSelectionStarted -= HandleSlotPhase;
        }
    }

    private void ShowCards(IReadOnlyList<Card> cards)
    {
        if (panelRoot != null)
            panelRoot.SetActive(true);

        ClearContainer();

        foreach (Card cardPrefab in cards)
        {
            if (cardPrefab == null)
                continue;

            // 카드 프리팹(비주얼 포함)을 그대로 생성.
            Card card = Instantiate(cardPrefab, cardContainer);
            Skill captured = card.Skill; // 클로저 캡처 안전용

            // 카드 프리팹의 버튼에 선택 동작을 연결.
            Button button = card.GetComponentInChildren<Button>();
            if (button != null)
                button.onClick.AddListener(() => controller.SelectCard(captured));
        }
    }

    // 카드 선택되면 카드 패널은 닫고 슬롯 패널이 이어받는다.
    private void HandleSlotPhase(Skill selected)
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    private void ClearContainer()
    {
        if (cardContainer == null)
            return;

        for (int i = cardContainer.childCount - 1; i >= 0; i--)
            Destroy(cardContainer.GetChild(i).gameObject);
    }
}
