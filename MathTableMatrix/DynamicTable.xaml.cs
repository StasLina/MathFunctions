using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static MathTableMatrix.DynamicTableControl;

namespace MathTableMatrix
{
    /// <summary>
    /// Логика взаимодействия для DynamicTable.xaml
    /// </summary>
    /// 

    public class DynamicTableControlModel : INotifyPropertyChanged
    {
        private ObservableCollection<DynamicRow> _tableData;

        public ObservableCollection<DynamicRow> TableData
        {
            get { return _tableData; }
            set
            {
                _tableData = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public double ColumnCount
        {
            get; set;
        }
    }

    public class DynamicTableController
    {
        DynamicTableControl _view;
        //DynamicTableControlModel _model;



        public DynamicTableController(DynamicTableControl view, DynamicTableControlModel model = null)
        {
            //Model = model;
            View = view;
        }

        public DynamicTableControl View { get => _view; set => _view = value; }
        public DynamicTableControlModel Model { get => (DynamicTableControlModel)View.DataContext;}

        public void InitializeDynamicTable(int rows, int cols)
        {
            DynamicTableControlModel model = new DynamicTableControlModel();
            var tableData = new ObservableCollection<DynamicRow>();

            // Создаём строки таблицы
            for (int i = 0; i < rows; i++)
            {
                var row = new DynamicRow(cols)
                {
                    RowHeader = $"R{i + 1}"
                }
                ;
                tableData.Add(row);
            }

            // Генерация колонок
            View.DGDataGrid.Columns.Clear();
            for (int j = 0; j < cols; j++)
            {
                View.DGDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = $"C{j + 1}",
                    Binding = new System.Windows.Data.Binding($"Values[{j}]") // Указываем путь привязки
                    {
                        Mode = BindingMode.TwoWay, // Двусторонняя привязка
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged // Мгновенное обновление
                    }
                });
            }
            model.TableData = tableData;

            //Model = model;

            View.DataContext = model;
        }

        public int RowCount
        {
            get
            {
                if (Model != null)
                {
                    return Model.TableData.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int ColumnCount
        {
            get
            {
                if (Model == null) return 0;
                if (Model.TableData == null) return 0;
                if (Model.TableData.Count == 0) return 0;
                return Model.TableData[0].Values.Count;
            }
        }

        public double[,] GetData()
        {
            int colCount = ColumnCount;
            int rowCount = RowCount;
            var returnValue = new double[rowCount, colCount];

            for (int idxRow = 0; idxRow < rowCount; ++idxRow)
            {
                DynamicRow rowValues = Model.TableData[idxRow];

                for (int idxColumn = 0; idxColumn < rowValues.Values.Count; ++idxColumn)
                {
                    object value = rowValues.Values[idxColumn];
                    if(value is double doubleValue)
                    {
                        returnValue[idxRow, idxColumn] = doubleValue;
                    }
                    if(value is string stringValue)
                    {
                        returnValue[idxRow, idxColumn] = double.Parse(stringValue);
                    }
                }
            }
            return returnValue;
        }

        public double[] GetDataColumn(int column = 0)
        {
            int colCount = ColumnCount;
            int rowCount = RowCount;
            var returnValue = new double[rowCount];

            if(column>= rowCount)
            {
                return returnValue;
            }

            for (int idxRow = 0; idxRow < rowCount; ++idxRow)
            {
                DynamicRow rowValues = Model.TableData[idxRow];
                returnValue[idxRow] = (double)rowValues.Values[column];
            }
            return returnValue;
        }

        public void SetData(double[,] data)
        {
            InitializeDynamicTable(data.GetLength(0), data.GetLength(1));
            for (int rowIdx = 0, rowIdxEnd = data.GetLength(0); rowIdx < rowIdxEnd; ++rowIdx)
            {
                var rowValues = Model.TableData[rowIdx].Values;
                for (int colIdx = 0, colIdxEnd = data.GetLength(1); colIdx < colIdxEnd; ++colIdx)
                {
                    rowValues[colIdx] = data[rowIdx, colIdx];
                }
            }
        }

        public void SetData(double[] data)
        {
            InitializeDynamicTable(data.GetLength(0), 1);
            for (int rowIdx = 0, rowIdxEnd = data.GetLength(0); rowIdx < rowIdxEnd; ++rowIdx)
            {
                Model.TableData[rowIdx].Values[0] = data[rowIdx];
            }
        }

        public void AddRow()
        {
            if (View.DGDataGrid.Columns.Count == 0) return;

            var newRow = new DynamicRow(View.DGDataGrid.Columns.Count);
            Model.TableData.Add(newRow);
        }

        public void RemoveRow()
        {
            if (Model.TableData.Count > 0)
            {
                Model.TableData.RemoveAt(Model.TableData.Count - 1);
            }
        }
    }

    public partial class DynamicTableControl : UserControl
    {
        public DynamicTableControl()
        {
            InitializeComponent();

            // Инициализация данных
            //InitializeDynamicTable(5, 5); // Таблица 5x5

            // Создаём стиль для RowHeader
            Style rowHeaderStyle = new Style(typeof(DataGridRowHeader));

            // Добавляем Setter, который привязывает Content к свойству RowHeader
            rowHeaderStyle.Setters.Add(new Setter(DataGridRowHeader.ContentProperty, new Binding("RowHeader")));

            // Назначаем стиль DataGrid
            DGDataGrid.RowHeaderStyle = rowHeaderStyle;

            DataContext = this;
        }

        // Обработчик события PreviewTextInput
        //private void InputField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        //{
        //    // Пример: ограничение ввода только цифр
        //    if (sender is DataGrid grid)
        //    {
        //        if (grid.CurrentCell.Item is DynamicRow row)
        //        {
        //            string text = grid.CurrentCell.Item.ToString();
        //            string header = (string)grid.CurrentCell.Column.Header;
        //            header = header.Substring(1);

        //            int column = int.Parse(header) - 1;

        //            string currentText = row.Values[column].ToString();
        //            //if (IsDouble(e.Text, textBox.CaretIndex, textBox.Text))

        //            //    int i = grid.SelectedIndex;
        //            if (!IsInputValid(text))
        //            {
        //                e.Handled = true; // Отменить ввод, если данные не соответствуют условиям
        //            }
        //        }

        //    }
        //}

        private void InputField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                TextBox textBox = e.EditingElement as TextBox;
                if (textBox != null)
                {
                    double value;
                    if (!double.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    {
                        MessageBox.Show("Значение должно быть числом с плавающей точкой.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        e.Cancel = true;
                        textBox.Text = "0";
                    }
                }
            }
        }

        // Пример: проверка, является ли ввод только цифрами
        private bool IsInputValid(string input)
        {
            // Допустим, мы разрешаем только ввод цифр
            return double.TryParse(input, out _); // Возвращает true, если ввод можно преобразовать в число
        }


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
    }

    

    // Класс для динамической строки таблицы
    public class DynamicRow : INotifyPropertyChanged
    {
        private readonly ObservableCollection<double> _values;
        private string rowHeader;

        public string RowHeader
        {
            get => rowHeader;
            set
            {
                rowHeader = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<double> Values
        {
            get => _values;
        }

        public DynamicRow(int columnCount)
        {
            _values = new ObservableCollection<double> ();
            for (int i = 0; i < columnCount; i++)
            {
                _values.Add((double)0); // Инициализируем нулями
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
