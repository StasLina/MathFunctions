using MathFunctionWPF.Models;
using System;
using System.Collections.Generic;
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
using MathData;

namespace MathFunctionWPF.Views
{
    /// <summary>
    /// Логика взаимодействия для BubleSortView.xaml
    /// </summary>
    public partial class BubleSortView : UserControl
    {
        public BubleSortView()
        {
            InitializeComponent();
            DataContext = new BubbleModel();
        }

        public delegate void SaveClickEvent(List<double> e);
        public SaveClickEvent eventSaveClick { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Получаем кнопку, которая была нажата
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {
                //DataGridCell cell = Utils.Utils.GetParent<DataGridCell>(clickedButton);

                //// Получаем строку, к которой принадлежит эта ячейка
                //DataGridRow row = GetParent<DataGridRow>(cell);

                //// Извлекаем данные из строки (например, экземпляр объекта RecordSortResults)
                //var rowData = (RecordSortResults)row.Item;

                //// Доступ к данным, привязанным к строке
                //List<double> results = (List<double>)rowData.Results;
                
                //WInputNumbers wInputNumbers = new WInputNumbers(results);

                //wInputNumbers.ClickButton.Click += (object sender, RoutedEventArgs e) =>
                //{
                //    eventSaveClick?.Invoke(results);
                //};
                //wInputNumbers.ShowDialog();
            }
        }

        

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Drawing.Width = e.NewSize.Width;
            Drawing.Height = e.NewSize.Height;

            // Если нужно оповестить OxyPlot о перерисовке
            if (Drawing.Model != null)
            {
                Drawing.Model.InvalidatePlot(true);
            }

        }
    }


}
