using System.Collections;  // 引入System.Collections命名空间，用于使用集合相关的类和接口
using UnityEngine;  // 引入UnityEngine命名空间，用于使用Unity引擎的核心功能

public class SetAlpha : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;  // 声明SpriteRenderer类型的私有变量，用于渲染精灵
    private Color originalColor;  // 声明私有Color类型变量，用于存储树木的原始颜色
    private float fadedAlpha = 0.5f;  // 声明私有float类型变量，用于设置树木淡化后的透明度

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();  // 获取当前对象的SpriteRenderer组件
        originalColor = spriteRenderer.color; // 保存精灵渲染器的原始颜色
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        // 当玩家进入触发区域时，透明度设置为淡化后的透明度
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
