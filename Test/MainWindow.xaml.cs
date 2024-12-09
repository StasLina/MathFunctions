using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MathTableMatrix
{
    public partial class MainWindow : Window
    {
        private DataGrid _dataGrid;
        DynamicTableControl _tableControl;
        public MainWindow()
        {
            InitializeComponent();
            Title = "Dynamic DataGrid in WPF";
            Width = 600;
            Height = 400;

            // Создаём основной контейнер
            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Для кнопок
            mainGrid.RowDefinitions.Add(new RowDefinition()); // Для DataGrid
            Content = mainGrid;

            // Создаём кнопки для изменения размеров таблицы
            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10) };

            var addRowButton = new Button { Content = "Add Row", Margin = new Thickness(5) };
            addRowButton.Click += (s, e) => AddRow();

            var removeRowButton = new Button { Content = "Remove Row", Margin = new Thickness(5) };
            removeRowButton.Click += (s, e) => RemoveRow();

            buttonPanel.Children.Add(addRowButton);
            buttonPanel.Children.Add(removeRowButton);

            Grid.SetRow(buttonPanel, 0);
            mainGrid.Children.Add(buttonPanel);


            DynamicTableController dynamicTableController = new DynamicTableController(
                new DynamicTableControl(), new DynamicTableControlModel());
            // Создаём DataGrid
            _tableControl = dynamicTableController.View;

            dynamicTableController.InitializeDynamicTable(5, 5);

            Grid.SetRow(_tableControl, 1);
            mainGrid.Children.Add(_tableControl);
        }

        private void AddRow()
        {
            //_tableControl.AddRow();
        }

        private void RemoveRow()
        {
            //_tableControl.RemoveRow();
        }
    }
}
