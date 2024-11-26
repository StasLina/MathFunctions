using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    /// 

    public class MyData
    {
        public double Value { get; set; }
    }

    public partial class UserControl1 : UserControl
    {
        List<double> _listElements;

        static readonly  string _comma =".";
        public UserControl1(List<double> listElements)
        {

            _listElements = listElements;
            InitializeComponent();

            DataCollection =  new ObservableCollection<MyData>();

            foreach(var element in _listElements)
            {
                DataCollection.Add(new MyData { Value = element});
            }
            //{
            //    new MyData { Value = 1.23 },
            //    new MyData { Value = 4.56 },
            //    new MyData { Value = 7.89 }
            //};

            this.BSave.Click += BSave_Click;
            
            // Привязываем данные к DataGrid
            DataGrid.ItemsSource = DataCollection;
            

            // Обработчик ввода текста в ячейки DataGrid
            DataGrid.PreviewTextInput += DataGrid_PreviewTextInput;
            DataGrid.CellEditEnding += DataGrid_CellEditEnding;

            this.Loaded += UserControl1_Loaded;
            DataGrid.LoadingRow += DataGrid_LoadingRow;
        }

        private void DataGrid_LoadingRow(object? sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void UserControl1_Loaded(object sender, RoutedEventArgs e)
        {
            //DataGrid.Columns[0].Width = DataGridLength.SizeToHeader;
            DataGrid.Columns[0].CanUserSort = false;
            DataGrid.Columns[0].Header = "Значения";

        }
   

        public ObservableCollection<MyData> DataCollection { get; set; }

        private void DataGrid_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
            // Проверяем, что вводимый символ является числом или допустимым разделителем десятичной точки
            DataGrid? dataGrid = sender as DataGrid;

            DataGridCellInfo currentCell = dataGrid.CurrentCell;

            if (currentCell.Column.GetCellContent(currentCell.Item) is TextBox textBox)
            {
                string currentText = textBox.Text;

                if (Common.IsDouble(e.Text, textBox.CaretIndex,currentText,_comma))
                {
                    e.Handled = false;
                }
            }
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var cell = e.EditingElement as TextBox;
            if (cell != null)
            {
                _isDataChange = true;


                
                // Проверяем, является ли введенное значение числом
                //if (!double.TryParse(cell.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                //{
                //    MessageBox.Show("Please enter a valid number.");
                //    e.Cancel = true;  // Отменяем завершение редактирования ячейки
                //}
                //else
                //{
                //    // Присваиваем значение обратно в модель, если оно валидно
                //    var myData = e.Row.Item as MyData;

                //    if (myData != null)
                //    {
                //        myData.Value = result;
                //    }
                //}
            }

        }
        
        bool _isDataChange = false;
        public bool IsDataChange { get { return _isDataChange; } }

        private void BSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        public void Save()
        {
            _listElements.Clear();

            foreach (var item in DataCollection)
            {
                _listElements.Add(item.Value);
            }

            _isDataChange = false;
            // Логика для сохранения данных, если нужно
        }


        

    }
}


namespace MathFunctionWPF
{
    public static class Common
    {

        public static bool IsDouble(string inputCharacter, int CaretIndex, string allText, string comma = ".")
        {
            // Провяем что входной тект число
            var regex = new Regex(@$"[^0-9\{comma}\+\-]+");
            //e.Handled = regex.IsMatch(e.Text);
            if (regex.IsMatch(inputCharacter))
            {
                return false;
            }

            {
                // Ввели +\-
                if ((inputCharacter == "+" || inputCharacter == "-") && CaretIndex != 0)
                {
                    return false;
                }

                // Уже есть запятая?
                if (inputCharacter == comma && allText.Contains(comma))
                {
                    return false;
                }
            }

            return true;
        }


        public static bool IsNumber(string inputCharacter, int CaretIndex)
        {
            // Провяем что входной тект число
            var regex = new Regex(@$"[^0-9\+\-]+");
            //e.Handled = regex.IsMatch(e.Text);
            if (regex.IsMatch(inputCharacter))
            {
                return false;
            }

            {
                // Ввели +\-
                if ((inputCharacter == "+" || inputCharacter == "-") && CaretIndex != 0)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsNumberPositive(string inputCharacter)
        {
            // Провяем что входной тект число
            var regex = new Regex(@$"[^0-9]+");
            //e.Handled = regex.IsMatch(e.Text);
            if (regex.IsMatch(inputCharacter))
            {
                return false;
            }

            return true;
        }
    }
}