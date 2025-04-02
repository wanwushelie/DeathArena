using System.Collections.Generic;
using UnityEngine;

public class FlowchartLayout : MonoBehaviour
{
    public float HorizontalSpacing = 200f;
    public float VerticalSpacing = 100f;
    
    public void AutoLayout(List<FlowchartParser.Node> nodes, List<FlowchartParser.Connection> connections)
    {
        // 简单层级布局算法（可扩展为更复杂的布局）
        Dictionary<string, int> nodeLevels = new Dictionary<string, int>();
        
        // 计算节点层级
        foreach (var node in nodes)
        {
            if (!nodeLevels.ContainsKey(node.Id))
                nodeLevels[node.Id] = 0;
            
            foreach (var conn in connections.FindAll(c => c.To == node.Id))
            {
                nodeLevels[node.Id] = Mathf.Max(nodeLevels[node.Id], 
                    nodeLevels.GetValueOrDefault(conn.From, 0) + 1);
            }
        }

        // 按层级排列位置
        Dictionary<int, float> levelX = new Dictionary<int, float>();
        foreach (var node in nodes)
        {
            int level = nodeLevels[node.Id];
            if (!levelX.ContainsKey(level))
                levelX[level] = 0;

            // 在Unity中设置实际位置（示例使用Transform，实际可以用RectTransform）
            node.Position = new Vector2(
                level * HorizontalSpacing,
                -levelX[level] * VerticalSpacing
            );

            levelX[level] += 1;
        }
    }
}