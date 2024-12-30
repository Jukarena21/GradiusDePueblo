using UnityEngine;
using System.Collections.Generic;

public enum WeaponType
{
    Basic,      // Default weapon
    Missile,    // Homing missiles
    Double,     // Shoots at an angle
    Twin,       // Two parallel shots
    Option,     // Floating pods that shoot with player
    Question,   // "?" weapon
    Exclamation // "!" weapon
}

public class WeaponSystem : MonoBehaviour
{
    [Header("Basic Attack Settings")]
    [SerializeField] private float basicFireRate = 0.2f;
    [SerializeField] private GameObject basicProjectilePrefab;
    [SerializeField] private Transform firePoint; // Reference to the point where projectiles spawn
    
    [Header("Weapon Upgrade Settings")]
    [SerializeField] private int maxWeaponLevel = 3;
    
    [Header("Missile Settings")]
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private float missileFireRate = 0.5f;
    [SerializeField] private float missileSpreadAngle = 15f; // For multiple missiles

    [Header("Double Shot Settings")]
    [SerializeField] private GameObject doubleProjectilePrefab;
    [SerializeField] private float doubleFireRate = 0.3f;
    [SerializeField] private float doubleAngle = 30f;
    private float lastDoubleFireTime;

    [Header("Twin Shot Settings")]
    [SerializeField] private GameObject twinProjectilePrefab;
    [SerializeField] private float twinFireRate = 0.25f;
    [SerializeField] private float twinSeparation = 0.3f; // Distance between parallel shots
    private float lastTwinFireTime;

    [Header("Option Settings")]
    [SerializeField] private GameObject optionPodPrefab;
    private List<OptionPod> activePods = new List<OptionPod>();

    [Header("Debug Weapon Settings")]
    [SerializeField] private bool missileUnlocked = false;
    [SerializeField] private bool doubleUnlocked = false;
    [SerializeField] private bool twinUnlocked = false;
    [SerializeField] private bool optionUnlocked = false;
    [SerializeField] private bool questionUnlocked = false;
    [SerializeField] private bool exclamationUnlocked = false;

    [Header("Debug Weapon Levels")]
    [SerializeField, Range(0, 3)] private int missileLevel = 0;
    [SerializeField, Range(0, 3)] private int doubleLevel = 0;
    [SerializeField, Range(0, 3)] private int twinLevel = 0;
    [SerializeField, Range(0, 3)] private int optionLevel = 0;
    [SerializeField, Range(0, 3)] private int questionLevel = 0;
    [SerializeField, Range(0, 3)] private int exclamationLevel = 0;

    [Header("Shield Settings")]
    [SerializeField] private GameObject shieldOrbPrefab;
    private List<ShieldOrb> activeShieldOrbs = new List<ShieldOrb>();

    [Header("Bomb Settings")]
    [SerializeField] private GameObject bombEffectPrefab;
    [SerializeField] private float bombCooldown = 2f;
    private float lastBombTime;

    // Dictionary to track weapon levels
    private Dictionary<WeaponType, int> weaponLevels = new Dictionary<WeaponType, int>();
    private Dictionary<WeaponType, bool> unlockedWeapons = new Dictionary<WeaponType, bool>();
    
    // Cooldown tracking
    private float lastFireTime;
    private float lastMissileFireTime;
    
    private void Awake()
    {
        InitializeWeapons();
    }

    private void InitializeWeapons()
    {
        // Initialize all weapons with level 0
        foreach (WeaponType weapon in System.Enum.GetValues(typeof(WeaponType)))
        {
            weaponLevels[weapon] = 0;
            unlockedWeapons[weapon] = weapon == WeaponType.Basic;
        }
        
        // Set basic weapon to level 1
        weaponLevels[WeaponType.Basic] = 1;

        // Apply debug settings
        CheckDebugChanges();
    }

