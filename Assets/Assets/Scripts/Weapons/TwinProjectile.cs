using UnityEngine;

public class TwinProjectile : ProjectileBase
{
    [Header("Twin Shot VFX")]
    [SerializeField] private GameObject hitEffectPrefab;

    private void Awake()
    {
        poolTag = "TwinProjectile";
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