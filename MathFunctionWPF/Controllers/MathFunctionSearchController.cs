using MathFunctionWPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace MathFunctionWPF.Controllers
{
    internal class MathFunctionSearchController : IBaseController
    {
        TypeMathMethod _currentType = TypeMathMethod.MainMenu;
        MathFunctionSearchView _view;

        public MathFunctionSearchController(MathFunctionSearchView view)
        {
            _view = view;
        }

        public void MethodChanged(TypeMathMethod newMethod)
        {
            //throw new NotImplementedException();
        }

        public MathFunctionSearchView View { get => _view; }


    }
}
