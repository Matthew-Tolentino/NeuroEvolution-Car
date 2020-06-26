/*
 * Code used here is taken from: https://www.youtube.com/watch?v=kvTuYziy_QE&t=309s
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterMovement : MonoBehaviour
{
    public float MotorForce, SteerForce, BrakeForce;
    public WheelCollider FrontRWheel, FrontLWheel, BackRWheel, BackLWheel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float v = Input.GetAxis("Vertical") * MotorForce;
        float h = Input.GetAxis("Horizontal") * SteerForce;

        BackRWheel.motorTorque = v;
        BackLWheel.motorTorque = v;

        FrontRWheel.steerAngle = h;
        FrontLWheel.steerAngle = h;

        if (Input.GetKey(KeyCode.Space))
        {
            BackLWheel.brakeTorque = BrakeForce;
            BackRWheel.brakeTorque = BrakeForce;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            BackLWheel.brakeTorque = 0;
            BackRWheel.brakeTorque = 0;
        }

        if (Input.GetAxis("Vertical") == 0)
        {
            BackLWheel.brakeTorque = BrakeForce;
            BackRWheel.brakeTorque = BrakeForce;
        }
        else
        {
            BackLWheel.brakeTorque = 0;
            BackRWheel.brakeTorque = 0;
        }
    }
}
