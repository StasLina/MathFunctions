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
using MathFunctionWPF.MathMethods;
using System.Net.Http.Headers;

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
        private FunctionInputData _functionInputModel;
        private IFunctionOutputView _functionOutputView;
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

        MathFunctionViewModel _mathFunctionViewModel;
        private void Init()
        {
            MathFunctionViewModel? model = _view.DataContext as MathFunctionViewModel;
            if (model != null)
            {
                _mathFunctionViewModel = model;

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

                _functionInputModel = (FunctionInputData)_functionInputView.DataContext;
                _functionInputView.AddFunctionStringChangedListener(this.TextFunctionChange);
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

                // Окно смены 
                MethodListControl methodListControl = new MethodListControl();
                model.ListMethods = methodListControl;
                methodListControl.MethodChanged += MethodChanged;

                MethodChanged(TypeMathMethod.Bisection);
            }
        }

        private void MethodChanged(TypeMathMethod typeMethod)
        {
            switch (typeMethod)
            {
                case TypeMathMethod.Bisection:
                    {
                        InitBisectionMethod();
                        break;
                    }
                case TypeMathMethod.GoldenSearch:
                    {
                        InitGoldenSearch();
                        break;
                    }
            }
        }

        // intersection 
        // min
        // max

        private void InitBisectionMethod()
        {
            _mathFunctionViewModel.TypeMethod = TypeMathMethod.Bisection;

            // Иницилизируем окно описания
            _mathFunctionViewModel.DescriptionView = new BisectionMethodDescription();

            if (_mathFunctionViewModel.CalculationView != null && _mathFunctionViewModel.CalculationView is FunctionOutputViewIntersection)
            {
                _functionOutputView = (FunctionOutputViewIntersection)_mathFunctionViewModel.CalculationView;
            }
            else
            {
                _functionOutputView = new FunctionOutputViewIntersection();
                _mathFunctionViewModel.CalculationView = _functionOutputView;
            }

            _functionOutputView.AddListenerUpdateFunction(UpdateFunctionView);
            _functionOutputView.AddListenerUpdatePlotter(UpdatePlotterView);
        }

        private void InitGoldenSearch()
        {
            _mathFunctionViewModel.TypeMethod = TypeMathMethod.GoldenSearch;

            _mathFunctionViewModel.DescriptionView = new GoldemSectionDescription();

            if (_mathFunctionViewModel.CalculationView != null && _mathFunctionViewModel.CalculationView is FunctionOutputMinMaxView)
            {

                _functionOutputView = (FunctionOutputMinMaxView)_mathFunctionViewModel.CalculationView;
            }
            else
            {
                _functionOutputView = new FunctionOutputMinMaxView();
                _mathFunctionViewModel.CalculationView = _functionOutputView;
            }

            _functionOutputView.AddListenerUpdateFunction(UpdateFunctionView);
            _functionOutputView.AddListenerUpdatePlotter(UpdatePlotterView);
        }

        private void TextFunctionChange(TextBox textBox)
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
                org.mariuszgromada.math.mxparser.Expression expression = new org.mariuszgromada.math.mxparser.Expression(textBox.Text);
                double newValue = expression.calculate();

                if (newValue != double.NaN)
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

                return false;
                //double newValue;
                //if (double.TryParse(textBox.Text, out newValue))
                //{
                //    if (oldValue != newValue)
                //    {
                //        updateValue = newValue;
                //        return true;
                //    }
                //}
                //else
                //{
                //    throw new Exception("Введено не число");
                //}
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
            try
            {
                if (_calculation == null)
                {
                    return;
                }
                Func<double, double> func = _calculation.Calculate;

                switch (_mathFunctionViewModel.TypeMethod)
                {
                    case TypeMathMethod.Bisection:
                        {
                            //double value = Dihtomia.Calc(func,_functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy);
                            double value = BisectionMethod.Calc(func, _functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy);
                            string text;
                            if (_functionInputModel.IncrementRate < 0)
                            {
                                text = value.ToString($"F{-1 * _functionInputModel.IncrementRate}");
                            }
                            else
                            {
                                text = value.ToString();
                            }

                            _functionOutputView.SetResult(TypeMathResult.Intespection, text);
                            break;
                        }

                    case TypeMathMethod.GoldenSearch:
                        {
                            //double value = Dihtomia.Calc(func,_functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy);
                            double value = GoldenSectionSearch.Calc(func, _functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy);
                            _calculation.IsInverse = true;
                            double value2 = GoldenSectionSearch.Calc(func, _functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy);
                            _calculation.IsInverse = false;
                            string minimalValue, maximalValue;
                            if (_functionInputModel.IncrementRate < 0)
                            {
                                minimalValue = value.ToString($"F{-1 * _functionInputModel.IncrementRate}");
                                maximalValue = value2.ToString($"F{-1 * _functionInputModel.IncrementRate}");
                            }
                            else
                            {
                                minimalValue = value.ToString();
                                maximalValue = value2.ToString();
                            }

                            _functionOutputView.SetResult(TypeMathResult.Minimum, minimalValue);
                            _functionOutputView.SetResult(TypeMathResult.Maximum, maximalValue);

                            _functionOutputView.SetResult(TypeMathResult.MinimumValue, func(value).ToString());
                            _functionOutputView.SetResult(TypeMathResult.MaximumValue, func(value2).ToString());
                            break;
                        }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Ошибка");
            }

        }

        private void UpdatePlotterView()
        {
            if (_calculation != null)
            {
                Func<double, double> func = _calculation.Calculate;
                var pm = new PlotModel
                {
                    Title = "Метод дихтомии",
                    Subtitle = "",
                    PlotType = PlotType.Cartesian,
                    Background = OxyColors.White
                };
                pm.Series.Add(new FunctionSeries(val => { return 0; }, _functionInputModel.XStart, _functionInputModel.XEnd, Math.Pow(10, _functionInputModel.IncrementRate), "F(x)=0"));
                pm.Series.Add(new FunctionSeries(func, _functionInputModel.XStart, _functionInputModel.XEnd, Math.Pow(10, _functionInputModel.IncrementRate), _calculation.Formula));
                _graphPlotter.SetPlotterModel(pm);
            }
        }
    }



}
