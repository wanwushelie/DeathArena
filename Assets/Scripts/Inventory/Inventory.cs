using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Inventory
{
    [System.Serializable]
    public class Slot
    {
        public string itemName; // 物品名称
        public Sprite icon; // 物品图标
        public int currentCount; // 当前数量
        public int maxAllowed; // 最大允许数量
        public int price; // 价格
        public bool isSellable; // 是否可出售
        public Item item; // 物品对象
        public PlantData plantData; // 植物数据（如果适用）

        public Slot()
        {
            itemName = "";
            currentCount = 0;
            maxAllowed = 99;
            price = 0;
            isSellable = false;
            plantData = null;
            item = null;
        }

        // 检查槽位是否为空
        public bool isEmpty
        {
            get
            {
                return string.IsNullOrEmpty(itemName) && currentCount == 0;
            }
        }

        // 检查是否可以添加该物品
        public bool CanAddItem(string itemName)
        {
            return this.itemName == itemName && currentCount < maxAllowed;
        }

        // 向槽位中添加物品
        public void AddItem(Item item, int count = 1)
        {
            this.itemName = item.itemData.itemName;
            this.icon = item.itemData.icon;
            this.price = item.itemData.price;
            this.maxAllowed = item.itemData.maxAllowed;
            this.plantData = item.plantData;
            this.isSellable = item.itemData.isSellable;
            this.item = item;
            currentCount += count;
        }
        // 移除一个物品
        public void RemoveItem()
        {
            if (currentCount > 0)
            {
                currentCount--;

                if (currentCount == 0)
                {
                    ClearAll();// 如果数量为0，则清空槽位
                }
            }
        }
        // 移除所有物品
        public void RemoveAllItems()
        {
            currentCount = 0;
            icon = null;
            itemName = "";
            plantData = null;
        }

        // 清空槽位
        public void ClearAll()
        {
            itemName = string.Empty;
            currentCount = 0;
            maxAllowed = 99;
            price = 0;
            isSellable = false;
            plantData = null;
            item = null;
        }
        // 获取剩余空间
        public int GetRemainingSpace()
        {
            return maxAllowed - currentCount;
        }
    }

    public List<Slot> slots = new List<Slot>();  // 槽位列表
    public Slot selectedSlot = null;  // 当前选中的槽位

    public Inventory(int numSlots)
    {
        for (int i = 0; i < numSlots; i++)
        {
            Slot slot = new Slot();
            slots.Add(slot);
        }
    }

    // 返回只读列表 
    public IReadOnlyList<Slot> GetSlots => slots.AsReadOnly();

    // 向库存中添加物品
    public bool Add(Item item)
    {
        // 如果槽位的类型与要添加的物品类型相同且未超过最大允许数量
        foreach (Slot slot in slots)
        {
            if (slot.itemName == item.itemData.itemName && slot.CanAddItem(item.itemData.itemName))
            {
                if (item.isDropped)
                {
                    int itemCount = item.GetDroppedItemCount();
                    slot.AddItem(item, itemCount);
                }
                else
                {
                    slot.AddItem(item);
                }
                return true;
            }
        }

        // 如果没有相同类型的槽位或槽位已满，无法添加物品
        foreach (Slot slot in slots)
        {
            // 如果是空槽位
            if (slot.itemName == "")
            {
                if (item.isDropped)
                {
                    int itemCount = item.GetDroppedItemCount();
                    slot.AddItem(item, itemCount);
                }
                else
                {
                    slot.AddItem(item);
                }
                return true;
            }
        }
        return false;
    }
    // 移除物品
    public void Remove(int index, bool isDrop = false)
    {
        if (isDrop)
        {
            slots[index].RemoveAllItems();
        }
        else
        {
            slots[index].RemoveItem();
        }
    }
    // 移动槽位
    public void MoveSlot(int fromIndex, int toIndex, Inventory toInventory, int numToMove = 1, string fromInventoryName = "")
    {
        Slot fromSlot = slots[fromIndex];
        Slot toSlot = toInventory.slots[toIndex];

        if (toSlot.isEmpty || toSlot.CanAddItem(fromSlot.itemName))
        {
            for (int i = 0; i < numToMove; i++)
            {
                toSlot.AddItem(fromSlot.item);
                Player.Instance.inventoryManager.AddInventory(fromInventoryName, fromSlot.item);
                fromSlot.RemoveItem();
            }
        }
    }
    // 选择槽位
    public void SelectSlot(int index)
    {
        if (slots != null && slots.Count > 0)
        {
            selectedSlot = slots[index];
        }
    }
    // 清空库存
    public void Clear()
    {
        foreach (var slot in slots)
        {
            slot.ClearAll();
        }
    }
}