using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodControl : MonoBehaviour
{

    public ServoControl hip;
    public ServoControl arm;
    public ServoControl leg;

    [Range(0.0f, 1.0f)]
    public float hipPosition;
    
    [Range(0.0f, 1.0f)]
    public float armPosition;

    [Range(0.0f, 1.0f)]
    public float legPosition;

    // Start is called before the first frame update
    void Start()
    {
        hipPosition = hip.CalculatePosition();
        armPosition = arm.CalculatePosition();
        legPosition = leg.CalculatePosition();
    }

    // Update is called once per frame
    void Update()
    {
        hip.SetTargetPosition(hipPosition);
        arm.SetTargetPosition(armPosition);
        leg.SetTargetPosition(legPosition);
    }

    public void SetTargets(float hipTarget, float armTarget, float legTarget) {
        this.hipPosition = hipTarget;
        this.armPosition = armTarget;
        this.legPosition = legTarget;
    }

    public void SetHipTarget(float hipTarget) {
        this.hipPosition = hipTarget;
    }

    public void SetArmTarget(float armTarget) {
        this.armPosition = armTarget;
    }

    public void SetLegTarget(float legTarget) {
        this.legPosition = legTarget;
    }

}
