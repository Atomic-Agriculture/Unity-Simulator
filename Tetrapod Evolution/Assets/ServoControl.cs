using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ServoAxis
{
    X,
    Y,
    Z
}

public class ServoControl : PidListener
{
    private HingeJoint hinge;
    private float min, max;

    [Range(0.0f, 1.0f)]
    public float currentPosition;

    public bool servoEnabled = true;
    public int MOTOR_FORCE = 100000;
    public ServoAxis servoAxis = ServoAxis.X;

    // Start is called before the first frame update
    void Start()
    {
        hinge = gameObject.GetComponent(typeof(HingeJoint)) as HingeJoint;
        min = hinge.limits.min;
        max = hinge.limits.max;

        InitController();
    }

    // Update is called once per frame
    void Update()
    {
        currentPosition = CalculatePosition();
        controller.UpdateController();
    }

    public void SetTargetPosition(float newTarget)
    {
        float targetPosition = newTarget;
        if (targetPosition > 1.0f) {
            targetPosition = 1.0f;
        }
        if (targetPosition < 0.0f) {
            targetPosition = 0.0f;
        }
        controller.SetTarget(newTarget);
    }

    float CorrectEulerAngle(float angle)
    {
        while (angle > max)
        {
            angle -= 360;
        }
        while (angle < min)
        {
            angle += 360;
        }
        return angle;
    }

    public float CalculatePosition()
    {
        float angle = 0.0f;
        if (servoAxis == ServoAxis.X)
        {
            angle = this.transform.localRotation.eulerAngles.x;
        }
        else if (servoAxis == ServoAxis.Y) 
        {
            angle = this.transform.localRotation.eulerAngles.y;
        }
        else if (servoAxis == ServoAxis.Z) 
        {
            angle = this.transform.localRotation.eulerAngles.z;
        }
        else
        {
            Debug.Log("ServoAxis not assigned.");
        }

        return CorrectEulerAngle(angle - min) / (max - min);
    }

    public override float GetPosition() {
        return CalculatePosition();
    }

    public override float GetResponse() {
        // TODO Fix this, never clamping
        return hinge.velocity;
    }

    public override void SetControlSignal(float controlSignal) {
        JointMotor m = hinge.motor;
        m.targetVelocity = controlSignal;
        m.force = MOTOR_FORCE;
        m.freeSpin = false;
        hinge.motor = m;
    }
}
