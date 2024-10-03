using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathFunctionWPF.MathMethods
{
    internal class ItegratioMethods
    {
    }


    class NumericalIntegration
    {
        // Метод прямоугольников (левый)
        public static double RectangleMethod(Func<double, double> func, double a, double b, int n)
        {
            double h = (b - a) / n;  // Шаг
            double sum = 0.0;

            for (int i = 0; i < n; i++)
            {
                double x = a + i * h;
                sum += func(x);  // Суммируем значения функции в левых точках отрезков
            }

            return sum * h;  // Умножаем на шаг для получения результата
        }

        // Метод трапеций
        public static double TrapezoidMethod(Func<double, double> func, double a, double b, int n)
        {
            double h = (b - a) / n;  // Шаг
            double sum = 0.5 * (func(a) + func(b));  // Добавляем концы отрезков с весом 1/2

            for (int i = 1; i < n; i++)
            {
                double x = a + i * h;
                sum += func(x);  // Суммируем значения функции во внутренних точках
            }

            return sum * h;  // Умножаем на шаг для получения результата
        }


        // Метод парабол (формула Симпсона)
        public static double SimpsonMethod(Func<double, double> func, double a, double b, int n)
        {
            if (n % 2 != 0)  // Симпсоновская формула требует четное число интервалов
            {
                throw new ArgumentException("n должно быть четным числом");
            }

            double h = (b - a) / n;  // Шаг
            double sum = func(a) + func(b);  // Суммируем значения функции на концах отрезка

            for (int i = 1; i < n; i++)
            {
                double x = a + i * h;
                if (i % 2 == 0)
                {
                    sum += 2 * func(x);  // Четные индексы с коэффициентом 2
                }
                else
                {
                    sum += 4 * func(x);  // Нечетные индексы с коэффициентом 4
                }
            }

            return sum * h / 3.0;  // Окончательная формула Симпсона
        }
    }


}
