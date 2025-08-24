using UnityEngine;

public class Destroyable : HealthBase
{
    public override void OnHealthDepleted()
    {
        Destroy(this.gameObject);
    }
}
