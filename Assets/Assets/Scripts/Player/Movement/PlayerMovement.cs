using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float baseMovementSpeed = 5f;
    [SerializeField] private float turboSpeedMultiplier = 2f;
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float deceleration = 15f;
    
    [Header("Boundaries")]
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    [SerializeField] private float minY = -4f;
    [SerializeField] private float maxY = 4f;
    
    [Header("Turbo Gauge Settings")]
    [SerializeField] private float maxTurboGauge = 100f;
    [SerializeField] private float turboDepletionRate = 30f;
    [SerializeField] private float turboRechargeRate = 15f;
    
    private float currentTurboGauge;
    private bool isTurboActive;
    private Vector2 movement;
    private Vector2 currentVelocity;
    private Vector2 targetVelocity;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentTurboGauge = maxTurboGauge;
    }

    private void Update()
    {
        // Get input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        
        // Normalize movement vector for consistent speed in all directions
        if (movement.magnitude > 0)
        {
            movement = movement.normalized;
        }
        
        // Handle turbo input
        HandleTurbo();
        
        // Handle turbo gauge recharge
        HandleTurboGaugeRecharge();

        // Calculate target velocity
        float currentSpeed = baseMovementSpeed;
        if (isTurboActive && currentTurboGauge > 0)
        {
            currentSpeed *= turboSpeedMultiplier;
        }
        targetVelocity = movement * currentSpeed;
    }

    private void FixedUpdate()
    {
        // Smoothly adjust current velocity towards target velocity
        currentVelocity = Vector2.MoveTowards(
            currentVelocity,
            targetVelocity,
            (movement.magnitude > 0 ? acceleration : deceleration) * Time.fixedDeltaTime
        );

        // Apply movement
        rb.linearVelocity = currentVelocity;

        // Clamp position
        ClampPosition();
    }

    private void HandleTurbo()
    {
        isTurboActive = Input.GetKey(KeyCode.LeftShift) && currentTurboGauge > 0;
        
        if (isTurboActive)
        {
            currentTurboGauge = Mathf.Max(0, currentTurboGauge - turboDepletionRate * Time.deltaTime);
        }
    }

    private void HandleTurboGaugeRecharge()
    {
        if (!isTurboActive)
        {
            currentTurboGauge = Mathf.Min(maxTurboGauge, currentTurboGauge + turboRechargeRate * Time.deltaTime);
        }
    }

    private void ClampPosition()
    {
        Vector2 clampedPosition = rb.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        rb.position = clampedPosition;
    }

    public float GetTurboGaugePercentage()
    {
        return currentTurboGauge / maxTurboGauge;
    }
} 