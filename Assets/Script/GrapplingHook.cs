using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(DistanceJoint2D))]
[RequireComponent(typeof(LineRenderer))]
public class GrapplingHook : MonoBehaviour
{
    [Header("Hook Settings")]
    public LayerMask hookableLayer;
    public float maxHookDistance = 25f;
    public float hookTravelSpeed = 40f;
    
    [Header("Reel Settings")]
    public float autoReelSpeed = 10f;
    public float fastReelSpeed = 25f;

    private Rigidbody2D rb;
    private DistanceJoint2D joint;
    private LineRenderer line;

    private Vector2 hookTargetPosition;
    private Vector2 currentHookPosition;
    private GhostTrail ghostTrail;

    private enum HookState { Inactive, Firing, Attached }
    private HookState state = HookState.Inactive;

    public bool IsGrappling => state == HookState.Attached;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        Physics2D.positionIterations = Mathf.Max(Physics2D.positionIterations, 12);
        Physics2D.velocityIterations = Mathf.Max(Physics2D.velocityIterations, 12);
        
        joint = GetComponent<DistanceJoint2D>();
        joint.enabled = false;
        joint.autoConfigureDistance = false;
        
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;

        ghostTrail = GetComponent<GhostTrail>();
    }

    void Update()
    {
        HandleInput();
        UpdateHookLogic();
        UpdateRopeVisuals();
    }

    private void HandleInput()
    {
        // 1. Fire the hook when M1 is pressed
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2)transform.position).normalized;

            // Raycast strictly towards where the mouse is
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxHookDistance, hookableLayer);

            if (hit.collider != null)
            {
                // 3. Mark the exact spot to stick
                hookTargetPosition = hit.point;
                currentHookPosition = transform.position;
                state = HookState.Firing;
                
                line.positionCount = 2; 
            }
        }
        
        // 1. If the player interrupts the hook (by releasing M1) -> destroying and resettin
        if (Input.GetMouseButtonUp(0))
        {
            DetachHook();
        }
    }

    private void UpdateHookLogic()
    {
        if (state == HookState.Firing)
        {
            // 4. Travels from player to object before sticking
            currentHookPosition = Vector2.MoveTowards(currentHookPosition, hookTargetPosition, hookTravelSpeed * Time.deltaTime);

            if (Vector2.Distance(currentHookPosition, hookTargetPosition) < 0.1f)
            {
                currentHookPosition = hookTargetPosition;
                AttachHook();
            }
        }
        else if (state == HookState.Attached)
        {
            float minRopeLength = 0.5f; 
            float speed = Input.GetKey(KeyCode.LeftShift) ? fastReelSpeed : autoReelSpeed;
            joint.distance = Mathf.MoveTowards(joint.distance, minRopeLength, speed * Time.deltaTime);
        }
    }

    private void AttachHook()
    {
        state = HookState.Attached;
        joint.enabled = true;
        joint.connectedAnchor = hookTargetPosition;
        joint.connectedBody = null; 
        
        // Lock the distance so they hang and swing like a pendulum
        joint.distance = Vector2.Distance(transform.position, hookTargetPosition);

        if (ghostTrail != null) ghostTrail.isEmitting = true;
    }

    private void DetachHook()
    {
        state = HookState.Inactive;
        joint.enabled = false;
        line.positionCount = 0;

        if (ghostTrail != null) ghostTrail.isEmitting = false;
    }

    private void UpdateRopeVisuals()
    {
        if (state == HookState.Inactive) return;

        // Visual line from player to moving hook / attached point
        line.SetPosition(0, transform.position);
        line.SetPosition(1, currentHookPosition);
    }
}