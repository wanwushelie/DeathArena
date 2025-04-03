using UnityEngine;
using PolyNav;
using System.Collections.Generic;
using System.Collections;

public class NpcMove : MonoBehaviour
{
    [SerializeField] private CropManager cropManager;  // 农作物管理器引用
    [SerializeField] private TileManager tileManager; // 新增TileManager引用
    private PolyNavAgent polyNavAgent;
    private Animator anim;
    private SpriteRenderer sprite;
    private Vector2 direction;
    private float timer;
    private bool isWalking;
    private float idleTime = 3f;

    public string playerTag = "Player";
    public float runAwayTime = 3f;
    private bool isRunningAway;
    private Vector3Int currentTargetCell; // 当前目标单元格

    [Header("收割设置")]
    public float harvestDelay = 5f; // 收割前的等待时间
    
    private Coroutine harvestRoutine; // 当前收割协程
    private bool isHarvesting; // 是否正在收割

    private void Start()
    {
        anim = GetComponent<Animator>();
        polyNavAgent = GetComponent<PolyNavAgent>();
        sprite = GetComponent<SpriteRenderer>();

        // 确保引用存在
        if (cropManager == null) cropManager = FindObjectOfType<CropManager>();
        if (tileManager == null) tileManager = FindObjectOfType<TileManager>();

        // 订阅到达事件
        polyNavAgent.OnDestinationReached += OnReachedCrop;

        timer = idleTime;
        isWalking = false;
        isRunningAway = false;

        polyNavAgent.OnMovementUpdated += UpdateAnimationFromAgent;
        ChooseRandomCropTarget();
    }

    // 新增到达目标回调
    private void OnReachedCrop()
    {
        if (tileManager == null || isHarvesting) return;

        // 将primeGoal转换为单元格坐标
        Vector3Int cellPos = tileManager.seedMap.WorldToCell(polyNavAgent.primeGoal);
        // 启动收割协程
        harvestRoutine = StartCoroutine(HarvestCoroutine(cellPos));
        // // 移除种子瓦片
        // if (tileManager.seedMap.HasTile(cellPos))
        // {
        //     tileManager.seedMap.SetTile(cellPos, null);
        //     tileManager.SetTileState(cellPos, "Plowed"); // 重置地块状态
        // }

        // // 重新选择目标
        // ChooseRandomCropTarget();
    }

    // 收割协程
    private IEnumerator HarvestCoroutine(Vector3Int cellPos)
    {
        isHarvesting = true;
        polyNavAgent.Stop(); // 停止导航
        
        float timer = 0;
        Vector3 originalPosition = transform.position;

        // 等待期间持续检测位置变化
        while (timer < harvestDelay)
        {
            if (Vector3.Distance(transform.position, originalPosition) > 0.1f)
            {
                // 如果位置发生变动（被玩家赶走），取消收割
                isHarvesting = false;
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // 正式移除种子
        if (tileManager.seedMap.HasTile(cellPos))
        {
            tileManager.seedMap.SetTile(cellPos, null);
            tileManager.SetTileState(cellPos, "Plowed");
        }

        isHarvesting = false;
        ChooseRandomCropTarget(); // 重新选择目标
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (!isWalking && !isRunningAway)
        {
            if (timer <= 0)
            {
                StartWalkingToCrops();
            }
        }
        else if (isRunningAway)
        {
            if (timer <= 0)
            {
                isRunningAway = false;
                ChooseRandomCropTarget();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            RunAwayFromPlayer(other.transform.position);
        }
    }

    private void UpdateAnimationFromAgent(Vector2 direction, float speed)
    {
        anim.SetFloat("Horizontal", direction.x);
        anim.SetFloat("Vertical", direction.y);
        anim.SetFloat("Speed", speed);
        sprite.flipX = direction.x < 0;
    }

    private void StartWalkingToCrops()
    {
        isWalking = true;
        timer = polyNavAgent.maxSpeed / polyNavAgent.maxForce * 2;
        ChooseRandomCropTarget();
    }

    private void StopWalking()
    {
        isWalking = false;
        timer = idleTime;
        polyNavAgent.Stop();
        anim.SetBool("isWalking", false);
    }

    // 核心修改方法：只选择农作物位置
    private void ChooseRandomCropTarget()
    {
        if (cropManager == null || isHarvesting) return;

        List<Vector2> cropPositions = cropManager.GetPlantedWorldPositions();

        if (cropPositions.Count > 0)
        {
            Vector2 targetPos = cropPositions[Random.Range(0, cropPositions.Count)];
            polyNavAgent.SetDestination(targetPos);

            // 记录当前目标单元格
            currentTargetCell = tileManager.seedMap.WorldToCell(targetPos);
        }
        else
        {
            Debug.Log("No crops available, NPC will stay idle");
            StopWalking();
        }
    }

    private void RunAwayFromPlayer(Vector3 playerPosition)
    {
        // 如果正在收割，立即中断
        if (isHarvesting)
        {
            if (harvestRoutine != null)
                StopCoroutine(harvestRoutine);
            isHarvesting = false;
        }
        isRunningAway = true;
        timer = runAwayTime;

        direction = (transform.position - playerPosition).normalized;
        polyNavAgent.SetDestination(transform.position + (Vector3)direction * 10f);
    }

    // 在OnDestroy中取消订阅
    private void OnDestroy()
    {
        if (harvestRoutine != null)
            StopCoroutine(harvestRoutine);
        polyNavAgent.OnDestinationReached -= OnReachedCrop;
    }
}