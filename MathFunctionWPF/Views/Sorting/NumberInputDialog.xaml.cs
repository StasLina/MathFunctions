using MathFunctionWPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Xml;
using MathFunctionWPF.Models;

namespace MathFunctionWPF.Views.Sorting
{
    /// <summary>
    /// Логика взаимодействия для NumberInputDialog.xaml
    /// </summary>
    public partial class NumberInputDialog : Window
    {
        NumberInputDialogViewModel _model = new NumberInputDialogViewModel();

        //public ObservableCollection<InputField> InputFields { get; set; }



        public NumberInputDialog()
        {
            InitializeComponent();
            DataContext = _model;  // Устанавливаем DataContext на ViewModel
            //Items.ItemsSource = _model.InputFields;  // Привязываем ItemsSource к InputFields


            //InputFields = new ObservableCollection<InputField>
            //{
            //    new InputField { Label = "Количество точек:", ValidationType = Models.ValidationType.PositiveNumber},
            //    new InputField { Label = "От:" ,ValidationType = Models.ValidationType.Double},
            //    new InputField { Label = "До:" ,ValidationType = Models.ValidationType.Double}
            //};
            //DataContext = _model;
        }

        //public ObservableCollection<InputField> InputFields { get { return _model.InputFields; } }

        public delegate void InputNumbmberEvent(NumberInputDialog view);

        public InputNumbmberEvent EventInputNumber { get; set; }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка, является ли введенное значение числом
            //if (double.TryParse(inputTextBox.Text, out double result))
            //{
            try
            {
                EventInputNumber?.Invoke(this);
                this.DialogResult = true; // Закрытие окна с результатом OK
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Иcключение");
            }
            //}
            //else
            //{
            //    MessageBox.Show("Пожалуйста, введите правильное число.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; // Закрытие окна с результатом Cancel
        }

        private void NumberInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            // Запрещаем ввод всего, кроме цифр и клавиши Backspace
            if (!char.IsDigit((char)KeyInterop.KeyFromVirtualKey((int)e.Key)))
            {
                e.Handled = false;
            }

            if(e.Key == Key.Back)
            {
                e.Handled = true;
            }
        }

        private void InputField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is InputField inputField)
            {
                e.Handled = true;
                // Выбираем правило проверки на основе ValidationType
                switch (inputField.ValidationType)
                {
                    case Models.ValidationType.Double:
                        if (Common.IsDouble(e.Text, textBox.CaretIndex, textBox.Text))
                        {
                            e.Handled = false;
                        }
                        break;

                    case Models.ValidationType.Number:
                        if (Common.IsNumber(e.Text, textBox.CaretIndex))
                            e.Handled = false;
                        break;
                    case Models.ValidationType.PositiveNumber:
                        if (Common.IsNumberPositive(e.Text))
                            e.Handled = false;
                        break;
                    case Models.ValidationType.None:
                        e.Handled = false;
                        break;
                    default:
                        e.Handled = false;
                        break;
                }
            }
        }
    }
}
