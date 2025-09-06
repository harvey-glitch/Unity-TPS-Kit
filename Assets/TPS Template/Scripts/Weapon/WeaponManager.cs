using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
public class WeaponManager : MonoBehaviour
{
    // instance
    public static WeaponManager instance { get; private set; }

    [Header("Weapon Transfrom Setup")]
    [SerializeField] Transform weaponHolder;
    [SerializeField] Transform weaponPivot;
    [SerializeField] Transform weaponRecoil;

    [Header("Bullet Tracer")]
    [SerializeField] bool addBulletTracer;
    [SerializeField] TrailRenderer trailPrefab;

    [Header("Cinemachine Camera")]
    [SerializeField] CinemachineImpulseSource impulseSource;

    // read only variable for external access
    public bool hasActiveWeapon =>
        activeWeapon != null && activeWeapon.gameObject.activeInHierarchy;

    // references and components
    Camera _camera;
    WeaponRigController _rigController;

    // tracks the next time the weapon can be fire
    Dictionary<Weapon.WeaponCategory, Weapon> equippedWeapons = new();

    Weapon activeWeapon;

    float _nextFireTime;
    bool _currentlyAiming;
    Vector3 weaponPivotPos;
    Quaternion weaponPivotRot;

    Vector3 targetRecoilPos;
    Vector3 targetRecoilRot;
    Vector3 currentRecoilPos;
    Vector3 currentRecoilRot;

    public bool isAiming => _currentlyAiming;
    void Awake()
    {
        #region singleton
        // Make sure only one instance exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // destroy duplicate
            return;
        }

        instance = this;
        #endregion

        _rigController ??= GetComponent<WeaponRigController>();
        impulseSource ??= GetComponent<CinemachineImpulseSource>();

