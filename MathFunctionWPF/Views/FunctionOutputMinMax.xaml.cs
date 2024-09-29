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
    /// Логика взаимодействия для FunctionOutputMinMax.xaml
    /// </summary>
    public partial class FunctionOutputMinMaxView : UserControl , IFunctionOutputView
    {
        public FunctionOutputMinMaxView()
        {
            InitializeComponent();

            DataContext = new FunctionOutputMinMaxModel();
        }

        List<ButtonClick> _listButtonUpdateFuncPlotterDelegates = new List<ButtonClick>();
        List<ButtonClick> _listCalcFunctionDelegates = new List<ButtonClick>();

        void IFunctionOutputView.AddListenerUpdatePlotter(ButtonClick listener)
        {
            _listButtonUpdateFuncPlotterDelegates.Add(listener);
        }

        void IFunctionOutputView.AddListenerUpdateFunction(ButtonClick listener)
        {
            _listCalcFunctionDelegates.Add(listener);
        }

        void IFunctionOutputView.SetResult(MathFunctionWPF.Views.TypeMathResult typeResult, string value)
        {
            switch (typeResult)
            {
                case TypeMathResult.Minimum:
                    {
                        MinValue.Text = value;
                        break;
                    }
                case TypeMathResult.Maximum:
                    {
                        MaxValue.Text = value;
                        break;
                    }
                case TypeMathResult.MinimumValue:
                    {
                        MinValueY.Text = value;
                        break;
                    }
                case TypeMathResult.MaximumValue:
                    {
                        MaxValueY.Text = value;
                        break;
                    }
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var call in _listButtonUpdateFuncPlotterDelegates)
            {
                call();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (var call in _listCalcFunctionDelegates)
            {
                call();
            }
        }

    }
}
