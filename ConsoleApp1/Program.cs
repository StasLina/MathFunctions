using System;
using System.Linq;

class Program
{
    static double[] LeastSquares(double[] x, double[] y, int degree)
    {
        // Формируем матрицу X (дизайн-матрица)
        int n = x.Length;
        double[,] X = new double[n, degree + 1];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j <= degree; j++)
            {
                X[i, j] = Math.Pow(x[i], j);
            }
        }

        // Вычисляем X^T * X
        double[,] XT = Transpose(X);
        double[,] XT_X = MultiplyMatrices(XT, X);

        // Вычисляем X^T * y
        double[] XT_y = MultiplyMatrixVector(XT, y);

        // Решаем систему линейных уравнений (XT_X * a = XT_y)
        double[] coefficients = SolveLinearSystem(XT_X, XT_y);

        return coefficients;
    }

    static double[,] Transpose(double[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        double[,] result = new double[cols, rows];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[j, i] = matrix[i, j];
            }
        }

        return result;
    }

    static double[,] MultiplyMatrices(double[,] A, double[,] B)
    {
        int rowsA = A.GetLength(0);
        int colsA = A.GetLength(1);
        int colsB = B.GetLength(1);
        double[,] result = new double[rowsA, colsB];

        for (int i = 0; i < rowsA; i++)
        {
            for (int j = 0; j < colsB; j++)
            {
                for (int k = 0; k < colsA; k++)
                {
                    result[i, j] += A[i, k] * B[k, j];
                }
            }
        }

        return result;
    }

    static double[] MultiplyMatrixVector(double[,] matrix, double[] vector)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        double[] result = new double[rows];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[i] += matrix[i, j] * vector[j];
            }
        }

        return result;
    }

    static double[] SolveLinearSystem(double[,] A, double[] b)
    {
        int n = A.GetLength(0);
        double[] x = new double[n];
        double[,] augmentedMatrix = new double[n, n + 1];

        // Формируем расширенную матрицу
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                augmentedMatrix[i, j] = A[i, j];
            }
            augmentedMatrix[i, n] = b[i];
        }

        // Прямой ход метода Гаусса
        for (int i = 0; i < n; i++)
        {
            // Нормализация строки
            double pivot = augmentedMatrix[i, i];
            for (int j = 0; j < n + 1; j++)
            {
                augmentedMatrix[i, j] /= pivot;
            }

            // Обнуление столбца
            for (int k = 0; k < n; k++)
            {
                if (k != i)
                {
                    double factor = augmentedMatrix[k, i];
                    for (int j = 0; j < n + 1; j++)
                    {
                        augmentedMatrix[k, j] -= factor * augmentedMatrix[i, j];
                    }
                }
            }
        }

        // Извлекаем решение
        for (int i = 0; i < n; i++)
        {
            x[i] = augmentedMatrix[i, n];
        }

        return x;
    }

    static void Main(string[] args)
    {
        // Данные (x, y)
        double[] x = { 1, 2, 3, 4, 5 };
        double[] y = { 2, 3.9, 6.1, 7.9, 10.2 };

        // Для n=1 (линейная аппроксимация)
        double[] coeffs_n1 = LeastSquares(x, y, 1);
        Console.WriteLine("Коэффициенты для n=1: " + string.Join(", ", coeffs_n1));

        // Для n=2 (аппроксимация квадратичной функцией)
        double[] coeffs_n2 = LeastSquares(x, y, 2);
        Console.WriteLine("Коэффициенты для n=2: " + string.Join(", ", coeffs_n2));
    }
}
