using MathFunctionWPF.MathMethods;
using org.mariuszgromada.math.mxparser;

namespace MathFunctionWPF.Models
{
    internal class FunctionCalculation
    {
        FunctionInputData _sourceData { get; set; }
        Function _function;
        string _functionExpression;

        double _multiplier = 1;

        Argument _argument0;
        string _argName;
        Expression _der1Exp, _der2Exp, _der3Exp, _der4Exp;

        public bool IsInverse
        {
            get
            {
                return _multiplier < 0;
            }
            set
            {
                if (value)
                {
                    _multiplier = -1;
                }
                else
                {
                    _multiplier = 1;
                }
            }
        }

        public FunctionCalculation(FunctionInputData sourceData)
        {
            SetFunctionSourceData(sourceData);
        }

        public double Calculate(double argX)
        {
            return _multiplier * _function.calculate(argX);
        }

        public double CalculateDer2(double argX)
        {
            _der2Exp.setArgumentValue(_argName, argX);
            return _multiplier * _der2Exp.calculate();
        }

        public double CalculateDer3(double argX)
        {
            _der3Exp.setArgumentValue(_argName, argX);
            return _multiplier * _der3Exp.calculate();
        }

        public double CalculateDer4(double argX)
        {
            _der4Exp.setArgumentValue(_argName, argX);
            return _multiplier * _der4Exp.calculate();
        }

        public double CalculateDer1(double argX)
        {
            _der1Exp.setArgumentValue(_argName, argX);

            return _der1Exp.calculate();
        }

        public void SetFunctionSourceData(FunctionInputData data)
        {
            _sourceData = data;
            UpdateFunction();
        }
        public int CountArgs()
        {
            return _function.getArgumentsNumber();
        }
        void UpdateFunction()
        {
            var function = new Function(_sourceData.Formula);
            if (function.checkSyntax())
            {
                _function = function;
                _functionExpression = function.getFunctionExpressionString();
                _argument0 = _function.getArgument(0);
                _argName = _argument0.getArgumentName();

                string expressionString = $"der(der({_functionExpression}, {_argName}), {_argName})";
                _der2Exp = new Expression(expressionString);
                _der2Exp.addArguments(new Argument(_argName, 0));

                expressionString = $"der({_functionExpression}, {_argName})";
                _der1Exp = new Expression(expressionString);
                _der1Exp.addArguments(new Argument(_argName, 0));

                expressionString = $"der(der(der({_functionExpression}, {_argName}), {_argName}), {_argName})";
                _der3Exp = new Expression(expressionString);
                _der3Exp.addArguments(new Argument(_argName, 0));

                expressionString = $"der(der(der(der({_functionExpression}, {_argName}), {_argName}), {_argName}), {_argName})";
                _der4Exp = new Expression(expressionString);
                _der4Exp.addArguments(new Argument(_argName, 0));

            }
            else
            {
                throw new Exception(_sourceData.Formula + " не является формулой");
            }
        }

        public string Formula
        {
            get
            {
                return _sourceData.Formula;
            }
        }

        public double SecondDerivativeCheck(double x, double h)
        {
            // Производная с обеих сторон
            double firstDerivativeLeft = CentralDifferenceSecondDerivative(x - h, h);
            double firstDerivativeRight = CentralDifferenceSecondDerivative(x + h, h);

            // Возвращаем разницу производных (приближение второй производной)
            return (firstDerivativeRight - firstDerivativeLeft) / (2 * h);
        }

        public double CentralDifferenceSecondDerivative(double x, double h)
        {
            double fx_minus_h = _function.calculate(x - h);
            double fx = _function.calculate(x);
            double fx_plus_h = _function.calculate(x + h);
            return (fx_plus_h - 2 * fx - fx_minus_h) / (h * h);
        }

        public double CalculateDer3(double x, double h)
        {
            return (CalculateDer2(x - h) - CalculateDer2(x + h)) / (2 * h);
        }



