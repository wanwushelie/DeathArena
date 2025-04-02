// FlowchartManager.cs
using UnityEngine;

public class FlowchartManager : MonoBehaviour
{
    // 唯一管理实例
    private FlowchartParser _parser = new FlowchartParser();
    public FlowchartRenderer renderer;
    public FlowchartLayout layout;

    [TextArea(5, 10)]
    public string mermaidText = @"graph LR
        A[开始] --> B{条件判断}
        B -->|是| C[执行操作]
        B -->|否| D[结束]";

    void Start()
    {
        ParseAndRender();
    }

    public void ParseAndRender()
    {
        // 解析文本
        _parser.Parse(mermaidText);
        
        // 自动布局
        layout.AutoLayout(_parser.Nodes, _parser.Connections);
        
        // 渲染图表
        renderer.Render(_parser.Nodes, _parser.Connections);
    }
}