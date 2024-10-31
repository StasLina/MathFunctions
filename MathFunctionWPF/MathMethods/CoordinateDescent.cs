using MathFunctionWPF.Models;
using System.Threading.Tasks.Dataflow;

namespace MathFunctionWPF.MathMethods
{
    internal class CoordinateDescent
    {
        // Целевая функция для минимизации

        // Метод координатного спуска
        public static double Calc1Arg(FunctionCalculation calculation, double minX, double maxX, double learningRate, int iterations)
        {
            double currentPoint = minX; // Текущая точка

            for (int iter = 0; iter < iterations; iter++)
            {
                // Пробуем увеличить текущий аргумент
                double pointPlus = currentPoint + learningRate;
                double costPlus = calculation.Calculate(pointPlus);

                // Пробуем уменьшить текущий аргумент
                double pointMinus = currentPoint - learningRate;
                double costMinus = calculation.Calculate(pointMinus);

                // Выбираем направление, которое уменьшает значение функции
                if (costPlus < costMinus)
                {
                    currentPoint = pointPlus;
                }
                else
                {
                    currentPoint = pointMinus;
                }

                if (currentPoint < minX)
                {
                    return minX;
                }

                if (currentPoint > maxX)
                {
                    return maxX;
                }
            }

            return currentPoint;
        }
        public static double[] Calc2Arg(FunctionCalculation calculation, double x, double y, double learningRate, int iterations)
        {
            for (int iter = 0; iter < iterations; iter++)
            {
                double[,] matrix = new double[4, 3];

                const int countArgs = 2;
                var countValues = Convert.ToInt32(Math.Pow(2, countArgs));

                for (int i = 0, iEnd = matrix.GetLength(0); i < iEnd; ++i)
                {
                    matrix[i, 0] = x;
                    matrix[i, 1] = y;
                    for (int j = 0, jEnd = matrix.GetLength(1) - 1; j < jEnd; ++j)
                    {
                        // Проходим по столбцам
                        if ((i / (Math.Pow(2, countArgs - j) % 2)) == 1)
                        {
                            matrix[i, j] -= learningRate;
                        }
                        else
                        {
                            matrix[i, j] += learningRate;
                        }
                    }
                }

                int minFuncIdx = -1;
                double minFuncVal = calculation.Calculate2Arg(x, y);

                for (int i = 0, iEnd = matrix.GetLength(0); i < iEnd; ++i)
                {
                    int jEnd = matrix.GetLength(1) - 1;
                    matrix[i, jEnd] = calculation.Calculate2Arg(matrix[i, 0], matrix[i, 1]);

                    if (matrix[i, jEnd] < minFuncIdx)
                    {
                        minFuncIdx = i;
                    }
                }

                if(minFuncIdx == -1)
                {
                    return new double[2] { x, y };
                }
                else
                {
                    x = matrix[minFuncIdx, 0];
                    y = matrix[minFuncIdx, 1];
                }
            }

            return new double[2] { x, y };
        }


    }
}