using UnityEngine;

public class PostBoxInteraction : MonoBehaviour
{
    [SerializeField] private Player player; // 引用玩家对象
    [SerializeField] private InGameUI inGameUI; // 引用游戏内UI对象
    [SerializeField] private string postBoxTag = "PostBox"; // 邮筒的Tag

    private bool isPlayerInPostBox = false; // 标记玩家是否在邮筒区域
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(postBoxTag))
        {
            isPlayerInPostBox = true; // 标记玩家进入邮筒区域
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(postBoxTag))
        {
            isPlayerInPostBox = false; // 标记玩家离开邮筒区域
        }
    }

    private void Update()
    {
        if (isPlayerInPostBox)
        {
            HandlePostBoxInteraction();
        }
    }

    private void HandlePostBoxInteraction()
    {
        if (inGameUI.speechBubble.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SoundManager.Instance.Play("拾取音效"); // 播放音效
                inGameUI.ShowPostPanel(); // 显示邮箱面板并隐藏对话气泡
            }
        }
    }
}