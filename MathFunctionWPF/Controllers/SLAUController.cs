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

        public SLAUController(SLAUMainControl view)
        {
            _view = view;
            _model = new SLAUMainControlModel();

            _dataListMethod = new ObservableCollection<MethodBase>()
            {
                gaus, squre, progonki, simpleIter, hirestDown, complexGradient
            };

            _model.ListMethodsControl = _dataListMethod;
            _model.MatrixDataContent = new DataGrid();
            _model.VectorDataContent = new DataGrid();
            _view.DataContext = _model;

            //_view.Calcculcate.
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

        private void CreateMatrixButton_Click(object sender, RoutedEventArgs e)
        {
            int rows, cols;
            // Создаём таблицу
            {
                //    if (int.TryParse(RowCountTextBox.Text, out rows) && int.TryParse(ColCountTextBox.Text, out cols))
                //    {
                //        // Создаём матрицу с заданными размерами
                //        var matrix = new List<List<double>>();

                //        for (int i = 0; i < rows; i++)
                //        {
                //            var row = new List<double>();
                //            for (int j = 0; j < cols; j++)
                //            {
                //                row.Add(0);  // Изначально заполняем 0
                //            }
                //            matrix.Add(row);
                //        }

                //        // Заполнение DataGrid
                //        MatrixDataGrid.ItemsSource = matrix;
                //    }
                //    else
                //    {
                //        MessageBox.Show("Введите корректные размеры матрицы.");
                //    }
                //}
                //// Создаём вектор
                //{
                //    int vectorSize;
                //    if (int.TryParse(VectorSizeTextBox.Text, out vectorSize))
                //    {
                //        // Создаём вектор с заданным количеством элементов
                //        List<double> vector = new List<double>(new double[vectorSize]);

                //        // Заполнение текста для вектора
                //        VectorTextBox.Text = string.Join(", ", vector);
                //    }
                //    else
                //    {
                //        MessageBox.Show("Введите корректный размер вектора.");
                //    }
                //}
            }


        }
    }
}
