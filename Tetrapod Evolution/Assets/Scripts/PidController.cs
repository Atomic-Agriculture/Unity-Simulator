using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PidController : MonoBehaviour
{
    // Settings
    public float target = 0.0f;
    public float proportionalConstant = 0.0f;
    public float integralConstant = 0.0f;
    public float derivativeConstant = 0.0f;
    public float saturationThreshold = 0.05f;

    // Input
    private PidListener actuator;

    // Internal State
    private float integralSum;
    private float lastError;
    private float prevControlSignal;

    public bool clampIntegral;

    public float proportionalPath;
    public float integralPath;
    public float derivativePath;


    // Start is called before the first frame update
    void Start()
    {
        actuator = gameObject.GetComponent(typeof(PidListener)) as PidListener;
        integralSum = 0.0f;
        lastError = 0.0f;
        prevControlSignal = 0.0f;
    }

    // Logic Loop
    public void UpdateController()
    {
        float error = CalculateError();
        clampIntegral = IsSaturated() && IsWindingUp(error);
        float controlSignal = CalculateControlSignal(error);

        actuator.SetControlSignal(controlSignal);

        lastError = error;
        prevControlSignal = controlSignal;
    }

    // Getters/Setters

    public void SetTarget(float newTarget)
    {
        target = newTarget;
    }

    public void ConfigureConstants(float kp, float ki, float kd)
    {
        proportionalConstant = kp;
        integralConstant = ki;
        derivativeConstant = kd;
    }

    public void ClearIntegralSum() {
        integralSum = 0;
    }

    // Calculation

    bool IsSaturated()
    {
        // Prev Control Signal does not match response
        return Mathf.Abs(prevControlSignal - actuator.GetResponse()) >= saturationThreshold;
    }

    bool IsWindingUp(float error)
    {
        if (prevControlSignal > 0 && error > 0)
        {
            return true;
        }
        else if (prevControlSignal < 0 && error < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    float CalculateError()
    {
        return target - actuator.GetPosition();
    }

    float CalculateControlSignal(float error)
    {
        float signal = 0.0f;
        signal = ProportionalPath(error) +
            (clampIntegral ? IntegralPath(0) : IntegralPath(error)) +
            DerivativePath(error);
        return signal;
    }

    float ProportionalPath(float error)
    {
        float value = error * proportionalConstant;
        proportionalPath = value;
        return value;
    }

    float IntegralPath(float error)
    {
        integralSum += error;
        integralPath = integralSum * integralConstant;
        return integralPath;
    }

    float DerivativePath(float error)
    {
        float output = ((error - lastError) / Time.deltaTime) * derivativeConstant;
        derivativePath = output;
        return output;
    }
}
