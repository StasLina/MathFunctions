using MathFunctionWPF.Models;
using MathFunctionWPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using MathFunctionWPF.SLAU.Controls;
using MathFunctionWPF.SLAU.ViewModels;
using MathFunctionWPF.SLAU.Models;
using System.Windows;
using System.ComponentModel;

namespace MathFunctionWPF.Controllers
{
    class SLAUController : IBaseController
    {
        private TypeMathMethod _method;

        SLAUMainControl _view = null;

        SLAUMainControlModel _model;

        Gaus gaus = new Gaus() { IsChecked = true };
        Squre squre = new Squre() { IsChecked = true };
        Progonki progonki = new Progonki() { IsChecked = true };
        SimpleIter simpleIter = new SimpleIter() { IsChecked = true };
        HirestDown hirestDown = new HirestDown() { IsChecked = true };
        ComplexGradient complexGradient = new ComplexGradient() { IsChecked = true };
        ObservableCollection<MethodBase> _dataListMethod;

        MathTableMatrix.DynamicTableController _matrixGrid, _vectorGrid;

        private List<DynamicRow> _tableData;
        public SLAUController(SLAUMainControl view)
        {
            _view = view;
            _model = new SLAUMainControlModel();

            _dataListMethod = new ObservableCollection<MethodBase>()
            {
                gaus, squre, progonki, simpleIter, hirestDown, complexGradient
            };

            _model.ListMethodsControl = _dataListMethod;

            

            // Создаём таблицы


            _matrixGrid = new MathTableMatrix.DynamicTableController(
            new MathTableMatrix.DynamicTableControl(), new MathTableMatrix.DynamicTableControlModel());
            _matrixGrid.InitializeDynamicTable(_model.Rows, _model.Columns);

            _vectorGrid = new MathTableMatrix.DynamicTableController(
            new MathTableMatrix.DynamicTableControl(), new MathTableMatrix.DynamicTableControlModel());
            _vectorGrid.InitializeDynamicTable(_model.Rows, 1);    

            _model.MatrixDataContent = _matrixGrid.View;
            _model.VectorDataContent = _vectorGrid.View;
            _view.DataContext = _model;

            _view.BChangeSize.Click += CreateMatrixButton_Click;

            _vectorGrid.View.SizeChanged += View_SizeChanged;
            //_view.Calcculcate.
        }

        private void View_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //throw new NotImplementedException();
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
        private void InitializeDynamicTable(int rows, int cols)
        {
            _tableData = new List<DynamicRow>();

            for (int i = 0; i < rows; i++)
            {
                var row = new DynamicRow();
                for (int j = 0; j < cols; j++)
                {
                    row[$"Col{j + 1}"] = 0; // Заполняем нулями
                }
                _tableData.Add(row);
            }
        }
        private void CreateMatrixButton_Click(object sender, RoutedEventArgs e)
        {

            int rows = _model.Rows, cols = _model.Columns;
            // Создаём таблицу
            {
                if (rows > 0 && cols > 0)
                {
                    _matrixGrid.InitializeDynamicTable(rows, cols);
                    _vectorGrid.InitializeDynamicTable(rows, 1);
                }
                else
                {
                    MessageBox.Show("Введите корректные размеры матрицы.");
                }
            }

        }
    }

    public class DynamicRow : INotifyPropertyChanged
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        public object this[string propertyName]
        {
            get => _values.ContainsKey(propertyName) ? _values[propertyName] : null;
            set
            {
                if (_values.ContainsKey(propertyName))
                {
                    _values[propertyName] = value;
                }
                else
                {
                    _values.Add(propertyName, value);
                }
                OnPropertyChanged(propertyName);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
