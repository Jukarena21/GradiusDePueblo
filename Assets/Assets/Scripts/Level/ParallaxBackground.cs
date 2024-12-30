using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxElement
    {
        public GameObject prefab;
        public int sortingOrder;
        public Vector2 spawnInterval; // For randomly spawned objects like meteors
        public bool randomizeScale = true;
        public Vector2 scaleRange = new Vector2(0.5f, 2f);
        public bool randomizeRotation = true;
        public Vector2 rotationSpeedRange = new Vector2(-30f, 30f);
    }

    [Header("Background Elements")]
    [SerializeField] private ParallaxElement starfieldLayer;
    [SerializeField] private ParallaxElement[] pulsarLayers;
    [SerializeField] private ParallaxElement[] planetLayers;
    [SerializeField] private ParallaxElement[] debrisLayers;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRightX = 12f;
    [SerializeField] private float despawnLeftX = -12f;
    [SerializeField] private float minY = -6f;
    [SerializeField] private float maxY = 6f;

    private void Start()
    {
        // Initialize starfield (continuous background)
        if (starfieldLayer.prefab != null)
        {
            GameObject starfield = Instantiate(starfieldLayer.prefab, Vector3.zero, Quaternion.identity, transform);
            SetupParallaxLayer(starfield, starfieldLayer);
        }

        // Start spawning other elements with longer intervals
        if (pulsarLayers != null && pulsarLayers.Length > 0)
        {
            // Spawn one pulsar every 8-15 seconds
            InvokeRepeating(nameof(SpawnPulsar), Random.Range(0f, 3f), Random.Range(8f, 15f));
        }

        if (planetLayers != null && planetLayers.Length > 0)
        {
            // Spawn one planet every 20-30 seconds
            InvokeRepeating(nameof(SpawnPlanet), Random.Range(0f, 5f), Random.Range(20f, 30f));
        }

        if (debrisLayers != null && debrisLayers.Length > 0)
        {
            // Spawn debris every 3-7 seconds
            InvokeRepeating(nameof(SpawnDebris), Random.Range(0f, 2f), Random.Range(3f, 7f));
        }
    }

    private void SetupParallaxLayer(GameObject obj, ParallaxElement element)
    {
        ParallaxLayer layer = obj.GetComponent<ParallaxLayer>();
        if (layer == null) layer = obj.AddComponent<ParallaxLayer>();
        
        // Set sorting order
        SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = element.sortingOrder;
        }

        // Randomize scale if needed
        if (element.randomizeScale)
        {
            float randomScale = Random.Range(element.scaleRange.x, element.scaleRange.y);
            obj.transform.localScale = Vector3.one * randomScale;
        }

        // Set random initial rotation and rotation speed, but not for starfield
        if (element.randomizeRotation && element != starfieldLayer)
        {
            // Random initial rotation
            obj.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
            
            // Set random rotation speed
            float rotationSpeed = Random.Range(element.rotationSpeedRange.x, element.rotationSpeedRange.y);
            layer.SetRotationSpeed(rotationSpeed);
        }
    }

    private void SpawnBackgroundElement(ParallaxElement element)
    {
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(spawnRightX, randomY, 0);
        
        GameObject obj = Instantiate(element.prefab, spawnPosition, Quaternion.identity, transform);
        SetupParallaxLayer(obj, element);

        // Get the ParallaxLayer component to check its scroll speed
        ParallaxLayer layer = obj.GetComponent<ParallaxLayer>();
        if (layer != null)
        {
            // Calculate time needed to move from spawnRightX to despawnLeftX
            float totalDistance = spawnRightX - despawnLeftX;
            float minSpeed = layer.GetMinScrollSpeed();
            
            // Add some buffer time (1.5x) to ensure object is off screen
            float destroyTime = (totalDistance / minSpeed) * 1.5f;
            
            Destroy(obj, destroyTime);
        }
    }

    private void SpawnPulsar()
    {
        // Spawn only one random pulsar instead of all
        if (pulsarLayers.Length > 0)
        {
            int randomIndex = Random.Range(0, pulsarLayers.Length);
            SpawnBackgroundElement(pulsarLayers[randomIndex]);
        }
    }

    private void SpawnPlanet()
    {
        // Spawn only one random planet instead of all
        if (planetLayers.Length > 0)
        {
            int randomIndex = Random.Range(0, planetLayers.Length);
            SpawnBackgroundElement(planetLayers[randomIndex]);
        }
    }

    private void SpawnDebris()
    {
        // Spawn only one random debris instead of all
        if (debrisLayers.Length > 0)
        {
            int randomIndex = Random.Range(0, debrisLayers.Length);
            SpawnBackgroundElement(debrisLayers[randomIndex]);
        }
    }
} 