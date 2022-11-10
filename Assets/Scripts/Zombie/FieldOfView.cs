using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FieldOfView : MonoBehaviour
{
    public UnityAction<Transform> seePlayer;
    public UnityAction unseePlayer;

    private float originRadius;
    public float radius;
    [Range(0,360)]
    public float angle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;

    public float OriginRadius
    {
        get
        {
            return originRadius;
        }
    }

    private void Start()
    {
        originRadius = radius;
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
        RaycastHit hit;

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, out hit, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                    seePlayer?.Invoke(target);
                }
                else
                {
                    canSeePlayer = false;
                    unseePlayer?.Invoke();
                }
            }
            else
            {
                canSeePlayer = false;
                unseePlayer?.Invoke();
            }
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
            unseePlayer?.Invoke();
        }
    }
}
