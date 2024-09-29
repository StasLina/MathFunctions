namespace MathFunctionWPF.MathMethods
{
    //public class Dihtomia()
    //{
    //    public static double Calc(Func<double, double> _func,double a, double b, double e)
    //    {
    //        double c = (a + b) / 2;

    //        while (b - a >= e)
    //        {
    //            if (_func(a) * _func(c) < 0)
    //            {
    //                b = c;
    //            }
    //            else
    //            {
    //                a = c;
    //            }

    //            c = (a + b) / 2;
    //        }

    //        double y1 = Math.Abs(_func(a)), y2 = Math.Abs(_func(b));
    //        if (y1 > y2)
    //        {
    //            return b;
    //        }
    //        else
    //        {
    //            return a;
    //        }
    //    }
    //}

    public class BisectionMethod
    {
        public static double Calc(Func<double, double> _func, double a, double b, double e)
        {
            // Проверка, что значения a и b подходят
            if (_func(a) * _func(b) >= 0)
            {
                throw new ArgumentException("Функция должна менять знак на интервале [a, b].");
            }

            double midpoint = 0;

            while ((b - a) / 2 > e) // Пока длина интервала больше заданной точности
            {
                midpoint = (a + b) / 2; // Находим середину

                // Проверяем, где функция меняет знак
                if (_func(midpoint) == 0) // Если мы нашли корень
                {
                    return midpoint;
                }
                else if (_func(a) * _func(midpoint) < 0) // Корень находится в [a, midpoint]
                {
                    b = midpoint;
                }
                else // Корень находится в [midpoint, b]
                {
                    a = midpoint;
                }
            }

            return (a + b) / 2; // Возвращаем приближенный корень
        }
    }



}
