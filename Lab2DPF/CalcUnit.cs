using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab2DPF;
internal static class CalcUnit
{
    public static List<double> GivenValues = new List<double>() { 5.41, 5.09, 7.33, 8.05, 9.87 };
    
}

public class CubicSplineSecDerivative
{
    private double[] a, b, c, d, x;

    public void Build(double start, double end, double[] y = null)
    {
        if(y == null) { y = CalcUnit.GivenValues.ToArray(); }

        int n = y.Length - 1;
        a = new double[n + 1];
        b = new double[n];
        c = new double[n + 1];
        d = new double[n];
        x = new double[n + 1];

        // x vals
        double step = (end - start) / n;
        for (int i = 0; i <= n; i++)
            x[i] = start + i * step;

        double[] h = new double[n];
        for (int i = 0; i < n; i++)
            h[i] = x[i + 1] - x[i];

        double[] alpha = new double[n];
        for (int i = 1; i < n; i++)
            alpha[i] = (3 / h[i]) * (y[i + 1] - y[i]) - (3 / h[i - 1]) * (y[i] - y[i - 1]);

        double[] l = new double[n + 1];
        double[] mu = new double[n + 1];
        double[] z = new double[n + 1];

        l[0] = 1;
        mu[0] = 0;
        z[0] = 0;

        for (int i = 1; i < n; i++)
        {
            l[i] = 2 * (x[i + 1] - x[i - 1]) - h[i - 1] * mu[i - 1];
            mu[i] = h[i] / l[i];
            z[i] = (alpha[i] - h[i - 1] * z[i - 1]) / l[i];
        }

        l[n] = 1;
        z[n] = 0;
        c[n] = 0;

        for (int j = n - 1; j >= 0; j--)
        {
            c[j] = z[j] - mu[j] * c[j + 1];
            b[j] = (y[j + 1] - y[j]) / h[j] - h[j] * (c[j + 1] + 2 * c[j]) / 3;
            d[j] = (c[j + 1] - c[j]) / (3 * h[j]);
            a[j] = y[j];
        }
    }

    public double Interpolate(double xp)
    {
        int n = x.Length - 1;
        int i = FindSegment(xp);

        double dx = xp - x[i];
        return a[i] + b[i] * dx + c[i] * dx * dx + d[i] * dx * dx * dx;
    }

    private int FindSegment(double xp)
    {
        int n = x.Length - 1;
        if (xp <= x[0]) return 0;
        if (xp >= x[n]) return n - 1;

        // Binary search is nice when there is a lot of data and it's sorted
        int low = 0;
        int high = n;
        while (low <= high)
        {
            int mid = (low + high) / 2;
            if (xp < x[mid])
                high = mid - 1;
            else if (xp > x[mid + 1])
                low = mid + 1;
            else
                return mid;
        }
        return n - 1;
    }
}

public class CubicSplineMatrix 
{
    private double[] coefs;
    private double[] xValues;
    private double[] yValues;
    private double i1;
    private double i2;

    public CubicSplineMatrix(double[] yValues, double i1, double i2)
    {
        if (yValues == null || yValues.Length != 5)
            throw new ArgumentException("Необхідно надати точно 5 значень y");

        this.yValues = yValues;
        this.i1 = i1;
        this.i2 = i2;
        this.coefs = new double[8];

        CalculateXValues();
        CalculateCoefficients();
    }

    private void CalculateXValues()
    {
        xValues = new double[5];
        double step = (i2 - i1) / (yValues.Length - 1);

        for (int i = 0; i < yValues.Length; i++)
        {
            xValues[i] = i1 + i * step;
        }
    }

