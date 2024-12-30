using UnityEngine;

public class SlowPatternEnemy : EnemyBase
{
    [Header("Pattern Settings")]
    [SerializeField] private float circleRadius = 2f;
    [SerializeField] private float rotationSpeed = 1f;
    
    private Vector2 centerPoint;
    private float angle;

    protected override void Awake()
    {
        base.Awake();
        poolTag = "SlowEnemy";
    }

    protected override void Start()
    {
        base.Start();
        centerPoint = transform.position;
        moveSpeed = 3f; // Slow movement
        fireRate = 2f; // Shoots less frequently
    }

    protected override void Move()
    {
        // Update center point
        centerPoint += Vector2.left * moveSpeed * Time.deltaTime;
        
        // Circular movement pattern
        angle += rotationSpeed * Time.deltaTime;
        float x = centerPoint.x + circleRadius * Mathf.Cos(angle);
        float y = centerPoint.y + circleRadius * Mathf.Sin(angle);
        
        transform.position = new Vector3(x, y, 0);
    }
} 