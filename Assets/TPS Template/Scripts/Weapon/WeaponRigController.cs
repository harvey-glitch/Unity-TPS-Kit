using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponRigController : MonoBehaviour
{
    [Header("Rigs Setup")]
    [SerializeField] Rig handRig;
    [SerializeField] Rig basePoseRig;
    [SerializeField] Rig upperbodyRig;

    [Header("IK Targets")]
    [SerializeField] TwoBoneIKConstraint rightHandIK;
    [SerializeField] TwoBoneIKConstraint leftHandIK;
    [SerializeField] Transform rightHandIKTarget;
    [SerializeField] Transform leftHandIKTarget;

    float _currentAimWeight;

    MultiAimConstraint _multiAimContraint;

    void Awake()
    {
        _multiAimContraint ??= basePoseRig.GetComponentInChildren<MultiAimConstraint>();
    }

    void Start()
    {
        SetHandRigWeight(0);
    }

    void Update()
    {
        if (!WeaponManager.instance.hasActiveWeapon && WeaponManager.instance.isAiming)
            return;

        UpdateRigWeights();
    }

    public void SetRigIKTarget()
    {
        rightHandIK.data.target = rightHandIKTarget;
        leftHandIK.data.target = leftHandIKTarget;
    }

    public void UpdateRigIKTarget(Transform rightHand, Transform leftHand)
    {
        rightHandIKTarget.position = rightHand.position;
        rightHandIKTarget.rotation = rightHand.rotation;

        leftHandIKTarget.position = leftHand.position;
        leftHandIKTarget.rotation = leftHand.rotation;
    }

    public void UpdateRigWeights()
    {
        _currentAimWeight = WeaponManager.instance.isAiming ? 1f : 0f;

        _multiAimContraint.weight = _currentAimWeight;
        upperbodyRig.weight = _currentAimWeight;
    }

    public void SetHandRigWeight(float newWeight)
    {
        if (handRig.weight != newWeight)
        {
            handRig.weight = newWeight;
        }
    }
}
