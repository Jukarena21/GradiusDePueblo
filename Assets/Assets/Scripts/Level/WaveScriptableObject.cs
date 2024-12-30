using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Wave", menuName = "Game/Wave Data")]
public class WaveScriptableObject : ScriptableObject
{
    [System.Serializable]
    public class EnemySpawnData
    {
        public string enemyPoolTag;
        public FormationScriptableObject formation;
        public Vector2 spawnPosition;      // Formation center position
        public float spawnDelay;          // Delay before starting formation
        public int enemyCount;
        public float healthMultiplier = 1f;
        public bool waitForPreviousGroup;  // Wait for previous group to be destroyed?
    }

    [System.Serializable]
    public class WaveModifiers
    {
        public float healthMultiplier = 1f;    // Base health multiplier for all enemies
        public float speedMultiplier = 1f;     // Movement speed multiplier
        public float fireRateMultiplier = 1f;  // How fast enemies shoot
        public float projectileSpeedMultiplier = 1f;
    }

    public string waveName;
    public float delayBeforeWave;
    public List<EnemySpawnData> enemyGroups;  // Changed from enemySpawns to enemyGroups
    public WaveModifiers modifiers;
    public bool waitForWaveClear;         // Wait for all enemies to be destroyed before next wave?
} 