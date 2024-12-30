using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IPooledObject
{
    [Header("Base Settings")]
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected string projectilePoolTag = "EnemyProjectile";
    [SerializeField] protected float fireRate = 2f;
    [SerializeField] protected float destroyBoundary = -12f;
    [SerializeField] protected int maxHealth = 1;
    
    protected float nextFireTime;
    protected Vector2 direction;
    protected bool canShoot = true;
    protected Transform player;
    protected Rigidbody2D rb;
    protected string poolTag;
    protected int currentHealth;
    protected float currentFireRate;
    protected float projectileSpeedMultiplier = 1f;

    public virtual void InitializeInFormation(Vector2 moveDirection, float speed, float healthMultiplier, float fireRateMultiplier, float projSpeedMult)
    {
        direction = moveDirection.normalized;
        moveSpeed = speed;
        currentHealth = Mathf.RoundToInt(maxHealth * healthMultiplier);
        currentFireRate = fireRate * fireRateMultiplier;
        projectileSpeedMultiplier = projSpeedMult;
        OnObjectSpawn();
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Debug.Log($"Enemy {gameObject.name} health depleted");
            var waveManager = FindObjectOfType<WaveManager>();
            if (waveManager != null)
            {
                waveManager.OnEnemyDestroyed(gameObject);
                Debug.Log("Called OnEnemyDestroyed");
            }
            ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
        }
    }

    public virtual void OnObjectSpawn()
    {
        currentHealth = Mathf.RoundToInt(maxHealth);
        nextFireTime = Time.time + fireRate;
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.isKinematic = true;
        rb.gravityScale = 0;
        gameObject.tag = "Enemy";
    }

    protected virtual void Start()
    {
        nextFireTime = Time.time + fireRate;
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    protected virtual void Update()
    {
        Move();
        if (canShoot) TryShoot();
        CheckBoundary();
    }

    protected virtual void Move()
    {
        // Each enemy will implement its own movement
    }

    protected virtual void TryShoot()
    {
        if (Time.time >= nextFireTime && player != null)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    protected virtual void Shoot()
    {
        if (player != null)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            
            ObjectPool.Instance.SpawnFromPool(projectilePoolTag, transform.position, rotation);
        }
    }

    protected virtual void CheckBoundary()
    {
        if (transform.position.x < destroyBoundary)
        {
            Debug.Log($"Enemy {gameObject.name} reached boundary");
            var waveManager = FindObjectOfType<WaveManager>();
            if (waveManager != null)
            {
                waveManager.OnEnemyDestroyed(gameObject);
                Debug.Log("Called OnEnemyDestroyed from boundary check");
            }
            ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerProjectile") || 
            (other.CompareTag("ShieldOrb") && other.GetComponent<ShieldOrb>()?.CanDealDamage() == true))
        {
            if (other.CompareTag("PlayerProjectile"))
            {
                ObjectPool.Instance.ReturnToPool("PlayerProjectile", other.gameObject);
            }
            
            TakeDamage(1);
        }
    }
} 