using HQFPSTemplate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : Entity
{
    [SerializeField]
    private Transform target;

    private NavMeshAgent navMeshAgent;

    [SerializeField]
    private Rigidbody[] rigidbodies;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        Death.AddListener(StopFollow);
        Death.AddListener(EnableRagdoll);
    }

    void Update()
    {
        if(target != null)
            navMeshAgent.destination = target.position;
    }

    void StopFollow()
    {
        navMeshAgent.destination = this.transform.position;
        target = null;
    }

    void DisableRagdoll()
    {
        foreach(Rigidbody r in rigidbodies)
        {
            r.isKinematic = true;
        }
    }

    void EnableRagdoll()
    {
        foreach (Rigidbody r in rigidbodies)
        {
            r.isKinematic = false;
            navMeshAgent.enabled = false;
            GetComponent<Animator>().enabled = false;
        }
    }
}
