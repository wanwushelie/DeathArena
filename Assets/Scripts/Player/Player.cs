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

        treeChopping.Initialize(this, stateManager);
        tileInteraction.Initialize(this, tileManager, stateManager);
        playerMovement.Initialize(this, stateManager);
        mailboxInteraction.Initialize(this, stateManager);
    }

    private void Update()
    {
        if (GameManager.instance.timeManager.isDayEnding)
            return;

        playerMovement.UpdateMovement();
        treeChopping.UpdateTreeChopping();
        tileInteraction.UpdateTileInteraction();
        mailboxInteraction.UpdateMailboxInteraction();
    }

    private void FixedUpdate()
    {
        playerMovement.FixedUpdateMovement();
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