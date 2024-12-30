using UnityEngine;

public class EnemyProjectile : MonoBehaviour, IPooledObject
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float destroyBoundary = -12f;
    private float spawnTime;

    public void OnObjectSpawn()
    {
        spawnTime = Time.time;
    }

    private void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
        
        if (transform.position.x < destroyBoundary || Time.time > spawnTime + lifetime)
        {
            ObjectPool.Instance.ReturnToPool("EnemyProjectile", gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Deal damage to player (we'll implement this later)
            ObjectPool.Instance.ReturnToPool("EnemyProjectile", gameObject);
        }
    }
} 