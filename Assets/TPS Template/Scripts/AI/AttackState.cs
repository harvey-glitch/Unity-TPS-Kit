using UnityEngine;

public class AttackState : IEnemyState
{
    EnemyController _enemy;

    public AttackState(EnemyController enemy)
    {
        _enemy = enemy;
    }


    public void OnStateEnter()
    {
        // initialize variables here
        _enemy.Agent.isStopped = true;
        _enemy.Agent.velocity = Vector3.zero;
        _enemy.Agent.ResetPath();
        _enemy.Animator.SetTrigger("Attack");
    }

    public void OnStateStay()
    {
        // execute main logic here
    }

    public void OnStateExit()
    {
        // reset variables here
        _enemy.Agent.isStopped = false;
    }

    public void OnEventEnd()
    {
        Vector3 origin = _enemy.transform.position;
        Vector3 target = _enemy.Target.position;
        float attackRange = _enemy.AttackRange;
        float distance = Vector3.Distance(origin, target);

        if (distance <= attackRange)
        {
            _enemy.OnStateChange(_enemy.Attack);
        }
        else
        {
            _enemy.OnStateChange(_enemy.Chase);
        }
    }

    public void OnEventTrigger(Vector3 origin, int radius, int damage, LayerMask layer)
    {
        //DamageManager.instance.AreaDamage(origin, radius, damage, layer);
    }
}
