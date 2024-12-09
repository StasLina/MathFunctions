using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
            get;set;
        }
    }

    public class DynamicTableController
    {
        DynamicTableControl _view;
        DynamicTableControlModel _model;



        public DynamicTableController(DynamicTableControl view, DynamicTableControlModel model)
        {
            Model = model;
            View = view;
        }

        public DynamicTableControl View { get => _view; set => _view = value; }
        public DynamicTableControlModel Model { get => _model; set => _model = value; }

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

            Model = model;
                
            View.DataContext = model;
           
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
        private void InputField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Пример: ограничение ввода только цифр
            if (!IsInputValid(e.Text))
            {
                e.Handled = true; // Отменить ввод, если данные не соответствуют условиям
            }
        }

        // Пример: проверка, является ли ввод только цифрами
        private bool IsInputValid(string input)
        {
            // Допустим, мы разрешаем только ввод цифр
            return double.TryParse(input, out _); // Возвращает true, если ввод можно преобразовать в число
        }
    }

    // Класс для динамической строки таблицы
    public class DynamicRow : INotifyPropertyChanged
    {
        private readonly ObservableCollection<object> _values;
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

        public ObservableCollection<object> Values
        {
            get => _values;
        }

        public DynamicRow(int columnCount)
        {
            _values = new ObservableCollection<object>();
            for (int i = 0; i < columnCount; i++)
            {
                _values.Add(0); // Инициализируем нулями
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
