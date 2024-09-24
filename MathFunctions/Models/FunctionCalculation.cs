using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;

namespace MathFunctions.Models
{
    internal class FunctionCalculation
    {
        FunctionSourceData _sourceData { get; set; }
        Function _function;
        
        public FunctionCalculation(FunctionSourceData sourceData)
        {
            SetFunctionSourceData(sourceData);
        }

        public double Calculate(double argX)
        {
            return _function.calculate(argX);
        }

        public void SetFunctionSourceData(FunctionSourceData data)
        {
            _sourceData = data;
            UpdateFunction();
        }

        void UpdateFunction()
        {
            _function = new Function(_sourceData.Formula);
        }
    }
}
