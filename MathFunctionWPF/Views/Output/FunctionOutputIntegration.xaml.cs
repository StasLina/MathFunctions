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
    /// Логика взаимодействия для FunctionOutputIntegration.xaml
    /// </summary>
    public partial class FunctionOutputIntegration : UserControl, IFunctionIntegrationOutputView
    {
        public FunctionOutputIntegration()
        {
            InitializeComponent();
        }

        List<ButtonClick> _listButtonUpdateFuncPlotterDelegates = new List<ButtonClick>();
        List<ButtonClick> _listCalcFunctionCountDelegates = new List<ButtonClick>();
        List<ButtonClick> _listCalcRecnatgelIntegralDelegates = new List<ButtonClick>();
        List<ButtonClick> _listCalcTrapecialIntegralDelegates = new List<ButtonClick>();
        List<ButtonClick> _listCalcSimpsonIntegralDelegates = new List<ButtonClick>();

        void IFunctionOutputView.AddListenerUpdatePlotter(ButtonClick listener)
        {
            _listButtonUpdateFuncPlotterDelegates.Add(listener);
        }

        void IFunctionOutputView.AddListenerUpdateFunction(ButtonClick listener) { }

        void IFunctionIntegrationOutputView.AddListenerCalcCount(MathFunctionWPF.Views.ButtonClick listener)
        {
            _listCalcFunctionCountDelegates.Add(listener);
        }

        void IFunctionIntegrationOutputView.AddListenerRectangelIntegral(MathFunctionWPF.Views.ButtonClick listener)
        {
            _listCalcRecnatgelIntegralDelegates.Add(listener);

        }

        void IFunctionIntegrationOutputView.AddListenerTrapecialIntegral(MathFunctionWPF.Views.ButtonClick listener)
        {
            _listCalcTrapecialIntegralDelegates.Add(listener);
        }

        void IFunctionIntegrationOutputView.AddListenerSimpsonIntegral(MathFunctionWPF.Views.ButtonClick listener)
        {
            _listCalcSimpsonIntegralDelegates.Add(listener);
        }

        void IFunctionOutputView.SetResult(MathFunctionWPF.Views.TypeMathResult typeResult, string value)
        {
            switch (typeResult)
            {
                case TypeMathResult.IntegralRectangelValue:
                    {
                        Rectangel.Text = value;
                        break;
                    }
                case TypeMathResult.IntegralTrapezeValue:
                    {
                        Trapecial.Text = value;
                        break;
                    }
                case TypeMathResult.IntegralSimpsonValue:
                    {
                        Simpson.Text = value;
                        break;
                    }
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var send in _listButtonUpdateFuncPlotterDelegates)
            {
                send();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (var send in _listCalcFunctionCountDelegates)
            {
                send();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            foreach (var send in _listCalcRecnatgelIntegralDelegates)
            {
                send();
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

            foreach (var send in _listCalcTrapecialIntegralDelegates)
            {
                send();
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            foreach (var send in _listCalcSimpsonIntegralDelegates)
            {
                send();
            }
            
        }
    }
}
