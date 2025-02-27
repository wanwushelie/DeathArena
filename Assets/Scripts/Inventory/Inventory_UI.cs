using UnityEngine;

/* 负责单个库存的UI显示 */

public class Inventory_UI : InventoryBase
{
    public GameObject inventoryPanel;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        inventory = Player.Instance.inventoryManager.GetInventoryByName(inventoryName);
        if (inventory == null) return;

        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}
