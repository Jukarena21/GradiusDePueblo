using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Vector2 scrollSpeedRange = new Vector2(1f, 2f);
    [SerializeField] private bool shouldLoop = true;
    [SerializeField] private bool randomizeSpeedOnStart = true;
    
    private float rotationSpeed = 0f;
    private float length;
    private float startPosX;
    private Vector2 offset;
    private Material material;
    private SpriteRenderer spriteRenderer;
    private float currentScrollSpeed;

    private void Start()
    {
        startPosX = transform.position.x;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            // For sprite-based backgrounds
            length = spriteRenderer.bounds.size.x;
        }
        else
        {
            // For material-based backgrounds (like stars)
            material = GetComponent<Renderer>().material;
        }

        // Set random scroll speed within range
        if (randomizeSpeedOnStart)
        {
            currentScrollSpeed = Random.Range(scrollSpeedRange.x, scrollSpeedRange.y);
        }
        else
        {
            currentScrollSpeed = scrollSpeedRange.x;
        }
    }

    private void Update()
    {
        if (material != null)
        {
            // For material-based scrolling (like stars)
            offset = material.mainTextureOffset;
            offset.x -= Time.deltaTime * currentScrollSpeed;
            material.mainTextureOffset = offset;
        }
        else
        {
            // For sprite-based parallax
            // Move left based on scroll speed
            transform.position += Vector3.left * (currentScrollSpeed * Time.deltaTime);

            // Apply rotation
            if (rotationSpeed != 0)
            {
                transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            }

            // Check if we need to loop
            if (shouldLoop && transform.position.x <= startPosX - length)
            {
                transform.position = new Vector3(startPosX, transform.position.y, transform.position.z);
                
                // Optionally randomize speed again when looping
                if (randomizeSpeedOnStart)
                {
                    currentScrollSpeed = Random.Range(scrollSpeedRange.x, scrollSpeedRange.y);
                }
            }
        }
    }

    // Method to manually set scroll speed (useful for external control)
    public void SetScrollSpeed(float speed)
    {
        currentScrollSpeed = speed;
    }

    // Method to manually set scroll speed range and randomize
    public void SetScrollSpeedRange(float min, float max)
    {
        scrollSpeedRange = new Vector2(min, max);
        if (randomizeSpeedOnStart)
        {
            currentScrollSpeed = Random.Range(scrollSpeedRange.x, scrollSpeedRange.y);
        }
    }

    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }

    public float GetMinScrollSpeed()
    {
        return scrollSpeedRange.x;
    }
} 