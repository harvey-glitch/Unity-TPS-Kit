using UnityEngine;

public class IdleState : IEnemyState
{
    EnemyController _enemy;

    public IdleState(EnemyController enemy)
    {
        this._enemy = enemy;
    }

    public void OnStateEnter()
    {
        // initialize variables here
        _enemy.Agent.isStopped = true;
    }

    public void OnStateStay()
    {
        // execute main logic here
        SearchForPlayer();
    }

    public void OnStateExit()
    {
        // reset variables here
        _enemy.Agent.isStopped = false;
    }

    void SearchForPlayer()
    {
        // expose variables from main controller
        Vector3 origin = _enemy.transform.position;
        Vector3 target = _enemy.Target.position;
        float visionRange = _enemy.VisionRange;
        LayerMask obstructLayer = _enemy.ObstructLayer;

        // cached the distance first for later use
        float distance = Vector3.Distance(origin, target);

        // check if the target is within the vision range
        if (distance <= visionRange)
        {
            // calculate the direction and distance from the target
            Vector3 direction = (target - origin).normalized;
            direction.y = 0f; // ignore height difference
            _enemy.RotateTowards(direction);

            // check if the direction is within 90 degree (45 degree on each side)
            if (Vector3.Angle(_enemy.transform.forward, direction) <= 90f / 2)
            {
                // lastly check if the target is visible, not blocked by obstacles
                if (!Physics.Raycast(origin, direction, distance, obstructLayer))
                {
                    Debug.Log("player detected");
                    _enemy.OnStateChange(_enemy.Chase);
                }
            }
        }
    }
}
