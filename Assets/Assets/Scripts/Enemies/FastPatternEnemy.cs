using UnityEngine;

public class FastPatternEnemy : EnemyBase
{
    [Header("Pattern Settings")]
    [SerializeField] private float amplitude = 2f;
    [SerializeField] private float frequency = 2f;
    
    private float startY;
    private float time;

    protected override void Start()
    {
        base.Start();
        startY = transform.position.y;
        moveSpeed = 8f; // Fast movement
        fireRate = 1.5f; // Shoots frequently
    }

    protected override void Move()
    {
        time += Time.deltaTime;
        
        // Sine wave movement pattern
        float newY = startY + amplitude * Mathf.Sin(time * frequency);
        float newX = transform.position.x - moveSpeed * Time.deltaTime;
        
        transform.position = new Vector3(newX, newY, 0);
    }

    protected override void Awake()
    {
        base.Awake();
        poolTag = "FastEnemy";
    }
} 