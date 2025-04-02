using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleObjectsController : MonoBehaviour
{
    [System.Serializable]
    public class ToggleItem
    {
        public Button toggleButton;
        public GameObject targetObject;
    }

    public List<ToggleItem> toggleItems = new List<ToggleItem>();

    void Start()
    {
        // 初始化所有按钮和物体状态
        foreach (var item in toggleItems)
        {
            if (item.toggleButton != null && item.targetObject != null)
            {
                // 设置初始状态
                item.targetObject.SetActive(false);
                
                // 添加点击事件
                item.toggleButton.onClick.AddListener(() => ToggleObject(item));
            }
        }
    }

    private void ToggleObject(ToggleItem item)
    {
        if (item.targetObject != null)
        {
            // 切换物体显示状态
            item.targetObject.SetActive(!item.targetObject.activeSelf);
        }
    }
}