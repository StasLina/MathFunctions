using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathFunctionWPF.Models;

namespace MathFunctionWPF.MathMethods
{
    class NewtonMethod
    {
        public static double Calc(FunctionCalculation function, double xStart, double xEnd, double tolerance, int maxIterations)
        {
            double x = xStart + (xEnd - xStart) / 2; // Начальное приближение
            int iteration = 0;

            while (iteration < maxIterations)
            {
                double fx = function.Calculate(x); // Вычисляем значение функции в текущей точке
                double dfx = function.CalculateDer1(x); // Вычисляем значение производной в текущей точке

                // Проверяем, достигли ли мы заданной точности
                if (Math.Abs(fx) < tolerance)
                {
                    return x; // Возвращаем найденный корень
                }

                // Проверяем, что производная не слишком мала
                if (Math.Abs(dfx) < 1e-10)
                {
                    throw new Exception("Производная слишком мала, метод не работает.");
                }

                // Итерационное приближение по формуле Ньютона
                x = x - fx / dfx;

                if (x < xStart)
                {
                    throw new Exception("Достигнута минимальная граница");
                }
                if (x > xEnd)
                {
                    throw new Exception("Достигнута максимальная граница");
                }
                iteration++; // Увеличиваем счётчик итераций
            }

            throw new Exception("Превышено количество итераций, решение не найдено.");
        }
    }
}
