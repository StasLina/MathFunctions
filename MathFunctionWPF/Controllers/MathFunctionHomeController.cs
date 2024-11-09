using MathFunctionWPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathFunctionWPF.Views;

namespace MathFunctionWPF.Controllers
{
    internal class MathFunctionHomeController : IBaseController
    {
        TypeMathMethod _currentType = TypeMathMethod.MainMenu;
        MathFunctionHomeView _view;

        public MathFunctionHomeController(MathFunctionHomeView view)
        {
            this._view = view;
        }

        public void MethodChanged(TypeMathMethod newMethod)
        {
            //throw new NotImplementedException();
        }

        public MathFunctionHomeView View { get { return _view; } }
    }
}
