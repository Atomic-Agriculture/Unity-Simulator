using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneProps : PidListener
{
    private Rigidbody rb;
    private BoxCollider bc;
    public float forceApplied;

    public float forceCap;

    public float currentHeight;
    [Range(0.0f, 100.0f)]
    public float targetHeight;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
        forceCap = rb.mass * 10 * 10;
        
        InitController();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        controller.SetTarget(targetHeight);
        controller.UpdateController();
    }

    public override float GetPosition()
    {
        currentHeight = gameObject.transform.position.y + bc.center.y - (bc.size.y / 2);
        return currentHeight;
    }

    public override float GetResponse()
    {
        return forceApplied;
    }


    public override void SetControlSignal(float controlSignal)
    {
        forceApplied = (forceCap < controlSignal) ? forceCap : (-forceCap > controlSignal) ? -forceCap : controlSignal;
        rb.AddForce(Vector3.up * forceApplied);
    }
}
