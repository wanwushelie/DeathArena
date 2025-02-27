using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// 定义一个编辑器窗口类
public class FindScriptReferences : EditorWindow
{
    // 在Unity菜单中添加一个菜单项来打开这个窗口
    [MenuItem("Tools/Find Script References")]
    public static void ShowWindow()
    {
        // 打开或聚焦窗口
        GetWindow<FindScriptReferences>("Find Script References");
    }

    // 定义一些成员变量
    private string scriptName = ""; // 用户输入的目标脚本名称
    private List<GameObject> referencedObjects = new List<GameObject>(); // 用于存储找到的对象

    // 定义窗口的GUI布局
    void OnGUI()
    {
        // 添加一个标题
        GUILayout.Label("Find Script References", EditorStyles.boldLabel);

        // 添加一个文本框，让用户输入脚本名称
        GUILayout.Label("Enter Script Name:");
        scriptName = GUILayout.TextField(scriptName);

        // 添加一个按钮，点击后开始搜索
        if (GUILayout.Button("Search"))
        {
            // 清空之前的搜索结果
            referencedObjects.Clear();
            // 调用搜索方法
            SearchForScript();
        }

        // 显示搜索结果
        GUILayout.Label("Referenced Objects:", EditorStyles.boldLabel);
        foreach (var obj in referencedObjects)
        {
            // 为每个找到的对象添加一个可点击的标签
            if (GUILayout.Button(obj.name, EditorStyles.label))
            {
                // 点击后选中该对象
                Selection.activeObject = obj;
            }
        }
    }

    // 定义搜索方法
    private void SearchForScript()
    {
        // 获取场景中所有的GameObject
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            // 检查对象是否绑定了目标脚本
            var script = obj.GetComponent(scriptName);
            if (script != null)
            {
                // 如果绑定了目标脚本，将该对象添加到结果列表中
                referencedObjects.Add(obj);
            }
        }
    }
}