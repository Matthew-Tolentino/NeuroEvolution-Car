/*
 *  This script is used to hold all variables and methods about individual cars. It's purpose to to provide the logic that
 *  car's will have to follow and provide methods to manipulate every car instance. 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Car : MonoBehaviour
{

    public delegate void f(double value);

    // Refrences to other scripts
    public Movement move;
    public Eyes eyes;

    // Gate/Lap variables
    private Stack gateStack;
    private bool finishedTrack;
    public int lap = 0;

    // Variabels to handle stagnant cars
    private float timer = 0.0f;
    private float timeLimit = 5.0f;
    private float distanceLimit = 7.0f;

    //Car state variables
    private int carNumber = -1;

    private double score = 0;
    private double fitness = 0;
    private double velocityX = 0;
    private double velocityZ = 0;
    private double speed = 0;
    private double rotation = 0;  // only need y rotation since x and z rotation are locked
    private Vector3 lastPosition;
    private bool crashed = false;
    private bool stagnant = false;

    private NeuralNetwork brain;
    private int brain_inputs = 11; 
    private int brain_hidden = 11;
    private int brain_outputs = 4;

    void Start()
    {
        brain = new NeuralNetwork(brain_inputs, brain_hidden, brain_outputs);
        lastPosition = this.transform.position;
        this.name = "Car" + carNumber;

        // Find out which track is being used
        string sceneName = SceneManager.GetActiveScene().name;

        // Initalize Gate stack
        gateStack = new Stack();
        gateStack.Push("FinalGate");

        // Finish Gate stack initalizaiton based on track
        if (sceneName == "1stTrack")
        {
            Debug.Log("1st Track gate initalized");
            for (int i = 8; i > 0; i--)
            {
                gateStack.Push("Gate" + i);
            }
        }
        else if (sceneName == "2ndTrack")
        {
            Debug.Log("2nd Track gate initalized");
            for (int i = 13; i > 0; i--)
            {
                gateStack.Push("Gate" + i);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("distances: ");
            printArray(eyes.GetDistances());
        }

        // Check every certain amount of time if the car has moved significantly from its last position
        // If not then consider it stagnant
        timer += Time.deltaTime;
        if (timer >= timeLimit)
        {
            float distance = Vector3.Distance(lastPosition, this.transform.position);
            if (distance < distanceLimit && lap == 0)
                stagnant = true;
            else
            {
                lastPosition = this.transform.position;
                this.score += distance;
            }
            timer = 0.0f;
            if (distanceLimit < 10.0f) distanceLimit += 0.5f;
        }

        // Other inputs for car neural network
        this.speed = (double) GetComponent<Rigidbody>().velocity.magnitude;
        this.rotation = (double) GetComponent<Transform>().rotation.eulerAngles.y;
        this.velocityX = (double) GetComponent<Rigidbody>().velocity.x;
        this.velocityZ = (double) GetComponent<Rigidbody>().velocity.z;

        // For every update the car survives its score increases
        if (!crashed && !stagnant)
            score++;
    }

    // Run neural network calculations and predict an movement
    public void think()
    {
        double[] wallDistances = eyes.GetDistances();
        double[] inputs = new double[brain_inputs];
        inputs[0] = wallDistances[0];
        inputs[1] = wallDistances[1];
        inputs[2] = wallDistances[2];
        inputs[3] = wallDistances[3];
        inputs[4] = wallDistances[4];
        inputs[5] = wallDistances[5];
        inputs[6] = wallDistances[6];
        inputs[7] = this.speed;
        inputs[8] = this.rotation;
        inputs[9] = this.velocityX;
        inputs[10] = this.velocityZ;

        double[] output = brain.predict(inputs);

        if (output[0] < .5) move.Forward();
        if (output[1] < .5) move.Backwards();
        if (output[2] < .5) move.TurnLeft();
        if (output[3] < .5) move.TurnRight();
    }

    public void mutate(double rate)
    {
        this.brain.mutate(rate);
    }

    void OnCollisionEnter(Collision obj)
    {
        // Wall collision
        if (obj.gameObject.tag == "wall")
        {
            crashed = true;
        }
    }

    // Trigger functions handle gate collision and reward
    void OnTriggerEnter(Collider obj)
    {
        if (obj.gameObject.tag == "gate" && gateStack.Count != 0)
        {
            if ((string) gateStack.Peek() != obj.gameObject.name && !finishedTrack)
            {
                crashed = true;
            }
        }
    }

    void OnTriggerExit(Collider obj)
    {
        if (obj.gameObject.tag == "gate" && gateStack.Count != 0)
        {
            if ((string) gateStack.Peek() == "FinalGate")
            {
                finishedTrack = true;
            }
            this.score += 1000;
            gateStack.Pop();
        }
        if (obj.gameObject.name == "FinalGate")
        {
            this.lap++;
        }
    }

    // Public Methods
    public void AddScore(double num)
    {
        this.score += num;
    }

    public NeuralNetwork GetBrain()
    {
        return this.brain;
    }

    public bool GetCrashed()
    {
        return this.crashed;
    }

    public bool GetStagnant()
    {
        return this.stagnant;
    }

    public int GetCarNumber()
    {
        return this.carNumber;
    }

    public double GetScore()
    {
        return this.score;
    }

    public double GetFitness()
    {
        return this.fitness;
    }

    public void SetBrain(NeuralNetwork brain)
    {
        this.brain = brain.copy();
    }

    public void SetCarNumber(int num)
    {
        this.carNumber = num;
    }

    public void SetFitness(double fitness)
    {
        this.fitness = fitness;
    }

    // Note: Havent used this method yet, can use as later UI addition
    public void SetDistanceLimit(float limit)
    {
        this.distanceLimit = limit;
    }

    // Print functions for debugging
    public void printBrain()
    {
        brain.print();
    }
   
    public void printSpeed()
    {
        Debug.Log(this.name + " speed: " + this.speed);
    }

    public void printArray(double[] arr)
    {
        string strToPrint = "[";
        for (int i = 0; i < arr.Length - 1; i++)
        {
            strToPrint += arr[i] + ", ";
        }
        strToPrint += arr[arr.Length-1] + "]";
        Debug.Log(strToPrint);
    }
}

/* Best Brains:
 * 
 * Track 1: 136+ Laps
    -0.45, 1.47, 0.65, 0.69, 1.62, -0.0700000000000001, 0.89, -2.06, 2.39, -0.5, 0.85, 
    0.34, -0.61, 1.44, 0.2, 0.5, -0.63, 0.2, -0.55, -0.02, 0.48, -0.7, 
    -1.96, -1.18, 0.0900000000000001, 0.84, 0.98, -0.74, 0.09, 0.05, 0.26, 0.0100000000000001, -0.14, 
    0.47, -0.72, -0.38, -0.73, -0.0199999999999999, -0.12, -0.0399999999999999, -1.93, -0.08, -0.15, -0.75, 
    -1.72, -1.71, 0.91, -1.27, -0.53, -1.38, -1.59, 0.12, 0.57, -1.01, -0.79, 
    0.14, 0.39, 0.59, -0.34, 0.69, 0.6, 0.89, -0.34, 0.23, 0.12, -0.29, 
    -1.06, -0.51, 1.26, -0.72, -0.39, 1.07, -1.58, 0.17, -0.46, -0.13, -1.25, 
    -0.69, -0.2, -0.35, 0.89, 2.95, 0.84, 2.18, -0.36, -0.1, 0.24, 0.18, 
    0.96, -0.91, 0.51, -0.0100000000000001, 1.07, 0.5, 2.94, 0.53, -1.5, 0.19, 0.83, 
    1.22, -0.26, -0.16, -0.27, 0.85, -0.05, 0.12, 0.53, -0.83, -0.59, -0.1, 
    -1.59, 0.8, -0.27, 2.1, 0.42, -0.67, 1.78, -0.35, 0.52, 1.18, 1.31,

   Track 2: 155+ laps (left & right turning)
    1.55, 3.24, 2.72, -1.43, -5.13, -3.22, 1.65, 0.79, 3.6, -1.93, 2.34, 
    -1.38, -1.37, 2.36, 0.18, -0.29, -3.22, -0.52, -0.69, 0.71, 0.0700000000000003, -0.4, 
    0.79, -0.6, -4.12, 0.62, 1.23, -0.17, 0.74, 1.5, -0.49, -1.11, -4.51, 
    -3.43, -0.73, 3.05, 0.61, 2.69, -3.47, 0.149999999999999, 1.06, 1.67, -3.58, -1.71, 
    -5.16, 1.25, -1.96, -3.13, -2.98, 1.67, -1.3, -3.72, -1.1, -0.55, 2.89, 
    -2.89, -0.4, 0.23, -3.05, -3.16, -2.01, 4.2, -3.81, -3.06, 1.77, -1.2, 
    5.12, 2.73, 1.64, 3.45, 0.36, 0.92, -1.31, -1.48, -2.65, 1.08, -1.78, 
    1.89, -5.66, 1.23, -2.04, -1.3, -3.37, -2.61, -1.41, -3.8, 0.45, -5.2, 
    -2.1, 1.14, -4.91, 0.9, 6.93, 0.3, -0.980000000000001, -1.4, 1.72, -2.26, -1.3, 
    2.07, -1.34, 6.32, 2.65, 0.63, -1.2, 1.61, -4.66, 2.38, 1.18, -3.5, 
    2.94, 1.33, -2.6, 0.86, 3.17, -3.48, 1, 3.5, -1.78, -6.14, 0.91,
  */


