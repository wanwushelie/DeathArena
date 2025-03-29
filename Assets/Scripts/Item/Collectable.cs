using UnityEngine;

[RequireComponent(typeof(Item))]
public class Collectable : MonoBehaviour
{
    //是否可以拾取
    public bool canCollect = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (player)
        {
            Item item = GetComponent<Item>();

            if (item != null && item.canInteract)
            {
                // player.isPicking = true; // 设置 Picking 标志
                // player.anim.SetTrigger("isPicking"); // 触发 Picking 动画
                // StartCoroutine(player.WaitForPickingAnimation()); // 等待动画播放完毕
                // player.isPicking = false; // 设置 Picking 标志

                // SoundManager.Instance.Play("EFFECT/Pick", SoundType.EFFECT);
                player.inventoryManager.Add(item);
                Destroy(this.gameObject);
            }
        }
    }
}
