using UnityEngine;
using System.Collections; // 注意：必须引入 System.Collections
using System.Collections.Generic;

[System.Serializable]
public class SpawnableObject
{
    public GameObject prefab;
    public int spawnCount = 10;
    public float avoidRadius = 0.5f;
    public Transform parent;
}

public class MultiObjectGenerator : MonoBehaviour
{
    public Transform areaStart;
    public Transform areaEnd;
    public List<SpawnableObject> objectsToSpawn = new List<SpawnableObject>();
    public bool generateOnStart = false;

    void Start()
    {
        if (generateOnStart) GenerateAll();
    }

    [ContextMenu("Generate All Objects")]
    public void GenerateAll()
    {
        foreach (var obj in objectsToSpawn)
        {
            for (int i = 0; i < obj.spawnCount; i++)
            {
                Vector2 spawnPos = GetValidSpawnPosition(obj);
                Instantiate(obj.prefab, spawnPos, Quaternion.identity, obj.parent);
            }
        }
    }

    private Vector2 GetValidSpawnPosition(SpawnableObject config)
    {
        Vector2 spawnPos;
        bool positionValid;
        int attempts = 0;

        do
        {
            spawnPos = GetRandomPositionInArea();
            positionValid = (config.avoidRadius <= 0) || 
                !Physics2D.OverlapCircle(spawnPos, config.avoidRadius);
            attempts++;
        }
        while (!positionValid && attempts < 100);

        return spawnPos;
    }

    private Vector2 GetRandomPositionInArea()
    {
        float x = Random.Range(areaStart.position.x, areaEnd.position.x);
        float y = Random.Range(areaStart.position.y, areaEnd.position.y);
        return new Vector2(x, y);
    }

    private void OnDrawGizmosSelected()
    {
        if (areaStart == null || areaEnd == null) return;

        Vector3 start = areaStart.position;
        Vector3 end = areaEnd.position;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(start, new Vector3(end.x, start.y, start.z));
        Gizmos.DrawLine(new Vector3(end.x, start.y, start.z), end);
        Gizmos.DrawLine(end, new Vector3(start.x, end.y, start.z));
        Gizmos.DrawLine(new Vector3(start.x, end.y, start.z), start);
    }

    [ContextMenu("Generate All Objects (Async)")]
    public void GenerateAllAsync()
    {
        StartCoroutine(GenerateAllCoroutine());
    }

    // 修正：使用非泛型 IEnumerator
    IEnumerator GenerateAllCoroutine()
    {
        foreach (var obj in objectsToSpawn)
        {
            for (int i = 0; i < obj.spawnCount; i++)
            {
                Vector2 spawnPos = GetValidSpawnPosition(obj);
                Instantiate(obj.prefab, spawnPos, Quaternion.identity, obj.parent);

                if (i % 10 == 0) 
                    yield return null;
            }
        }
    }
}