using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureControl : MonoBehaviour
{

    public PodControl arm1;
    public PodControl arm2;
    public PodControl arm3;
    public PodControl arm4;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        // arm1.SetTargets(MapAxisToServo("Pod 1 Hip"), MapAxisToServo("Pod 1 Arm"), MapAxisToServo("Pod 1 Leg"));
        // arm2.SetTargets(MapAxisToServo("Pod 2 Hip"), MapAxisToServo("Pod 2 Arm"), MapAxisToServo("Pod 2 Leg"));
        // arm3.SetTargets(MapAxisToServo("Pod 3 Hip"), MapAxisToServo("Pod 3 Arm"), MapAxisToServo("Pod 3 Leg"));
        // arm4.SetTargets(MapAxisToServo("Pod 4 Hip"), MapAxisToServo("Pod 4 Arm"), MapAxisToServo("Pod 4 Leg"));
    }

    private float MapAxisToServo(string axisName)
    {
        return (Input.GetAxis(axisName) / 2) + 0.5f;
    }

    public PodControl[] GetArms()
    {
        PodControl[] arms = {arm1, arm2, arm3, arm4};
        return arms;
    }
}
