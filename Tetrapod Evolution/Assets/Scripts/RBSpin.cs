using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBSpin : MonoBehaviour
{

    private HingeJoint rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent(typeof(HingeJoint)) as HingeJoint;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(rb.name + "'s angular velocity is " + rb.velocity);
    }
}
