using UnityEngine;

public abstract class HealthBase : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] int maxHealth = 100;

    protected int _currentHealth;

    void Start()
    {
        _currentHealth = maxHealth;
    }

    public void OnDamageTaken(int amount)
    {
        _currentHealth = Mathf.Max(_currentHealth - amount, 0);

        if (_currentHealth == 0)
        {
            OnHealthDepleted();
        }
    }

    public abstract void OnHealthDepleted();
}
