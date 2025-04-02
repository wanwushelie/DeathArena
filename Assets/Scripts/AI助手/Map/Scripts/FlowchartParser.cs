using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine; 

public class FlowchartParser
{
    public class Node
    {
        public string Id;
        public string Text;
        public NodeType Type;
        public Vector2 Position; // 新增位置属性
    }

    public class Connection
    {
        public string From;
        public string To;
        public string Label;
    }

    public enum NodeType { Default, Round, Square, Diamond }

    public List<Node> Nodes = new List<Node>();
    public List<Connection> Connections = new List<Connection>();

    public void Parse(string input)
    {
        Nodes.Clear();
        Connections.Clear();

        foreach (var line in input.Split('\n'))
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("graph")) continue;

            // 解析节点和连接
            var connectionMatch = Regex.Match(trimmed, 
                @"(\w+)\s*(\[.*?\]|\{.*?\}|\(.*?\))?\s*-->\s*\|?(.*?)\|?\s*(\w+)\s*(\[.*?\]|\{.*?\}|\(.*?\))?");
            
            if (connectionMatch.Success)
            {
                // 处理连接关系
                var from = connectionMatch.Groups[1].Value;
                var to = connectionMatch.Groups[4].Value;
                var label = connectionMatch.Groups[3].Value;

                Connections.Add(new Connection { From = from, To = to, Label = label });
                
                // 解析节点形状
                ParseNode(from, connectionMatch.Groups[2].Value);
                ParseNode(to, connectionMatch.Groups[5].Value);
            }
        }
    }

    private void ParseNode(string id, string shapeText)
    {
        var node = Nodes.Find(n => n.Id == id);
        if (node == null)
        {
            node = new Node { Id = id, Text = id };
            Nodes.Add(node);
        }

        node.Type = shapeText switch
        {
            string s when s.StartsWith("[") => NodeType.Square,
            string s when s.StartsWith("(") => NodeType.Round,
            string s when s.StartsWith("{") => NodeType.Diamond,
            _ => NodeType.Default
        };

        var textMatch = Regex.Match(shapeText, @"\[(.*?)\]|\{(.*?)\}|\((.*?)\)");
        if (textMatch.Success)
        {
            node.Text = textMatch.Groups[1].Success ? textMatch.Groups[1].Value :
                textMatch.Groups[2].Success ? textMatch.Groups[2].Value :
                textMatch.Groups[3].Value;
        }
    }
}