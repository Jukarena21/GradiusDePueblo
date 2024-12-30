using UnityEngine;

public class MissileProjectile : ProjectileBase
{
    [Header("Missile Settings")]
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float initialDelay = 0.5f; // Time before missile starts tracking
    [SerializeField] private float maxLifetime = 5f; // Maximum time the missile can exist
    [SerializeField] private float retargetInterval = 0.5f; // How often to look for new target if current is lost

    private Transform target;
    private float timeSinceSpawn;
    private float nextRetargetTime;

    private void Awake()
    {
        poolTag = "MissileProjectile";
    }

    public override void OnObjectSpawn()
    {
        base.OnObjectSpawn();
        timeSinceSpawn = 0f;
        nextRetargetTime = 0f;
        FindNearestTarget();
    }

    protected override void Update()
    {
        timeSinceSpawn += Time.deltaTime;
        
        // Check for maximum lifetime
        if (timeSinceSpawn >= maxLifetime)
        {
            ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
            return;
        }

        if (timeSinceSpawn < initialDelay)
        {
            // Move straight initially
            base.Update();
            return;
        }

        // If target is lost or destroyed, try to find new target periodically
        if (target == null && Time.time >= nextRetargetTime)
        {
            FindNearestTarget();
            nextRetargetTime = Time.time + retargetInterval;
            
            if (target == null)
            {
                // If still no target, continue straight
                base.Update();
                return;
            }
        }

        if (target != null)
        {
            // Calculate direction to target
            Vector2 direction = (target.position - transform.position).normalized;
            
            // Calculate angle to target
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            // Smoothly rotate towards target
            float currentAngle = transform.rotation.eulerAngles.z;
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, newAngle);
        }

        // Move forward in the direction we're facing
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void FindNearestTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = detectionRange;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        target = closestEnemy;
    }
} 