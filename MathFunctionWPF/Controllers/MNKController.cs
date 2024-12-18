using MathFunctionWPF.SLAU.Controls;
using MathFunctionWPF.SLAU.ViewModels;
using MathFunctionWPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using MathFunctionWPF.MNK.views;
using MathFunctionWPF.MNK.viewmodels;

namespace MathFunctionWPF.Controllers
{
    class MNKController : IBaseController
    {
        private TypeMathMethod _method;

        MKKMainControl _view = null;

        MKKMainControlModel _model;

        public Control View => _view;

        public MNKController(MKKMainControl view)
        {
            _view = view;
        }

        public void MethodChanged(TypeMathMethod newMethod)
        {
            switch (newMethod)
            {
                case TypeMathMethod.MNK:
                    {
                        _method = TypeMathMethod.MNK;
                    }
                    break;
            }
        }
    }
}
