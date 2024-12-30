using UnityEngine;

public class StraightLineEnemy : EnemyBase
{
    protected override void Start()
    {
        base.Start();
        moveSpeed = 5f; // Medium speed
        fireRate = 1f; // Regular fire rate
        direction = Vector2.left;
    }

    protected override void Move()
    {
        // Simple straight line movement
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    protected override void Awake()
    {
        base.Awake();
        poolTag = "StraightEnemy";
    }
} 