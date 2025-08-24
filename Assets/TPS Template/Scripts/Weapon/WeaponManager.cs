using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour
{
    // instance
    public static WeaponManager instance { get; private set; }

    [Header("Weapon")]
    [SerializeField] Weapon activeWeapon;
    [SerializeField] Transform weaponHolder;
    [SerializeField] Transform weaponPivot;

    [Header("Bullet Tracer")]
    [SerializeField] bool addBulletTracer;
    [SerializeField] TrailRenderer trailPrefab;

    // read only variable for external access
    public bool hasActiveWeapon =>
        activeWeapon != null && activeWeapon.gameObject.activeInHierarchy;

    // references and components
    Camera _camera;
    WeaponRigController _rigController;
    
    // tracks the next time the weapon can be fire
    float _nextFireTime;
    bool _currentlyAiming;

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
        _camera = Camera.main;
    }

    void Update()
    {
        HandlePoseBlending();

        // safe check if theres an active weapon first
        if (!hasActiveWeapon && InputHandler.Instance.GetAttackInput())
        {
            Debug.LogWarning("No active weapon found!");
            return;
        }
        
        // ray base shooting logic
        HitScanShoot();
    }

    public void HitScanShoot()
    {
        if (InputHandler.Instance.GetAttackInput() && Time.time >= _nextFireTime)
        {
            // cast a ray from the center of camera 
            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 rayPoint = Vector3.zero;

            // store the point where the ray hit
            if (Physics.Raycast(ray, out RaycastHit hit, activeWeapon.MaxRange))
            {
                rayPoint = hit.point;
                activeWeapon.InflictDamage(hit.transform.gameObject);
            }
            else
            {
                rayPoint = ray.GetPoint(activeWeapon.MaxRange);
            }

            if (addBulletTracer)
            {
                // create a direction from weapon barrel to hit point
                Vector3 direction = (rayPoint - activeWeapon.Muzzle.position).normalized;

                // spawn trail
                TrailRenderer trail = Instantiate(
                    trailPrefab, activeWeapon.Muzzle.position, Quaternion.LookRotation(direction));

                // move the trail towards hit point
                StartCoroutine(SpawnTrail(trail, rayPoint, hit));
                activeWeapon.PlayMuzzleFlash(); // play muzzle flash
            }

            _nextFireTime = Time.time + (1f / activeWeapon.Firerate);
        }
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
        if (activeWeapon != null)
        {
            activeWeapon.gameObject.SetActive(false);
        }

        if (newWeapon != null)
        {
            activeWeapon = newWeapon;
            activeWeapon.transform.SetParent(weaponPivot.transform, false);
            activeWeapon.transform.localPosition = Vector3.zero;
            activeWeapon.transform.localRotation = Quaternion.identity;

            // offset the weapon pivot for appropriate positioning
            Vector3 basePosition = activeWeapon.positionOffset;
            Vector3 baseRrotation = activeWeapon.rotationOffset;
            UpdateWeaponOrientation(basePosition, baseRrotation);

            // move the IK targets to weapon grips
            _rigController.UpdateRigIKTarget(activeWeapon.RightHandGrip, activeWeapon.LeftHandGrip);

            _rigController.SetHandRigWeight(1);
        }
    }

    void EquippedChildWeapon()
    {
        // loop though all weapon holder child and equipped if one weapon found by default
        for (int i = 0; i < weaponPivot.childCount; i++)
        {
            Transform child = weaponPivot.GetChild(i);
            Weapon weapon = child.GetComponent<Weapon>();

            if (weapon != null)
            {
                EquipWeapon(weapon);
                break; // stop if you only want one equipped
            }
        }
    }

    void UpdateWeaponOrientation(Vector3 position, Vector3 rotation)
    {
        if (weaponPivot != null)
        {
            weaponPivot.transform.localPosition = position;
            weaponPivot.transform.localRotation = Quaternion.Euler(rotation);
        }
    }

    void HandlePoseBlending()
    {
        bool isAiming = InputHandler.Instance.GetAttackInput();

        if (isAiming != _currentlyAiming)
        {
            _currentlyAiming = isAiming;

            Vector3 position = _currentlyAiming ?
                       activeWeapon.aimPositionOffset : activeWeapon.positionOffset;

            Vector3 rotation = _currentlyAiming ?
                activeWeapon.aimRotationOffset : activeWeapon.rotationOffset;

            UpdateWeaponOrientation(position, rotation);
        }
    }
}
