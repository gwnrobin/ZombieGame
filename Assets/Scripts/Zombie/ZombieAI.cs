using HQFPSTemplate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : Entity
{
    public readonly Message Scream = new Message();
    public readonly Message Attack = new Message();

    public LayerMask HitMask = new LayerMask();

    private PlayerBaseState currentState;
    private PlayerStateFactory states;

    public readonly Message MoveCycleEnded = new Message();

    public Transform target;
    public Transform attackOrigin;

    public NavMeshAgent navMeshAgent;
    public Animator animator;
 
    public FieldOfView view;

    [SerializeField]
    private Rigidbody[] rigidbodies;

    private Vector2 velocity;
    private Vector2 smoothDeltaPosition;

    public float normalDetectRange;
    public float wanderRadius;

    public float wanderSpeed;
    public float runSpeed;

    public bool seePlayer = false;
    [SerializeField]
    private float playerLostAfterSeconds = 3;
    private float playerLost = 0;

    public float viewAngle = 130;

    public PlayerBaseState CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            currentState = value;
        }
    }

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        view = GetComponent<FieldOfView>();

        Death.AddListener(StopFollow);
        Death.AddListener(EnableRagdoll);

        view.seePlayer += SetTarget;
        view.unseePlayer += StopFollow;

        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = true;

        states = new PlayerStateFactory(this);
        currentState = states.Wander();
        currentState.EnterState();
    }

    private void OnAnimatorMove()
    {
        Vector3 rootPosition = animator.rootPosition;
        rootPosition.y = navMeshAgent.nextPosition.y;
        transform.position = rootPosition;
        transform.rotation = animator.rootRotation;
        navMeshAgent.nextPosition = rootPosition;
    }

    void Update()
    {
        currentState.UpdateStates();

        Velocity.Set(navMeshAgent.velocity);

        SynchronizeAnimatorAndAgent();
        if(target != null)
            navMeshAgent.destination = target.position;
    }

    private void SynchronizeAnimatorAndAgent()
    {
        Vector3 worldDeltaPosition = navMeshAgent.nextPosition - transform.position;
        worldDeltaPosition.y = 0;

        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        velocity = smoothDeltaPosition / Time.deltaTime;
        if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            velocity = Vector2.Lerp(Vector2.zero, velocity, navMeshAgent.remainingDistance / navMeshAgent.stoppingDistance);
        }

        bool shouldMove = velocity.magnitude > .5f && navMeshAgent.remainingDistance > navMeshAgent.velocity.magnitude;

        animator.SetBool("move", shouldMove);
        animator.SetFloat("velx", velocity.x);
        animator.SetFloat("vely", velocity.y);
    }

    public void SetPath()
    {
        Vector3 point;
        if(RandomPoint(transform.position, wanderRadius, out point))
        {
            navMeshAgent.SetDestination(point);
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    private void SetTarget(Transform target)
    {
        this.target = target.transform;
        seePlayer = true;
        playerLost = 0;
    }

    private void StopFollow()
    {
        playerLost += Time.deltaTime;
        if(playerLost > playerLostAfterSeconds)
        {
            target = null;
            seePlayer = false;
        }
    }


    private void DisableRagdoll()
    {
        foreach(Rigidbody r in rigidbodies)
        {
            r.isKinematic = true;
        }
    }

    private void EnableRagdoll()
    {
        foreach (Rigidbody r in rigidbodies)
        {
            r.isKinematic = false;
            navMeshAgent.enabled = false;
            animator.enabled = false;
        }
    }
}
