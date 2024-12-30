using UnityEngine;

public class DoubleProjectile : ProjectileBase
{
    [Header("Double Shot VFX")]
    [SerializeField] private GameObject hitEffectPrefab;

    protected override void OnHit()
    {
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
        base.OnHit();
    }

    private void Awake()
    {
        poolTag = "DoubleProjectile";
    }
} 