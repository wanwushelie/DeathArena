// FlowchartTest.cs
using UnityEngine;

public class FlowchartTest : MonoBehaviour
{
    public FlowchartManager flowchartManager;

    void Start()
    {
        flowchartManager.ParseAndRender();
    }
}