    private void Update()
    {
        // Handle firing input
        if (Input.GetKey(KeyCode.Space) && Time.time >= lastFireTime + basicFireRate)
        {
            FireWeapons();
            lastFireTime = Time.time;
        }
    }

    private void FireWeapons()
    {
        // Fire basic weapon
        if (weaponLevels[WeaponType.Basic] > 0)
        {
            FireBasicWeapon();
        }

        // Fire additional unlocked weapons
        foreach (var weapon in unlockedWeapons)
        {
            if (weapon.Value && weapon.Key != WeaponType.Basic)
            {
                FireWeaponType(weapon.Key);
            }
        }
    }

    private void FireBasicWeapon()
    {
        if (basicProjectilePrefab != null)
        {
            Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
            ObjectPool.Instance.SpawnFromPool("BasicProjectile", spawnPosition, Quaternion.identity);
            
            // Fire from all active option pods
            foreach (var pod in activePods)
            {
                pod.FireWeapon("BasicProjectile", Quaternion.identity);
            }
        }
    }

    private void FireWeaponType(WeaponType type)
    {
        int currentLevel = weaponLevels[type];
        Debug.Log($"Attempting to fire weapon: {type} at level {currentLevel}");
        
        switch (type)
        {
            case WeaponType.Missile:
                FireMissiles(currentLevel);
                break;
            case WeaponType.Double:
                FireDouble(currentLevel);
                break;
            case WeaponType.Twin:
                FireTwin(currentLevel);
                break;
            case WeaponType.Option:
                FireOptions(currentLevel);
                break;
            case WeaponType.Question:
                FireQuestion(currentLevel);
                break;
            case WeaponType.Exclamation:
                FireExclamation(currentLevel);
                break;
        }
    }

