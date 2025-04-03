using UnityEngine;  // 引入Unity引擎的核心命名空间
using System.Collections;  // 引入System.Collections命名空间，用于使用IEnumerator等类型

public class MiningInteraction : MonoBehaviour
{
    private Player player;  // 声明一个Player类型的私有变量，用于引用玩家对象
    private RaycastHit2D rayHit;  // 声明一个RaycastHit2D类型的私有变量，用于存储射线检测的结果
    private StateManager stateManager;  // 声明一个StateManager类型的私有变量，用于引用状态管理对象

    /// <summary>
    /// 初始化MiningInteraction类的实例。
    /// </summary>
    /// <param name="playerRef">玩家对象的引用。</param>
    /// <param name="stateManagerRef">状态管理对象的引用。</param>
    public void Initialize(Player playerRef, StateManager stateManagerRef)
    {
        player = playerRef;  // 将传入的玩家对象引用赋值给私有变量player
        stateManager = stateManagerRef;  // 将传入的状态管理对象引用赋值给私有变量stateManager
    }

    /// <summary>
    /// 更新挖矿交互逻辑。
    /// </summary>
    public void UpdateMiningInteraction()
    {
        if (stateManager.IsMining)  // 检查是否正在挖矿
            return;  // 如果正在挖矿，则直接返回，不执行后续逻辑

        CheckStoneInteraction();  // 检查与石头的交互
    }

    /// <summary>
    /// 检查与石头的交互。
    /// </summary>
    private void CheckStoneInteraction()
    {
        // 从玩家的刚体位置沿着最后移动方向发射一条长度为1的射线，检测名为"Stone"的层
        rayHit = Physics2D.Raycast(player.GetComponent<Rigidbody2D>().position, stateManager.LastMoveDirection, 1f, LayerMask.GetMask("Stone"));
        if (rayHit.collider != null)  // 检查射线是否击中了碰撞体
        {
            Stone stone = rayHit.collider.GetComponent<Stone>();  // 获取碰撞体上的Stone组件
            if (stone != null)  // 检查是否成功获取到Stone组件
            {
                // 检查玩家工具栏中选中的物品是否为"Axe"
                if (player.inventoryManager.toolbar.selectedSlot != null && player.inventoryManager.toolbar.selectedSlot.itemName == "镐子")
                {
                    if (Input.GetMouseButtonDown(0))  // 检查是否按下鼠标左键
                    {
                        StartMining(stone);  // 开始挖矿
                    }
                }
            }
        }
    }

    /// <summary>
    /// 开始挖矿操作。
    /// </summary>
    /// <param name="stone">要挖掘的石头对象。</param>
    private void StartMining(Stone stone)
    {
        SoundManager.Instance.Play("EFFECT/Mining", SoundType.EFFECT);  // 播放挖矿音效
        stateManager.IsMining = true;  // 设置挖矿状态为真
        player.anim.SetTrigger("isHoeing");  // 触发玩家的砍树动画
        stone.hitCount++;  // 增加石头的被击打次数
        StartCoroutine(ResetMiningState());  // 启动协程，用于重置挖矿状态
    }

    /// <summary>
    /// 重置挖矿状态的协程。
    /// </summary>
    private IEnumerator ResetMiningState()
    {
        yield return new WaitForSeconds(0.7f);  // 等待0.7秒
        stateManager.IsMining = false;  // 设置挖矿状态为假
    }
}