using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowPassTest : MonoBehaviour
{
    public GameObject controlBody;
    public GameObject signalBody;

    public float amplitude;

    [Range(0.0f, 1000.0f)]
    public float frequency;

    [Range(0.0f, 1.0f)]
    public float alphaValue;

    public float signalAmplitude;
    private bool signalIsIncreasing;

    private float timeCounter;
    private float controlBodyStartY;
    private float signalBodyStartY;

    private float previousControlSignal;
    private float previousDerivative;
    private float runningSum;


    // Start is called before the first frame update
    void Start()
    {
        timeCounter = 0.0f;
        runningSum = 0.0f;
        controlBodyStartY = controlBody.transform.position.y;
        signalBodyStartY = signalBody.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        timeCounter += Time.deltaTime;

        Vector3 newControlBodyTransform = controlBody.transform.position;
        Vector3 newSignalBodyTransform = signalBody.transform.position;

        float controlSignal = amplitude * Mathf.Sin(frequency * timeCounter);

        newControlBodyTransform.y = controlBodyStartY + controlSignal;

        float derivative = ((controlSignal - runningSum) - previousControlSignal) / Time.deltaTime;
        newSignalBodyTransform.y = signalBodyStartY + derivative;

        runningSum += derivative * alphaValue;

        if (derivative > previousDerivative)
        {
            if (!signalIsIncreasing) {
                signalIsIncreasing = true;
                signalAmplitude = previousDerivative;
            }
        }
        else
        {
            if (signalIsIncreasing) {
                signalIsIncreasing = false;
                signalAmplitude = previousDerivative;
            }
        }

        previousDerivative = derivative;
        previousControlSignal = (controlSignal - runningSum);

        controlBody.transform.position = newControlBodyTransform;
        signalBody.transform.position = newSignalBodyTransform;
    }
}