        public List<double> FindDiscontinuities(double start, double end)
        {
            if (false)
            {
                return FindDiscontinuitiesOld(start, end);
            }
            else
            {
                return FindDiscontinuitiesNew(start, end);
            }
        }
        public List<double> FindDiscontinuitiesOld(double start, double end)
        {

            const double minStep = 0.00002;
            double step = 0.00001;
            List<double> discontinuities = new List<double>();

            // Итерируем по диапазону от start до end с шагом step
            for (double x = start; x <= end; x += step)
            {
                // Вычисляем значения функции и её производных в точке x и x + step
                double fx = _function.calculate(x);
                double fx_next = _function.calculate(x + step);

                if (double.IsNaN(fx_next))
                {
                    discontinuities.Add(x - step);
                    discontinuities.Add(x + step);
                    x += step;
                    continue;
                }
                double dfx = CalculateDer1(x);
                double dfx_next = CalculateDer1(x + step);

                double ddfx = CalculateDer2(x);
                //double ddfx_next = CalculateDer2(x + step);

                //double dddfx = CalculateDer3(x, 0.001);
                //double dddfx_next = CalculateDer3(x + step, 0.001);

                // Динамические пороги, зависящие от производных
                // Ожидаемое значение равно 
                //double thresholdFunction = Math.Max(1e-6, (Math.Abs(dfx * step + ddfx * step * step / 2) * 1.001));
                double dddfx = CalculateDer3(x);
                double thresholdFunction = dfx * step + ddfx * step * step / 2 + dddfx * Math.Pow(step, 3) / 6;
                //double thresholdDer1 = Math.Max(1e-6, step * Math.Abs(ddfx));
                //double thresholdDer2 = Math.Max(1e-6, step * Math.Abs(dddfx));

                // Проверка разрывов
                //if (Math.Abs(fx_next - fx) > thresholdFunction
                if (fx_next - fx > thresholdFunction
                     && (fx_next - fx) - thresholdFunction > step)
                {
                    if (dfx_next - dfx > ddfx * step)//|| (ddfx > 0 && dfx_next < dfx || ddfx < 0 && dfx_next > dfx)
                    {
                        if (CalculateDer2(x + step) - ddfx > CalculateDer3(x) * step)
                        {
                            discontinuities.Add(x);
                            discontinuities.Add(x + step);
                            x += step;
                            continue;
                        }
                    }

                    else if (Math.Abs(dfx_next - dfx) < 0.01)
                    {
                        if (step < 0.05) step *= 2;
                    }
                    else
                    {
                        //Уменьшаем шаг
                        if (step > minStep) step /= 2;
                    }
                }
                else if ((dfx_next - dfx > ddfx * step))//|| (ddfx > 0 && dfx_next < dfx || ddfx < 0 && dfx_next > dfx)
                {
                    if (CalculateDer2(x + step) - ddfx > CalculateDer3(x, step) * step && (fx_next - fx) - thresholdFunction > step)
                    {
                        discontinuities.Add(x);
                        discontinuities.Add(x + step);
                        x += step;
                        continue;
                    }
                    else
                    {
                        if (step < 0.05) step *= 2;
                    }
                    //discontinuities.Add(x);
                }
                else
                {
                    if (step < 0.05) step *= 2;
                }
            }

            return discontinuities;
        }


        public List<double> FindDiscontinuitiesNew(double start, double end)
        {
            const double defaultStep = 0.00001;
            double step = defaultStep;
            List<double> discontinuities = new List<double>();
            const double minStep = 0.002;
            const double maxStep = 0.05;

            // Итерируем по диапазону от start до end с шагом step
            for (double x = start; x <= end; x += step)
            {
                // Вычисляем значения функции и её производных в точке x и x + step
                double fx = _function.calculate(x);
                double fx_next = _function.calculate(x + step);
                double dfx = CalculateDer1(x);

                //step = fx / dfx;
                //step = Math.Abs(step);
                //if (step< minStep)
                //{
                //    step = minStep;
                //}
                //if (step> maxStep)
                //{
                //    step = maxStep;
                //}


                if (double.IsNaN(fx_next))
                {
                    discontinuities.Add(x - step);
                    discontinuities.Add(x + step);
                    x += step;
                    step = defaultStep;
                    continue;
                }
                double dfx_next = CalculateDer1(x + step);

                double ddfx = CalculateDer2(x);

                // Динамические пороги, зависящие от производных
                double dddfx = CalculateDer3(x);

                double thresholdFunction = dfx * step + ddfx * step * step / 2 + dddfx * Math.Pow(step, 3) / 6;
                if (thresholdFunction > 0 && fx_next < fx || thresholdFunction < 0 && fx_next > fx)
                {
                    discontinuities.Add(x);
                    discontinuities.Add(x + step);
                    x += step;
                    step = defaultStep;
                    continue;
                }
                else
                {
                    if (fx_next - fx > thresholdFunction && (fx_next - fx) - thresholdFunction > step)
                    {
                        if (dfx_next - dfx > ddfx * step)//|| (ddfx > 0 && dfx_next < dfx || ddfx < 0 && dfx_next > dfx)
                        {
                            if (CalculateDer2(x + step) - ddfx > CalculateDer3(x) * step)
                            {
                                // Слишком высокая скорость изменения функции
                                //if (step > minStep) step /= 2;
                                continue;
                            }
                        }

                        else if (Math.Abs(dfx_next - dfx) < 0.01)
                        {
                            if (step < maxStep) step *= 2;
                        }
                        else
                        {
                            //Уменьшаем шаг
                            if (step > minStep) step /= 2;
                        }
                    }
                    else if ((dfx_next - dfx > ddfx * step))//|| (ddfx > 0 && dfx_next < dfx || ddfx < 0 && dfx_next > dfx)
                    {
                        if (CalculateDer2(x + step) - ddfx > dddfx * step && (fx_next - fx) - thresholdFunction > step)
                        {
                            // Слишком высокая скорость изменения функции
                            //if (step > minStep) step /= 2;
                            continue;
                        }
                        else
                        {
                            if (step < maxStep) step *= 2;
                        }
                    }
                    else
                    {
                        if (step < maxStep) step *= 2;
                    }
                }

            }

            return discontinuities;
        }


