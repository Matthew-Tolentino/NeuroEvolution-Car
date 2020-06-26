/*
 *  Class was based off of TheCodingTrain's toy neural network:
 *      https://github.com/CodingTrain/Toy-Neural-Network-JS/tree/master/lib
 */

using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public delegate double func(double value);


// Passable Mutation Function's ----
class sigmoid
{
    public double func1(double x)
    {
        return 1 / (1 + Math.Exp(-x));
    }

    public double func2(double y)
    {
        return y * (1 - y);
    }
}

class tanh
{
    public double func1(double x)
    {
        return Math.Tanh(x);
    }

    public double func2(double y)
    {
        return 1 - (y * y);
    }
}
// --------------------------------

class ActivationFunction
{
    public sigmoid sig = new sigmoid();
    public tanh tan = new tanh();
}

static class Globals
{
    public static double mutationRate = 0;

    // Set the learning rate and activation function
    public static double learningRate = 0.1;
    public static ActivationFunction activation_function = new ActivationFunction();
}

public class NeuralNetwork
{
    int input_nodes;
    int hidden_nodes;
    int output_nodes;

    matrix weights_ih; // matrix weights of input and hidden
    matrix weights_ho; // matrix weights of hidden and output

    matrix bias_h;
    matrix bias_o;

    public NeuralNetwork(int input_nodes, int hidden_nodes, int output_nodes)
    {
        this.input_nodes = input_nodes;
        this.hidden_nodes = hidden_nodes;
        this.output_nodes = output_nodes;

        this.weights_ih = new matrix(this.hidden_nodes, this.input_nodes);
        this.weights_ho = new matrix(this.output_nodes, this.hidden_nodes);
        this.weights_ih.randomize();
        this.weights_ho.randomize();

        this.bias_h = new matrix(this.hidden_nodes, 1);
        this.bias_o = new matrix(this.output_nodes, 1);
        this.bias_h.randomize();
        this.bias_o.randomize();
    }

    public NeuralNetwork(NeuralNetwork lastGen)
    {
        this.input_nodes = lastGen.input_nodes;
        this.hidden_nodes = lastGen.hidden_nodes;
        this.output_nodes = lastGen.output_nodes;

        this.weights_ih = lastGen.weights_ih.copy();
        this.weights_ho = lastGen.weights_ho.copy();

        this.bias_h = lastGen.bias_h.copy();
        this.bias_o = lastGen.bias_o.copy();
    }

    public double[] predict(double[] input_array)
    {
        matrix inputs = matrix.fromArray(input_array);
        matrix hidden = matrix.multiply(this.weights_ih, inputs);
        hidden.add(bias_h);
        hidden.map(Globals.activation_function.sig.func1);

        // Generating outputs
        matrix output = matrix.multiply(this.weights_ho, hidden);
        output.add(this.bias_o);
        output.map(Globals.activation_function.sig.func1);

        return output.toArray();
    }

    // This function is never used in this car project
    public double[] feedForward(double[] input_array)
    {
        // Generating the hidden outputs
        matrix inputs = matrix.fromArray(input_array);
        matrix hidden = matrix.multiply(this.weights_ih, inputs);
        hidden.add(this.bias_h);
        // Activation fucntion!
        hidden.map(sigmoid);

        // Generating the output's output
        matrix output = matrix.multiply(this.weights_ho, hidden);
        output.add(this.bias_o);
        output.map(sigmoid);

        // Send back to the caller
        return output.toArray();
    }

    public NeuralNetwork copy()
    {
        return new NeuralNetwork(this);
    }

    // Takes in a weight from NeuralNetwork and changes it based on the mutation rate
    public double mutateFunc(double val)
    {
        if ((double) UnityEngine.Random.Range(0, 100) / 100 < Globals.mutationRate)
        {
            double change = (double) UnityEngine.Random.Range(-100, 100) / 100;
            return val + change;
        } 
        else
        {
            return val;
        }
    }

    // Called on the nueral network to initalize mutation to its weights
    public void mutate(double rate)
    {
        Globals.mutationRate = rate;
        weights_ho.map(mutateFunc);
        weights_ih.map(mutateFunc);
        bias_h.map(mutateFunc);
        bias_o.map(mutateFunc);
    }

    // Prints out neural network's weights
    public void print()
    {
        string strToPrint = "";
        for (int i = 0; i < this.weights_ih.row; i++)
        {
            for (int j = 0; j < this.weights_ih.col; j++)
            {
                strToPrint += this.weights_ih.data[i, j] + ", ";
            }
            strToPrint += "\n";
        }
        Debug.Log(strToPrint);
    }

    public double sigmoid(double x)
    {
        return 1 / (1 + Math.Exp(-x));
    }
}
