using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;

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

    private TileManager tileManager;
    private Vector3Int targetPosition;
    private Vector3Int[] validTiles;
    private bool isValidTile = false;
    private bool isHoeing = false;
    private bool isWatering = false;
    private bool isAxing = false;
    private bool isMoving = false;
    public bool isPicking = false;
    public bool isHarvesting = false;
    public bool isMining = false; //是否在开采石块

    public float stamina = 100f; // 当前精力值
    public float maxStamina = 100f; // 最大精力值
    public float staminaRecoveryRate = 5f; // 精力恢复速率（每秒恢复的量）
    public float health = 100f; // 当前血量
    public float maxHealth = 100f; // 最大血量
    public float satiation = 100f; // 当前饱腹值
    public float maxSatiation = 100f; // 最大饱腹值
    public float satiationDecreaseRate = 1f; // 饱腹值减少速率（每秒减少的量）




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
        UpdateAnimation();//获得输入，更新动画
        PlantInteracted();// 处理植物交互逻辑，包括锄头开垦、播种、浇水和收获等操作。
        Hit();// 处理玩家与树的交互逻辑，包括斧头砍树和树的生长等操作。
        Hit1();// 处理玩家与石块的交互逻辑，包括斧头开采石块和石块的生长等操作。
        
        // 随时间恢复精力值
        if (stamina < maxStamina)
        {
            stamina += staminaRecoveryRate * Time.deltaTime;
            stamina = Mathf.Min(stamina, maxStamina); // 确保不超过最大值
        }

        // 随时间减少饱腹值
        satiation -= satiationDecreaseRate * Time.deltaTime;
        if (satiation <= 0)
        {
            // 当饱腹值为0时开始减少血量
            health -= satiationDecreaseRate * Time.deltaTime;
            if (health <= 0)
            {
                // 当血量为0时游戏结束
                GameOver();
            }
        }

        // 确保血量和饱腹值不低于0
        satiation = Mathf.Max(satiation, 0);
        health = Mathf.Max(health, 0);

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldMousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(mousePosition);
            worldMousePosition.z = 0;

            if (Vector2.Distance((Vector2)transform.position, (Vector2)worldMousePosition) < 5f)
            {
                FoodInteracted(); // 新增食物交互检测
                Debug.Log("点击到自己");
            }
        }

    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.timeManager.isDayEnding && !isHoeing && !isWatering && !isAxing && !isPicking && !isHarvesting && !isMining)
            Move();
    }

    public void EatFood()
    {
        FoodData food = inventoryManager.toolbar.selectedSlot.foodData;
        inventoryManager.toolbar.selectedSlot.RemoveItem();  // 减少种子数量
        // 恢复饱腹值和血量
        satiation = Mathf.Min(satiation + food.satiationRecovery, maxSatiation);
        health = Mathf.Min(health + food.healthRecovery, maxHealth);

        if (inventoryManager.toolbar.selectedSlot.isEmpty)
        {
            inventoryManager.toolbar.selectedSlot = null;
        }

        // 播放食用动画和音效
        // anim.SetTrigger("Eat");
        // SoundManager.Instance.Play("EFFECT/Eat", SoundType.EFFECT);
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
    public void FoodInteracted()
    {
        // 如果正在其他交互状态，不进行后续操作
        if (isHoeing || isWatering || isAxing || isPicking || isHarvesting || isMining) return;
        
        // 检查工具栏是否有选中物品
        if (inventoryManager == null || 
            inventoryManager.toolbar == null || 
            inventoryManager.toolbar.selectedSlot == null)
        {
            return;
        }
        if (inventoryManager.toolbar.selectedSlot.itemName == "椰子")
        {
            EatFood(); // 执行食用逻辑
        }
    }

    private void GameOver()
    {
        // 清空游戏数据并返回主菜单
        // SaveData.instance.DeleteSavedFiles();
        // GameManager.instance.timeManager.isDayEnding = true;
        // 跳转到主菜单场景
        // SceneManager.LoadScene("MainMenu");
        satiation = 50f;
        health = 50f;

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

        // Picking 动画
        if (isPicking)
        {
            anim.SetTrigger("isPicking");
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
        else if (isHarvesting)
        {
            yield return new WaitForSeconds(0.2f);
            isHarvesting = false;
        }
        else if (isMining)
        {
            yield return new WaitForSeconds(0.2f);
            isMining = false;
        }
    }

    public IEnumerator WaitForPickingAnimation()
    {
        if (isPicking)
        {
            yield return new WaitForSeconds(0.2f); // 根据实际动画时长调整
            isPicking = false;
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
                // 检查精力值是否足够
                if (stamina >= 10f) // 假设砍树消耗10点精力
                {
                    //SoundManager.Instance.Play("EFFECT/HITTREE", SoundType.EFFECT);
                    SoundManager.Instance.Play("砍树音效");
                    isAxing = true;
                    anim.SetTrigger("isAxing");
                    tree.hitCount++;
                    ConsumeStamina(5f); // 消耗精力值
                    StartCoroutine(WaitForAnimation());
                }
                else
                {
                    Debug.Log("精力不足，无法砍树！");
                    // 可以在这里添加UI提示，告知玩家精力不足
                }
            }
        }
    }

    private void Hit1()
    {
        rayHit = Physics2D.Raycast(rb.position, lastMoveDirection, 1f, LayerMask.GetMask("Stone"));
        if (rayHit.collider != null)
        {  
            Stone stone = rayHit.collider.GetComponent<Stone>();
            if (Input.GetMouseButtonDown(0))
            {
                // 检查精力值是否足够
                if (stamina >= 10f) // 假设砍树消耗10点精力
                {
                    //SoundManager.Instance.Play("EFFECT/HITTREE", SoundType.EFFECT);
                    SoundManager.Instance.Play("开采石块音效");
                    isMining = true;
                    anim.SetTrigger("isHoeing");//用锄地的动画
                    stone.hitCount++;
                    Debug.Log(stone.hitCount);
                    ConsumeStamina(5f); // 消耗精力值
                    StartCoroutine(WaitForAnimation());
                }
                else
                {
                    Debug.Log("精力不足，无法砍树！");
                    // 可以在这里添加UI提示，告知玩家精力不足
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
                            // 检查精力值是否足够
                            if (stamina >= 5f) // 假设锄地消耗8点精力
                            {
                                isHoeing = true; 
                                anim.SetTrigger("isHoeing"); 
                            }
                            else
                            {
                                Debug.Log("精力不足，无法锄地！");
                                // 可以在这里添加UI提示，告知玩家精力不足
                                return;
                            }
                        }

                        // 植物成熟的情况
                        if (tileState == "Grown")
                        {
                            // 检查精力值是否足够
                            if (stamina >= 5f) // 假设收获消耗10点精力
                            {
                                tileManager.RemoveTile(targetPosition); 
                                GameManager.instance.plantGrowthManager.HarvestPlant(targetPosition); 
                                ConsumeStamina(5f); // 消耗精力值
                                isHarvesting = true;
                                anim.SetTrigger("isHarvesting"); // 设置收获动画触发器
                            }
                            else
                            {
                                Debug.Log("精力不足，无法收获！");
                                // 可以在这里添加UI提示，告知玩家精力不足
                                return;
                            }
                        }
                    }

                    // 开垦土地后
                    if (tileName == "PlowedTile")
                    {
                        // 如果选中的物品是水稻种子或番茄种子
                        if (inventoryManager.toolbar.selectedSlot.itemName == "RiceSeed" || inventoryManager.toolbar.selectedSlot.itemName == "TomatoSeed" || inventoryManager.toolbar.selectedSlot.itemName == "PlantSeed")
                        {
                            // 检查精力值是否足够
                            if (stamina >= 5f) // 假设播种消耗5点精力
                            {
                                Sowing(); 
                                ConsumeStamina(5f); // 消耗精力值
                            }
                            else
                            {
                                Debug.Log("精力不足，无法播种！");
                                // 可以在这里添加UI提示，告知玩家精力不足
                                return;
                            }
                        }
                        // 如果选中的物品是浇水工具
                        else if (inventoryManager.toolbar.selectedSlot.itemName == "Watering")
                        {
                            // 检查精力值是否足够
                            if (stamina >= 5f) // 假设浇水消耗5点精力
                            {
                                isWatering = true; 
                                anim.SetTrigger("isWatering"); 
                            }
                            else
                            {
                                Debug.Log("精力不足，无法浇水！");
                                // 可以在这里添加UI提示，告知玩家精力不足
                                return;
                            }
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
        ConsumeStamina(8f); // 假设锄地消耗8点精力
        StartCoroutine(WaitForAnimation());
    }

    private void Watering()
    {
        // SoundManager.Instance.Play("EFFECT/Watering", SoundType.EFFECT, 1, 1);
        SoundManager.Instance.Play("浇水音效");

        tileManager.WaterTile(targetPosition);
        ConsumeStamina(5f); // 假设浇水消耗5点精力
        StartCoroutine(WaitForAnimation());
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

    public void ConsumeStamina(float amount)
    {
        stamina -= amount;
        stamina = Mathf.Max(stamina, 0); // 确保精力值不低于0
    }

}