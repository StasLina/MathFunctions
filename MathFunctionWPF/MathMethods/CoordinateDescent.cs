using MathFunctionWPF.Models;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;

namespace MathFunctionWPF.MathMethods
{
    internal class CoordinateDescent
    {
        // Целевая функция для минимизации

        // Метод координатного спуска
        public static double Calc1Arg(FunctionCalculation calculation, double minX, double maxX, double xBegin, double learningRate, int iterations)
        {
            double currentPoint = xBegin; // Текущая точка

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
            double minFuncVal = calculation.Calculate2Arg(x, y);

            for (int iter = 0; iter < iterations; iter++)
            {
                double[] deltas = { learningRate, -learningRate }; // Шаги по направлениям
                double[] bestPoint = { x, y };
                bool updated = false;

                // Проходим по каждой координате x и y
                for (int coord = 0; coord < 2; coord++)
                {
                    // Для каждой координаты проходим по двум направлениям
                    foreach (double delta in deltas)
                    {
                        double newX = x;
                        double newY = y;

                        // Изменение либо x, либо y в зависимости от coord
                        if (coord == 0) newX += delta;
                        else newY += delta;

                        // Вычисляем значение функции в новой точке
                        double newFuncVal = calculation.Calculate2Arg(newX, newY);

                        // Если найдено меньшее значение функции, обновляем координаты
                        if (newFuncVal < minFuncVal)
                        {
                            minFuncVal = newFuncVal;
                            bestPoint[0] = newX;
                            bestPoint[1] = newY;
                            updated = true;
                        }
                    }
                }

                // Если не произошло обновления, достигнут локальный минимум
                if (!updated)
                {
                    break;
                }

                // Обновляем текущие координаты
                x = bestPoint[0];
                y = bestPoint[1];
            }

            return new double[] { x, y };
        }


        public static double[] Calc2ArgOld(FunctionCalculation calculation, double x, double y, double learningRate, int iterations)
        {
            double minFuncVal = calculation.Calculate2Arg(x, y);
            for (int iter = 0; iter < iterations; iter++)
            {
                double[,] matrix_forwardxy = new double[4, 3];

                const int countArgs = 2;
                var countValues = Convert.ToInt32(Math.Pow(2, countArgs));

                for (int i = 0, iEnd = matrix_forwardxy.GetLength(0); i < iEnd; ++i)
                {
                    matrix_forwardxy[i, 0] = x;
                    matrix_forwardxy[i, 1] = y;
                    for (int j = 0, jEnd = matrix_forwardxy.GetLength(1) - 1; j < jEnd; ++j)
                    {
                        // Проходим по столбцам
                        if (Math.Ceiling(i / Math.Pow(2, countArgs - j)) % 2 == 1)
                        {
                            matrix_forwardxy[i, j] -= learningRate;
                        }
                        else
                        {
                            matrix_forwardxy[i, j] += learningRate;
                        }
                    }
                }

                double[,] matrix = new double[4, 3];

                for (int i = 0, iEnd = matrix_forwardxy.GetLength(0); i < iEnd; ++i)
                {
                    matrix[i, 0] = x;
                    matrix[i, 1] = y;

                    if (Math.Ceiling(i / Math.Pow(2, 1)) % 2 == 1)
                    {
                        matrix[i, i % 2] += learningRate;
                    }
                    else
                    {
                        matrix[i, i % 2] -= learningRate;
                    }
                }

                int minFuncIdx = -1;


                for (int i = 0, iEnd = matrix_forwardxy.GetLength(0); i < iEnd; ++i)
                {
                    int jEnd = matrix_forwardxy.GetLength(1) - 1;
                    matrix_forwardxy[i, jEnd] = calculation.Calculate2Arg(matrix_forwardxy[i, 0], matrix_forwardxy[i, 1]);

                    matrix[i, jEnd] = calculation.Calculate2Arg(matrix[i, 0], matrix[i, 1]);

                    if (matrix_forwardxy[i, jEnd] < minFuncVal)
                    {
                        minFuncIdx = i;
                        minFuncVal = matrix_forwardxy[i, jEnd];
                    }

                    if (matrix[i, jEnd] < minFuncVal)
                    {
                        minFuncIdx = i;
                        minFuncVal = matrix[i, jEnd];
                    }
                }

                if (minFuncIdx == -1)
                {
                    return new double[2] { x, y };
                }
                else
                {
                    x = matrix_forwardxy[minFuncIdx, 0];
                    y = matrix_forwardxy[minFuncIdx, 1];
                }
            }

            return new double[2] { x, y };
        }


    }
}