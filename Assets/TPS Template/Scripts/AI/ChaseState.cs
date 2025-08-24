using UnityEngine;

public class ChaseState : IEnemyState
{
    // variables
    EnemyController _enemy;
    Vector3 _lastTargetPosition;
    float _nextUpdate;

    public ChaseState(EnemyController enemy)
    {
        this._enemy = enemy;
    }

    public void OnStateEnter()
    {
        // initialize variables here
        _lastTargetPosition = _enemy.Target.position;
        _enemy.Agent.SetDestination(_enemy.Target.position);
        _nextUpdate = Time.time;
    }

    public void OnStateStay()
    {
        // execute main logic here
        ChaseTarget();
    }

    public void OnStateExit()
    {
        // reset variables here
        _lastTargetPosition = Vector3.zero;
    }

    void ChaseTarget()
    {
        Vector3 target = _enemy.Target.position;
        float distance = _enemy.Agent.remainingDistance;
        float threshold = _enemy.AttackRange;

        if (Time.time >= _nextUpdate)
        {
            // check if the target has moved significally
            if (Vector3.Distance(_lastTargetPosition, target) > 1.0f)
            {
                _lastTargetPosition = _enemy.Target.position;
                _enemy.Agent.SetDestination(_enemy.Target.position);
            }

            _nextUpdate= Time.time + 0.3f;
        }

        if (!_enemy.Agent.pathPending && distance <= threshold)
        {
            _enemy.OnStateChange(_enemy.Attack);
        }

        Vector3 direction = (target - _enemy.transform.position).normalized;
        direction.y = 0f; // ignore height difference
        _enemy.RotateTowards(direction);
    }
}
