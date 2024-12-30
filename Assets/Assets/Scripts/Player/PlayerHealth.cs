using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private GameOverScreen gameOverScreen;
    
    private void Start()
    {
        gameOverScreen = FindObjectOfType<GameOverScreen>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("EnemyProjectile"))
        {
            Die();
        }
    }

    private void Die()
    {
        gameObject.SetActive(false); // Hide the player
        gameOverScreen?.ShowGameOver(false); // Show game over screen with defeat
    }
} 