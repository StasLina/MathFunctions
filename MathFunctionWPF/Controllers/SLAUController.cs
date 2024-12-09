using MathFunctionWPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MathFunctionWPF.Controllers
{
    class SLAUController : IBaseController
    {
        private TypeMathMethod _method;

        SLAU.Controls.SLAUMainControl _view = null;

        public SLAUController(SLAU.Controls.SLAUMainControl view)
        {
            _view = view;
        }

        public Control View { get => _view; }

        public void MethodChanged(TypeMathMethod newMethod)
        {
            switch (newMethod)
            {
                case TypeMathMethod.SLAU:
                    {
                        _method = TypeMathMethod.SLAU;
                    }
                    break;
            }
        }
    }
}
