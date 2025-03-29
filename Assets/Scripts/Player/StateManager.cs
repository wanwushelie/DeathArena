using System.Collections.Generic;
using UnityEngine;

public class StateManager
{
    // 状态变量
    public bool IsMoving { get; set; } = false;
    public bool IsHoeing { get; set; } = false;
    public bool IsWatering { get; set; } = false;
    public bool IsAxing { get; set; } = false;
    public bool IsInteractingWithMailbox { get; set; } = false;
    public bool IsMining { get; set; } = false;


    // 动画状态变量
    public Vector2 LastMoveDirection { get; set; } = Vector2.zero;

    // 多状态判断方法
    public bool CanMove()
    {
        // 如果玩家正在浇水、耕地、砍树或与邮箱交互，则不能移动
        return !(IsWatering || IsHoeing || IsAxing );
    }

    public bool CanInteract()
    {
        // 如果玩家正在移动，则不能交互
        return !IsMoving;
    }
}