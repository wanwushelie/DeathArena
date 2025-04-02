using UnityEngine;
using UnityEngine.UI;
// 在文件顶部添加命名空间引用
using System.Collections.Generic;  // 添加这行

public class FlowchartRenderer : MonoBehaviour
{
    public GameObject nodePrefab;
    public GameObject connectionPrefab;
    public Transform container;

    public void Render(List<FlowchartParser.Node> nodes, List<FlowchartParser.Connection> connections)
    {
        Clear();

        // 创建节点
        foreach (var node in nodes)
        {
            var nodeObj = Instantiate(nodePrefab, container);
            
            // 设置节点位置（关键修改）
            // 如果是UI元素：
            RectTransform rect = nodeObj.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = node.Position;
            }
            // 如果是普通3D对象：
            // nodeObj.transform.position = node.Position;

            var controller = nodeObj.GetComponent<NodeController>();
            if (controller == null)
            {
                Debug.LogError("节点预制件缺少NodeController组件！");
                continue;
            }
            controller.Initialize(node);
        }

        // 创建连接
        foreach (var conn in connections)
        {
            var lineObj = Instantiate(connectionPrefab, container);
            lineObj.GetComponent<ConnectionController>().Initialize(
                FindNode(conn.From), 
                FindNode(conn.To),
                conn.Label
            );
        }
    }

    private NodeController FindNode(string id)
    {
        foreach (Transform child in container)
        {
            var controller = child.GetComponent<NodeController>();
            if (controller != null && controller.NodeId == id)
                return controller;
        }
        return null;
    }

    private void Clear()
    {
        // 清理现有对象...
    }
}