        _camera = Camera.main;
    }

    private void Start()
    {
        weaponPivotPos = weaponRecoil.transform.localPosition;
        weaponPivotRot = weaponRecoil.transform.localRotation;
    }

    void Update()
    {
        CheckIfAiming();

        // safe check if theres an active weapon first
        if (!hasActiveWeapon)
            return;

        PoseBlending();
        SmoothRecoil();
        HitScanShoot();
    }

    public void HitScanShoot()
    {
        switch (activeWeapon.Mode)
        {
            case Weapon.FireMode.auto:
                if (InputHandler.Instance.GetAttackInput() && Time.time >= _nextFireTime)
                {
                    Vector3 forwardDirection = _camera.transform.forward;
                    HitScan(forwardDirection);
                }
                break;

            case Weapon.FireMode.burst:
                if (InputHandler.Instance.GetAttackInput() && Time.time >= _nextFireTime)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Vector3 randomDirection = activeWeapon.GetSpreadDirection(_camera.transform.forward, 5f);
                        HitScan(randomDirection);
                    }
                }
                break;
        }
    }

    void HitScan(Vector3 direction)
    {
        // Create the ray from the camera position instead of center screen
        Ray ray = new Ray(_camera.transform.position, direction);
        Vector3 rayHitPoint = Vector3.zero;

        // store the point where the ray hit
        if (Physics.Raycast(ray, out RaycastHit hit, activeWeapon.MaxRange))
        {
            rayHitPoint = hit.point;

            if (hit.transform.TryGetComponent(out HealthBase health))
            {
                health.OnDamageTaken(activeWeapon.Damage);
                Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                if (rb == null)
                    return;

                rb.AddForceAtPosition(ray.direction.normalized * 2f, hit.point, ForceMode.Impulse);
            }
        }
        else
        {
            rayHitPoint = ray.GetPoint(activeWeapon.MaxRange);
        }

        if (addBulletTracer)
        {
            // create a direction from weapon barrel to hit point
            //Vector3 direction = (rayPoint - activeWeapon.Muzzle.position).normalized;

            // spawn trail
            TrailRenderer trail = Instantiate(
                trailPrefab, activeWeapon.Muzzle.position, Quaternion.LookRotation(direction));

            // move the trail towards hit point
            StartCoroutine(SpawnTrail(trail, rayHitPoint, hit));
            activeWeapon.PlayMuzzleFlash(); // play muzzle flash
        }

        // add recoil
        impulseSource.GenerateImpulse(ray.direction);
        targetRecoilPos += Vector3.back * activeWeapon.Recoil.z;
        targetRecoilRot += new Vector3(activeWeapon.Recoil.y, 0f, 0f);

        // add firerate
        _nextFireTime = Time.time + (1f / activeWeapon.Firerate);
    }

    IEnumerator SpawnTrail(TrailRenderer trail, Vector3 endPosition, RaycastHit rayHit)
    {
        float time = 0;
        float timeOffset = 0.05f; // add some offset before destroying trails or particles

        Vector3 startPosition = trail.transform.position;

        while(time < 1f)
        {
            trail.transform.position = Vector3.Lerp(
                startPosition, endPosition, time);

            time += Time.deltaTime / trail.time;
            yield return null;
        }
        trail.transform.position = endPosition;

        if (rayHit.collider != null)
        {
            // spawn impact particle
            GameObject impact = Instantiate(
                activeWeapon.ImpactEffect, endPosition, Quaternion.identity);

            if (impact.TryGetComponent(out ParticleSystem particle))
            {
                particle.Play();
                Destroy(impact, particle.main.duration + timeOffset);
            }
        }

        Destroy(trail.gameObject, trail.time + timeOffset);
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        if (newWeapon == null) return;

        var category = newWeapon.Category;

        // if we pick the same weapon category, remove the previous and equipped the new one
        if (equippedWeapons.ContainsKey(category) && equippedWeapons[category] != null)
        {
            Destroy(equippedWeapons[category].gameObject);
            //Destroy(equippedWeapons[category].gameObject);
            Debug.Log("duplicate exist");
        }

        // Disable currently active weapon (different category case)
        if (activeWeapon != null && activeWeapon != newWeapon)
        {
            activeWeapon.gameObject.SetActive(false);
        }

        equippedWeapons[category] = newWeapon; // update the dictionary to store the new weapon accodingly
        activeWeapon = newWeapon; // update the new active weapon to match what is picked

        // make sure the weapon is parented and placed on the right transform
        activeWeapon.transform.SetParent(weaponRecoil.transform, false);
        activeWeapon.transform.localPosition = Vector3.zero;
        activeWeapon.transform.localRotation = Quaternion.identity;

        // offset the weapon pivot for positioning
        Vector3 basePosition = activeWeapon.positionOffset;
        Vector3 baseRotation = activeWeapon.rotationOffset;
        UpdateWeaponOrientation(basePosition, baseRotation);

        // move the IK targets to weapon grips, making player hands hold the weapon
        _rigController.UpdateRigIKTarget(activeWeapon.RightHandGrip, activeWeapon.LeftHandGrip);

        _rigController.SetHandRigWeight(1);

        activeWeapon.gameObject.SetActive(true);
        Debug.Log(activeWeapon.name);
    }

    void UpdateWeaponOrientation(Vector3 position, Vector3 rotation)
    {
        weaponPivot.transform.localPosition = position;
        weaponPivot.transform.localRotation = Quaternion.Euler(rotation);
    }

    void PoseBlending()
    {
        Vector3 targetPos = isAiming ? activeWeapon.aimPositionOffset : activeWeapon.positionOffset;

        Vector3 targetRot = isAiming ? activeWeapon.aimRotationOffset : activeWeapon.rotationOffset;

        UpdateWeaponOrientation(targetPos, targetRot);
    }

    void SmoothRecoil()
    {
        float speedFactor = 1f * activeWeapon.RecoilSpeed;
        targetRecoilPos = Vector3.Lerp(
            targetRecoilPos, Vector3.zero, Time.deltaTime * speedFactor);

        currentRecoilPos = Vector3.Lerp(
            currentRecoilPos, targetRecoilPos, Time.deltaTime * speedFactor);

        targetRecoilRot = Vector3.Lerp(
            targetRecoilRot, Vector3.zero, Time.deltaTime * speedFactor);

        currentRecoilRot = Vector3.Lerp(
            currentRecoilRot, targetRecoilRot, Time.deltaTime * speedFactor);

        weaponRecoil.transform.localPosition = weaponPivotPos + currentRecoilPos;
        weaponRecoil.transform.localRotation = weaponPivotRot * Quaternion.Euler(currentRecoilRot);
    }

    public bool CheckIfAiming()
    {
        bool isAiming = InputHandler.Instance.GetAttackInput() && activeWeapon != null;

        if (isAiming != _currentlyAiming)
        {
            _currentlyAiming = isAiming;
        }

        return _currentlyAiming;
    }
}
