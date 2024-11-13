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

                    break;
            }
        }

        MathSortView _view;
        public MathSortingController(MathSortView sortView)
        {
            _view = sortView;
        }

    }
}
