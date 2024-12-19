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

        MathTableMatrix.DynamicTableControl dynamicTable;
        MathTableMatrix.DynamicTableController dynamicTableController;
        public MNKController(MKKMainControl view)
        {
            _view = view;
            _model = new MKKMainControlModel();

            _view.BCalc.Click += BCalc_Click;
            _view.BDraw.Click += BDraw_Click;
            _view.BResize.Click += BResize_Click;

            dynamicTable = new MathTableMatrix.DynamicTableControl();
            dynamicTableController = new MathTableMatrix.DynamicTableController(dynamicTable);
            dynamicTableController.InitializeDynamicTable(0, 0);
            _method = new TypeMathMethod();

            _model.InputView = dynamicTable;
            _model.ListInputControl = new System.Collections.ObjectModel.ObservableCollection<MethodNameList>()
            {
                new MethodNameList("RowCount")
                {
                    TypeTitle = "Количество значений",
                    Value = "Key",
                }
            };

            Reset();
        }

        private string GetValue(string key)
        {
            foreach (var item in _model.ListInputControl)
            {
                if (item.Key == key)
                {
                    return item.Value;
                }
            }
            return "";
        }

        private int GetRowCount()
        {
            string value = GetValue("RowCount");
            
            if (int.TryParse(value, out var rowCount))
            {
                if(rowCount> 0)
                    return rowCount;
            }
            return 0;
        }

        private void BResize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            dynamicTableController.InitializeDynamicTable(GetRowCount(), 2);
        }

        private void BDraw_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BCalc_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dataColumnX = dynamicTableController.GetDataColumn(0);
            if (dataColumnX.Length > 0)
            {
                var dataColumnY = dynamicTableController.GetDataColumn(1);

                double[] args = MNK.models.LeastSquares.Calculate(dataColumnX, dataColumnY, 1);
                double[] args2 = MNK.models.LeastSquares.Calculate(dataColumnX, dataColumnY, 2);

            }

        }

        void Reset()
        {
            _model.Func1Result = "Не определено";
            _model.Func2Result = "Не определено";
        }

        public void MethodChanged(TypeMathMethod newMethod)
        {
            switch (newMethod)
            {
                case TypeMathMethod.MNK:
                    {
                        _method = TypeMathMethod.MNK;
                        _view.DataContext = _model;
                    }
                    break;
            }
        }
    }
}
