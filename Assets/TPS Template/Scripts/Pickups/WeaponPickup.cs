using UnityEngine;

public class WeaponPickup : MonoBehaviour, IPickup
{
    [SerializeField] string weaponName;
    [SerializeField] Weapon weaponPrefab;

    public string Name => weaponName;
    public void OnPicked()
    {
        Weapon newWeapon = Instantiate(weaponPrefab);
        WeaponManager.instance.EquipWeapon(newWeapon);
        Destroy(gameObject);
    }
}
