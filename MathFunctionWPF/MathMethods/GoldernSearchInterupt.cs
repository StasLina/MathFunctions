using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MathFunctionWPF.Models;
namespace MathFunctionWPF.MathMethods
{
    internal class GoldernSearchInterupt
    {
        private static readonly double Phi = (1 + Math.Sqrt(5)) / 2;

        // Метод для нахождения минимума функции f на интервале [a, b]
        public static double Calc(FunctionCalculation functionCalculation, double a, double b, double tolerance = 1e-5)
        {
            // Инициализация границ интервала
            //double left = b - (b - a) / Phi;
            double right = a + (b - a) / Phi;

            // Выполняем поиск до достижения заданной точности
            while (Math.Abs(b - a) > tolerance)
            {
                if (functionCalculation.IsHaveDiscontinuities(a,right))
                {
                    // Усекаем правую часть
                    b = right;
                    right = a + (b - a) / Phi;
                }
                else
                {
                    //Усекаем левую часть
                    a = right;
                    //left = right;
                    right = a + (b - a) / Phi;
                }
            }

            // Возвращаем середину суженного интервала как результат
            return (a + b) / 2;
        }
    }
}