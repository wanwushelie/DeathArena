using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class FenceInteraction : MonoBehaviour
{
    private Player player;
    private TileManager tileManager;
    private StateManager stateManager;

    [Header("Tilemap References")]
    public Tilemap fenceTilemap;       // 栏杆所在的Tilemap
    public TileBase fenceTile;         // 栏杆瓦片资源
    public GameObject fenceItemPrefab; // 栏杆物品预制体

    public void Initialize(Player playerRef, TileManager tileManagerRef, StateManager stateManagerRef)
    {
        player = playerRef;
        tileManager = tileManagerRef;
        stateManager = stateManagerRef;
    }

    public void UpdateFenceInteraction()
    {
        if (!stateManager.CanInteract() || 
            player.inventoryManager.toolbar.selectedSlot == null)
            return;

        CheckFencePlacement();
        CheckFenceRemoval();
    }

    private void CheckFencePlacement()
    {
        if (player.inventoryManager.toolbar.selectedSlot.itemName != "栏杆")
            return;

        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldMousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(mousePosition);
        worldMousePosition.z = 0;

        Vector3Int targetPos = fenceTilemap.WorldToCell(worldMousePosition);

        if (Input.GetMouseButtonDown(0) && 
            IsValidPosition(targetPos) && 
            !fenceTilemap.HasTile(targetPos))
        {
            // 放置栏杆
            fenceTilemap.SetTile(targetPos, fenceTile);
            player.inventoryManager.toolbar.selectedSlot.RemoveItem();
            player.anim.SetTrigger("isPicking");
            SoundManager.Instance.Play("EFFECT/Build", SoundType.EFFECT);
        }
    }

    private void CheckFenceRemoval()
    {
        if (player.inventoryManager.toolbar.selectedSlot.itemName != "Aox")
            return;

        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldMousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(mousePosition);
        worldMousePosition.z = 0;

        Vector3Int targetPos = fenceTilemap.WorldToCell(worldMousePosition);

        if (Input.GetMouseButtonDown(0) && 
            fenceTilemap.HasTile(targetPos))
        {
            // 清除栏杆
            fenceTilemap.SetTile(targetPos, null);
            SpawnFenceItem(targetPos);
            player.anim.SetTrigger("isAxing");
            SoundManager.Instance.Play("EFFECT/Break", SoundType.EFFECT);
            StartCoroutine(ResetAxingState());
        }
    }

    private bool IsValidPosition(Vector3Int pos)
    {
        // 检测3x3范围内是否有耕地
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (tileManager.GetTileName(pos + new Vector3Int(x, y, 0)) == "PlowedTile")
                    return true;
            }
        }
        return false;
    }

    private void SpawnFenceItem(Vector3Int pos)
    {
        Vector3 spawnPos = fenceTilemap.GetCellCenterWorld(pos);
        GameObject fenceItem = Instantiate(fenceItemPrefab, spawnPos, Quaternion.identity);
        
        // 添加物理效果
        Rigidbody2D rb = fenceItem.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 randomForce = new Vector2(
                Random.Range(-1f, 1f), 
                Random.Range(2f, 3f)
            );
            rb.AddForce(randomForce, ForceMode2D.Impulse);
        }
    }

    private IEnumerator ResetAxingState()
    {
        yield return new WaitForSeconds(0.5f);
        stateManager.IsAxing = false;
    }
}