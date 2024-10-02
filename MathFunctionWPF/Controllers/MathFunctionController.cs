using System.Windows;
using System.Windows.Controls;
using MathFunctionWPF.Controls;
using MathFunctionWPF.Models;
using MathFunctionWPF.Views;
using OxyPlot;
using OxyPlot.Series;
using MathFunctionWPF.MathMethods;
using System.Text;

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
                _functionInputModel.UpdateWithData();
                _functionInputModel.Formula = "";
                TextFunctionChange(_functionInputView.FunctionString);

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

                MethodChanged(TypeMathMethod.Test);
                //MethodChanged(TypeMathMethod.Bisection);
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
                case TypeMathMethod.Test:
                    {
                        InitTest();
                        break;
                    }
            }
        }

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

        private void InitTest()
        {
            _mathFunctionViewModel.TypeMethod = TypeMathMethod.Test;

            _mathFunctionViewModel.DescriptionView = null;

            if (_mathFunctionViewModel.CalculationView != null && _mathFunctionViewModel.CalculationView is FunctionOutputTest)
            {

                _functionOutputView = (FunctionOutputTest)_mathFunctionViewModel.CalculationView;
            }
            else
            {
                _functionOutputView = new FunctionOutputTest();
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

                FunctionCalculation calculation;
                try
                {
                    _functionInputModel.Formula = textBox.Text;
                    calculation = new FunctionCalculation(_functionInputModel);
                    _calculation = calculation;
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
                _functionInputModel.PrecisionValue = _functionInputModel.CalcIncrementRate().ToString();
                //_functionInputView.Precision.Text = _functionInputModel.IncrementRate().ToString();
            }
        }

        private bool NumberFunctionHandler(TextBox textBox, double oldValue, ref double updateValue)
        {
            try
            {
                org.mariuszgromada.math.mxparser.Expression expression = new org.mariuszgromada.math.mxparser.Expression(textBox.Text);
                double newValue = expression.calculate();

                if (Double.IsNaN(newValue) == false)
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
                            string argValue, funcValue;

                            double incrementRate = double.Parse(_functionInputModel.PrecisionValue);
                            double valueFunc = GoldenSectionSearch.Calc(func, _functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy);

                            if (incrementRate < 0)
                            {
                                argValue = value.ToString($"F{-1 * incrementRate}");
                                funcValue = valueFunc.ToString($"F{-1 * incrementRate}");
                            }
                            else
                            {
                                argValue = value.ToString("F0");
                                funcValue = valueFunc.ToString("F0");
                            }

                            _functionOutputView.SetResult(TypeMathResult.IntespectionArgument, argValue);
                            _functionOutputView.SetResult(TypeMathResult.IntespectionValue, funcValue);
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

                            double incrementRate = double.Parse(_functionInputModel.PrecisionValue);

                            string minimalVuncValue, maximalFuncValue;
                            if (incrementRate < 0)
                            {
                                minimalValue = value.ToString($"F{-1 * incrementRate}");
                                maximalValue = value2.ToString($"F{-1 * incrementRate}");

                                minimalVuncValue = func(value).ToString($"F{-1 * incrementRate}");
                                maximalFuncValue = func(value2).ToString($"F{-1 * incrementRate}");
                            }
                            else
                            {
                                minimalValue = value.ToString();
                                maximalValue = value2.ToString();
                                minimalVuncValue = func(value).ToString("F0");
                                maximalFuncValue = func(value2).ToString("F0");
                            }

                            _functionOutputView.SetResult(TypeMathResult.MinimumArgument, minimalValue);
                            _functionOutputView.SetResult(TypeMathResult.MaximumArgument, maximalValue);

                            _functionOutputView.SetResult(TypeMathResult.MinimumValue, minimalVuncValue);
                            _functionOutputView.SetResult(TypeMathResult.MaximumValue, maximalFuncValue);
                            break;
                        }

                    case TypeMathMethod.Test:
                        {

                            double der1 = _calculation.CalculateDer1(_functionInputModel.XStart);
                            double der2 = _calculation.CalculateDer2(_functionInputModel.XStart);
                            _functionOutputView.SetResult(TypeMathResult.Derevative1, der1.ToString());
                            _functionOutputView.SetResult(TypeMathResult.Derevative2, der2.ToString());

                            var list = _calculation.FindDiscontinuities(_functionInputModel.XStart, _functionInputModel.XEnd);
                            StringBuilder stringBuilder = new StringBuilder();
                            stringBuilder.Append("Count: ");
                            stringBuilder.Append(list.Count);
                            stringBuilder.Append(",");

                            foreach (double val in list)
                            {
                                stringBuilder.Append(val);
                                stringBuilder.Append(",");
                            }
                            MessageBox.Show(stringBuilder.ToString());
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
                    Title = _calculation.Formula,
                    Subtitle = "",
                    PlotType = PlotType.Cartesian,
                    Background = OxyColors.White
                };
                double incrementRate = double.Parse(_functionInputModel.PrecisionValue);

                //pm.Series.Add(new FunctionSeries(val => { return 0; }, _functionInputModel.XStart, _functionInputModel.XEnd, Math.Pow(10, incrementRate), "F(x)=0"));
                //pm.Series.Add(new FunctionSeries(func, _functionInputModel.XStart, _functionInputModel.XEnd, Math.Pow(10, incrementRate), _calculation.Formula));
                pm.Series.Add(new FunctionSeries(val => { return 0; }, _functionInputModel.XStart, _functionInputModel.XEnd, 0.1, "F(x)=0"));
                pm.Series.Add(new FunctionSeries(func, _functionInputModel.XStart, _functionInputModel.XEnd, 0.1, _calculation.Formula));

                if (_mathFunctionViewModel.TypeMethod == TypeMathMethod.Test)
                {
                    //pm.Series.Add(new FunctionSeries(_calculation.CalculateDer1, _functionInputModel.XStart, _functionInputModel.XEnd, 0.1, "Производная1"));
                    //pm.Series.Add(new FunctionSeries(_calculation.CalculateDer2, _functionInputModel.XStart, _functionInputModel.XEnd, 0.1, "Производная2"));
                }
                _graphPlotter.SetPlotterModel(pm);
            }
        }

        /*
        int FuncAnalyzer()
        {
            Expression exp = new Expression("sin(x)");
            double start = 0;
            double end = Math.PI * 2;
            double step = 0.1;
            double tolerance = 0.01; // Порог изменения

            double previousDerivative = double.NaN;

            for (double x = start; x <= end;)
            {
                exp.setArgumentValue("x", x);
                double currentDerivative = exp.calculate();

                // Если предыдущее значение существует, сравним его с текущим
                if (!double.IsNaN(previousDerivative) && Math.Abs(currentDerivative - previousDerivative) > tolerance)
                {
                    // Уменьшаем шаг, если изменение производной велико
                    step /= 2;
                }
                else
                {
                    // Увеличиваем шаг, если изменения незначительны
                    step *= 1.5;
                }

                Console.WriteLine($"x: {x}, Производная: {currentDerivative}, Шаг: {step}");

                // Сохраняем текущее значение производной
                previousDerivative = currentDerivative;

                // Переходим к следующей точке
                x += step;
            }
        }
        */



    }
}
