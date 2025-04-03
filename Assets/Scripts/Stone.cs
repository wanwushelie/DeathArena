using UnityEngine;

public class Stone : MonoBehaviour
{
    public GameObject stonePrefab; // 石头掉落物预制体
    public int hitCount = 0;           // 被敲击次数
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
        Vector3 stonePosition = transform.position;  // 获取石头的位置

        // 实例化第一个石头预制体
        GameObject stone = Instantiate(stonePrefab, stonePosition, Quaternion.identity);
        Rigidbody2D rb = stone.GetComponent<Rigidbody2D>();  // 获取石头的刚体组件
        if (rb != null)
        {
            // 随机生成一个X轴和Y轴的力
            float randomX = Random.Range(-1f, 2f);
            float randomY = Random.Range(1f, -2f);
            rb.AddForce(new Vector2(randomX, randomY), ForceMode2D.Impulse);  // 给石头施加一个冲量力
        }

        // 实例化第二个石头预制体
        GameObject stone2 = Instantiate(stonePrefab, stonePosition, Quaternion.identity);
        Rigidbody2D rb2 = stone.GetComponent<Rigidbody2D>();  // 获取石头的刚体组件
        if (rb2 != null)
        {
            // 随机生成一个X轴和Y轴的力
            float randomX = Random.Range(-1f, 2f);
            float randomY = Random.Range(1f, -3f);
            rb2.AddForce(new Vector2(randomX, randomY), ForceMode2D.Impulse);  // 给石头施加一个冲量力
        }
        

        SoundManager.Instance.Play("挖石头掉落的音效");
        Destroy(gameObject, 0.3f);
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