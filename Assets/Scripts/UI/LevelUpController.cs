using System;
using System.Collections.Generic;
using UnityEngine;

// 레벨업 → 카드 선택 → 슬롯 선택 → 장착 흐름을 관리한다.
// 실제 화면(카드/슬롯 패널)은 이 이벤트/메서드에 UI를 연결해 구성한다.
public class LevelUpController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerSkillController skillController;

    [Header("Card Pool")]
    [SerializeField] private Card[] cardPool;           // 등장 가능한 카드 프리팹들
    [SerializeField] private int choicesPerLevel = 3;   // 레벨업마다 제시할 카드 수

    // UI가 구독: 카드 선택지 제시 / 슬롯 선택 단계 진입
    public event Action<IReadOnlyList<Card>> OnCardsOffered;
    public event Action<Skill> OnSlotSelectionStarted;

    private Skill _selectedCard;
    private int _pendingLevelUps;

    private void Start()
    {
        if (playerStats != null)
            playerStats.OnLevelUp += HandleLevelUp;
    }

    private void OnDestroy()
    {
        if (playerStats != null)
            playerStats.OnLevelUp -= HandleLevelUp;
    }

    private void HandleLevelUp()
    {
        _pendingLevelUps++;

        // 이미 선택 중이면 큐만 쌓고, 아니면 새 선택 시작.
        if (GameManager.Instance != null && GameManager.Instance.State == GameState.LevelUp)
            return;

        BeginSelection();
    }

    private void BeginSelection()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.EnterLevelUp();
        else
            Debug.LogError("[LevelUp] 씬에 GameManager가 없어 게임이 멈추지 않습니다.", this);

        OnCardsOffered?.Invoke(RollChoices());
    }

    // 풀에서 choicesPerLevel개 뽑는다.
    // 한 번의 선택지 안에서는 중복이 없다. (획득 자체는 여러 번 가능)
    private List<Card> RollChoices()
    {
        List<Card> pool = new List<Card>();
        foreach (Card c in cardPool)
        {
            if (c != null)
                pool.Add(c);
        }

        List<Card> result = new List<Card>();
        int count = Mathf.Min(choicesPerLevel, pool.Count);

        for (int i = 0; i < count; i++)
        {
            int idx = UnityEngine.Random.Range(0, pool.Count);
            result.Add(pool[idx]);
            pool.RemoveAt(idx); // 같은 카드가 이번 선택지에 또 나오지 않도록 제외
        }

        return result;
    }

    // 스킬에 해당하는 카드 프리팹을 풀에서 찾는다. (카루셀이 같은 비주얼을 쓰도록)
    public Card GetCardPrefab(Skill skill)
    {
        if (skill == null)
            return null;

        foreach (Card c in cardPool)
        {
            if (c != null && c.Skill == skill)
                return c;
        }

        return null;
    }

    // UI: 카드 하나 선택 → 슬롯 선택 단계로.
    public void SelectCard(Skill card)
    {
        if (card == null)
            return;

        _selectedCard = card;
        OnSlotSelectionStarted?.Invoke(card);
    }

    // UI: 새 카드를 끼워넣을 위치 선택 → 삽입 후 다음 처리.
    // insertIndex 0 ~ SkillCount (앞의 각 스킬 사이 + 맨 뒤).
    public void SelectInsertPosition(int insertIndex)
    {
        if (_selectedCard == null || skillController == null)
            return;

        skillController.InsertSkill(insertIndex, _selectedCard);
        _selectedCard = null;
        _pendingLevelUps--;

        // 밀린 레벨업이 있으면 다음 선택으로, 없으면 게임 재개.
        if (_pendingLevelUps > 0)
            BeginSelection();
        else if (GameManager.Instance != null)
            GameManager.Instance.ResumeFromLevelUp();
    }
}
