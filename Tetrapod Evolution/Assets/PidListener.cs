using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PidListener : MonoBehaviour
{
    protected PidController controller;

    public abstract float GetPosition();
    public abstract float GetResponse();
    public abstract void SetControlSignal(float controlSignal);

    protected void InitController()
    {
        controller = gameObject.GetComponent(typeof(PidController)) as PidController;
    }
}
