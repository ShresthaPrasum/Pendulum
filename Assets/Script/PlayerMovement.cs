using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float swingForce = 15f;
    public float maxSwingSpeed = 12f;
    public float jumpForce = 12f;
    
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public LayerMask hookablelayer;

    private Rigidbody2D rb;
    private float horizontal;
    private bool isGrounded;
    private GrapplingHook hook;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        hook = GetComponent<GrapplingHook>();
    }

    void Update()
    {
        horizontal = 0f;
        if (Input.GetKey(KeyCode.A)) horizontal = -1f;
        if (Input.GetKey(KeyCode.D)) horizontal = 1f;

        Vector2 checkPos = groundCheck != null ? (Vector2)groundCheck.position : (Vector2)transform.position - new Vector2(0, transform.localScale.y / 2f);
        
        isGrounded = Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer | hookablelayer);
        

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void FixedUpdate()
    {
        if (hook != null && hook.IsGrappling)
        {
            if (Mathf.Abs(horizontal) > 0.01f)
            {
                rb.AddForce(new Vector2(horizontal * swingForce, 0f), ForceMode2D.Force);
            }

            Vector2 velocity = rb.linearVelocity;
            velocity.x = Mathf.Clamp(velocity.x, -maxSwingSpeed, maxSwingSpeed);
            rb.linearVelocity = velocity;
        }
        else
        {
            rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
        }
    }
}
