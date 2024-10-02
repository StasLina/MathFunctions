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
using static MathFunctionWPF.Views.FunctionOutputViewIntersection;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MathFunctionWPF.Views
{
    /// <summary>
    /// Логика взаимодействия для MethodListControl.xaml
    /// </summary>
    public partial class MethodListControl : UserControl
    {
        public delegate void MethodChangeDelegate(TypeMathMethod typeMethod);
        public event MethodChangeDelegate MethodChanged;

        public MethodListControl()
        {
            InitializeComponent();
            AddButton("D", TypeMathMethod.Bisection);
            AddButton("S", TypeMathMethod.GoldenSearch);
            AddButton("T", TypeMathMethod.Test);
        }

        private void AddButton(string name, TypeMathMethod method)
        {
            Button button = new Button
            {
                Width = 38, // Ширина кнопки
                Height = 38, // Высота кнопки для создания квадратной формы
                Content = name, // Текст на кнопке
                Margin = new Thickness(5) // Отступы между кнопками
            };
            // Подписка на событие нажатия на кнопку
            button.Click += Button_Click;

            // Добавляем кнопку в StackPanel
            ButtonPanel.Children.Add(button);

            MethodButtonModel methodButtonModel = new MethodButtonModel();
            button.DataContext = methodButtonModel;
            methodButtonModel.Method = method;
        }

        // Обработчик нажатия на кнопку
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            if (button != null)
            {
                MethodButtonModel? dataContext = button.DataContext as MethodButtonModel;
                if (dataContext != null)
                {
                    MethodChanged(dataContext.Method);
                }
            }

            // Показываем модальное окно при нажатии на кнопку
            //MessageBox.Show($"You clicked {button.Content}", "Button Clicked", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public enum TypeMathMethod
    {
        Bisection, GoldenSearch, Test
    };


    public class MethodButtonModel : INotifyPropertyChanged
    {

        public TypeMathMethod Method
        {
            get; set;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

}
