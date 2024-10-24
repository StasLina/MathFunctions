using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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
                //double ddfx = function.CalculateDer2(x); // Вычисляем значение производной в текущей точке

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
                    x = xStart;
                    //throw new Exception("Достигнута минимальная граница");
                }
                if (x > xEnd)
                {
                    x = xEnd;
                    //throw new Exception("Достигнута максимальная граница");
                }
                iteration++; // Увеличиваем счётчик итераций
            }

            throw new Exception("Превышено количество итераций, пересечение с нулём не найдено.");
        }
        public static double CalcMax(FunctionCalculation function, double xStart, double xEnd, double tolerance, int maxIterations)
        {

            double x = xStart + (xEnd - xStart) / 2; // Начальное приближение
            int iteration = 0;

            while (iteration < maxIterations)
            {
                double dfx = function.CalculateDer1(x); // Вычисляем значение производной в текущей точке
                double ddfx = function.CalculateDer2(x); // Вычисляем значение производной в текущей точке

                if (ddfx == 0)
                {
                    //if (dfx > 0) dfx *= -1;
                    //double newX = x - function.Calculate(x) / dfx;
                    //x = newX;

                    if (dfx > 0)
                    {
                        return xEnd;
                    }
                    else
                    {
                        return xStart;
                    }
                }
                else
                {

                    //if (ddfx < 0) ddfx *= -1;
                    if (dfx > 0 && ddfx < 0) ddfx *= -1;

                    double newX = x + dfx / ddfx;

                    // Проверяем, достигли ли мы заданной точности
                    if (Math.Abs(newX - x) < tolerance)
                    {
                        return x; // Возвращаем найденный корень
                    }


                    // Проверяем, что производная не слишком мала
                    if (Math.Abs(dfx) < 1e-10)
                    {
                        throw new Exception("Производная слишком мала, метод не работает.");
                    }

                    // Итерационное приближение по формуле Ньютона
                    x = newX;
                }
                if (x < xStart)
                {
                    return xStart;
                }
                if (x > xEnd)
                {
                    return xEnd;
                }
                iteration++; // Увеличиваем счётчик итераций
            }
            return x;
            //throw new Exception("Превышено количество итераций, решение не найдено.");
        }

        public static double CalcMin2(FunctionCalculation function, double xStart, double xEnd, double tolerance, int maxIterations)
        {
            double x = xStart + (xEnd - xStart) / 2; // Начальное приближение
            int iteration = 0;

            while (iteration < maxIterations)
            {
                double dfx = function.CalculateDer1(x); // Вычисляем значение производной в текущей точке
                double ddfx = function.CalculateDer2(x); // Вычисляем значение производной в текущей точке

                if (ddfx == 0)
                {
                    //if (dfx < 0) dfx *= -1;
                    //double newX = x - function.Calculate(x) / dfx;
                    //x = newX;

                    if (dfx < 0)
                    {
                        return xEnd;
                    }
                    else
                    {
                        return xStart;
                    }
                }
                else
                {
                    if (dfx < 0 && ddfx > 0) ddfx *= -1;

                    double newX = x - dfx / ddfx;

                    // Проверяем, достигли ли мы заданной точности
                    if (Math.Abs(newX - x) < tolerance)
                    {
                        return x; // Возвращаем найденный корень
                    }


                    // Проверяем, что производная не слишком мала
                    if (Math.Abs(dfx) < 1e-10)
                    {
                        throw new Exception("Производная слишком мала, метод не работает.");
                    }

                    // Итерационное приближение по формуле Ньютона
                    x = newX;

                }
                if (x < xStart)
                {
                    return xStart;
                }
                if (x > xEnd)
                {
                    return xEnd;
                }
                iteration++; // Увеличиваем счётчик итераций
            }
            return x;
        }


        public static double CalcMin(FunctionCalculation function, double xStart, double xEnd, double tolerance, int maxIterations)
        {
            double x = xStart + (xEnd - xStart) / 2; // Начальное приближение
            int iteration = 0;

            while (iteration < maxIterations)
            {
                double dfx = function.CalculateDer1(x); // Вычисляем значение производной в текущей точке
                double ddfx = function.CalculateDer2(x); // Вычисляем значение производной в текущей точке

                if (ddfx == 0)
                {
                    if (dfx < 0)
                    {
                        return xEnd;
                    }
                    else
                    {
                        return xStart;
                    }
                }
                else
                {
                    double newX = x - dfx / ddfx;

                    // Проверяем, достигли ли мы заданной точности
                    if (Math.Abs(newX - x) < tolerance)
                    {
                        return x; // Возвращаем найденный корень
                    }

                    // Проверяем, что производная не слишком мала
                    if (Math.Abs(dfx) < 1e-10)
                    {
                        throw new Exception("Производная слишком мала, метод не работает.");
                    }

                    // Итерационное приближение по формуле Ньютона
                    x = newX;

                }
                if (x < xStart)
                {
                    return xStart;
                }
                if (x > xEnd)
                {
                    return xEnd;
                }
                iteration++; // Увеличиваем счётчик итераций
            }
            return x;
            //throw new Exception("Превышено количество итераций, решение не найдено.");

        }
    }
}
