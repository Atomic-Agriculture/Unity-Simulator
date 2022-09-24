using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlSystem
{
    BangBang,
    PID
}

public enum ServoAxis
{
    X,
    Y,
    Z
}

public class ServoControl : MonoBehaviour
{

    private HingeJoint hinge;
    private JointLimits limits;
    private JointMotor motor;
    private float min, max;
    public float currentMotorTarget;

    public float position;

    [Range(0.0f, 1.0f)]
    public float targetPosition;
    public bool servoEnabled = true;

    public int MOTOR_FORCE = 100000;
    public int MOTOR_SPEED = 500;

    public ControlSystem controlType = ControlSystem.PID;
    public float threshold = 0.01f;
    public float proportionalConstant = 1000;
    public float derivativeConstant = 100;
    public float integralConstant = 0;

    private Queue<float> errors = new Queue<float>();
    private float lastError;
    private int queueLength = 1000;

    public ServoAxis servoAxis = ServoAxis.X;

    // Start is called before the first frame update
    void Start()
    {
        hinge = gameObject.GetComponent(typeof(HingeJoint)) as HingeJoint;
        limits = hinge.limits;
        min = limits.min;
        max = limits.max;
        motor.force = MOTOR_FORCE;
        motor.targetVelocity = 0;
        targetPosition = CalculatePosition();
    }

    // Update is called once per frame
    void Update()
    {
        position = CalculatePosition();

        if (servoEnabled)
        {
            float error = targetPosition - position;
            errors.Enqueue(error);
            UpdateForce(error);
            lastError = error;

        } else {

            motor.force = 0;

        }

        currentMotorTarget = motor.targetVelocity;
        hinge.motor = motor;

        if (errors.Count >= queueLength) {
            errors.Dequeue();
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

    public void IncTargetPosition(float inc)
    {
        targetPosition += inc;
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

        return CorrectEulerAngle(angle - limits.min) / (limits.max - limits.min);
    }

    void UpdateForce(float error)
    {
        if (controlType == ControlSystem.BangBang)
        {
            BangBangControl(error);
        }
        else if (controlType == ControlSystem.PID)
        {
            PidControl(error);
        }
    }

    void BangBangControl(float error)
    {
        if (-error > threshold)
        {
            motor.targetVelocity = -MOTOR_SPEED;
            motor.force = MOTOR_FORCE;
        }
        else if (error > threshold)
        {
            motor.targetVelocity = MOTOR_SPEED;
            motor.force = MOTOR_FORCE;
        }
        else
        {
            motor.targetVelocity = 0;
            motor.force = 0;
        }
    }

    void PidControl(float error) {
        motor.force = MOTOR_FORCE;

        float targetSpeed = CalculateProportionalComp(error) +
                            CalculateDeriviativeComp(error) +
                            CalculateIntegralComp();

        motor.targetVelocity = targetSpeed;
    }

    float CalculateProportionalComp(float error) {
        return (float)(error * proportionalConstant);
    }

    float CalculateDeriviativeComp(float error) {
        return (float)((error - lastError) * Time.deltaTime * derivativeConstant);
    }

    float CalculateIntegralComp() {
        float error_sum = 0;
        foreach (var error in errors)
        {
            error_sum += error;
        }
        return (float)(error_sum * integralConstant);
    }
}
