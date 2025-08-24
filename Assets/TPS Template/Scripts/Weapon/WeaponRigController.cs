using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponRigController : MonoBehaviour
{
    [Header("Rigs")]
    [SerializeField] Rig handRig;
    [SerializeField] Rig basePoseRig;
    [SerializeField] Rig aimPoseRig;
    [SerializeField] Rig upperbodyRig;

    [Header("Rig Blending")]
    [SerializeField] float rigBlendSpeed;

    [Header("IK Targets")]
    [SerializeField] TwoBoneIKConstraint rightHandIK;
    [SerializeField] TwoBoneIKConstraint leftHandIK;
    [SerializeField] Transform rightHandIKTarget;
    [SerializeField] Transform leftHandIKTarget;

    float _currentAimWeight;
    float _currentBaseWeight;
    bool _currentAiming;

    void Start()
    {
        SetHandRigWeight(0);
    }

    void Update()
    {
        if (!WeaponManager.instance.hasActiveWeapon && _currentAiming)
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
        bool isAiming = InputHandler.Instance.GetAttackInput();

        if (isAiming != _currentAiming)
        {
            // update the aim state
            _currentAiming = isAiming;
        }

        _currentBaseWeight = Mathf.Lerp(
            _currentBaseWeight, _currentAiming ? 0f : 1f, Time.deltaTime * rigBlendSpeed);

        _currentAimWeight = Mathf.Lerp(
            _currentAimWeight, _currentAiming ? 1f : 0f, Time.deltaTime * rigBlendSpeed);

        basePoseRig.weight = _currentBaseWeight;
        aimPoseRig.weight = _currentAimWeight;
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