        public List<double> FindDiscontinuitiesNew2(double start, double end)
        {
            double step = 0.1;
            List<double> discontinuities = new List<double>();

            // Итерируем по диапазону от start до end с шагом step
            for (double x = start; x <= end; x += step)
            {
                // Вычисляем значения функции и её производных в точке x и x + step
                double fx = _function.calculate(x);
                double fx_next = _function.calculate(x + step);

                if (double.IsNaN(fx_next))
                {
                    discontinuities.Add(x - step);
                    discontinuities.Add(x + step);
                    x += step;
                    continue;
                }
                double dfx = CalculateDer1(x);

                double ddfx = CalculateDer2(x);

                // Динамические пороги, зависящие от производных
                double dddfx = CalculateDer3(x);

                double thresholdFunction = dfx * step + ddfx * step * step / 2 + dddfx * Math.Pow(step, 3) / 6;
                if (thresholdFunction > 0 && fx_next < fx || thresholdFunction < 0 && fx_next > fx)
                {
                    discontinuities.Add(x);
                    discontinuities.Add(x + step);
                    x += step;
                    continue;
                }
            }

            return discontinuities;
        }

        public bool IsHaveDiscontinuities(double x1, double x2)
        {
            double fx = _function.calculate(x1);
            double fx_next = _function.calculate(x2);
            double step = x2 - x1;
            if (double.IsNaN(fx))
            {
                return true;
            }

            if (double.IsNaN(fx_next))
            {
                return true;
            }

            double dfx = CalculateDer1(x1);
            double dfx_next = CalculateDer1(x2);

            double ddfx = CalculateDer2(x1);
            // Ожидаемое значение равно 
            double thresholdFunction = Math.Max(1e-6, (Math.Abs(dfx * step + ddfx * step * step / 2) * 1.001));
            // Проверка разрывов

            if (Math.Abs(fx_next - fx) > thresholdFunction)
            {
                if (dfx_next - dfx > ddfx * step || (ddfx > 0 && dfx_next < dfx || ddfx < 0 && dfx_next > dfx))
                {
                    //discontinuities.Add(x);
                }
                else if (CalculateDer2(x2 + step) - ddfx > CalculateDer3(x1, step) * step)
                {
                    return true;
                }
            }
            else if (dfx_next - dfx > ddfx * step)//|| (ddfx > 0 && dfx_next < dfx || ddfx < 0 && dfx_next > dfx)
            {
                if (CalculateDer2(x2) - ddfx > CalculateDer3(x1, step) * step)
                {
                    return true;
                }
                //discontinuities.Add(x);
            }
            return false;
        }

        // Пример использования
        //public static void Detect(string[] args)
        //{
        //    // Пример функции
        //    //Func<double, double> f = (x) => (x != 0) ? 1 / x : double.PositiveInfinity; // Пример с разрывом в 0
        //    //Func<double, double> f_der1 = (x) => -1 / (x * x);  // Производная f
        //    //Func<double, double> f_der2 = (x) => 2 / (x * x * x);  // Вторая производная f
        //    //Func<double, double> f_der3 = (x) => -6 / (x * x * x * x);  // Третья производная f

        //    // Находим разрывы
        //    List<double> discontinuities = FindDiscontinuities(f, f_der1, f_der2, f_der3, -10, 10, 0.01, 10);

        //    // Вывод разрывов
        //    foreach (double point in discontinuities)
        //    {
        //        Console.WriteLine($"Найден разрыв в точке: {point}");
        //    }
        //}
    }
}
