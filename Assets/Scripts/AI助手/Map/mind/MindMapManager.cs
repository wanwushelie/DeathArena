using System.Collections.Generic;
using UnityEngine;

public class MindMapManager : MonoBehaviour
{
    public GameObject nodePrefab;
    public Canvas canvas;

    private List<Node> nodes = new List<Node>();

    private void Start()
    {
        // 创建中心主题节点
        Node centralNode = CreateNode("中心主题");

        // 创建第一层子节点
        Node branch1 = CreateChildNode(centralNode, "分支1", new Vector2(200, 0));
        Node branch2 = CreateChildNode(centralNode, "分支2", new Vector2(-200, 0));

        // 创建第二层子节点
        CreateChildNode(branch1, "子分支1.1", new Vector2(100, -50));
        CreateChildNode(branch1, "子分支1.2", new Vector2(100, 50));
        CreateChildNode(branch2, "子分支2.1", new Vector2(-100, -50));
        CreateChildNode(branch2, "子分支2.2", new Vector2(-100, 50));
    }

    public Node CreateNode(string text, Vector2 offset = default(Vector2))
    {
        GameObject nodeGO = Instantiate(nodePrefab, canvas.transform);
        Node node = nodeGO.GetComponent<Node>();
        node.SetText(text);

        RectTransform nodeRect = node.rectTransform;
        nodeRect.anchoredPosition = offset;

        nodes.Add(node);
        return node;
    }

    public Node CreateChildNode(Node parent, string text, Vector2 offset)
    {
        Vector2 parentPosition = parent.rectTransform.anchoredPosition;
        Vector2 childPosition = parentPosition + offset;
        return CreateNode(text, childPosition);
    }
}