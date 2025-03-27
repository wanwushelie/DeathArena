using PolyNav;
using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    [SerializeField]
    // private PolyNavAgent _agent;
    private List<PolyNavAgent> _agents; // 用于存储所有代理的列表
    [SerializeField]
    private Camera _camera;

    void Start()
    {
        // 初始化代理列表
        _agents = new List<PolyNavAgent>();
        // 找到场景中所有的 PolyNavAgent 并添加到列表中
        PolyNavAgent[] allAgents = FindObjectsOfType<PolyNavAgent>();
        foreach (PolyNavAgent agent in allAgents)
        {
            _agents.Add(agent);
            Debug.Log("Added agent: " + agent.gameObject.name); // 调试：输出添加的代理名称
        }
        Debug.Log("Total agents found: " + _agents.Count); // 调试：输出找到的代理总数
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Vector2 touchDownPos = Input.mousePosition;
            // Vector2 worldPos = _camera.main.ScreenToWorldPoint(touchDownPos);
            // RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
          
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldMousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(mousePosition);
            worldMousePosition.z = 0;
            RaycastHit2D hit = Physics2D.Raycast(worldMousePosition, Vector2.zero);

            Debug.Log("Mouse clicked at screen position: " + mousePosition); // 调试：输出鼠标点击的屏幕位置
            Debug.Log("Mouse clicked at world position: " + worldMousePosition); // 调试：输出鼠标点击的世界位置

            if (hit.collider != null)
            {
                Debug.Log("Hit collider: " + hit.collider.gameObject.name); // 调试：输出射线检测到的碰撞体名称
                // _agent.SetDestination(hit.point);
                // 为所有代理设置目标位置
                foreach (PolyNavAgent agent in _agents)
                {
                    Debug.Log("Setting destination for agent: " + agent.gameObject.name); // 调试：输出为代理设置目标位置的信息
                    agent.SetDestination(hit.point);
                }
            }
            else
            {
                Debug.Log("No collider hit"); // 调试：如果没有检测到碰撞体，输出提示信息
            }
        }
    }
}