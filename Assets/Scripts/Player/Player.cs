using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    private Rigidbody2D rb;
    public Animator anim;

    public InventoryManager inventoryManager;
    public Tilemap houseRoofTileMap;
    public int money = 0;

    private TileManager tileManager;
    public Vector3Int targetPosition;

    private StateManager stateManager;
    private TreeChopping treeChopping;
    private TileInteraction tileInteraction;
    private PlayerMovement playerMovement;
    private MailboxInteraction mailboxInteraction;
    private MiningInteraction miningInteraction;
    private ItemPickup itemPickup;

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
        anim = GetComponent<Animator>();

        inventoryManager = GetComponent<InventoryManager>();
        tileManager = FindObjectOfType<TileManager>();

        // 初始化状态管理器
        stateManager = new StateManager();

        // 初始化行为类
        treeChopping = GetComponent<TreeChopping>();
        tileInteraction = GetComponent<TileInteraction>();
        playerMovement = GetComponent<PlayerMovement>();
        mailboxInteraction = GetComponent<MailboxInteraction>();
        miningInteraction = GetComponent<MiningInteraction>();
        itemPickup = GetComponent<ItemPickup>();

        treeChopping.Initialize(this, stateManager);
        tileInteraction.Initialize(this, tileManager, stateManager);
        playerMovement.Initialize(this, stateManager);
        mailboxInteraction.Initialize(this, stateManager);
        miningInteraction.Initialize(this, stateManager);
        itemPickup.Initialize(this, stateManager);
    }

    private void Update()
    {
        if (GameManager.instance.timeManager.isDayEnding)
            return;

        playerMovement.UpdateMovement();
        treeChopping.UpdateTreeChopping();
        tileInteraction.UpdateTileInteraction();
        mailboxInteraction.UpdateMailboxInteraction();
        miningInteraction.UpdateMiningInteraction();
        itemPickup.UpdateItemPickup();

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

            if (Vector2.Distance((Vector2)transform.position, (Vector2)worldMousePosition) < 2f)
            {
                FoodInteracted(); // 新增食物交互检测
                Debug.Log("点击到自己");
            }
        }

    }

    private void FixedUpdate()
    {
        playerMovement.FixedUpdateMovement();
    }

    // 恢复精力值的方法
    public void RecoverStamina(float amount)
    {
        stamina += amount;
        stamina = Mathf.Min(stamina, maxStamina); // 确保不超过最大值
    }

    // 消耗精力值的方法
    public void ConsumeStamina(float amount)
    {
        stamina -= amount;
        stamina = Mathf.Max(stamina, 0); // 确保不低于0
    }

    // 恢复血量的方法
    public void RecoverHealth(float amount)
    {
        health += amount;
        health = Mathf.Min(health, maxHealth); // 确保不超过最大值
    }

    // 恢复饱腹值的方法
    public void RecoverSatiation(float amount)
    {
        satiation += amount;
        satiation = Mathf.Min(satiation, maxSatiation); // 确保不超过最大值
    }

    // 游戏结束的方法
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

    public void EatFood()
    {
        if (inventoryManager.toolbar.selectedSlot == null)
            return;

        FoodData food = inventoryManager.toolbar.selectedSlot.foodData;
        if (food == null)
            return;

        // 恢复饱腹值和血量
        RecoverSatiation(food.satiationRecovery);
        RecoverHealth(food.healthRecovery);

        // 消耗食物
        inventoryManager.toolbar.selectedSlot.RemoveItem();

        if (inventoryManager.toolbar.selectedSlot.isEmpty)
        {
            inventoryManager.toolbar.selectedSlot = null;
        }

        // 播放食用动画和音效
        anim.SetTrigger("Eat");
        SoundManager.Instance.Play("EFFECT/Eat", SoundType.EFFECT);
    }    

    public void FoodInteracted()
    {       
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("DayEndCheckPoint"))
        {
            InGameUI.instance.dayEndPanel.SetActive(true);
        }
        else if (other.gameObject.CompareTag("HouseRoof"))
        {
            if (houseRoofTileMap != null)
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
            if (houseRoofTileMap != null)
                houseRoofTileMap.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    public void DropItem(Item item, int itemCount)
    {
        Vector3 spawnLocation = transform.position;
        Vector3 spawnOffset = Random.insideUnitCircle * 1.25f;

        Item droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);
        droppedItem.SetDroppedItemCount(itemCount);
        droppedItem.rigid.AddForce(spawnOffset * 0.3f, ForceMode2D.Impulse);
    }

    public void SetPosition()
    {
        transform.position = new Vector2(-0.5f, 0);
        stateManager.LastMoveDirection = new Vector2(0, -1);
        anim.enabled = true;
        anim.SetFloat("LastHorizontal", stateManager.LastMoveDirection.x);
        anim.SetFloat("LastVertical", stateManager.LastMoveDirection.y);
    }

    public bool IsAxing()
    {
        return stateManager.IsAxing;
    }
}