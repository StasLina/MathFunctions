using MathFunctionWPF.Models;
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
        public static double RectangleMethod(Func<double, double> func, double a, double b, double n)
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
        public static double TrapezoidMethod(Func<double, double> func, double a, double b, double n)
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
        public static double SimpsonMethod(Func<double, double> func, double a, double b, double n)
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

        public static double CalculateCountIterations(FunctionCalculation calculation, double a, double b, double epsilon)
        {
            double count;
            double derivative4Max = calculation.CalculateDer4(a);

            //if (double.IsNaN(derivative4Max))
            //{
            //    derivative4Max = calculation.CalculateDer4(b);
            //}

            if (derivative4Max > epsilon/2)
            {
                //double derivative4Temp = calculation.CalculateDer4((b - a) / 2);

                //if (Math.Abs(derivative4Temp) > Math.Abs(derivative4Max))
                //{
                //    derivative4Max = derivative4Temp;
                //}

                //derivative4Temp = calculation.CalculateDer4(b);

                //if (Math.Abs(derivative4Temp) > Math.Abs(derivative4Max))
                //{
                //    derivative4Max = derivative4Temp;
                //}

                derivative4Max = Math.Abs(derivative4Max);
                //count = Math.Pow(Math.Pow(b - a, 5) * derivative4Max / 180 / epsilon, 1 / 4);
                count = Math.Pow(b - a, 5) * derivative4Max / 180 / epsilon;
                count = Math.Pow(count, 0.25);
                count = Math.Ceiling(count);
                
                if (count % 2 != 0)
                {
                    ++count;
                }
            }
            else
            {
                // Вторая проивзодная линеная
                double derivative2a = calculation.CalculateDer2(a);

                if (derivative2a != 0)
                {
                    double derivative2b = calculation.CalculateDer2(b);
                    double derivative2Max = Math.Abs(derivative2a);

                    if (derivative2Max < Math.Abs(derivative2b))
                    {
                        derivative2Max = Math.Abs(derivative2b);
                    }

                    count = Math.Pow(Math.Pow(b - a, 3) / 12 / epsilon * derivative2Max, 1 / 2);
                    // Количество трапеция равно O(1/n^2) поэтому квадрат сохраняем
                    //count = Math.Pow(b - a, 3) / 12 / epsilon * derivative2Max;
                    count = Math.Ceiling(count);
                }
                else
                {
                    // Вторая производная равна нулю линейная функция
                    double derivative1a = calculation.CalculateDer1(a);

                    if (derivative1a != 0)
                    {
                        count = Math.Pow(b - a, 2) / 2 / epsilon * derivative1a;
                        count = Math.Ceiling(count);
                    }
                    else
                    {
                        // Константная функция
                        count = 1;
                    }
                }
            }

            return count;
        }
    }

}
