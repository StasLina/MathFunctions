using MathData;
using MathFunctionWPF.Controllers;
using MathFunctionWPF.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MathFunctionWPF.SLAU.Controls
{
    /// <summary>
    /// Логика взаимодействия для ResultTableControl.xaml
    /// </summary>
    /// 




    public partial class ResultTableControl : UserControl
    {
        public event EventHandler? EventSaveClick;
        
        public ResultTableControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Получаем кнопку, которая была нажата
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {
                DataGridCell cell = GetParent<DataGridCell>(clickedButton);

                // Получаем строку, к которой принадлежит эта ячейка
                DataGridRow row = GetParent<DataGridRow>(cell);

                // Извлекаем данные из строки (например, экземпляр объекта RecordSortResults)
                var rowData = (Models.RecordSlauResults)row.Item;

                if (rowData.ResultMatrix == null) return;

                var resultView = new WInputNumbers(rowData.ResultMatrix.ToList());


                var resultViewModel = new ViewModels.ResultViewModels();

                var matrixGrid = new MathTableMatrix.DynamicTableController(
                new MathTableMatrix.DynamicTableControl(), new MathTableMatrix.DynamicTableControlModel());
                matrixGrid.SetData(rowData.ResultMatrix);

                resultView.ClickButton.Click += (object sender, RoutedEventArgs e) =>
                {
                    // new EventArgs();
                    EventSaveClick?.Invoke(this, new Events.ArgsResultSave() { Controller = matrixGrid });
                };

                resultView.ShowDialog();

                //var resultView = new Views.ResultView();


                //resultViewModel.Table = matrixGrid.View;
                //resultView.DataContext = resultViewModel;

                //// Доступ к данным, привязанным к строке
                //resultView.BSave.Click += (object sender, RoutedEventArgs e) =>
                //{
                //    // new EventArgs();
                //    EventSaveClick?.Invoke(this, new Events.ArgsResultSave() { Controller = matrixGrid});
                //};

                //resultView.ShowDialog();
            }
        }

        private T GetParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            // Если родитель не найден, возвращаем null
            if (parent == null)
                return null;

            // Если это нужный тип, возвращаем его
            if (parent is T parentT)
                return parentT;

            // Иначе рекурсивно ищем дальше
            return GetParent<T>(parent);
        }
    }
}
