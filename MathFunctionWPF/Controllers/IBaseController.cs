using System.Windows.Controls;

namespace MathFunctionWPF.Controllers
{
    internal interface IBaseController
    {
        public void MethodChanged(MathFunctionWPF.Views.TypeMathMethod newMethod);
        public Control View { get;}   
    }

}
