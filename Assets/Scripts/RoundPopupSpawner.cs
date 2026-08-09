using UnityEngine;

// 라운드가 바뀔 때마다 배경 영역 안에 팝업창 스프라이트를 랜덤으로 하나씩 띄운다.
// 라운드 표시 UI 대신, 화면이 점점 팝업으로 뒤덮이며 진행도를 보여주는 연출.
public class RoundPopupSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private SpriteRenderer area;   // 배경 일러스트1_0 (팝업이 놓일 범위)
    [SerializeField] private Transform popupParent; // 비우면 area의 자식으로 생성

    [Header("Popup")]
    [SerializeField] private Sprite[] popupSprites; // Assets/Art/Popup 의 스프라이트들
    [SerializeField] private int popupsPerRound = 1;
    [SerializeField] private int sortingOrderOffset = 1; // 배경보다 얼마나 위에 그릴지

    [Header("Variation")]
    [SerializeField] private Vector2 scaleRange = new Vector2(0.9f, 1.15f);

    [Header("Background Variants")]
    [Tooltip("라운드마다 이 중 하나만 랜덤으로 켠다. (예: pc1_0, pc2_0)")]
    [SerializeField] private GameObject[] backgroundVariants;

    private void Start()
    {
        if (spawner != null)
            spawner.OnRoundStarted += HandleRoundStarted;
    }

    private void OnDestroy()
    {
        if (spawner != null)
            spawner.OnRoundStarted -= HandleRoundStarted;
    }

    private void HandleRoundStarted(int round)
    {
        // 배경은 1라운드부터 매 라운드 랜덤으로 교체된다.
        PickRandomBackground();

        // 팝업은 1라운드엔 없고, 이후 라운드마다 쌓여 총 (라운드 - 1)개가 된다.
        if (round <= 1)
            return;

        for (int i = 0; i < popupsPerRound; i++)
            SpawnPopup();
    }

    // 배경 후보 중 하나만 켜고 나머지는 끈다.
    private void PickRandomBackground()
    {
        if (backgroundVariants == null || backgroundVariants.Length == 0)
            return;

        int chosen = Random.Range(0, backgroundVariants.Length);

        for (int i = 0; i < backgroundVariants.Length; i++)
        {
            if (backgroundVariants[i] != null)
                backgroundVariants[i].SetActive(i == chosen);
        }
    }

    private void SpawnPopup()
    {
        if (area == null || popupSprites == null || popupSprites.Length == 0)
            return;

        Sprite sprite = popupSprites[Random.Range(0, popupSprites.Length)];
        if (sprite == null)
            return;

        float scale = Random.Range(scaleRange.x, scaleRange.y);

        GameObject popup = new GameObject($"Popup_{sprite.name}");
        popup.transform.SetParent(popupParent != null ? popupParent : area.transform, false);
        popup.transform.localScale = Vector3.one * scale;
        popup.transform.rotation = Quaternion.identity; // OS 팝업창처럼 항상 수평

        SpriteRenderer renderer = popup.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingLayerID = area.sortingLayerID;
        renderer.sortingOrder = area.sortingOrder + sortingOrderOffset;

        popup.transform.position = RandomPointInArea(sprite, scale);
    }

    // 팝업이 배경 밖으로 삐져나오지 않도록 스프라이트 크기의 절반만큼 안쪽으로 넣는다.
    private Vector3 RandomPointInArea(Sprite sprite, float scale)
    {
        Bounds bounds = area.bounds;
        Vector2 half = (Vector2)sprite.bounds.size * 0.5f * scale;

        float minX = bounds.min.x + half.x;
        float maxX = bounds.max.x - half.x;
        float minY = bounds.min.y + half.y;
        float maxY = bounds.max.y - half.y;

        // 팝업이 배경보다 크면 중앙에 배치.
        float x = minX <= maxX ? Random.Range(minX, maxX) : bounds.center.x;
        float y = minY <= maxY ? Random.Range(minY, maxY) : bounds.center.y;

        return new Vector3(x, y, area.transform.position.z);
    }
}
