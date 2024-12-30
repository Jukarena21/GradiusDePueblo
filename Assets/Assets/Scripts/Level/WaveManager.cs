using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private List<WaveScriptableObject> waves;
    [SerializeField] private float delayBetweenWaves = 3f;
    [SerializeField] private float baseHealthMultiplier = 1f;
    [SerializeField] private float healthMultiplierIncreasePerWave = 0.1f;

    [Header("Spawn Settings")]
    [SerializeField] private float rightSpawnX = 12f;
    [SerializeField] private float minY = -4f;
    [SerializeField] private float maxY = 4f;

    [Header("Powerup System")]
    [SerializeField] private WeaponSystem weaponSystem;
    [SerializeField] private PowerupMessage powerupMessage;
    [SerializeField] private int groupsPerPowerup = 3;
    private List<WeaponType> availableWeapons;
    private Dictionary<WeaponType, int> weaponLevels = new Dictionary<WeaponType, int>();
    private int groupsCleared = 0;

    private int currentWaveIndex = 0;
    private bool isSpawningWave = false;
    private List<GameObject> activeEnemies = new List<GameObject>();

    public int CurrentWave => currentWaveIndex;

    private void Start()
    {
        ScoreManager.Instance.ResetScore();
        InitializeAvailableWeapons();
        StartNextWave();
        Debug.Log("WaveManager started. Groups per powerup: " + groupsPerPowerup);
    }

    private void InitializeAvailableWeapons()
    {
        // Define all possible weapons
        WeaponType[] allWeapons = new WeaponType[]
        {
            WeaponType.Twin,      
            WeaponType.Double,    
            WeaponType.Missile,   
            WeaponType.Question,  
            WeaponType.Option     
        };

        // Initialize the list and dictionary
        availableWeapons = new List<WeaponType>(allWeapons);
        foreach (var weapon in allWeapons)
        {
            weaponLevels[weapon] = 0;
        }

        Debug.Log($"Available weapons initialized. Count: {availableWeapons.Count}");
    }

    private void StartNextWave()
    {
        if (currentWaveIndex < waves.Count)
        {
            Debug.Log($"Starting wave {currentWaveIndex + 1} of {waves.Count}");
            StartCoroutine(SpawnWave(waves[currentWaveIndex]));
            currentWaveIndex++;
        }
        else
        {
            Debug.Log("All waves completed! Game finished!");
            // Handle game completion
            var gameOverScreen = FindObjectOfType<GameOverScreen>();
            if (gameOverScreen != null)
            {
                gameOverScreen.ShowGameOver(true); // Show victory screen
            }
        }
    }

    private IEnumerator SpawnWave(WaveScriptableObject wave)
    {
        if (wave == null)
        {
            Debug.LogError("Wave is null!");
            yield break;
        }

        isSpawningWave = true;
        Debug.Log($"Starting Wave: {wave.waveName}");

        yield return new WaitForSeconds(wave.delayBeforeWave);

        if (wave.enemyGroups == null || wave.enemyGroups.Count == 0)
        {
            Debug.LogError($"Wave {wave.waveName} has no enemy groups!");
            yield break;
        }

        for (int groupIndex = 0; groupIndex < wave.enemyGroups.Count; groupIndex++)
        {
            var enemyGroup = wave.enemyGroups[groupIndex];
            Debug.Log($"Processing enemy group {groupIndex + 1} of {wave.enemyGroups.Count} in {wave.waveName}");

            if (enemyGroup == null)
            {
                Debug.LogError("Enemy group is null!");
                continue;
            }

            yield return StartCoroutine(SpawnFormation(enemyGroup, wave.modifiers));

            if (enemyGroup.waitForPreviousGroup || groupIndex == wave.enemyGroups.Count - 1)
            {
                yield return StartCoroutine(WaitForEnemiesCleared());
            }
        }

        Debug.Log($"Wave {wave.waveName} completed!");
        isSpawningWave = false;

        // Start next wave if there is one
        if (currentWaveIndex < waves.Count)
        {
            Debug.Log($"Moving to next wave. Current index: {currentWaveIndex}, Total waves: {waves.Count}");
            StartNextWave();
        }
        else
        {
            Debug.Log("All waves completed! Game finished!");
            // Handle game completion
            var gameOverScreen = FindObjectOfType<GameOverScreen>();
            if (gameOverScreen != null)
            {
                gameOverScreen.ShowGameOver(true); // Show victory screen
            }
        }
    }

    private IEnumerator SpawnFormation(WaveScriptableObject.EnemySpawnData enemyGroup, WaveScriptableObject.WaveModifiers modifiers)
    {
        if (enemyGroup == null || enemyGroup.formation == null)
        {
            Debug.LogError("Enemy group or formation is null!");
            yield break;
        }

        Debug.Log($"Starting to spawn formation with {enemyGroup.enemyCount} enemies. Tag: {enemyGroup.enemyPoolTag}");
        
        if (string.IsNullOrEmpty(enemyGroup.enemyPoolTag))
        {
            Debug.LogError("Enemy pool tag is null or empty!");
            yield break;
        }

        float totalHealthMultiplier = baseHealthMultiplier * 
                                    (1 + healthMultiplierIncreasePerWave * currentWaveIndex) * 
                                    modifiers.healthMultiplier * 
                                    enemyGroup.healthMultiplier;

        int spawnedCount = 0;
        for (int i = 0; i < enemyGroup.enemyCount; i++)
        {
            foreach (var point in enemyGroup.formation.points)
            {
                yield return new WaitForSeconds(point.spawnDelay);

                Vector3 spawnPosition = new Vector3(
                    rightSpawnX + point.relativePosition.x,
                    enemyGroup.spawnPosition.y + point.relativePosition.y,
                    0
                );

                GameObject enemy = ObjectPool.Instance.SpawnFromPool(
                    enemyGroup.enemyPoolTag,
                    spawnPosition,
                    Quaternion.identity
                );

                if (enemy != null)
                {
                    EnemyBase enemyComponent = enemy.GetComponent<EnemyBase>();
                    if (enemyComponent != null)
                    {
                        enemyComponent.InitializeInFormation(
                            point.moveDirection,
                            point.moveSpeed * modifiers.speedMultiplier,
                            totalHealthMultiplier,
                            modifiers.fireRateMultiplier,
                            modifiers.projectileSpeedMultiplier
                        );
                    }

                    activeEnemies.Add(enemy);
                    spawnedCount++;
                    Debug.Log($"Spawned enemy {spawnedCount}, active count: {activeEnemies.Count}");
                }
                else
                {
                    Debug.LogError($"Failed to spawn enemy with tag: {enemyGroup.enemyPoolTag}");
                }
            }
        }

        Debug.Log($"Formation spawn complete. Total spawned: {spawnedCount}");
    }

    private void OnEnemyGroupCleared()
    {
        groupsCleared++;
        Debug.Log($"Group cleared! Total groups cleared: {groupsCleared}");
        Debug.Log($"Groups until powerup: {groupsPerPowerup - (groupsCleared % groupsPerPowerup)}");

        if (groupsCleared % groupsPerPowerup == 0)
        {
            Debug.Log("Powerup condition met!");
            GivePlayerPowerup();
        }
    }

    private IEnumerator WaitForEnemiesCleared()
    {
        Debug.Log($"WaitForEnemiesCleared started with {activeEnemies.Count} enemies");
        
        while (activeEnemies.Count > 0)
        {
            activeEnemies.RemoveAll(enemy => enemy == null);
            yield return new WaitForSeconds(0.5f);
        }

        // Group is cleared, increment counter and check for powerup
        groupsCleared++;
        Debug.Log($"Group cleared! Total groups cleared: {groupsCleared}");
        Debug.Log($"Groups until powerup: {groupsPerPowerup - (groupsCleared % groupsPerPowerup)}");

        if (groupsCleared % groupsPerPowerup == 0)
        {
            Debug.Log("Powerup condition met!");
            GivePlayerPowerup();
        }
    }

    private void GivePlayerPowerup()
    {
        if (availableWeapons.Count == 0)
        {
            Debug.Log("No more weapons to upgrade");
            return;
        }

        if (weaponSystem == null)
        {
            Debug.LogError("WeaponSystem reference is missing!");
            return;
        }

        // Get list of weapons that aren't maxed out
        List<WeaponType> validWeapons = availableWeapons.FindAll(w => 
            weaponLevels[w] < 3 || !weaponSystem.IsWeaponUnlocked(w));

        if (validWeapons.Count == 0)
        {
            Debug.Log("All weapons are maxed out!");
            return;
        }

        // Randomly select a weapon from valid ones
        int randomIndex = Random.Range(0, validWeapons.Count);
        WeaponType selectedWeapon = validWeapons[randomIndex];

        bool isUnlock = !weaponSystem.IsWeaponUnlocked(selectedWeapon);
        Debug.Log($"Selected weapon: {selectedWeapon}, isUnlock: {isUnlock}");

        if (isUnlock)
        {
            weaponSystem.UnlockWeapon(selectedWeapon);
            weaponLevels[selectedWeapon] = 1;
            powerupMessage.ShowPowerupMessage(selectedWeapon, 1, true);
            Debug.Log($"Unlocked {selectedWeapon}");
        }
        else
        {
            int oldLevel = weaponSystem.GetWeaponLevel(selectedWeapon);
            weaponSystem.UpgradeWeapon(selectedWeapon);
            int newLevel = weaponSystem.GetWeaponLevel(selectedWeapon);
            weaponLevels[selectedWeapon] = newLevel;
            powerupMessage.ShowPowerupMessage(selectedWeapon, newLevel, false);
            Debug.Log($"Upgraded {selectedWeapon} from level {oldLevel} to {newLevel}");
        }

        // If weapon is maxed out, remove it from available weapons
        if (weaponLevels[selectedWeapon] >= 3)
        {
            availableWeapons.Remove(selectedWeapon);
            Debug.Log($"Removed {selectedWeapon} from available weapons. Remaining: {availableWeapons.Count}");
        }
    }

    private void Update()
    {
        // Clean up null references from destroyed enemies
        activeEnemies.RemoveAll(enemy => enemy == null);
    }

    // Call this when an enemy is destroyed
    public void OnEnemyDestroyed(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            Debug.Log($"Enemy destroyed! Remaining enemies: {activeEnemies.Count}");
            
            // Add score based on enemy type and current wave
            int baseScore = 0;
            if (enemy.GetComponent<FastPatternEnemy>())
                baseScore = 150;
            else if (enemy.GetComponent<SlowPatternEnemy>())
                baseScore = 100;
            else if (enemy.GetComponent<StraightLineEnemy>())
                baseScore = 50;

            int finalScore = baseScore * CurrentWave;
            ScoreManager.Instance.AddScore(finalScore);
        }
        else
        {
            Debug.LogWarning("Enemy destroyed but not found in activeEnemies list!");
        }
    }
} 