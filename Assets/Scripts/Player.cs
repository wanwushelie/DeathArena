using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    private Rigidbody2D rb;
    public Animator anim;
    private RaycastHit2D rayHit;
    private Vector2 movement;
    private Vector2 lastMoveDirection;
    private float moveSpeed = 2.5f;

    public InventoryManager inventoryManager;
    public Tilemap houseRoofTileMap;
    public int money = 0;
    private bool isPlayerInPostBox = false;

    private TileManager tileManager;
    private Vector3Int targetPosition;
    private Vector3Int[] validTiles;
    private bool isValidTile = false;
    private bool isHoeing = false;
    private bool isWatering = false;
    private bool isAxing = false;
    private bool isMoving = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); ;

        inventoryManager = GetComponent<InventoryManager>();
        tileManager = FindObjectOfType<TileManager>();
    }

    private void Update()
    {
        if (GameManager.instance.timeManager.isDayEnding)
            return;

        GetInput();
        UpdateAnimation();
        PlantInteracted();
        Hit();
        HandlePostBoxInteraction();

    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.timeManager.isDayEnding && !isHoeing && !isWatering && !isAxing)
            Move();
    }

    private void Move()
    {
        isMoving = true;
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void GetInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // 对角线移动时保持速度一致
        if (movement.magnitude > 1)
        {
            movement = movement.normalized;
        }

        // 保存最后移动方向
        if (movement != Vector2.zero)
        {
            lastMoveDirection = movement;
        }
    }

    private void UpdateAnimation()
    {
        // 行走动画
        anim.SetFloat("Horizontal", movement.x);
        anim.SetFloat("Vertical", movement.y);
        anim.SetFloat("Speed", movement.sqrMagnitude);

        // 闲置动画
        if (movement.sqrMagnitude == 0)
        {
            anim.SetFloat("LastHorizontal", lastMoveDirection.x);
            anim.SetFloat("LastVertical", lastMoveDirection.y);

            isMoving = false;
            MouseSelect mouseSelect = GetComponentInChildren<MouseSelect>();
            mouseSelect.SetSpriteColor(new Color(0, 0, 0, 0));
        }
    }

    private IEnumerator WaitForAnimation()
    {
        if (isHoeing)
        {
            yield return new WaitForSeconds(0.2f);
            isHoeing = false;
        }
        else if (isWatering)
        {
            yield return new WaitForSeconds(0.2f);
            isWatering = false;
        }
        else if (isAxing)
        {
            yield return new WaitForSeconds(0.7f);
            isAxing = false;
        }
    }

    private void HandlePostBoxInteraction()
    {
        if (isPlayerInPostBox)
        {
            if (InGameUI.instance.speechBubble.activeSelf)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    // SoundManager.Instance.Play("EFFECT/Pick", SoundType.EFFECT);
                    SoundManager.Instance.Play("拾取音效");
                    InGameUI.instance.ShowPostPanel();
                }
            }
        }
    }

    private void Hit()
    {
        rayHit = Physics2D.Raycast(rb.position, lastMoveDirection, 1f, LayerMask.GetMask("Tree"));
        if (rayHit.collider != null)
        {
            Tree tree = rayHit.collider.GetComponent<Tree>();
            if (Input.GetMouseButtonDown(0))
            {
                if (inventoryManager.toolbar.selectedSlot.itemName == "Axe")
                {
                    //SoundManager.Instance.Play("EFFECT/HITTREE", SoundType.EFFECT);
                    SoundManager.Instance.Play("砍树音效");
                    isAxing = true;
                    anim.SetTrigger("isAxing");
                    tree.hitCount++;
                    StartCoroutine(WaitForAnimation());
                }
            }
        }
    }

    private void CheckValidTiles()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldMousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(mousePosition);
        worldMousePosition.z = 0;

        targetPosition = new Vector3Int(Mathf.FloorToInt(worldMousePosition.x), Mathf.FloorToInt(worldMousePosition.y), 0);

        // 将玩家的世界坐标转换为网格坐标
        Vector3 playerPosition = transform.position;
        Vector3Int gridPlayerPosition = new Vector3Int(Mathf.FloorToInt(playerPosition.x), Mathf.FloorToInt(playerPosition.y), 0);

        validTiles = new Vector3Int[]
        {
            gridPlayerPosition,
            gridPlayerPosition + new Vector3Int(0,1,0),
            gridPlayerPosition + new Vector3Int(0,-1,0),
            gridPlayerPosition + new Vector3Int(1,0,0),
            gridPlayerPosition + new Vector3Int(-1,0,0),
        };

        isValidTile = false;

        foreach (var tile in validTiles)
        {
            if (tile == targetPosition)
            {
                isValidTile = true;
                break;
            }
        }

    }
    /// <summary>
    /// 处理植物交互逻辑，包括锄头开垦、播种、浇水和收获等操作。
    /// </summary>
    public void PlantInteracted()
    {
        // 如果正在锄地或浇水，不进行后续操作
        if (isHoeing || isWatering) return; 
        // 如果TileManager未初始化，不进行后续操作
        if (tileManager == null) return; 
        // 如果InventoryManager、Toolbar或选中的插槽未初始化，不进行后续操作
        if (inventoryManager == null || inventoryManager.toolbar == null || inventoryManager.toolbar.selectedSlot == null) return; 
        // 如果选中的插槽物品名称为空，不进行后续操作
        if (inventoryManager.toolbar.selectedSlot.itemName == null) return; 

        // 检查鼠标点击的位置是否为有效瓷砖
        CheckValidTiles(); 

        // 如果是有效瓷砖且玩家没有移动
        if (isValidTile && !isMoving)
        {
            // 获取子对象中的MouseSelect组件
            MouseSelect mouseSelect = GetComponentInChildren<MouseSelect>(); 
            if (mouseSelect != null)
            {
                // 设置鼠标选择的目标位置
                mouseSelect.SetTargetPosition(targetPosition); 
            }

            // 当鼠标左键按下时
            if (Input.GetMouseButtonDown(0))
            {
                // 根据目标位置设置动画方向
                SetAnimationDirection(targetPosition); 

                // 获取目标位置的瓷砖名称
                string tileName = tileManager.GetTileName(targetPosition); 
                // 获取目标位置的瓷砖状态
                string tileState = tileManager.GetTileState(targetPosition); 

                // 如果瓷砖名称不为空
                if (tileName != null)
                {
                    // 如果选中的物品是锄头
                    if (inventoryManager.toolbar.selectedSlot.itemName == "Hoe")
                    {
                        // 开垦新土地
                        if (tileName == "InteractableTile")
                        {
                            // 设置锄地状态为真
                            isHoeing = true; 
                            // 触发锄地动画
                            anim.SetTrigger("isHoeing"); 
                        }

                        // 植物成熟的情况
                        if (tileState == "Grown")
                        {
                            // 移除目标位置的瓷砖
                            tileManager.RemoveTile(targetPosition); 
                            // 收获目标位置的植物
                            GameManager.instance.plantGrowthManager.HarvestPlant(targetPosition); 
                        }
                    }

                    // 开垦土地后
                    if (tileName == "PlowedTile")
                    {
                        // 如果选中的物品是水稻种子或番茄种子
                        if (inventoryManager.toolbar.selectedSlot.itemName == "RiceSeed" || inventoryManager.toolbar.selectedSlot.itemName == "TomatoSeed" || inventoryManager.toolbar.selectedSlot.itemName == "PlantSeed")
                        {
                            // 进行播种操作
                            Sowing(); 
                        }
                        // 如果选中的物品是浇水工具
                        else if (inventoryManager.toolbar.selectedSlot.itemName == "Watering")
                        {
                            // 设置浇水状态为真
                            isWatering = true; 
                            // 触发浇水动画
                            anim.SetTrigger("isWatering"); 
                        }
                    }
                }
            }
        }
    }

    private void SetAnimationDirection(Vector3 targetPosition)
    {
        Vector3 playerPosition = transform.position;
        Vector3Int gridPlayerPosition = new Vector3Int(Mathf.FloorToInt(playerPosition.x), Mathf.FloorToInt(playerPosition.y), 0);

        // 比较当前位置和目标位置来计算方向
        Vector3 direction = (targetPosition - gridPlayerPosition).normalized;

        // 将方向的x、y值设置为动画参数
        anim.SetFloat("Horizontal", direction.x);
        anim.SetFloat("Vertical", direction.y);

        // 保存上一次移动方向
        lastMoveDirection = direction;
    }

    private void Hoeing()
    {
        // SoundManager.Instance.Play("EFFECT/Plow", SoundType.EFFECT);
        SoundManager.Instance.Play("锄地音效");
        tileManager.SetInteracted(targetPosition);
        StartCoroutine(WaitForAnimation());
    }

    private void Watering()
    {
        // SoundManager.Instance.Play("EFFECT/Watering", SoundType.EFFECT, 1, 1);
        SoundManager.Instance.Play("浇水音效");

        tileManager.WaterTile(targetPosition);
        StartCoroutine(WaitForAnimation());
    }

    private void Sowing()
    {
        PlantData plantData = inventoryManager.toolbar.selectedSlot.plantData;
        inventoryManager.toolbar.selectedSlot.RemoveItem();  // 减少种子数量
        GameManager.instance.plantGrowthManager.PlantSeed(targetPosition, plantData);  // 播种

        // 如果种子用完，清空插槽
        if (inventoryManager.toolbar.selectedSlot.isEmpty)
        {
            inventoryManager.toolbar.selectedSlot = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("DayEndCheckPoint"))
        {
            InGameUI.instance.dayEndPanel.SetActive(true);
        }
        else if (other.gameObject.CompareTag("HouseRoof"))
        {
            if(houseRoofTileMap != null)
                houseRoofTileMap.color = new Color(1f, 1f, 1f, 0f);
        }
        else if (other.gameObject.CompareTag("PostBox"))
        {
            isPlayerInPostBox = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("DayEndCheckPoint"))
        {
            InGameUI.instance.dayEndPanel.SetActive(false);
        }
        else if (other.gameObject.CompareTag("HouseRoof"))
        {
            if(houseRoofTileMap != null)
                houseRoofTileMap.color = new Color(1f, 1f, 1f, 1f);
        }
        else if (other.gameObject.CompareTag("PostBox"))
        {
            isPlayerInPostBox = false;
        }
    }

    public void DropItem(Item item, int itemCount)
    {
        Debug.Log(item.name);
        Vector3 spawnLocation = transform.position;

        Vector3 spawnOffset = Random.insideUnitCircle * 1.25f;

        // 1. 创建掉落的物品对象
        Item droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);

        // 2. 设置掉落物品的数量
        droppedItem.SetDroppedItemCount(itemCount);

        droppedItem.rigid.AddForce(spawnOffset * 0.3f, ForceMode2D.Impulse);
    }

    public void SetPosition()
    {
        transform.position = new Vector2(-0.5f, 0);
        lastMoveDirection = new Vector2(0, -1);

        anim.enabled = true;
        // 更新动画的闲置方向
        anim.SetFloat("LastHorizontal", lastMoveDirection.x);
        anim.SetFloat("LastVertical", lastMoveDirection.y);
    }

    public bool IsAxing()
    {
        return isAxing;
    }
}