    // Placeholder methods for different weapon types
    private void FireMissiles(int level)
    {
        if (Time.time < lastMissileFireTime + missileFireRate) return;

        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        
        void FireMissileWithRotation(Vector3 pos, Quaternion rot)
        {
            ObjectPool.Instance.SpawnFromPool("MissileProjectile", pos, rot);
            foreach (var pod in activePods)
            {
                pod.FireWeapon("MissileProjectile", rot);
            }
        }

        switch (level)
        {
            case 1:
                FireMissileWithRotation(spawnPosition, Quaternion.identity);
                break;
            case 2:
                FireMissileWithRotation(spawnPosition, Quaternion.Euler(0, 0, missileSpreadAngle));
                FireMissileWithRotation(spawnPosition, Quaternion.Euler(0, 0, -missileSpreadAngle));
                break;
            case 3:
                FireMissileWithRotation(spawnPosition, Quaternion.identity);
                FireMissileWithRotation(spawnPosition, Quaternion.Euler(0, 0, missileSpreadAngle));
                FireMissileWithRotation(spawnPosition, Quaternion.Euler(0, 0, -missileSpreadAngle));
                break;
        }

        lastMissileFireTime = Time.time;
    }
    private void FireDouble(int level)
    {
        if (Time.time < lastDoubleFireTime + doubleFireRate) return;

        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        
        switch (level)
        {
            case 1:
                ObjectPool.Instance.SpawnFromPool("DoubleProjectile", spawnPosition, Quaternion.Euler(0, 0, doubleAngle));
                ObjectPool.Instance.SpawnFromPool("DoubleProjectile", spawnPosition, Quaternion.Euler(0, 0, -doubleAngle));
                break;
            case 2:
                ObjectPool.Instance.SpawnFromPool("DoubleProjectile", spawnPosition, Quaternion.Euler(0, 0, doubleAngle));
                ObjectPool.Instance.SpawnFromPool("DoubleProjectile", spawnPosition, Quaternion.Euler(0, 0, -doubleAngle));
                ObjectPool.Instance.SpawnFromPool("DoubleProjectile", spawnPosition, Quaternion.Euler(0, 0, doubleAngle/2));
                ObjectPool.Instance.SpawnFromPool("DoubleProjectile", spawnPosition, Quaternion.Euler(0, 0, -doubleAngle/2));
                break;
            case 3:
                ObjectPool.Instance.SpawnFromPool("DoubleProjectile", spawnPosition, Quaternion.Euler(0, 0, doubleAngle));
                ObjectPool.Instance.SpawnFromPool("DoubleProjectile", spawnPosition, Quaternion.Euler(0, 0, -doubleAngle));
                ObjectPool.Instance.SpawnFromPool("DoubleProjectile", spawnPosition, Quaternion.Euler(0, 0, doubleAngle/2));
                ObjectPool.Instance.SpawnFromPool("DoubleProjectile", spawnPosition, Quaternion.Euler(0, 0, -doubleAngle/2));
                ObjectPool.Instance.SpawnFromPool("DoubleProjectile", spawnPosition, Quaternion.Euler(0, 0, doubleAngle/3));
                ObjectPool.Instance.SpawnFromPool("DoubleProjectile", spawnPosition, Quaternion.Euler(0, 0, -doubleAngle/3));
                break;
        }

        lastDoubleFireTime = Time.time;
    }
    private void FireTwin(int level)
    {
        if (Time.time < lastTwinFireTime + twinFireRate) return;

        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        Vector3 upOffset = transform.up * twinSeparation;
        
        switch (level)
        {
            case 1:
                ObjectPool.Instance.SpawnFromPool("TwinProjectile", spawnPosition + upOffset/2, Quaternion.identity);
                ObjectPool.Instance.SpawnFromPool("TwinProjectile", spawnPosition - upOffset/2, Quaternion.identity);
                break;
            case 2:
                ObjectPool.Instance.SpawnFromPool("TwinProjectile", spawnPosition + upOffset, Quaternion.identity);
                ObjectPool.Instance.SpawnFromPool("TwinProjectile", spawnPosition, Quaternion.identity);
                ObjectPool.Instance.SpawnFromPool("TwinProjectile", spawnPosition - upOffset, Quaternion.identity);
                break;
            case 3:
                ObjectPool.Instance.SpawnFromPool("TwinProjectile", spawnPosition + upOffset, Quaternion.identity);
                ObjectPool.Instance.SpawnFromPool("TwinProjectile", spawnPosition + upOffset/2, Quaternion.identity);
                ObjectPool.Instance.SpawnFromPool("TwinProjectile", spawnPosition - upOffset/2, Quaternion.identity);
                ObjectPool.Instance.SpawnFromPool("TwinProjectile", spawnPosition - upOffset, Quaternion.identity);
                break;
        }

        lastTwinFireTime = Time.time;
    }
    private void FireOptions(int level)
    {
        // Only manage pod existence, they will fire automatically with the player
        UpdateOptionPods(level);
    }
    private void FireQuestion(int level)
    {
        // Empty - shields are managed by UpdateShieldOrbs
    }
    private void FireExclamation(int level)
    {
        if (Time.time < lastBombTime + bombCooldown) return;
        
        if (bombEffectPrefab != null)
        {
            // Spawn bomb effect at player position
            GameObject bombEffect = Instantiate(bombEffectPrefab, transform.position, Quaternion.identity);
            
            // Lock the weapon after use
            unlockedWeapons[WeaponType.Exclamation] = false;
            exclamationUnlocked = false; // Update debug checkbox
            
            lastBombTime = Time.time;
        }
    }

    private void UpdateOptionPods(int level)
    {
        int desiredPodCount = level;
        
        // Remove excess pods if level decreased
        while (activePods.Count > desiredPodCount)
        {
            int lastIndex = activePods.Count - 1;
            if (activePods[lastIndex] != null)
            {
                Destroy(activePods[lastIndex].gameObject);
            }
            activePods.RemoveAt(lastIndex);
        }
        
        // Add new pods if level increased
        while (activePods.Count < desiredPodCount)
        {
            if (optionPodPrefab != null)
            {
                // Calculate offset based on pod index (1, 2, or 3)
                float offset = (activePods.Count + 1);
                Vector3 spawnPosition = transform.position + Vector3.left * offset;
                
                GameObject newPod = Instantiate(optionPodPrefab, spawnPosition, Quaternion.identity);
                OptionPod podComponent = newPod.GetComponent<OptionPod>();
                if (podComponent != null)
                {
                    podComponent.SetOffset(offset);
                    activePods.Add(podComponent);
                }
            }
        }
    }

