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
        Minimum, Maximum, Intespection, MinimumValue, MaximumValue
    }

    internal interface IFunctionOutputView
    {
        public void AddListenerUpdatePlotter(ButtonClick listener);
        public void AddListenerUpdateFunction(ButtonClick listener);

        public void SetResult(TypeMathResult typeResult, string value);
    }
}
