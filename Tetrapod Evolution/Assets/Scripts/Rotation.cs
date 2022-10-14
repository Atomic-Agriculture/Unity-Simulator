using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{

    public Vector3 rotationVector;
    public float mag = 20;
    public Transform transform;

    // Start is called before the first frame update
    void Start()
    {
        transform = gameObject.GetComponent(typeof(Transform)) as Transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(gameObject.transform.position, rotationVector, mag * Time.deltaTime);
    }
}
