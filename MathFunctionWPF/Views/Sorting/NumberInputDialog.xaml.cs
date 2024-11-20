﻿using System;
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
using System.Windows.Shapes;

namespace MathFunctionWPF.Views.Sorting
{
    /// <summary>
    /// Логика взаимодействия для NumberInputDialog.xaml
    /// </summary>
    public partial class NumberInputDialog : Window
    {
        public NumberInputDialog()
        {
            InitializeComponent();
        }

        public delegate void InputNumbmberEvent(double v);

        public InputNumbmberEvent EventInputNumber { get; set; }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка, является ли введенное значение числом
            if (double.TryParse(inputTextBox.Text, out double result))
            {
                EventInputNumber?.Invoke(result);
                this.DialogResult = true; // Закрытие окна с результатом OK
                
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите правильное число.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; // Закрытие окна с результатом Cancel
        }

        private void NumberInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            // Запрещаем ввод всего, кроме цифр и клавиши Backspace
            if (!char.IsDigit((char)KeyInterop.KeyFromVirtualKey((int)e.Key)) && e.Key != Key.Back)
            {
                e.Handled = false;
            }
        }
    }
}
