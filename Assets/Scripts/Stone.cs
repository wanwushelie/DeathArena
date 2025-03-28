using UnityEngine;

public class Stone : MonoBehaviour
{
    public GameObject stonePrefab; // 石头掉落物预制体
    public Transform dropPos;      // 掉落位置
    public int hitCount;           // 被敲击次数
    private Color originalColor;
    private float fadedAlpha = 0.5f;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        // 被敲击3次后掉落石头
        if (hitCount == 3)
        {
            DropStone();
            hitCount = 0;
        }
    }

    private void DropStone()
    {
        SoundManager.Instance.Play("挖石头掉落的音效");
        Instantiate(stonePrefab, dropPos.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetStoneAlpha(fadedAlpha);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetStoneAlpha(originalColor.a);
        }
    }

    private void SetStoneAlpha(float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
}