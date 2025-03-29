using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player player;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movement;
    private float moveSpeed = 2.5f;
    private StateManager stateManager;

    public void Initialize(Player playerRef, StateManager stateManagerRef)
    {
        player = playerRef;
        stateManager = stateManagerRef;
        rb = playerRef.GetComponent<Rigidbody2D>();
        anim = playerRef.GetComponent<Animator>();
    }

    public void UpdateMovement()
    {
        GetInput();
        UpdateAnimation();
    }

    public void FixedUpdateMovement()
    {
        if (!GameManager.instance.timeManager.isDayEnding && stateManager.CanMove())
            Move();
    }

    private void GetInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement.magnitude > 1)
        {
            movement = movement.normalized;
        }

        if (movement != Vector2.zero)
        {
            stateManager.LastMoveDirection = movement;
        }
    }

    private void UpdateAnimation()
    {
        anim.SetFloat("Horizontal", movement.x);
        anim.SetFloat("Vertical", movement.y);
        anim.SetFloat("Speed", movement.sqrMagnitude);

        if (movement.sqrMagnitude == 0)
        {
            anim.SetFloat("LastHorizontal", stateManager.LastMoveDirection.x);
            anim.SetFloat("LastVertical", stateManager.LastMoveDirection.y);
            stateManager.IsMoving = false;
        }
        else
        {
            stateManager.IsMoving = true;
        }
    }

    private void Move()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}