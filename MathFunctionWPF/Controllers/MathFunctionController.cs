using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MathFunctionWPF.Controls;
using MathFunctionWPF.Models;
using MathFunctionWPF.Views;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace MathFunctionWPF.Controllers
{
    class MathFunctionController
    {
        public MathFunctionController(MathFunctionView view)
        {
            _view = view;
            Init();
        }

        MathFunctionView _view;

        public MathFunctionView View
        {
            get
            {
                return _view;
            }
        }

        private FunctionInputView _functionInputView;
        private FunctionSourceData _functionInputModel;
        private FunctionOutputView _functionOutputView;
        private GraphPlotter _graphPlotter;


        private FunctionCalculation __calculation;

        private FunctionCalculation _calculation
        {
            get
            {
                return __calculation;
            }
            set
            {
                __calculation = value;
                //UpdatePlotterView();
            }
        }

        private void Init()
        {
            MathFunctionViewModel? model = _view.DataContext as MathFunctionViewModel;
            if (model != null)
            {
                // Окошко входных данных
                var inputView = model.SourceDataView as FunctionInputView;

                if (inputView != null)
                {
                    _functionInputView = inputView;
                }
                else
                {
                    _functionInputView = new FunctionInputView();
                    model.SourceDataView = _functionInputView;
                }
                _functionInputModel = (FunctionSourceData)_functionInputView.DataContext;
                _functionInputView.AddFunctionStringChangedListener(this.TextFunctionHandler);
                _functionInputView.AddArgXStartChangedListener(this.UpdateXStartArg);
                _functionInputView.AddArgXEndChangedListener(this.UpdateXEndArg);
                _functionInputView.AddAverageChangedListener(this.UpdateAccuracyArg);

                // Окошко графика
                if (model.GraphPlotterView != null)
                {
                    if (model.GraphPlotterView is GraphPlotter)
                    {
                        _graphPlotter = (GraphPlotter)model.GraphPlotterView;
                    }
                }
                else
                {
                    _graphPlotter = new GraphPlotter();
                    model.GraphPlotterView = _graphPlotter;
                }

                if (model.CalculationView != null)
                {
                    if (model.CalculationView is FunctionOutputView)
                    {
                        _functionOutputView = (FunctionOutputView)model.CalculationView;
                    }
                }
                else
                {
                    _functionOutputView = new FunctionOutputView();
                    model.CalculationView = _functionOutputView;
                }
                _functionOutputView.AddListenerUpdateFunction(UpdateFunctionView);
                _functionOutputView.AddListenerUpdatePlotter(UpdatePlotterView);
                //UpdatePlotterView();
            }
        }


        private void TextFunctionHandler(TextBox textBox)
        {
            if (_functionInputModel.Formula != textBox.Text)
            {
                string oldName = _functionInputModel.Formula;

                try
                {
                    _functionInputModel.Formula = textBox.Text;
                    _calculation = new FunctionCalculation(_functionInputModel);
                }
                catch (Exception ex)
                {
                    MessageBoxResult result = MessageBox.Show(
                        ex.Message + ". Продолжить редактирование?", // Текст сообщения
                        "Ошибка ввода формулы", // Заголовок окна
                        MessageBoxButton.YesNo, // Кнопки "Да" и "Нет"
                        MessageBoxImage.Question // Иконка вопроса
                    );

                    // Обработка результата
                    if (result == MessageBoxResult.Yes)
                    {
                        //_functionInputView.IsFunctionFocusing = true;
                        _functionInputModel.Formula = oldName;
                        _functionInputView.ReturnedFocus = textBox;
                        //textBox.Focus();
                        //_functionInputView.IsFunctionFocusing = false;
                    }
                    else
                    {
                        _functionInputModel.Formula = oldName;
                        textBox.Text = oldName;
                    }
                }
            }
        }

        private void UpdateXStartArg(TextBox textBox)
        {
            double newValue = 0;
            if (NumberFunctionHandler(textBox, _functionInputModel.XStart, ref newValue))
            {
                _functionInputModel.XStart = newValue;
            }
        }
        private void UpdateXEndArg(TextBox textBox)
        {
            double newValue = 0;
            if (NumberFunctionHandler(textBox, _functionInputModel.XEnd, ref newValue))
            {
                _functionInputModel.XEnd = newValue;
            }
        }

        private void UpdateAccuracyArg(TextBox textBox)
        {
            double newValue = 0;
            if (NumberFunctionHandler(textBox, _functionInputModel.Accuracy, ref newValue))
            {
                _functionInputModel.Accuracy = newValue;
                _functionInputView.Precision.Text = _functionInputModel.IncrementRate.ToString();
            }


        }

        private bool NumberFunctionHandler(TextBox textBox, double oldValue, ref double updateValue)
        {
            try
            {
                double newValue;
                if (double.TryParse(textBox.Text, out newValue))
                {
                    if (oldValue != newValue)
                    {
                        updateValue = newValue;
                        return true;
                    }
                }
                else
                {
                    throw new Exception("Введено не число");
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = MessageBox.Show(
                    ex.Message + ". Продолжить редактирование?", // Текст сообщения
                    "Ошибка ввода числа", // Заголовок окна
                    MessageBoxButton.YesNo, // Кнопки "Да" и "Нет"
                    MessageBoxImage.Question // Иконка вопроса
                );

                // Обработка результата
                if (result == MessageBoxResult.Yes)
                {
                    _functionInputView.ReturnedFocus = textBox;
                }
                else
                {
                    textBox.Text = oldValue.ToString();
                }
                return false;
            }
            return false;
        }


        private void UpdateFunctionView()
        {
            if(_calculation == null)
            {
                return;
            }

            Func<double, double> func = _calculation.Calculate;
            //double value = Dihtomia.Calc(func,_functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy);
            double value = BisectionMethod.Calc(func,_functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy);
            string text;
            if (_functionInputModel.IncrementRate<0)
            {
                text = value.ToString($"F{-1*_functionInputModel.IncrementRate}");
            }
            else
            {
                text = value.ToString();
            }
            _functionOutputView.Result.Text = text;
        }

        private void UpdatePlotterView()
        {
            if (_calculation != null)
            {
                Func<double, double> func = _calculation.Calculate;
                var pm = new PlotModel
                {
                    Title = "Trigonometric functions",
                    Subtitle = "Example using the FunctionSeries",
                    PlotType = PlotType.Cartesian,
                    Background = OxyColors.White
                };
                pm.Series.Add(new FunctionSeries(func, _functionInputModel.XStart, _functionInputModel.XEnd, Math.Pow(10, _functionInputModel.IncrementRate), _calculation.Formula));
                _graphPlotter.SetPlotterModel(pm);
            }
        }
    }


    //public class Dihtomia()
    //{
    //    public static double Calc(Func<double, double> _func,double a, double b, double e)
    //    {
    //        double c = (a + b) / 2;

    //        while (b - a >= e)
    //        {
    //            if (_func(a) * _func(c) < 0)
    //            {
    //                b = c;
    //            }
    //            else
    //            {
    //                a = c;
    //            }

    //            c = (a + b) / 2;
    //        }

    //        double y1 = Math.Abs(_func(a)), y2 = Math.Abs(_func(b));
    //        if (y1 > y2)
    //        {
    //            return b;
    //        }
    //        else
    //        {
    //            return a;
    //        }
    //    }
    //}

    public class BisectionMethod
    {
        public static double Calc(Func<double, double> _func, double a, double b, double e)
        {
            // Проверка, что значения a и b подходят
            if (_func(a) * _func(b) >= 0)
            {
                throw new ArgumentException("Функция должна менять знак на интервале [a, b].");
            }

            double midpoint = 0;

            while ((b - a) / 2 > e) // Пока длина интервала больше заданной точности
            {
                midpoint = (a + b) / 2; // Находим середину

                // Проверяем, где функция меняет знак
                if (_func(midpoint) == 0) // Если мы нашли корень
                {
                    return midpoint;
                }
                else if (_func(a) * _func(midpoint) < 0) // Корень находится в [a, midpoint]
                {
                    b = midpoint;
                }
                else // Корень находится в [midpoint, b]
                {
                    a = midpoint;
                }
            }

            return (a + b) / 2; // Возвращаем приближенный корень
        }
    }



}
