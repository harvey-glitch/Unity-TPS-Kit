using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // use camera's forward vector
            Vector3 camForward = mainCamera.transform.forward;
            camForward.y = 0; // ignore vertical tilt
            camForward.Normalize();

            transform.rotation = Quaternion.LookRotation(camForward);
        }
    }
}
