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
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity))
        {
            float distanceToGround = hit.distance;
            visual.SetFloat("DistanceToGround", distanceToGround);
        }
    }

    private void Update()
    {
        transform.parent.transform.rotation = Quaternion.Euler(0, 0, 0);
        Debug.DrawRay(transform.position, -Vector3.up, Color.yellow);
    }
}
