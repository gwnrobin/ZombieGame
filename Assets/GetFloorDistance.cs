using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GetFloorDistance : MonoBehaviour
{
    [SerializeField]
    private VisualEffect visual;

    void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            float distanceToGround = hit.distance;
            visual.SetFloat("DistanceToGround", distanceToGround);
        }
    }
}
