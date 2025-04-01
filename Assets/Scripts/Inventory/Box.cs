using UnityEngine;

public class Box : MonoBehaviour
{
    public Animator boxAnimator;
    public string openAnimationParameter = "isOpen"; // 动画控制器中控制开关的参数名
    private bool isPlayerInRange = false;
    private bool isBoxOpen = false;

    private void Start()
    {
        // 确保初始状态下箱子是关闭的
        if (boxAnimator != null)
        {
            boxAnimator.SetBool(openAnimationParameter, isBoxOpen);
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetMouseButtonDown(0))
        {
            if (!isBoxOpen)
            {
                OpenBox();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (isBoxOpen)
            {
                CloseBox();
            }
        }
    }

    private void OpenBox()
    {
        isBoxOpen = true;
        if (boxAnimator != null)
        {
            boxAnimator.SetBool(openAnimationParameter, isBoxOpen);
        }
    }

    private void CloseBox()
    {
        isBoxOpen = false;
        if (boxAnimator != null)
        {
            boxAnimator.SetBool(openAnimationParameter, isBoxOpen);
        }
    }
}