using UnityEngine;

public class WeaponPickup : MonoBehaviour, IPickup
{
    [SerializeField] string weaponName;
    [SerializeField] Weapon weaponPrefab;
    [SerializeField] GameObject interactPrompt;

    bool nearPlayer;

    public void OnPicked()
    {
        Weapon newWeapon = Instantiate(weaponPrefab);
        WeaponManager.instance.EquipWeapon(newWeapon);
        Destroy(gameObject);
    }

    // method to enable and disable player
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            nearPlayer = true;
            interactPrompt.SetActive(nearPlayer);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            nearPlayer = false;
            interactPrompt.SetActive(nearPlayer);
        }
    }

    void Update()
    {
        if (nearPlayer && InputHandler.Instance.GetInteractInput())
        {
            OnPicked();
        }
    }
}
