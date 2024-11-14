using MathFunctionWPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathFunctionWPF.Controllers
{
    class MathSortingController : IBaseController
    {
        public void MethodChanged(TypeMathMethod newMethod)
        {
            //throw new NotImplementedException();
            switch (newMethod)
            {
                case TypeMathMethod.BubbleSort:
                    //_view.SortView.
                    break;
            }
        }

        MathSortView _view;

        public MathSortView View
        {
            get { return _view; }
        }
        public MathSortingController(MathSortView sortView)
        {
            _view = sortView;
        }


    }
}
