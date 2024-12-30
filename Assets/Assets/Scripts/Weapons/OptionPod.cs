using UnityEngine;
using System.Collections.Generic;

public class OptionPod : MonoBehaviour
{
    [Header("Pod Settings")]
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float minimumPlayerMovement = 0.01f;
    [SerializeField] private float historyUpdateInterval = 0.02f;
    
    private Transform player;
    private Queue<Vector3> positionHistory;
    private Vector3 lastPlayerPosition;
    private float lastHistoryUpdateTime;
    private float assignedOffset;
    private int historySize = 30;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        positionHistory = new Queue<Vector3>();
        lastPlayerPosition = player.position;
        
        // Initialize position history with proper spacing
        Vector3 initialPos = player.position;
        for (int i = 0; i < historySize; i++)
        {
            positionHistory.Enqueue(initialPos - (Vector3.left * assignedOffset));
        }
    }

    private void Update()
    {
        if (player == null) return;

        float playerMovement = Vector3.Distance(lastPlayerPosition, player.position);
        if (playerMovement >= minimumPlayerMovement && Time.time >= lastHistoryUpdateTime + historyUpdateInterval)
        {
            // Add new position
            positionHistory.Enqueue(player.position);
            lastPlayerPosition = player.position;
            lastHistoryUpdateTime = Time.time;

            // Maintain history size
            while (positionHistory.Count > historySize)
            {
                positionHistory.Dequeue();
            }

            // Get target position from history based on offset
            int targetIndex = Mathf.Min(
                Mathf.FloorToInt(assignedOffset * 5), 
                positionHistory.Count - 1
            );
            Vector3[] positions = positionHistory.ToArray();
            Vector3 targetPosition = positions[positions.Length - 1 - targetIndex];

            // Move towards target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    public void SetOffset(float podIndex)
    {
        assignedOffset = 1f * (podIndex); // Reduced from 0.4f to 0.25f for even tighter formation
        
        if (player != null)
        {
            // Set initial position with correct offset
            transform.position = player.position + (Vector3.left * assignedOffset);
            
            // Reset history with new offset
            positionHistory.Clear();
            Vector3 initialPos = player.position;
            for (int i = 0; i < historySize; i++)
            {
                positionHistory.Enqueue(initialPos);
            }
        }
    }

    public void FireWeapon(string projectileTag, Quaternion rotation)
    {
        ObjectPool.Instance.SpawnFromPool(projectileTag, transform.position, rotation);
    }
} 