using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombEffect : MonoBehaviour
{
    [Header("Bomb Settings")]
    [SerializeField] private float expandDuration = 0.3f;
    [SerializeField] private float maxRadius = 10f;
    [SerializeField] private float bossDamage = 50f;
    [SerializeField] private Color bombColor = Color.white;
    
    [Header("Visual Effects")]
    [SerializeField] private float fadeOutDuration = 0.2f;
    
    private SpriteRenderer spriteRenderer;
    private float currentRadius;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = bombColor;
        currentRadius = 0f;
        
        StartCoroutine(BombSequence());
    }

    private IEnumerator BombSequence()
    {
        // Expand effect
        float elapsedTime = 0f;
        while (elapsedTime < expandDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / expandDuration;
            
            // Update scale
            currentRadius = Mathf.Lerp(0, maxRadius, progress);
            transform.localScale = Vector3.one * (currentRadius * 2);
            
            // Destroy enemies within radius
            DestroyEnemiesInRadius();
            
            yield return null;
        }

        // Fade out
        elapsedTime = 0f;
        Color startColor = bombColor;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void DestroyEnemiesInRadius()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, currentRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy") && !collider.CompareTag("Boss"))
            {
                EnemyBase enemy = collider.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.TakeDamage(999); // This will trigger the pooling system through TakeDamage
                }
            }
            else if (collider.CompareTag("EnemyProjectile"))
            {
                ObjectPool.Instance.ReturnToPool("EnemyProjectile", collider.gameObject);
            }
        }
    }
} 