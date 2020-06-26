/*
 *  This script is the overseer of the projects generational population. Its purpose is to create every generation and call
 *  methods and functions from extending scripts to improve their training. All generational logic is located here.
 *  
 *  If a car is ever "stagnant" or "crashed" it will be removed from active generation and be put in saved cars
 */

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Populate : MonoBehaviour
{

    public int population = 10;
    public double mutateRate = 0.01;

    public Vector3 startPos;
    public GameObject carPrefab;
    public Text text;

    // Generation data
    private GameObject[] cars;
    private List<GameObject> savedCars;
    private int genCounter = 0;
    private bool newGen = true;
    private bool firstGen = true;

    // Keep track if new gen brain has already mutated
    private bool mutated = false;

    // Keep track of best fit brain
    //  Last gen Alpha brain and fitness
    private NeuralNetwork lastGenAlphaBrain;
    private double lastGenAlphaFitness;

    private NeuralNetwork nextGenBrain;
    private double nextGenFitness = -1;

    // Time Buffer
    private float time = 0.0f;
    private float timeLimit = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        startPos = GameObject.Find("Start").GetComponent<Transform>().position;

        cars = new GameObject[population];
        savedCars = new List<GameObject>();

        for (int i = 0; i < population; i++)
        {
            SpawnCar(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Debug statements, delete later
        if (Input.GetKey(KeyCode.W)) cars[0].GetComponent<Movement>().Forward();
        if (Input.GetKeyDown(KeyCode.P) && savedCars.Count != 0)
        {
            for (int i = 0; i < savedCars.Count; i++)
            {
                Car car = savedCars[i].GetComponent<Car>();

                //Debug.Log("Car " + car.GetCarNumber() + " brain:");
                car.printBrain();
            }
        }
        if (Input.GetKeyDown(KeyCode.O) && cars.Length  > 0)
        {
            for (int j = 0; j < cars.Length; j++)
            {
                Car car = cars[j].GetComponent<Car>();
                Debug.Log("Car" + car.GetCarNumber() + "brain: ");
                car.printBrain();

                //Debug.Log("Car " + car.GetCarNumber() + ": " + car.GetStagnant());
            }
        }

        // Kepp track of last gens alpha brain
        if (genCounter != 0)
        {
            lastGenAlphaBrain = nextGenBrain;
            lastGenAlphaFitness = nextGenFitness;
        }

        // Population and generation managment
        // If all cars crash start next generation
        if (cars.Length == 0)
        {
            genCounter++;
            nextGeneration();
            newGen = true;
            firstGen = false;
            mutated = false;
            time = 0.0f;
            Debug.Log("Generation: " + genCounter);
        }

        // Calculate fitness of alive cars to determine the most fit car currently and set its color
        calculateAliveFitness();
        GameObject alphaCar = cars[0];
        foreach (GameObject car in cars)
        {
            if (alphaCar.GetComponent<Car>().GetFitness() < car.GetComponent<Car>().GetFitness())
            {
                alphaCar = car;
            }
            car.GetComponent<Renderer>().material.color = Color.red;
        }
        alphaCar.GetComponent<Renderer>().material.color = Color.green;
        // Display alphaCar's lap and fitness
        text.text = "Lap: " + alphaCar.GetComponent<Car>().lap + "\nFitness " + Math.Round(alphaCar.GetComponent<Car>().GetFitness(), 3);

        // Handle when car crashes or remains stagnant 
        for (int i = 0; i < cars.Length; i++)
        {
            Car car = cars[i].GetComponent<Car>();
            if (car.GetCrashed() || car.GetStagnant())
            {
                car.AddScore(-10);
                savedCars.Add(cars[i]);

                List<GameObject> list = new List<GameObject>(cars);
                list.RemoveAt(i);

                cars[i].SetActive(false);

                cars = list.ToArray();
            }
        }

        // Time buffer to help instaniate object
        if (newGen)
        {
            time += Time.deltaTime;
        }

        // Update/mutate brain of new gen cars
        // Note: Should have placed this in nextGeneration function after Clean()
        if (newGen && time >= timeLimit && !firstGen && !mutated)
        {
            foreach (GameObject car in cars)
            {
                Car carComp = car.GetComponent<Car>();
                if (nextGenFitness > lastGenAlphaFitness)
                    carComp.SetBrain(nextGenBrain);
                else
                    carComp.SetBrain(lastGenAlphaBrain);
                carComp.mutate(mutateRate);
            }
            mutated = true;
        }

        // Make car brains output their movement
        foreach (GameObject car in cars)
        {
            Car carComp = car.GetComponent<Car>();
            // Time delay added at start of generation so unity can instantiate objects otherwise null error thrown
            if (time >= timeLimit)
            {
                carComp.think();
                newGen = false;
            }
        }
    }

    // Instantiates new car prefabs and saves them in cars array
    private void SpawnCar(int i)
    {
        cars[i] = Instantiate(carPrefab, startPos, Quaternion.identity);
        cars[i].GetComponent<Car>().SetCarNumber(i);
    }

    // Delete all cars in old generation
    private void Clean()
    {
        foreach (GameObject car in savedCars)
        {
            Destroy(car);
        }
        savedCars.Clear();
    }

    // Genetic Algorithm for neuroevolution

    // Create next genertaion:
    //  Find the car with the best fitness and save its brain
    //  Instantiate next generation of cars
    //  Clean the old generation
    private void nextGeneration()
    {
        GameObject alphaCar = calculateFitness();
        Car alphaCarComp = alphaCar.GetComponent<Car>();
        nextGenBrain = alphaCarComp.GetBrain();
        nextGenFitness = alphaCarComp.GetFitness();

        Debug.Log("Alpha Car: " + alphaCar.name);

        cars = new GameObject[population];

        for (int i = 0; i < population; i++)
        {
            SpawnCar(i);
        }

        // Clear genereation data for new generation
        Clean();
    }

    // Called after there are no alive cars
    private GameObject calculateFitness()
    {
        double sum = 0;
        GameObject alphaCar = savedCars[0];

        foreach (GameObject car in savedCars)
        {
            sum += car.GetComponent<Car>().GetScore();
        }

        foreach (GameObject car in savedCars)
        {
            Car carBrain = car.GetComponent<Car>();
            carBrain.SetFitness(carBrain.GetScore() / sum);

            // Find the car with the best fitness
            if (alphaCar.GetComponent<Car>().GetFitness() < car.GetComponent<Car>().GetFitness())
            {
                alphaCar = car;
            }
        }

        return alphaCar;
    }

    // Used to find the best fit car that is still alive
    private void calculateAliveFitness()
    {
        double sum = 0;
        foreach (GameObject car in cars)
        {
            sum += car.GetComponent<Car>().GetScore();
        }

        foreach (GameObject car in cars)
        {
            Car carBrain = car.GetComponent<Car>();
            carBrain.SetFitness(carBrain.GetScore() / sum);
        }
    }

    // Debug methods
    private void printCars()
    {
        string str = "";
        foreach (GameObject car in cars)
        {
            Car carComp = car.GetComponent<Car>();
            str += "Car" + carComp.GetCarNumber() + ", ";
        }
        Debug.Log(str);
    }
}
