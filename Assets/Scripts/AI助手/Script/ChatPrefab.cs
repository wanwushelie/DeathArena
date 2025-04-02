using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatPrefab : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Text m_Text;
    [SerializeField] private Button m_Button; // 新增按钮组件
    [SerializeField] private Text m_ReviewText; // 新增审查文本组件
    [SerializeField] public GameObject m_ReviewBubble; // 新增审查气泡组件

    private bool isReviewShown = false; // 标记审查信息是否显示

    public void SetText(string _msg)
    {
        m_Text.text = _msg;
    }

    public void SetReviewText(string _review)
    {
        m_ReviewText.text = _review;
    }

    public void ShowReview()
    {
        if (!isReviewShown)
        {
            m_ReviewBubble.SetActive(true);
            isReviewShown = true;
        }
        else
        {
            m_ReviewBubble.SetActive(false);
            isReviewShown = false;
        }
        // 强制重建布局，以确保 UI 元素的位置正确
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void SetupButton()
    {
        if (m_Button != null)
        {
            m_Button.onClick.AddListener(ShowReview);
        }
    }
}