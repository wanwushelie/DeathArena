using UnityEngine;
using System.Collections;

public class MiningInteraction : MonoBehaviour
{
    private Player player;
    private RaycastHit2D rayHit;
    private StateManager stateManager;

    public void Initialize(Player playerRef, StateManager stateManagerRef)
    {
        player = playerRef;
        stateManager = stateManagerRef;
    }

    public void UpdateMiningInteraction()
    {
        if (stateManager.IsMining)
            return;

        CheckStoneInteraction();
    }

    private void CheckStoneInteraction()
    {
        rayHit = Physics2D.Raycast(player.GetComponent<Rigidbody2D>().position, stateManager.LastMoveDirection, 1f, LayerMask.GetMask("Stone"));
        if (rayHit.collider != null)
        {
            Stone stone = rayHit.collider.GetComponent<Stone>();
            if (stone != null)
            {
                if (player.inventoryManager.toolbar.selectedSlot != null && player.inventoryManager.toolbar.selectedSlot.itemName == "Axe")
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        StartMining(stone);
                    }
                }
            }
        }
    }

    private void StartMining(Stone stone)
    {
        SoundManager.Instance.Play("EFFECT/Mining", SoundType.EFFECT);
        stateManager.IsMining = true;
        player.anim.SetTrigger("isAxing");
        stone.hitCount++;
        StartCoroutine(ResetMiningState());
    }

    private IEnumerator ResetMiningState()
    {
        yield return new WaitForSeconds(0.7f);
        stateManager.IsMining = false;
    }
}