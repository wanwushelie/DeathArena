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
        }
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


            if (hit.collider != null)
            {
                // _agent.SetDestination(hit.point);
                // 为所有代理设置目标位置
                foreach (PolyNavAgent agent in _agents)
                {
                    agent.SetDestination(hit.point);
                }
            }
        }
    }
}
