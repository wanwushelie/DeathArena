using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    public string text;
    public RectTransform rectTransform;
    public Text nodeText;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        nodeText = GetComponentInChildren<Text>();
        nodeText.text = text;
    }

    public void SetText(string newText)
    {
        text = newText;
        nodeText.text = newText;
    }
}