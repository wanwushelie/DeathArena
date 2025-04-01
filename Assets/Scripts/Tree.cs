using System.Collections;  // 引入System.Collections命名空间，用于使用集合相关的类和接口
using UnityEngine;  // 引入UnityEngine命名空间，用于使用Unity引擎的核心功能

public class Tree : MonoBehaviour
{
    private Animator anim;  // 声明Animator类型的私有变量，用于控制动画
    private SpriteRenderer spriteRenderer;  // 声明SpriteRenderer类型的私有变量，用于渲染精灵
    public Sprite newSprite;  // 声明公共的Sprite类型变量，用于存储新的精灵图
    public Transform fruitSpawnPos;  // 声明公共的Transform类型变量，用于指定水果生成的位置
    public Transform fallPos;  // 声明公共的Transform类型变量，用于指定水果掉落的目标位置
    public GameObject WoodPrefab;  // 声明公共的GameObject类型变量，用于存储木头预制体
    public GameObject fruitPrefab;  // 声明公共的GameObject类型变量，用于存储水果预制体
    public int hitCount;  // 声明公共的int类型变量，用于记录树木被击中的次数
    public bool isFruitTree;  // 声明公共的bool类型变量，用于判断是否为果树
    private bool isFruitDrop = false;  // 声明私有bool类型变量，用于标记水果是否已经掉落
    private float fruitOffset = 0.5f;  // 声明私有float类型变量，用于设置水果生成的偏移量
    private Color originalColor;  // 声明私有Color类型变量，用于存储树木的原始颜色
    private float fadedAlpha = 0.5f;  // 声明私有float类型变量，用于设置树木淡化后的透明度

    private void Start()
    {
        anim = GetComponent<Animator>();  // 获取当前对象的Animator组件
        spriteRenderer = GetComponent<SpriteRenderer>();  // 获取当前对象的SpriteRenderer组件
        originalColor = spriteRenderer.color; // 保存精灵渲染器的原始颜色
    }

    private void Update()
    {
        // 当树木被击中4次时，延迟0.3秒生成木头，并重置击中次数
        if (hitCount == 4)
        {
            Invoke("SpawnWood", 0.3f);  // 延迟0.3秒调用SpawnWood方法
            hitCount = 0;  // 重置击中次数
        }

        // 如果是果树，水果未掉落，击中次数为1且玩家未挥动斧头，则掉落水果并更换精灵图
        if (isFruitTree && !isFruitDrop && hitCount == 1 && !Player.Instance.IsAxing())
        {
            anim.enabled = false;  // 禁用动画组件
            ChangeSprite();  // 调用ChangeSprite方法更换精灵图
            DropFruit();  // 调用DropFruit方法掉落水果
        }
    }

    private void ChangeSprite()
    {
        // 检查精灵渲染器和新精灵图是否存在，如果存在则更换精灵图
        if (spriteRenderer != null && newSprite != null)
        {
            spriteRenderer.sprite = newSprite;  // 将精灵渲染器的精灵图更换为新的精灵图
        }
    }

    private void DropFruit()
    {
        SoundManager.Instance.Play("EFFECT/Fall", SoundType.EFFECT);  // 播放水果掉落音效

        isFruitDrop = true;  // 标记水果已经掉落

        // 以fruitOffset为间隔生成3个水果
        for (int i = 0; i < 3; i++)
        {
            // 计算水果的生成位置
            Vector3 spawnPosition = fruitSpawnPos.position + new Vector3(i * fruitOffset - fruitOffset, 0, 0);
            GameObject fruit = Instantiate(fruitPrefab, spawnPosition, Quaternion.identity);  // 实例化水果预制体
            // 启动协程，将水果移动到指定位置
            StartCoroutine(MoveFruitToPosition(fruit, fallPos.position + new Vector3(i * fruitOffset - fruitOffset, 0, 0), 1f));
        }
    }

    private IEnumerator MoveFruitToPosition(GameObject fruit, Vector2 targetPosition, float duration)
    {
        Vector3 startPosition = fruit.transform.position;  // 记录水果的起始位置
        float elapsedTime = 0f;  // 记录已经经过的时间

        Item interactable = fruit.GetComponent<Item>();  // 获取水果的Item组件
        if (interactable != null)
            interactable.canInteract = false;  // 禁用水果的交互功能

        // 在指定的时间内将水果从起始位置移动到目标位置
        while (elapsedTime < duration)
        {
            // 使用线性插值计算水果的当前位置
            fruit.transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;  // 更新已经经过的时间
            yield return null;  // 暂停协程，等待下一帧继续执行
        }

        fruit.transform.position = targetPosition;  // 将水果移动到目标位置
        interactable.canInteract = true;  // 启用水果的交互功能
    }

    private void SpawnWood()
    {
        anim.enabled = true;  // 启用动画组件

        Vector3 spawnPosition = transform.position;  // 获取树木的位置

        // 实例化第一个木头预制体
        GameObject wood = Instantiate(WoodPrefab, spawnPosition, Quaternion.identity);
        Rigidbody2D rb = wood.GetComponent<Rigidbody2D>();  // 获取木头的刚体组件
        if (rb != null)
        {
            // 随机生成一个X轴和Y轴的力
            float randomX = Random.Range(-1f, 2f);
            float randomY = Random.Range(1f, -2f);
            rb.AddForce(new Vector2(randomX, randomY), ForceMode2D.Impulse);  // 给木头施加一个冲量力
        }

        // 实例化第二个木头预制体
        GameObject wood2 = Instantiate(WoodPrefab, spawnPosition, Quaternion.identity);
        Rigidbody2D rb2 = wood2.GetComponent<Rigidbody2D>();  // 获取木头的刚体组件
        if (rb2 != null)
        {
            // 随机生成一个X轴和Y轴的力
            float randomX = Random.Range(-1f, 2f);
            float randomY = Random.Range(1f, -3f);
            rb2.AddForce(new Vector2(randomX, randomY), ForceMode2D.Impulse);  // 给木头施加一个冲量力
        }

        //SoundManager.Instance.Play("EFFECT/FallTree", SoundType.EFFECT);
        SoundManager.Instance.Play("树木倒下的音效");  // 播放树木倒下的音效
        anim.SetTrigger("isFalling");  // 设置动画触发器，触发树木倒下的动画

        Destroy(gameObject, 1f);  // 延迟1秒销毁树木对象
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 当玩家进入树木的触发区域时，将树木的透明度设置为淡化后的透明度
        if (other.CompareTag("Player"))
        {
            SetTreeAlpha(fadedAlpha);  // 调用SetTreeAlpha方法设置树木的透明度
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // 当玩家离开树木的触发区域时，将树木的透明度恢复为原始透明度
        if (other.CompareTag("Player"))
        {
            SetTreeAlpha(originalColor.a);  // 调用SetTreeAlpha方法设置树木的透明度
        }
    }

    private void SetTreeAlpha(float alpha)
    {
        Color color = spriteRenderer.color;  // 获取精灵渲染器的当前颜色
        color.a = alpha;  // 设置颜色的透明度
        spriteRenderer.color = color;  // 将修改后的颜色应用到精灵渲染器
    }

}
