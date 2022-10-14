using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ServoAxis
{
    X,
    Y,
    Z
}

public enum ControlScheme
{
    TorqueControl,
    MotorControl
}

public class ServoControl : PidListener
{
    private HingeJoint hinge;
    private Rigidbody rb;
    private float min, max;

    [Range(0.0f, 1.0f)]
    public float currentPosition;
    [Range(0.0f, 1.0f)]
    public float targetPosition;

    public bool servoEnabled = true;
    public ControlScheme controlScheme = ControlScheme.MotorControl;
    public float torqueApplied = 0f;
    public float torqueCap = 500f;
    public ServoAxis servoAxis = ServoAxis.X;

    // Start is called before the first frame update
    void Start()
    {
        hinge = GetComponent<HingeJoint>();


        rb = GetComponent<Rigidbody>();
        min = hinge.limits.min;
        max = hinge.limits.max;
        torqueApplied = 0.0f;
        
        InitController();
    }

    // Update is called once per frame
    void Update()
    {
        if (servoEnabled)
        {
            controller.SetTarget(targetPosition);
            controller.UpdateController();
        }
        else
        {
            controller.ClearIntegralSum();
        }
    }

    public void SetTargetPosition(float newTarget)
    {
        targetPosition = newTarget;
        if (targetPosition > 1.0f) {
            targetPosition = 1.0f;
        }
        if (targetPosition < 0.0f) {
            targetPosition = 0.0f;
        }
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
        currentPosition = CalculatePosition();
        return currentPosition;
    }

    public override float GetResponse() {
        if (controlScheme == ControlScheme.TorqueControl)
        {
            return torqueApplied;
        }
        else if (controlScheme == ControlScheme.MotorControl)
        {
            return hinge.velocity;
        }
        else
        {
            Debug.LogError("Unsupported Control Scheme");
            return 0;
        }
    }

    public override void SetControlSignal(float controlSignal)
    {
        if (controlScheme == ControlScheme.TorqueControl)
        {
            hinge.useMotor = false;
            torqueApplied = (torqueCap < controlSignal) ? torqueCap : (-torqueCap > controlSignal) ? -torqueCap : controlSignal;

            if (servoAxis == ServoAxis.X)
            {
                rb.AddTorque(new Vector3(1, 0, 0) * torqueApplied);
            }
            else if (servoAxis == ServoAxis.Y)
            {
                rb.AddTorque(new Vector3(0, 1, 0) * torqueApplied);
            }
            else if (servoAxis == ServoAxis.Z)
            {
                rb.AddTorque(new Vector3(1, 0, 0) * torqueApplied);
            }
        }
        else if (controlScheme == ControlScheme.MotorControl)
        {
            JointMotor motor = hinge.motor;
            motor.force = torqueCap;
            motor.targetVelocity = controlSignal;
            motor.freeSpin = false;
            hinge.useMotor = true;
            hinge.motor = motor;
        }
    }
}
