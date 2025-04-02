using UnityEngine;
using UnityEngine.UI;

public class ConnectionController : MonoBehaviour
{
    [Header("连接设置")]
    public LineRenderer lineRenderer;
    public Text labelText;
    public float arrowSize = 10f;

    public void Initialize(NodeController startNode, NodeController endNode, string label)
    {
        // 计算中间点
        Vector3 startPos = startNode.transform.position;
        Vector3 endPos = endNode.transform.position;

        // 设置线条
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        // 设置标签位置和内容
        if(labelText != null)
        {
            labelText.text = label;
            labelText.transform.position = (startPos + endPos)/2;
        }
    }
}