using UnityEngine;

public class BasicProjectile : ProjectileBase
{
    [Header("Basic Projectile VFX")]
    [SerializeField] private GameObject hitEffectPrefab;

    private void Awake()
    {
        poolTag = "BasicProjectile";
    }

    protected override void OnHit()
    {
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
        base.OnHit();
    }
} 