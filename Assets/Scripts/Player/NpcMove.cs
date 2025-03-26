using UnityEngine;
using PolyNav;  // 确保包含 PolyNav 命名空间引用
using System.Collections.Generic;

public class NpcMove : MonoBehaviour
{
    public List<string> targetTags;  // 定义目标物体标签列表，公开变量
    private PolyNavAgent polyNavAgent;  // 定义 PolyNavAgent 组件
    private Animator anim;  // 定义动画组件
    private SpriteRenderer sprite;  // 定义精灵渲染器组件
    private Vector2 direction;  // 定义移动方向
    private float timer;  // 定义计时器
    private bool isWalking;  // 定义是否正在行走的标志
    private float idleTime = 3f;  // 定义闲置时间

    public string playerTag = "Player";  // 玩家标签
    public float runAwayTime = 3f;  // 逃跑时间
    private bool isRunningAway;  // 是否正在逃跑

    // 初始化方法，在对象启用时调用
    private void Start()
    {
        anim = GetComponent<Animator>();  // 获取动画组件
        polyNavAgent = GetComponent<PolyNavAgent>();  // 获取 PolyNavAgent 组件
        sprite = GetComponent<SpriteRenderer>();  // 获取精灵渲染器组件

        timer = idleTime;  // 初始化计时器为闲置时间
        isWalking = false;  // 初始状态为闲置
        isRunningAway = false;  // 初始状态为未逃跑

        // 订阅 PolyNavAgent 的移动更新事件
        polyNavAgent.OnMovementUpdated += UpdateAnimationFromAgent;

        // 选择随机目标位置或进行随机移动
        ChooseRandomTargetOrMove();
    }

    // 每帧更新方法
    private void Update()
    {
        timer -= Time.deltaTime;  // 计时器递减

        if (!isWalking && !isRunningAway)  // 如果处于闲置状态且未逃跑
        {
            // 闲置状态下时间结束则开始行走或随机移动
            if (timer <= 0) 
            {
                StartWalkingOrMove();  // 开始行走或随机移动
            }
        }
        else if (isRunningAway)  // 如果正在逃跑
        {
            if (timer <= 0)  // 逃跑时间结束
            {
                isRunningAway = false;  // 设置逃跑标志为假
                ChooseRandomTargetOrMove();  // 选择随机目标物体或进行随机移动
            }
        }
    }

    // 碰撞检测方法，当发生碰撞时调用
    private void OnCollisionEnter2D(Collision2D other)
    {
        // 遍历目标标签列表
        foreach (string tag in targetTags)
        {
            if (other.gameObject.CompareTag(tag))  // 如果碰撞对象是目标物体
            {
                StopWalking();  // 停止行走
                Invoke("ChooseRandomTargetOrMove", idleTime);  // 闲置时间后选择随机目标物体或进行随机移动
                break;
            }
        }

        if (other.gameObject.CompareTag(playerTag))  // 如果碰撞对象是玩家
        {
            RunAwayFromPlayer(other.transform.position);  // 从玩家位置逃跑
        }
    }

    // 更新动画状态机的方法，根据 PolyNavAgent 的移动信息更新动画
    private void UpdateAnimationFromAgent(Vector2 direction, float speed)
    {
        anim.SetFloat("Horizontal", direction.x);
        anim.SetFloat("Vertical", direction.y);
        anim.SetFloat("Speed", speed);

        // 根据移动方向翻转精灵
        sprite.flipX = direction.x < 0;
    }

    // 开始行走或随机移动的方法
    private void StartWalkingOrMove()
    {
        isWalking = true;  // 设置行走标志为真
        timer = polyNavAgent.maxSpeed / polyNavAgent.maxForce * 2;  // 根据速度和加速度计算行走时间
        ChooseRandomTargetOrMove();  // 选择随机目标物体或进行随机移动
    }

    // 停止行走的方法
    private void StopWalking()
    {
        isWalking = false;  // 设置行走标志为假
        timer = idleTime;  // 重置计时器为闲置时间
        polyNavAgent.Stop();  // 停止 PolyNavAgent
        anim.SetBool("isWalking", false);  // 设置动画状态为闲置
    }

    // 选择随机目标物体或进行随机移动的方法
    private void ChooseRandomTargetOrMove()
    {
        bool targetFound = false;

        // 遍历目标标签列表
        foreach (string tag in targetTags)
        {
            // 获取场景中所有目标物体
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);

            if (targets.Length > 0)
            {
                // 随机选择一个目标物体
                GameObject target = targets[Random.Range(0, targets.Length)];

                // 设置 PolyNavAgent 的目标位置为目标物体的位置
                polyNavAgent.SetDestination(target.transform.position);
                targetFound = true;
                break;
            }
        }

        // 如果没有找到目标物体，则进行随机移动
        if (!targetFound)
        {
            // 随机选择一个方向并归一化
            direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

            // 将方向转换为 Vector3，并设置 PolyNavAgent 的目标位置为随机方向的位置
            polyNavAgent.SetDestination(transform.position + (Vector3)direction * 10f);  // 假设移动距离为10个单位
        }
    }

    // 从玩家位置逃跑的方法
    private void RunAwayFromPlayer(Vector3 playerPosition)
    {
        isRunningAway = true;  // 设置逃跑标志为真
        timer = runAwayTime;  // 重置计时器为逃跑时间

        // 计算与玩家相反的方向并归一化
        direction = (transform.position - playerPosition).normalized;

        // 设置 PolyNavAgent 的目标位置为与玩家相反的方向的位置
        polyNavAgent.SetDestination(transform.position + (Vector3)direction * 10f);  // 假设逃跑距离为10个单位
    }

}