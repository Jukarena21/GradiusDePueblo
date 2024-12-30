using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Formation", menuName = "Game/Formation Data")]
public class FormationScriptableObject : ScriptableObject
{
    [System.Serializable]
    public class FormationPoint
    {
        public Vector2 relativePosition;  // Position relative to formation center
        public float spawnDelay;          // Delay before spawning this enemy
        public Vector2 moveDirection;      // Initial movement direction
        public float moveSpeed;           // Optional override for enemy speed
    }

    public string formationName;
    public FormationPoint[] points;
    public AnimationCurve pathCurve;      // Optional: For curved paths
    public bool loopPath;                 // Should the formation path loop?
    public float formationSpacing = 1f;   // Space between enemies in formation
    public float formationSpeed = 1f;     // Overall formation movement speed
} 