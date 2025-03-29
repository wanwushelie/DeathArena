using UnityEngine;

public class MailboxInteraction : MonoBehaviour
{
    private Player player;
    private StateManager stateManager;

    public void Initialize(Player playerRef, StateManager stateManagerRef)
    {
        player = playerRef;
        stateManager = stateManagerRef;
    }

    public void UpdateMailboxInteraction()
    {
        if (GameManager.instance.timeManager.isDayEnding)
            return;

        if (stateManager.IsInteractingWithMailbox)
        {
            if (InGameUI.instance.speechBubble.activeSelf)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    SoundManager.Instance.Play("EFFECT/Pick", SoundType.EFFECT);
                    InGameUI.instance.ShowPostPanel();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PostBox"))
        {
            stateManager.IsInteractingWithMailbox = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PostBox"))
        {
            stateManager.IsInteractingWithMailbox = false;
        }
    }
}