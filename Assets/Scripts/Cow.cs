using UnityEngine;

// 定义 Cow 类，继承自 MonoBehaviour
public class Cow : MonoBehaviour
{
    private Rigidbody2D rigid; // 定义刚体组件
    private Animator anim; // 定义动画组件
    private SpriteRenderer sprite; // 定义精灵渲染器组件
    private Vector2 direction; // 定义移动方向
    private float timer; // 定义计时器
    private bool isWalking; // 定义是否正在行走的标志
    private float walkSpeed = 1.2f; // 定义行走速度
    private float walkTime = 3f; // 定义行走时间
    private float idleTime = 3f; // 定义闲置时间

    // 初始化方法，在对象启用时调用
    private void Start()
    {
        anim = GetComponent<Animator>(); // 获取动画组件
        rigid = GetComponent<Rigidbody2D>(); // 获取刚体组件
        sprite = GetComponent<SpriteRenderer>(); // 获取精灵渲染器组件

        timer = walkTime; // 初始化计时器为行走时间
        isWalking = true; // 初始状态为行走
        ChooseRandomDirection(); // 选择随机行走方向
    }

    // 每帧更新方法
    private void Update()
    {
        timer -= Time.deltaTime; // 计时器递减

        if (isWalking) // 如果正在行走
        {
            MoveCow(); // 移动奶牛

            if (timer <= 0) // 如果行走时间结束
            {
                StopWalking(); // 停止行走
            }
        }
        else // 如果处于闲置状态
        {
            // 闲置状态下时间结束则开始行走
            if (timer <= 0) 
            {
                StartWalking(); // 开始行走
            }
        }
    }

    // 碰撞检测方法，当发生碰撞时调用
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Cow") || other.gameObject.CompareTag("Fence")) // 如果碰撞对象是奶牛或围栏
        {    
            StopWalking(); // 停止行走
            Invoke("ChooseRandomDirection", idleTime); // 闲置时间后选择随机方向
        }
    }

    // 移动奶牛的方法
    private void MoveCow()
    {
        rigid.velocity = direction * walkSpeed; // 设置刚体速度
        anim.SetBool("isWalking", true); // 设置动画状态为行走

        // 根据移动方向翻转精灵
        sprite.flipX = direction.x < 0; 
    }

    // 开始行走的方法
    private void StartWalking()
    {
        isWalking = true; // 设置行走标志为真
        timer = walkTime; // 重置计时器为行走时间
        ChooseRandomDirection(); // 选择随机行走方向
    }

    // 停止行走的方法
    private void StopWalking()
    {
        rigid.velocity = Vector2.zero; // 重置刚体速度为零
        isWalking = false; // 设置行走标志为假
        timer = idleTime; // 重置计时器为闲置时间
        anim.SetBool("isWalking", false); // 设置动画状态为闲置
    }

    // 选择随机方向的方法
    private void ChooseRandomDirection()
    {
        // 随机选择一个方向并归一化
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized; 

        // 根据方向翻转精灵
        sprite.flipX = direction.x < 0; 
    }
}
