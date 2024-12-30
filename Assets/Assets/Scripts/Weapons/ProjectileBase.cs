using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour, IPooledObject
{
    [Header("Base Projectile Settings")]
    [SerializeField] protected float speed = 10f;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected float lifetime = 3f;
    
    protected string poolTag;
    private float spawnTime;
    
    public virtual void OnObjectSpawn()
    {
        spawnTime = Time.time;
    }

    protected virtual void Update()
    {
        Move();
        
        // Check lifetime
        if (Time.time > spawnTime + lifetime)
        {
            ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
        }
    }

    protected virtual void Move()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // Check if hit enemy
        if (other.CompareTag("Enemy"))
        {
            // Deal damage to enemy (we'll implement this later)
            // IEnemy enemy = other.GetComponent<IEnemy>();
            // if (enemy != null) enemy.TakeDamage(damage);
            
            OnHit();
        }
    }

    protected virtual void OnHit()
    {
        ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
    }

    protected virtual void Start()
    {
        // Any base start logic here
    }
} 