    private void CalculateCoefficients()
    {
        double[,] matrix = new double[8, 8];
        double[] result = new double[8];

        matrix[0, 0] = 1;
        matrix[0, 1] = xValues[0];
        matrix[0, 2] = Math.Pow(xValues[0], 2);
        matrix[0, 3] = Math.Pow(xValues[0], 3);

        matrix[1, 0] = 1;
        matrix[1, 1] = xValues[1];
        matrix[1, 2] = Math.Pow(xValues[1], 2);
        matrix[1, 3] = Math.Pow(xValues[1], 3);

        matrix[2, 0] = 1;
        matrix[2, 1] = xValues[2];
        matrix[2, 2] = Math.Pow(xValues[2], 2);
        matrix[2, 3] = Math.Pow(xValues[2], 3);

        matrix[3, 4] = 1;
        matrix[3, 5] = xValues[2];
        matrix[3, 6] = Math.Pow(xValues[2], 2);
        matrix[3, 7] = Math.Pow(xValues[2], 3);

        matrix[4, 4] = 1;
        matrix[4, 5] = xValues[3];
        matrix[4, 6] = Math.Pow(xValues[3], 2);
        matrix[4, 7] = Math.Pow(xValues[3], 3);

        matrix[5, 4] = 1;
        matrix[5, 5] = xValues[4];
        matrix[5, 6] = Math.Pow(xValues[4], 2);
        matrix[5, 7] = Math.Pow(xValues[4], 3);

        matrix[6, 1] = 1;
        matrix[6, 2] = 2 * xValues[2];
        matrix[6, 3] = 3 * Math.Pow(xValues[2], 2);
        matrix[6, 5] = -1;
        matrix[6, 6] = -2 * xValues[2];
        matrix[6, 7] = -3 * Math.Pow(xValues[2], 2);

        matrix[7, 2] = 2;
        matrix[7, 3] = 6 * xValues[2];
        matrix[7, 6] = -2;
        matrix[7, 7] = -6 * xValues[2];

        result[0] = yValues[0];
        result[1] = yValues[1];
        result[2] = yValues[2];
        result[3] = yValues[2];
        result[4] = yValues[3];
        result[5] = yValues[4];
        result[6] = 0;
        result[7] = 0;

        coefs = SolveMatrix(matrix, result);
    }

    private double[] SolveMatrix(double[,] matrix, double[] result)
    {
        int n = result.Length;
        double[,] augmentedMatrix = new double[n, n + 1];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                augmentedMatrix[i, j] = matrix[i, j];
            }
            augmentedMatrix[i, n] = result[i];
        }

        for (int i = 0; i < n; i++)
        {
            int maxRow = i;
            for (int k = i + 1; k < n; k++)
            {
                if (Math.Abs(augmentedMatrix[k, i]) > Math.Abs(augmentedMatrix[maxRow, i]))
                {
                    maxRow = k;
                }
            }

            if (maxRow != i)
            {
                for (int j = i; j <= n; j++)
                {
                    double temp = augmentedMatrix[i, j];
                    augmentedMatrix[i, j] = augmentedMatrix[maxRow, j];
                    augmentedMatrix[maxRow, j] = temp;
                }
            }

            for (int k = i + 1; k < n; k++)
            {
                double factor = augmentedMatrix[k, i] / augmentedMatrix[i, i];
                for (int j = i; j <= n; j++)
                {
                    augmentedMatrix[k, j] -= factor * augmentedMatrix[i, j];
                }
            }
        }

        double[] solution = new double[n];
        for (int i = n - 1; i >= 0; i--)
        {
            solution[i] = augmentedMatrix[i, n];
            for (int j = i + 1; j < n; j++)
            {
                solution[i] -= augmentedMatrix[i, j] * solution[j];
            }
            solution[i] /= augmentedMatrix[i, i];
        }

        return solution;
    }

    public double GetY(double x)
    {
        double y;
        if (x < xValues[2])
        {
            y = coefs[0] + coefs[1] * x + coefs[2] * Math.Pow(x, 2) + coefs[3] * Math.Pow(x, 3);
        }
        else
        {
            y = coefs[4] + coefs[5] * x + coefs[6] * Math.Pow(x, 2) + coefs[7] * Math.Pow(x, 3);
        }
        return y;
    }
}
