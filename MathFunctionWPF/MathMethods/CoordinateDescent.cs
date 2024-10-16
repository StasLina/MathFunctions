using MathFunctionWPF.Models;

namespace MathFunctionWPF.MathMethods
{
    internal class CoordinateDescent
    {
        // Целевая функция для минимизации

        // Метод координатного спуска
        public static double Calc(FunctionCalculation calculation, double minX, double maxX, double learningRate, int iterations)
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

                if(currentPoint < minX)
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
    }
}