using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelloWorld : MonoBehaviour
{
    public string myName;
    
    void Start () 
    {
        Debug.Log("I am alive and my name is " + myName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
