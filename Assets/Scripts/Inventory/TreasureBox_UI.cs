// TreasureBox_UI.cs
using UnityEngine;

public class TreasureBox_UI : InventoryBase
{
    public GameObject treasureBoxPanel;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        inventory = Player.Instance.inventoryManager.GetInventoryByName("TreasureBox");
        if (inventory == null) return;

        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}