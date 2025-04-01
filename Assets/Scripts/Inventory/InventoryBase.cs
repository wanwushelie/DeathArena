using System.Collections.Generic;
using UnityEngine;

public class InventoryBase : MonoBehaviour
{
    [SerializeField] protected List<Slot_UI> slots = new List<Slot_UI>();
    [SerializeField] protected Canvas canvas;
    protected Inventory inventory;
    public string inventoryName;

    protected virtual void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
    }

    protected virtual void Start()
    {
        inventory = Player.Instance.inventoryManager.GetInventoryByName(inventoryName);
        SetUpSlot();
    }

    protected virtual void Update()
    {
        Refresh();
    }

    // 刷新库存
    public void Refresh()
    {
        if (slots.Count == inventory.slots.Count)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (inventory.slots[i].itemName != "")
                {
                    slots[i].SetItem(inventory.slots[i]);
                }
                else
                {
                    slots[i].EmptyItem();
                }
            }
        }
    }

    public void DropItem(Item item, int itemCount)
    {
        Vector3 spawnLocation = Player.Instance.transform.position;
        Vector3 spawnOffset = Random.insideUnitCircle * 1.25f;

        Item droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);
        droppedItem.SetDroppedItemCount(itemCount);
        // droppedItem.rigid.AddForce(spawnOffset * 0.3f, ForceMode2D.Impulse);
    }

    public void Remove()
    {
        if (inventory == null || InGameUI.instance.draggedSlot == null) return;

        int slotID = InGameUI.instance.draggedSlot.slotID;
        var slotData = inventory.slots[slotID];

        if (string.IsNullOrEmpty(slotData.itemName)) return;

        Item itemToDrop = GameManager.instance.itemManager.GetItemByName(slotData.itemName);
        if (itemToDrop != null)
        {
            // 1. 保存掉落物品的数量并将其掉落
            // Player.Instance.DropItem(itemToDrop, slotData.currentCount);
            DropItem(itemToDrop, slotData.currentCount);
            // 2. 从库存中删除该槽位的物品
            inventory.Remove(slotID, true);
        }
        Refresh();
        InGameUI.instance.draggedSlot = null;
    }


    public void SlotBeginDrag(Slot_UI slot)
    {
        InGameUI.instance.draggedSlot = slot;
        InGameUI.instance.draggedIcon = Instantiate(slot.itemIcon);
        InGameUI.instance.draggedIcon.transform.SetParent(canvas.transform);
        InGameUI.instance.draggedIcon.raycastTarget = false;
        InGameUI.instance.draggedIcon.rectTransform.sizeDelta = new Vector2(100, 100);

        MoveToMousePosition(InGameUI.instance.draggedIcon.gameObject);
    }

    public void SlotDrag()
    {
        MoveToMousePosition(InGameUI.instance.draggedIcon.gameObject);
    }

    public void SlotEndDrag()
    {
        Destroy(InGameUI.instance.draggedIcon.gameObject);
        InGameUI.instance.draggedIcon = null;
    }

    public void SlotDrop(Slot_UI slot)
    {
        if (slot == null || InGameUI.instance.draggedSlot == null) return;

        var draggedSlot = InGameUI.instance.draggedSlot;
        if (InGameUI.instance.dragSingle)
        {
            draggedSlot.inventory.MoveSlot(draggedSlot.slotID, slot.slotID, slot.inventory);
        }
        else
        {
            draggedSlot.inventory.MoveSlot(draggedSlot.slotID, slot.slotID, slot.inventory, draggedSlot.inventory.slots[draggedSlot.slotID].currentCount);

        }

        InGameUI.instance.RefreshAllInventory();
    }

    public void MoveToMousePosition(GameObject toMove)
    {
        if (canvas != null)
        {
            Vector2 position;

            // 将屏幕上的鼠标位置转换为画布的本地坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out position);

            // 将转换后的本地坐标再次转换为世界坐标，并将游戏对象移动到该位置
            toMove.transform.position = canvas.transform.TransformPoint(position);
        }
    }

    protected void SetUpSlot()
    {
        int counter = 0;

        foreach (Slot_UI slot in slots)
        {
            slot.slotID = counter;
            counter++;
            slot.inventory = inventory;
        }
    }

}
