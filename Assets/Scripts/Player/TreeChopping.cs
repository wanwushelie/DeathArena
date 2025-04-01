using UnityEngine;
using System.Collections;

public class TreeChopping : MonoBehaviour
{
    private Player player;
    private RaycastHit2D rayHit;
    private StateManager stateManager;

    public void Initialize(Player playerRef, StateManager stateManagerRef)
    {
        player = playerRef;
        stateManager = stateManagerRef;
    }

    public void UpdateTreeChopping()
    {
        if (stateManager.IsAxing)
            return;

        CheckTreeInteraction();
    }

    private void CheckTreeInteraction()
    {
        rayHit = Physics2D.Raycast(player.GetComponent<Rigidbody2D>().position, stateManager.LastMoveDirection, 1f, LayerMask.GetMask("Tree"));
        if (rayHit.collider != null)
        {
            Tree tree = rayHit.collider.GetComponent<Tree>();
            if (tree != null)
            {
                if (player.inventoryManager.toolbar.selectedSlot != null && player.inventoryManager.toolbar.selectedSlot.itemName == "斧头")
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        StartChopping(tree);
                    }
                }
            }
        }
    }

    private void StartChopping(Tree tree)
    {
        SoundManager.Instance.Play("EFFECT/HITTREE", SoundType.EFFECT);
        stateManager.IsAxing = true;
        player.anim.SetTrigger("isAxing");
        tree.hitCount++;
        StartCoroutine(ResetChoppingState());
    }

    private IEnumerator ResetChoppingState()
    {
        yield return new WaitForSeconds(0.7f);
        stateManager.IsAxing = false;
    }
}