using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponType { primary, secondary }
    [Header("TYpe")]
    [SerializeField] WeaponType weaponType;

    [Header("Rigs")]
    [SerializeField] Transform rightHandGrip;
    [SerializeField] Transform leftHandGrip;

    [Header("Properties")]
    [SerializeField] float firerate = 0.2f;
    [SerializeField] float maxRange = 50.0f;
    [SerializeField] int damage = 10;
    [SerializeField] GameObject impactEffect;
    [SerializeField] Transform muzzle;
    [SerializeField] ParticleSystem muzzleFlash;

    [Header("Base Position")]
    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    [Header("Aiming Position Offset")]
    public Vector3 aimPositionOffset;
    public Vector3 aimRotationOffset;

    // Public read-only properties
    public Transform RightHandGrip => rightHandGrip;
    public Transform LeftHandGrip => leftHandGrip;
    public float Firerate => firerate;
    public float MaxRange => maxRange;
    public int Damage => damage;
    public GameObject ImpactEffect => impactEffect;
    public Transform Muzzle => muzzle;
    public WeaponType Type => weaponType;

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
}
