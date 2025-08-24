using UnityEngine;

[RequireComponent(typeof(CharacterController), (typeof(Animator)))]
public class ThirdPersonControl : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotateSpeed = 10f;

    CharacterController _controller;
    Camera _camera;
    Animator _animator;

    Vector3 _moveInput;

    public Vector3 MoveVector { get; private set; }

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator ??= GetComponent<Animator>();
        _camera = Camera.main;

        // lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // get camera-based forward and right and flatten
        Vector3 cameraForward = _camera.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = _camera.transform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        _moveInput = InputHandler.Instance.GetMoveInput();

        Move(cameraForward, cameraRight);
        Rotate(cameraForward);
        Animate();
    }

    private void OnAnimatorMove(){}

    void Move(Vector3 forward, Vector3 right)
    {
        // create a vector relative to camera
        MoveVector = forward * _moveInput.z + right * _moveInput.x;
        _controller.Move(MoveVector * moveSpeed * Time.deltaTime);
    }

    void Rotate(Vector3 forward)
    {
        Quaternion targetRot = Quaternion.LookRotation(forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
    }

    void Animate()
    {
        // Convert to local space
        Vector3 localMove = transform.InverseTransformDirection(MoveVector);

        _animator.SetFloat("H", localMove.x, 0.1f, Time.deltaTime);
        _animator.SetFloat("V", localMove.z, 0.1f, Time.deltaTime);
    }
}
