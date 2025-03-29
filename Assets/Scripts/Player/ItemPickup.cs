using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private Player player;
    private StateManager stateManager;

    public void Initialize(Player playerRef, StateManager stateManagerRef)
    {
        player = playerRef;
        stateManager = stateManagerRef;
    }

    public void UpdateItemPickup()
    {
        if (stateManager.IsAxing || stateManager.IsMining)
            return;

        CheckItemInteraction();
    }

    private void CheckItemInteraction()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            // 检测玩家周围是否有可拾取的物品
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f, LayerMask.GetMask("Default"));//所有层
            foreach (Collider2D collider in colliders)
            {
                Item item = collider.GetComponent<Item>();
                if (item != null && item.canInteract)
                {
                    PickupItem(item);
                    break;
                }
            }
        }
    }

    private void PickupItem(Item item)
    {
        // 播放拾取动画
        player.anim.SetTrigger("isPicking");
        SoundManager.Instance.Play("EFFECT/Pickup", SoundType.EFFECT);

        // 将物品添加到玩家的背包
        player.inventoryManager.Add(item);

        // 销毁物品对象
        Destroy(item.gameObject);
    }
}