using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfGravity : MonoBehaviour
{
    public Vector3 CenterOfMass2;

    public bool Awake;
    protected Rigidbody2D r;

    void Start()
    {
        r = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        r.centerOfMass = CenterOfMass2;
        r.WakeUp();
        Awake = !r.IsSleeping();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position + transform.rotation * CenterOfMass2, 0.1f);
    }
}