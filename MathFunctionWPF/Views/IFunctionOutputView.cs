using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathFunctionWPF.Views
{
    public delegate void ButtonClick();
    public enum TypeMathResult
    {
        MinimumArgument, 
        MaximumArgument, 
        IntespectionArgument, 
        MinimumValue, 
        MaximumValue, 
        IntespectionValue,
        Derevative1,
        Derevative2
    }

    internal interface IFunctionOutputView
    {
        public void AddListenerUpdatePlotter(ButtonClick listener);
        public void AddListenerUpdateFunction(ButtonClick listener);

        public void SetResult(TypeMathResult typeResult, string value);
    }
}
