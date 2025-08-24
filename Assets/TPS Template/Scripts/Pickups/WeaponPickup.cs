using UnityEngine;

public class WeaponPickup : MonoBehaviour, IPickup
{
    [SerializeField] Weapon weapon_prefab;

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            OnPicked();
        }
    }

    public void OnPicked()
    {
        Weapon newWeapon = Instantiate(weapon_prefab);
        WeaponManager.instance.EquipWeapon(newWeapon);
        Destroy(this.gameObject);
    }
}
