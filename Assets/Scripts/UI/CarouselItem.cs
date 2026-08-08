using UnityEngine;

// 카루셀 카드의 "논리 순서". 각도 배치는 이 값으로 하고,
// 형제(sibling) 순서는 그리기 순서(depth)용으로만 쓴다.
public class CarouselItem : MonoBehaviour
{
    public int LogicalIndex;

    // 실제 배치에 쓰이는 보간된 순서. LogicalIndex가 바뀌면 여기로 부드럽게 따라간다.
    [System.NonSerialized] public float DisplayIndex;
    [System.NonSerialized] public bool DisplayInitialized;
}
