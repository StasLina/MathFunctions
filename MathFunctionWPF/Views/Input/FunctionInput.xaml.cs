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
using static System.Net.Mime.MediaTypeNames;

namespace MathFunctionWPF.Views
{
    /// <summary>
    /// Логика взаимодействия для FunctionInput.xaml
    /// </summary>
    public delegate void FunctionStringChanged(TextBox textBox);

    public partial class FunctionInputView : UserControl
    {
        public FunctionInputView()
        {
            InitializeComponent();
            var model = new FunctionInputData();
            DataContext = model;

            foreach(var child in MyGrid.Children)
            {
                Control? control = child as Control;
                if(control != null)
                {
                    control.GotFocus += GotFocus;
                }
            }

            // Обновляем значения в соответсвтвии с моделью
            XStart.Text = model.XStart.ToString();
            XEnd.Text = model.XEnd.ToString();
            accuracy.Text = model.Accuracy.ToString();
            //Precision.Text = model.IncrementRate.ToString();
            model.PrecisionValue = model.CalcIncrementRate().ToString();
            FunctionString.Text = model.Formula.ToString();
        }

        List<FunctionStringChanged> _functionStringChangedDelegates = new List<FunctionStringChanged>();
        List<FunctionStringChanged> _argXStartChangedDelegates = new List<FunctionStringChanged>();
        List<FunctionStringChanged> _argXEndChangedDelegates = new List<FunctionStringChanged>();
        List<FunctionStringChanged> _averageChangeDelegates = new List<FunctionStringChanged>();

        public void AddFunctionStringChangedListener(FunctionStringChanged listener)
        {
            _functionStringChangedDelegates.Add(listener);
        }
        
        public void AddArgXStartChangedListener(FunctionStringChanged listener)
        {
            _argXStartChangedDelegates.Add(listener);
        }

        public void AddArgXEndChangedListener(FunctionStringChanged listener)
        {
            _argXEndChangedDelegates.Add(listener);
        }

        public void AddAverageChangedListener(FunctionStringChanged listener)
        {
            _averageChangeDelegates.Add(listener);
        }


        public void RemoveFunctionStringChangedListener(FunctionStringChanged listener)
        {
            _functionStringChangedDelegates.Remove(listener);
        }

        //public bool IsFunctionFocusing = false;

        private bool _handleEvent = false;

        Control _returnedFocus;
        public Control ReturnedFocus
        {
            get
            {
                return _returnedFocus;
            }
            set
            {
                _returnedFocus = value;
            }
        }

        private void FunctionString_LostFocus(object sender, RoutedEventArgs e)
        {
            
            if (FunctionString.Text != null)
            {
                foreach (var send in _functionStringChangedDelegates)
                {
                    send(FunctionString);
                }
            }
        }


        private void GotFocus(object sender, RoutedEventArgs e)
        {
            if (IsFocusReturned()) { return; }
        }

        bool IsFocusReturned()
        {
            if (_returnedFocus != null)
            {
                _returnedFocus.Focus();
                _returnedFocus = null;
                return true;
            }
            return false;
        }

        private void XStart_LostFocus(object sender, RoutedEventArgs e)
        {
            foreach (var send in _argXStartChangedDelegates)
            {
                send(XStart);
            }
        }

        private void XEnd_LostFocus(object sender, RoutedEventArgs e)
        {
            foreach (var send in _argXEndChangedDelegates)
            {
                send(XEnd);
            }
        }

        private void accuracy_LostFocus(object sender, RoutedEventArgs e)
        {
            foreach (var send in _averageChangeDelegates)
            {
                send(accuracy);
            }
        }

        private void Precision_LostFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}
