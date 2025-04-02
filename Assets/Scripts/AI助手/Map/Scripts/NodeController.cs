using UnityEngine;
using UnityEngine.UI;

public class NodeController : MonoBehaviour
{
    [Header("组件引用")]
    public Text textLabel; // 修改为普通Text组件
    public Image shapeImage;

    // 新增节点ID属性用于查找
    public string NodeId { get; private set; }

    public void Initialize(FlowchartParser.Node nodeData)
    {
        NodeId = nodeData.Id; // 设置节点ID
        textLabel.text = nodeData.Text;

        switch(nodeData.Type)
        {
            case FlowchartParser.NodeType.Diamond:
                shapeImage.color = Color.red;
                break;
            case FlowchartParser.NodeType.Round:
                shapeImage.color = Color.blue;
                break;
            default:
                shapeImage.color = Color.white;
                break;
        }
    }
}