using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpentShellScript : FracturedScript
{
    // Start is called before the first frame update
    void Start()
    {
        var rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 50;
        var torque = GV.Singleton().getRandompointOnPlane();
        torque.y += 14;
        rb.AddTorque( torque, ForceMode.Impulse);
        rb.AddForce(new Vector3(.2f, 1f, 0f) * 2, ForceMode.Impulse); 
    }

  
}
