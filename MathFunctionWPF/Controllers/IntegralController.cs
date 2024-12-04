using MathFunctionWPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MathFunctionWPF.Controllers
{
    internal class IntegralController : IBaseController
    {

        TypeMathMethod _method;
         Integral.Controls.IntegralControl  _view = null;

        public Control View { get => _view; }
        public void MethodChanged(TypeMathMethod newMethod)
        {
            //throw new NotImplementedException();
            switch (newMethod)
            {
                case TypeMathMethod.Integral:
                    {
                        _method = TypeMathMethod.Integral;
                    }
                    break;
            }
        }

        public IntegralController(Integral.Controls.IntegralControl sortView)
        {
            _view = sortView;
        }
    }
}
