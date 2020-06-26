/*
 *  This script is in charge of distance input seen by the car's. It's purposed to to provide information for the input of the
 *  neural network, specifically the distances from the wall to the car in a given direction.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyes : MonoBehaviour
{

    public float range = 10f;

    private Vector3 rightForward = new Vector3(1, 0, 2);
    private Vector3 rightForward2 = new Vector3(1, 0, 1);
    private Vector3 leftForward = new Vector3(-1, 0, 2);
    private Vector3 leftForward2 = new Vector3(-1, 0, 1);
    private Vector3 right = new Vector3(1, 0, 0);
    private Vector3 left = new Vector3(-1, 0, 0);

    private Vector3 temp = new Vector3(.2f, 0f, 0f);
    private Vector3 temp2 = new Vector3(-.2f, 0f, 0f);

    private int layerMask = 1 << 9; // Mask used so that raycast will only collide with GameObjects tagged with "walls"
    private bool seeLines = false;

    // Array that hold distances to walls in any given direction
    private double[] distances;

    void Start()
    {
        distances = new double[7];
        layerMask = ~layerMask;
    }

    void Update()
    {
        Look();

        // Press "T" to show lines of what car's are seeing
        if (Input.GetKeyDown(KeyCode.T) && seeLines == false) seeLines = true;
        else if (Input.GetKeyDown(KeyCode.T) && seeLines == true) seeLines = false;
        if (seeLines) seeLines = true;
    }

    // Calculate the raycast and save distances
    public void Look()
    {
        RaycastHit hit1;
        if(Physics.Raycast(this.transform.position, this.transform.forward, out hit1, range, layerMask))
        {
            if (seeLines) Debug.DrawRay(this.transform.position, this.transform.TransformDirection(Vector3.forward) * hit1.distance, Color.white);
            distances[0] = (double) hit1.distance;
        }
        else
            if (seeLines) Debug.DrawRay(this.transform.position, this.transform.TransformDirection(Vector3.forward) * range, Color.white);

        RaycastHit hit2;
        if (Physics.Raycast(this.transform.position, this.transform.TransformDirection(rightForward), out hit2, range, layerMask))
        {
            if (seeLines) Debug.DrawRay(this.transform.position, this.transform.TransformDirection(rightForward) * hit2.distance * temp2.magnitude * 2.25f, Color.white);
            distances[1] = (double) hit2.distance;
        }
        else
            if (seeLines) Debug.DrawRay(this.transform.position, this.transform.TransformDirection(rightForward) * range, Color.white);

        RaycastHit hit3;
        if (Physics.Raycast(this.transform.position, this.transform.TransformDirection(leftForward), out hit3, range, layerMask))
        {
            if (seeLines) Debug.DrawRay(this.transform.position, this.transform.TransformDirection(leftForward) * hit3.distance * temp.magnitude * 2.25f, Color.white);
            distances[2] = (double) hit3.distance;
        }
        else
            if (seeLines) Debug.DrawRay(this.transform.position, this.transform.TransformDirection(leftForward) * range, Color.white);

        RaycastHit hit4;
        if (Physics.Raycast(this.transform.position, this.transform.TransformDirection(right), out hit4, range, layerMask))
        {
            if (seeLines) Debug.DrawRay(this.transform.position, this.transform.TransformDirection(right) * hit4.distance, Color.white);
            distances[3] = (double) hit4.distance;
        }
        else
            if (seeLines) Debug.DrawRay(this.transform.position, this.transform.TransformDirection(right) * range, Color.white);

        RaycastHit hit5;
        if (Physics.Raycast(this.transform.position, this.transform.TransformDirection(left), out hit5, range, layerMask))
        {
            if (seeLines) Debug.DrawRay(this.transform.position, this.transform.TransformDirection(left) * hit5.distance, Color.white);
            distances[4] = (double) hit5.distance;
        }
        else
            if (seeLines) Debug.DrawRay(this.transform.position, this.transform.TransformDirection(left) * range, Color.white);

        RaycastHit hit6;
        if (Physics.Raycast(this.transform.position, this.transform.TransformDirection(rightForward2), out hit6, range, layerMask))
        {
            if (seeLines) Debug.DrawRay(this.transform.position, this.transform.TransformDirection(rightForward2) * hit6.distance * temp.magnitude * 3.5f, Color.white);
            distances[5] = (double)hit6.distance;
        }
        else
            if (seeLines) Debug.DrawRay(this.transform.position, this.transform.TransformDirection(rightForward2) * range, Color.white);

        RaycastHit hit7;
        if (Physics.Raycast(this.transform.position, this.transform.TransformDirection(leftForward2), out hit7, range, layerMask))
        {
            if (seeLines) Debug.DrawRay(this.transform.position, this.transform.TransformDirection(leftForward2) * hit7.distance * temp.magnitude * 3.5f, Color.white);
            distances[6] = (double) hit7.distance;
        }
        else
            if (seeLines) Debug.DrawRay(this.transform.position, this.transform.TransformDirection(leftForward2) * range, Color.white);

    }
    
    public double[] GetDistances()
    {
        return distances;
    }

    // Debug methods
    public void print()
    {
        string strToPrint = "{";
        for (int i = 0; i < distances.Length-1; i++)
        {
            strToPrint += distances[i] + ", ";
        }
        strToPrint += distances[distances.Length - 1] + "]";
        Debug.Log(strToPrint);
    }
}
