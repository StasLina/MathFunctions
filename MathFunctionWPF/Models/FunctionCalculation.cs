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
        Expression _der2Exp;
        Expression _der1Exp;

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

            return _der2Exp.calculate();
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

        public double CentralDifferenceSecondDerivative( double x, double h)
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
                    discontinuities.Add(x);
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
                double thresholdFunction = Math.Max(1e-6, (Math.Abs(dfx * step + ddfx * step * step / 2) * 1.001));
                //double thresholdDer1 = Math.Max(1e-6, step * Math.Abs(ddfx));
                //double thresholdDer2 = Math.Max(1e-6, step * Math.Abs(dddfx));

                if(x> 9.4247589999999832 & x < 3 * Math.PI)
                {
                    int a = 0;
                }
                // Проверка разрывов
                if (Math.Abs(fx_next - fx) > thresholdFunction
                    )
                {
                    if (dfx_next - dfx > ddfx * step || (ddfx > 0 && dfx_next < dfx || ddfx < 0 && dfx_next > dfx))
                    {
                        discontinuities.Add(x);
                    }
                    else if (CalculateDer2(x + step) - ddfx > CalculateDer3(x, step) *step) {
                        discontinuities.Add(x);
                    }
                    else
                    {
                        //Уменьшаем шаг
                        if (step > minStep) step /= 2;
                    }
                }
                else if (dfx_next - dfx > ddfx * step || (ddfx > 0 && dfx_next < dfx || ddfx < 0 && dfx_next > dfx))
                {
                    discontinuities.Add(x);
                }
                else
                {
                    step *= 2;
                }
            }

            return discontinuities;
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
