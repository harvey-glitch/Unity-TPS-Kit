using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    private PlayerInput _input;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _input = GetComponent<PlayerInput>(); // Get PlayerInput on the same GameObject
    }

    public Vector3 GetMoveInput()
    {
        Vector2 move = _input.actions["Move"].ReadValue<Vector2>();
        return new Vector3(move.x, 0f, move.y);
    }

    public bool GetAttackInput()
    {
        return _input.actions["Attack"].IsPressed();
    }
}
