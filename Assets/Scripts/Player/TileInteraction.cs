using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class TileInteraction : MonoBehaviour
{
    private Player player;
    private TileManager tileManager;
    private Vector3Int targetPosition;
    private Vector3Int[] validTiles;
    private bool isValidTile = false;
    private StateManager stateManager;

    public void Initialize(Player playerRef, TileManager tileManagerRef, StateManager stateManagerRef)
    {
        player = playerRef;
        tileManager = tileManagerRef;
        stateManager = stateManagerRef;
    }

    public void UpdateTileInteraction()
    {
        if (!stateManager.CanMove() || !stateManager.CanInteract())
            return;

        if (tileManager == null)
            return;

        if (player.inventoryManager == null || player.inventoryManager.toolbar == null || player.inventoryManager.toolbar.selectedSlot == null)
            return;

        if (player.inventoryManager.toolbar.selectedSlot.itemName == null)
            return;

        CheckValidTiles();

        if (isValidTile)
        {
            MouseSelect mouseSelect = player.GetComponentInChildren<MouseSelect>();
            if (mouseSelect != null)
            {
                mouseSelect.SetTargetPosition(targetPosition);
            }

            if (Input.GetMouseButtonDown(0))
            {
                SetAnimationDirection(targetPosition);

                string tileName = tileManager.GetTileName(targetPosition);
                string tileState = tileManager.GetTileState(targetPosition);

                if (tileName != null)
                {
                    HandleHoeing(tileName, tileState);
                    HandlePlanting(tileName);
                    HandleWatering(tileName);
                }
            }
        }
    }

    private void CheckValidTiles()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldMousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(mousePosition);
        worldMousePosition.z = 0;

        targetPosition = new Vector3Int(Mathf.FloorToInt(worldMousePosition.x), Mathf.FloorToInt(worldMousePosition.y), 0);

        Vector3 playerPosition = player.transform.position;
        Vector3Int gridPlayerPosition = new Vector3Int(Mathf.FloorToInt(playerPosition.x), Mathf.FloorToInt(playerPosition.y), 0);

        validTiles = new Vector3Int[]
        {
            gridPlayerPosition,
            gridPlayerPosition + new Vector3Int(0, 1, 0),
            gridPlayerPosition + new Vector3Int(0, -1, 0),
            gridPlayerPosition + new Vector3Int(1, 0, 0),
            gridPlayerPosition + new Vector3Int(-1, 0, 0),
        };

        isValidTile = false;

        foreach (var tile in validTiles)
        {
            if (tile == targetPosition)
            {
                isValidTile = true;
                break;
            }
        }
    }

    private void SetAnimationDirection(Vector3 targetPosition)
    {
        Vector3 playerPosition = player.transform.position;
        Vector3Int gridPlayerPosition = new Vector3Int(Mathf.FloorToInt(playerPosition.x), Mathf.FloorToInt(playerPosition.y), 0);

        Vector3 direction = (targetPosition - gridPlayerPosition).normalized;

        player.anim.SetFloat("Horizontal", direction.x);
        player.anim.SetFloat("Vertical", direction.y);

        stateManager.LastMoveDirection = direction;
    }

    private void HandleHoeing(string tileName, string tileState)
    {
        if (player.inventoryManager.toolbar.selectedSlot.itemName == "Hoe")
        {
            if (tileName == "InteractableTile")
            {
                stateManager.IsHoeing = true;
                player.anim.SetTrigger("isHoeing");
                SoundManager.Instance.Play("EFFECT/Plow", SoundType.EFFECT);
                tileManager.SetInteracted(targetPosition);
                StartCoroutine(ResetHoeingState());
            }

            if (tileState == "Grown")
            {
                tileManager.RemoveTile(targetPosition);
                player.anim.SetTrigger("isHavesting");
                SoundManager.Instance.Play("EFFECT/Plow", SoundType.EFFECT);
                GameManager.instance.plantGrowthManager.HarvestPlant(targetPosition);
            }
        }
    }

    private void HandlePlanting(string tileName)
    {
        if (tileName == "PlowedTile")
        {
            if (player.inventoryManager.toolbar.selectedSlot.itemName == "RiceSeed" || player.inventoryManager.toolbar.selectedSlot.itemName == "TomatoSeed")
            {
                Sowing();
            }
        }
    }

    private void HandleWatering(string tileName)
    {
        if (tileName == "PlowedTile")
        {
            if (player.inventoryManager.toolbar.selectedSlot.itemName == "Watering")
            {
                stateManager.IsWatering = true;
                player.anim.SetTrigger("isWatering");
                SoundManager.Instance.Play("EFFECT/Watering", SoundType.EFFECT, 1, 1);
                tileManager.WaterTile(targetPosition);
                StartCoroutine(ResetWateringState());
            }
        }
    }

    private void Sowing()
    {
        PlantData plantData = player.inventoryManager.toolbar.selectedSlot.plantData;
        player.inventoryManager.toolbar.selectedSlot.RemoveItem();

        if (player.inventoryManager.toolbar.selectedSlot.isEmpty)
        {
            player.inventoryManager.toolbar.selectedSlot = null;
        }

        GameManager.instance.plantGrowthManager.PlantSeed(targetPosition, plantData);
    }

    private IEnumerator ResetHoeingState()
    {
        yield return new WaitForSeconds(0.2f);
        stateManager.IsHoeing = false;
    }

    private IEnumerator ResetWateringState()
    {
        yield return new WaitForSeconds(0.2f);
        stateManager.IsWatering = false;
    }
}