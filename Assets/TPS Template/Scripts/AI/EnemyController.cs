using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), (typeof(Animator)))]
public class EnemyController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float rotateSpeed;

    [Header("Vision Setup")]
    [SerializeField] float visionRange;
    [SerializeField] LayerMask obstructLayer;

    [Header("Attack Setup")]
    [SerializeField] Transform target;
    [SerializeField] Transform damagePoint;
    [SerializeField] LayerMask attackLayer;
    [SerializeField] float attackRange;

    // states instances
    IEnemyState _currentState;
    IdleState _idleState;
    ChaseState _chaseState;
    AttackState _attackState;

    // variables
    Animator _animator;
    NavMeshAgent _agent;

    // read only variables for external access
    public NavMeshAgent Agent => _agent;
    public Transform Target => target;
    public Animator Animator => _animator;
    public float VisionRange => visionRange;
    public float AttackRange => attackRange;
    public LayerMask ObstructLayer => obstructLayer;
    public LayerMask AttackLayer => attackLayer;
    public Transform DamagePoint => damagePoint;
    public IdleState Idle => _idleState;
    public ChaseState Chase => _chaseState;
    public AttackState Attack => _attackState;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        // cretae states instances
        _idleState = new IdleState(this);
        _chaseState = new ChaseState(this);
        _attackState = new AttackState(this);

        // start with idle state by default
        OnStateChange(_idleState);
    }

    void Update()
    {
        _currentState?.OnStateStay();
        Animate();
    }

    public void OnStateChange(IEnemyState newState)
    {
        // exit the current state and update to a new one
        _currentState?.OnStateExit();
        _currentState = newState;
        _currentState.OnStateEnter();
    }

    #region Helper Methods
    public void RotateTowards(Vector3 target)
    {
        if (target != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(target);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeed * Time.deltaTime);
        }
    }

    void Animate()
    {
        float speed = Agent.velocity.magnitude; // current movement speed
        float maxSpeed = Agent.speed;           // max agent speed
        Animator.SetFloat("Velocity", speed / maxSpeed);
    }

    public void OnEventEnd()
    {
        if (_currentState is AttackState state)
        {
            state.OnEventEnd();
        }
    }

    public void OnEventTrigger()
    {
        if (_currentState is AttackState state)
        {
            state.OnEventTrigger(damagePoint.position, 1, 20, attackLayer);
        }
    }
    #endregion

    #region Debug Only
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, VisionRange);
    }
    #endregion
}
