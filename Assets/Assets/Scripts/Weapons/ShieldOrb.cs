using UnityEngine;

public class ShieldOrb : MonoBehaviour
{
    [Header("Shield Settings")]
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float orbitRadius = 1.5f;
    [SerializeField] private float damageLevel3 = 1f;
    
    private Transform player;
    private float angleOffset;
    private bool canDealDamage;
    private float startTime;

    public void Initialize(Transform playerTransform, float startAngle, bool level3Enabled, float initStartTime)
    {
        player = playerTransform;
        angleOffset = startAngle;
        canDealDamage = level3Enabled;
        startTime = initStartTime;
    }

    private void Update()
    {
        if (player == null) return;

        // Calculate current angle based on time since start
        float currentAngle = (Time.time - startTime) * rotationSpeed + angleOffset;
        
        // Calculate position around player
        float rad = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(
            Mathf.Cos(rad) * orbitRadius,
            Mathf.Sin(rad) * orbitRadius,
            0
        );
        
        transform.position = player.position + offset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check for enemy projectiles
        if (other.CompareTag("EnemyProjectile"))
        {
            Destroy(other.gameObject);
        }

        // Deal damage to enemies if level 3
        if (canDealDamage && other.CompareTag("Enemy"))
        {
            // Deal damage to enemy (we'll implement this later)
            // IEnemy enemy = other.GetComponent<IEnemy>();
            // if (enemy != null) enemy.TakeDamage(damageLevel3);
        }
    }

    public bool CanDealDamage()
    {
        return canDealDamage;
    }
} 