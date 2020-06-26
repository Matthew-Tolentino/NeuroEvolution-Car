/*
 *  Class was based off of TheCodingTrain's toy neural network:
 *      https://github.com/CodingTrain/Toy-Neural-Network-JS/tree/master/lib
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate double f(double value);

public class matrix
{
    public int row;
    public int col;
    public double[,] data;

    // Constructor
    public matrix(int row, int col)
    {
        this.row = row;
        this.col = col;
        this.data = new double[this.row, this.col];

        for (int i = 0; i < this.row; i++)
        {
            for (int j = 0; j < this.col; j++)
            {
                data[i, j] = 0.0;
            }
        }
    }

    public matrix copy()
    {
        matrix m = new matrix(this.row, this.col);
        for (int i = 0; i < this.row; i++)
        {
            for (int j = 0; j < this.col; j++)
            {
                m.data[i, j] = this.data[i, j];
            }
        }
        return m;
    }

    public static matrix fromArray(double[] arr)
    {
        matrix m = new matrix(arr.Length, 1);
        for (int i = 0; i < arr.Length; i++)
        {
            m.data[i, 0] = arr[i];
        }
        return m;
    }

    public double[] toArray()
    {
        double[] arr = new double[this.row * this.col];
        int index = 0;
        for (int i = 0; i < this.row; i++)
        {
            for (int j = 0; j < this.col; j++)
            {
                arr[index++] = this.data[i, j];
            }
        }
        return arr;
    }

    public void randomize()
    {
        for (int i = 0; i < this.row; i++)
        {
            for (int j = 0; j < this.col; j++)
            {
                this.data[i, j] = (double) Random.Range(-100, 100) / 100;
            }
        }
    }

    // Add function if another matrix is passed in 
    public void add(matrix n)
    {
        if (n.row == this.row && n.col == this.col)
        {
            for (int i = 0; i < this.row; i++)
            {
                for (int j = 0; j < this.col; j++)
                {
                    this.data[i, j] += n.data[i, j];
                }
            }
        }
        else Debug.Log("Row or columns are not equal");
    }

    // Add function if an integer is passed in
    public void add(double n)
    {
        for (int i = 0; i < this.row; i++)
        {
            for (int j = 0; j < this.col; j++)
            {
                data[i, j] += n;
            }
        }
    }

    public static matrix multiply(matrix m1, matrix m2)
    {
        if (m1.col != m2.row)
        {
            Debug.Log("Cols of A must equal rows of B");
            return null;
        }
        matrix newMatrix = new matrix(m1.row, m2.col);
        double sum = 0;
        for (int i = 0; i < newMatrix.row; i++)
        {
            for (int j = 0; j < newMatrix.col; j++)
            {
                sum = 0;
                for (int k = 0; k < m1.col; k++)
                {
                    sum += m1.data[i, k] * m2.data[k, j];
                }
                newMatrix.data[i, j] = sum;
            }
        }
        return newMatrix;
    }

    public void multiply(double n)
    {
        for (int i = 0; i < this.row; i++)
        {
            for (int j = 0; j < this.col; j++)
            {
                data[i, j] *= n;
            }
        }
    }

    public void map(f func)
    {
        for (int i = 0; i < this.row; i++)
        {
            for (int j = 0; j < this.col; j++)
            {
                double tempVal = this.data[i, j];
                this.data[i, j] = func(tempVal);
            }
        }
    }

    public matrix transpose()
    {
        matrix newMatrix = new matrix(col, row);
        for (int i = 0; i < this.row; i++)
        {
            for (int j = 0; j < this.col; j++)
            {
                newMatrix.data[j, i] = this.data[i, j];
            }
        }
        return newMatrix;
    }

    public void print()
    {
        string strToPrint = "";
        for (int i = 0; i < this.row; i++)
        {
            for (int j = 0; j < this.col; j++)
            {
                strToPrint += this.data[i, j] + ", ";
            }
            strToPrint += "\n";
        }
        Debug.Log(strToPrint);
    }
}
