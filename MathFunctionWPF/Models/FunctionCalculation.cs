using org.mariuszgromada.math.mxparser;

namespace MathFunctionWPF.Models
{
    internal class FunctionCalculation
    {
        FunctionInputData _sourceData { get; set; }
        Function _function;

        double _multiplier = 1;

        public bool IsInverse
        {
            get
            {
                return _multiplier < 0;
            }
            set
            {
                if(value)
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
    }
}
