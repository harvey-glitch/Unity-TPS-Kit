using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponCategory { primary, secondary }
    [Header("Type")]
    [SerializeField] WeaponCategory category;

    public enum FireMode
    {
        single, auto, burst 
    }
    [Header("Firing Mode")]
    [SerializeField] FireMode fireMode;

    [Header("Rigs")]
    [SerializeField] Transform rightHandGrip;
    [SerializeField] Transform leftHandGrip;

    [Header("Data")]
    [SerializeField] float firerate = 0.2f;
    [SerializeField] float maxRange = 50.0f;
    [SerializeField] int damage = 10;
    [SerializeField] Vector3 recoil;
    [SerializeField] float recoilSpeed = 15f;
    [SerializeField] Transform muzzle;

    [Header("Particles")]
    [SerializeField] GameObject impactEffect;
    [SerializeField] ParticleSystem muzzleFlash;

    [Header("Base Position")]
    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    [Header("Aiming Position Offset")]
    public Vector3 aimPositionOffset;
    public Vector3 aimRotationOffset;

    // Public read-only properties
    public WeaponCategory Category => category;
    public FireMode Mode => fireMode;
    public Transform RightHandGrip => rightHandGrip;
    public Transform LeftHandGrip => leftHandGrip;
    public float Firerate => firerate;
    public float MaxRange => maxRange;
    public float RecoilSpeed => recoilSpeed;
    public Vector3 Recoil => recoil;
    public int Damage => damage;
    public GameObject ImpactEffect => impactEffect;
    public Transform Muzzle => muzzle;

    public void PlayMuzzleFlash()
    {
        muzzleFlash.Play();
    }

    public void InflictDamage(GameObject target)
    {
        if (target.TryGetComponent(out HealthBase health))
        {
            health.OnDamageTaken(damage);
        }
    }

    public Vector3 GetSpreadDirection(Vector3 forward, float spreadAngle)
    {
        float randomX = Random.Range(-spreadAngle, spreadAngle);
        float randomY = Random.Range(-spreadAngle, spreadAngle);

        return Quaternion.Euler(randomX, randomY, 0) * forward;
    }
}
