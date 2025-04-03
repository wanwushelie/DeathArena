using UnityEngine;
using PolyNav;
using System.Collections.Generic;

public class NpcMove : MonoBehaviour
{
    [SerializeField] private CropManager cropManager;  // 农作物管理器引用
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

    private void Start()
    {
        anim = GetComponent<Animator>();
        polyNavAgent = GetComponent<PolyNavAgent>();
        sprite = GetComponent<SpriteRenderer>();

        // 确保CropManager引用存在
        if (cropManager == null)
        {
            cropManager = FindObjectOfType<CropManager>();
            if (cropManager == null)
            {
                Debug.LogError("CropManager not found in scene!");
            }
        }

        timer = idleTime;
        isWalking = false;
        isRunningAway = false;

        polyNavAgent.OnMovementUpdated += UpdateAnimationFromAgent;
        ChooseRandomCropTarget();
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
        if (cropManager == null) return;

        List<Vector2> cropPositions = cropManager.GetPlantedWorldPositions();
        
        if (cropPositions.Count > 0)
        {
            Vector2 targetPos = cropPositions[Random.Range(0, cropPositions.Count)];
            polyNavAgent.SetDestination(targetPos);
        }
        else
        {
            Debug.Log("No crops available, NPC will stay idle");
            StopWalking();
        }
    }

    private void RunAwayFromPlayer(Vector3 playerPosition)
    {
        isRunningAway = true;
        timer = runAwayTime;

        direction = (transform.position - playerPosition).normalized;
        polyNavAgent.SetDestination(transform.position + (Vector3)direction * 10f);
    }
}