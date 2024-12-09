using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WpfDynamicDataGrid
{
    public class MainWindow : Window
    {
        private DataGrid _dataGrid;
        private List<DynamicRow> _tableData;

        public MainWindow()
        {
            Title = "Dynamic DataGrid in WPF";
            Width = 600;
            Height = 400;

            // ������ ��������� Grid
            var grid = new Grid();
            Content = grid;

            // ������ DataGrid
            _dataGrid = new DataGrid
            {
                AutoGenerateColumns = true, // ������������� �������
                CanUserAddRows = false,    // ��������� ���������� ����� �������������
                CanUserDeleteRows = false, // ��������� �������� ����� �������������
                IsReadOnly = false,        // ��������� ��������������
                Margin = new Thickness(10)
            };

            // ��������� DataGrid � ����
            grid.Children.Add(_dataGrid);

            // ������������� ������
            InitializeDynamicTable(5, 5); // ������� 5x5

            // ����������� ������
            _dataGrid.ItemsSource = _tableData;
        }

        private void InitializeDynamicTable(int rows, int cols)
        {
            _tableData = new List<DynamicRow>();

            // ������ ������ �������
            for (int i = 0; i < rows; i++)
            {
                var row = new DynamicRow();

                // ��������� �������
                for (int j = 0; j < cols; j++)
                {
                    row[$"Col{j + 1}"] = 0; // ���������� ��� �������� ����� 0
                }

                _tableData.Add(row);
            }

            // ��������� ������� �������
            _dataGrid.AutoGenerateColumns = false; // ��������� �������������
            _dataGrid.Columns.Clear();

            for (int j = 0; j < cols; j++)
            {
                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = $"Col {j + 1}",
                    Binding = new System.Windows.Data.Binding($"Col{j + 1}")
                });
            }
        }
    }

    // ����� ��� ������������ ������ �������
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

    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            var app = new Application();
            var window = new MainWindow();
            app.Run(window);
        }
    }
}
