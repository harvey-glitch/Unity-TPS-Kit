using UnityEngine;
using TMPro;

public class PickupHandler : MonoBehaviour
{
    [SerializeField] float pickupRange = 3f;      // max distance to pickup
    [SerializeField] TextMeshProUGUI nameText;

    Camera _camera;
    WeaponPickup currentPickup;


    void Awake()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        // raycast from center of screen
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            WeaponPickup pickup = hit.collider.GetComponent<WeaponPickup>();

            if (pickup != null)
            {
                // show prompt
                currentPickup = pickup;
                nameText.text = "[E] " + pickup.Name;

                // press E to pick up
                if (Input.GetKeyDown(KeyCode.E))
                {
                    pickup.OnPicked();
                    nameText.text = string.Empty;
                }

                return;
            }
        }

        // no pickup in sight
        currentPickup = null;
        nameText.text = string.Empty;
    }
}
