/*
 * Code used in Update() is taken from: https://www.youtube.com/watch?v=kvTuYziy_QE&t=309s
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // NEW MOVEMENT //

    public float MotorForce, SteerForce, BrakeForce;
    public WheelCollider FrontRWheel, FrontLWheel, BackRWheel, BackLWheel;

    private float vDirection = 0;
    private float hDirection = 0;
    private bool applyBreak = false;

    // Used to detect if there is forward or backward input
    private bool forceInput = false;
    private bool turnInput = false;

    void Update()
    {
        float v = vDirection * MotorForce;
        float h = hDirection * SteerForce;

        if (forceInput)
        {
            BackRWheel.motorTorque = v;
            BackLWheel.motorTorque = v;
        }
        else
        {
            BackRWheel.motorTorque = 0;
            BackLWheel.motorTorque = 0;
        }

        if (turnInput)
        {
            FrontRWheel.steerAngle = h;
            FrontLWheel.steerAngle = h;
        }
        else
        {
            FrontRWheel.steerAngle = 0;
            FrontLWheel.steerAngle = 0;
        }

        if (applyBreak)
        {
            BackLWheel.brakeTorque = BrakeForce;
            BackRWheel.brakeTorque = BrakeForce;
        }
        else
        {
            BackLWheel.brakeTorque = 0;
            BackRWheel.brakeTorque = 0;
        }

        forceInput = false;
        turnInput = false;
        applyBreak = false;
    }

    public void Forward()
    {
        this.vDirection = 1;
        forceInput = true;
    }

    public void Backwards()
    {
        this.vDirection = -1;
        forceInput = true;
    }

    public void TurnRight()
    {
        this.hDirection = 1;
        turnInput = true;
    }

    public void TurnLeft()
    {
        this.hDirection = -1;
        turnInput = true;
    }

    public void Break()
    {
        this.applyBreak = true;
    }


    // OLD MOVEMENT //

    /*private Rigidbody rb;
    private Transform tr;

    private Vector3 eulerAngleVelocity;

    public float turnSpeed = 70.0f;
    public float accelerationSpeed = 120.0f;
    private float maxSpeed = 300.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        tr = gameObject.GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        *//*// Handle movement forward and backwards
        if (Input.GetKey(KeyCode.W)) Forward();
        else if (Input.GetKey(KeyCode.S)) Backwards();
        else rb.velocity *= 0.94f;

        // Handle rotation/turning 
        if (Input.GetKey(KeyCode.D)) TurnRight();
        else if (Input.GetKey(KeyCode.A)) TurnLeft();
        else eulerAngleVelocity = new Vector3(0, 0, 0);*//*

        Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }


    public void Forward()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        else
        {
            rb.AddForce(transform.forward * accelerationSpeed);
        }
    }


    public void Backwards()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = -rb.velocity.normalized * maxSpeed;
        }
        else
        {
            rb.AddForce(-transform.forward * accelerationSpeed);
        }
    }


    public void TurnLeft()
    {
        eulerAngleVelocity = new Vector3(0, -turnSpeed, 0);
    }


    public void TurnRight()
    {
        eulerAngleVelocity = new Vector3(0, turnSpeed, 0);
    }*/
}