using UnityEngine;

public class FrogMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;

    private Rigidbody2D rb;
    private SpriteRenderer rend;
    private Vector2 movementInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        movementInput = new Vector2(moveX, moveY).normalized;
        if (moveX > 0)
        {
            rend.flipX = true;
        }
        else if (moveX < 0)
        {
            rend.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movementInput * speed;
    }
}