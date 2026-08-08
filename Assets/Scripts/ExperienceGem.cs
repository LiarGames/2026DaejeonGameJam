using UnityEngine;

public class ExperienceGem : MonoBehaviour
{
    [Header("Temporary Visual")]
    [SerializeField] private SpriteRenderer gemRenderer;

    [SerializeField] private float attractionSpeed = 8f;
    [SerializeField] private float collectionDistance = 0.2f;

    private static Sprite temporarySquareSprite;
    private float experienceAmount;
    private PlayerStats target;
    private bool isCollected;

    private void Awake()
    {
        if (gemRenderer == null)
            gemRenderer = GetComponent<SpriteRenderer>();

        if (gemRenderer != null && gemRenderer.sprite == null)
            gemRenderer.sprite = GetTemporarySquareSprite();
    }

    public void Initialize(float experienceAmount)
    {
        this.experienceAmount = Mathf.Max(experienceAmount, 0f);
    }

    private void Update()
    {
        if (target == null || isCollected)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.transform.position,
            attractionSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.transform.position) <= collectionDistance)
            Collect();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (target != null || isCollected)
            return;

        PlayerStats playerStats = other.GetComponentInParent<PlayerStats>();
        if (playerStats != null)
            target = playerStats;
    }

    private void Collect()
    {
        if (isCollected || target == null)
            return;

        isCollected = true;
        target.GainExperience(experienceAmount);
        Destroy(gameObject);
    }

    private static Sprite GetTemporarySquareSprite()
    {
        if (temporarySquareSprite != null)
            return temporarySquareSprite;

        Texture2D texture = new Texture2D(1, 1);
        texture.name = "TemporaryExperienceGemTexture";
        texture.filterMode = FilterMode.Point;
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        temporarySquareSprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, 1f, 1f),
            new Vector2(0.5f, 0.5f),
            5f
        );
        temporarySquareSprite.name = "TemporaryExperienceGemSprite";

        return temporarySquareSprite;
    }
}
