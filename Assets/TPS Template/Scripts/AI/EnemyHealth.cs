using UnityEngine;

public class EnemyHealth : HealthBase
{
    public override void OnHealthDepleted()
    {
        Destroy(this.gameObject);
    }
}