    private void UpdateShieldOrbs(int level)
    {
        int desiredOrbCount = level;
        float baseAngle = 360f / desiredOrbCount; // Total angle divided by number of orbs
        float startTime = Time.time; // Use the same start time for all orbs
        
        // Remove excess orbs if level decreased
        while (activeShieldOrbs.Count > desiredOrbCount)
        {
            int lastIndex = activeShieldOrbs.Count - 1;
            if (activeShieldOrbs[lastIndex] != null)
            {
                Destroy(activeShieldOrbs[lastIndex].gameObject);
            }
            activeShieldOrbs.RemoveAt(lastIndex);
        }
        
        // First, destroy all existing orbs if count changed
        if (activeShieldOrbs.Count > 0 && activeShieldOrbs.Count != desiredOrbCount)
        {
            foreach (var orb in activeShieldOrbs)
            {
                if (orb != null) Destroy(orb.gameObject);
            }
            activeShieldOrbs.Clear();
        }
        
        // Then create all new orbs at once
        if (activeShieldOrbs.Count == 0 && desiredOrbCount > 0)
        {
            for (int i = 0; i < desiredOrbCount; i++)
            {
                if (shieldOrbPrefab != null)
                {
                    float angle = baseAngle * i;
                    GameObject newOrb = Instantiate(shieldOrbPrefab, transform.position, Quaternion.identity);
                    ShieldOrb orbComponent = newOrb.GetComponent<ShieldOrb>();
                    
                    if (orbComponent != null)
                    {
                        orbComponent.Initialize(transform, angle, level == 3, startTime);
                        activeShieldOrbs.Add(orbComponent);
                    }
                }
            }
        }
    }

    // Public methods for power-up system
    public void UnlockWeapon(WeaponType type)
    {
        if (!unlockedWeapons[type])
        {
            unlockedWeapons[type] = true;
            weaponLevels[type] = 1; // Always start at level 1 when unlocking

            // Special handling for specific weapon types
            switch (type)
            {
                case WeaponType.Question:
                    UpdateShieldOrbs(1);
                    break;
                case WeaponType.Option:
                    UpdateOptionPods(1);
                    break;
            }
        }
    }

    public void UpgradeWeapon(WeaponType type)
    {
        if (unlockedWeapons[type] && weaponLevels[type] < maxWeaponLevel)
        {
            weaponLevels[type]++;

            // Special handling for specific weapon types
            switch (type)
            {
                case WeaponType.Question:
                    UpdateShieldOrbs(weaponLevels[type]);
                    break;
                case WeaponType.Option:
                    UpdateOptionPods(weaponLevels[type]);
                    break;
            }
        }
    }

    public bool IsWeaponUnlocked(WeaponType type)
    {
        return unlockedWeapons[type];
    }

    public int GetWeaponLevel(WeaponType type)
    {
        return weaponLevels[type];
    }

    // Add this to handle cleanup when the weapon system is destroyed
    private void OnDestroy()
    {
        // Clean up option pods
        foreach (var pod in activePods)
        {
            if (pod != null) Destroy(pod.gameObject);
        }
        activePods.Clear();

        // Clean up shield orbs
        foreach (var orb in activeShieldOrbs)
        {
            if (orb != null) Destroy(orb.gameObject);
        }
        activeShieldOrbs.Clear();
    }

    // Add this method to check for debug changes in Update
    private void CheckDebugChanges()
    {
        // This method should be removed or commented out
        // It's overriding the powerup system's weapon levels
    }
} 