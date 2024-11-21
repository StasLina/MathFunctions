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
                var rowData = (RecordSortResults)row.Item;

                // Доступ к данным, привязанным к строке
                //MessageBox.Show($"Результат для {rowData.Tile}: {rowData.Results}");
                WInputNumbers wInputNumbers = new WInputNumbers((List<double>)rowData.Results);
                wInputNumbers.ShowDialog();
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
