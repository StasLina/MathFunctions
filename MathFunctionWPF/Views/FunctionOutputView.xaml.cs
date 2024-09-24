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
    /// Логика взаимодействия для FunctionOutputView.xaml
    /// </summary>
    public partial class FunctionOutputView : UserControl
    {
        public FunctionOutputView()
        {
            InitializeComponent();
            DataContext = new FunctionOutputModel();
        }

        public delegate void ButtonClick();

        List<ButtonClick> _listButtonUpdateFuncPlotterDelegates = new List<ButtonClick>();
        List<ButtonClick> _listCalcFunctionDelegates = new List<ButtonClick>();

        public void AddListenerUpdatePlotter(ButtonClick listener)
        {
            _listButtonUpdateFuncPlotterDelegates.Add(listener);
        }
        
        public void AddListenerUpdateFunction(ButtonClick listener)
        {
            _listCalcFunctionDelegates.Add(listener);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Рисуем график
            foreach(var call in _listButtonUpdateFuncPlotterDelegates)
            {
                call();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Вычисляем значение
            foreach (var call in _listCalcFunctionDelegates)
            {
                call();
            }
        }
    }
}
