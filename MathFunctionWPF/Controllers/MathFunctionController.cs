using System.Windows;
using System.Windows.Controls;
using MathFunctionWPF.Controls;
using MathFunctionWPF.Models;
using MathFunctionWPF.Views;
using OxyPlot;
using OxyPlot.Series;
using MathFunctionWPF.MathMethods;
using System.Text;
using MathFunctionWPF.Views.Descriptions;
using OxyPlot.Axes;

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

        private object _functionInputView;
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
                var inputView = InitFunctionInputView();
                _functionInputModel.UpdateWithData();
                // Обновляем формулу
                _functionInputModel.Formula = "";
                TextFunctionChange(inputView.FunctionString);

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

                MethodChanged(TypeMathMethod.Newton);
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
                case TypeMathMethod.Integration:
                    {
                        InitIntegration();
                        break;
                    }
                case TypeMathMethod.Newton:
                    {
                        InitNewtonMethod();
                        break;
                    }                
                case TypeMathMethod.CoordinateDesent:
                    {
                        InitCoordinateDesent();
                        break;
                    }
            }
        }

        private FunctionInputView InitFunctionInputView()
        {
            FunctionInputView inputView;
            if (_functionInputView is null)
            {
                inputView = null;
            }
            else
            {
                inputView = _mathFunctionViewModel.SourceDataView as FunctionInputView;

            }

            if (inputView != null)
            {
                _functionInputView = inputView;
            }
            else
            {
                inputView = new FunctionInputView();
                _mathFunctionViewModel.SourceDataView = inputView;

                if (_functionInputModel == null)
                {
                    _functionInputModel = (FunctionInputData)inputView.DataContext;
                }
                else
                {
                    inputView.DataContext = _functionInputModel;
                }

                inputView.AddFunctionStringChangedListener(this.TextFunctionChange);
                inputView.AddArgXStartChangedListener(this.UpdateXStartArg);
                inputView.AddArgXEndChangedListener(this.UpdateXEndArg);
                inputView.AddAverageChangedListener(this.UpdateAccuracyArg);
            }


            //_functionInputModel.UpdateWithData();
            return inputView;

        }

        private FunctionInputIntegralView InitFunctionIntegrationInputView()
        {
            FunctionInputIntegralView inputView;
            if (_functionInputView is null)
            {
                inputView = null;
            }
            else
            {
                inputView = _mathFunctionViewModel.SourceDataView as FunctionInputIntegralView;

            }

            if (inputView != null)
            {
                _functionInputView = inputView;
            }
            else
            {
                inputView = new FunctionInputIntegralView();
                _mathFunctionViewModel.SourceDataView = inputView;

                if (_functionInputModel == null)
                {
                    _functionInputModel = (FunctionInputData)inputView.DataContext;
                }
                else
                {
                    inputView.DataContext = _functionInputModel;
                }

                inputView.AddFunctionStringChangedListener(this.TextFunctionChange);
                inputView.AddArgXStartChangedListener(this.UpdateXStartArg);
                inputView.AddArgXEndChangedListener(this.UpdateXEndArg);
                inputView.AddAverageChangedListener(this.UpdateAccuracyArg);
                inputView.AddCountStepsChangedListener(this.UpdateCountStepsArg);
            }


            //_functionInputModel.UpdateWithData();
            return inputView;

        }

        private void InitIntegration()
        {
            InitFunctionIntegrationInputView();

            _mathFunctionViewModel.TypeMethod = TypeMathMethod.Integration;

            _mathFunctionViewModel.DescriptionView = null;

            if (_mathFunctionViewModel.CalculationView != null && _mathFunctionViewModel.CalculationView is FunctionOutputIntegration)
            {

                _functionOutputView = (FunctionOutputIntegration)_mathFunctionViewModel.CalculationView;
            }
            else
            {
                _functionOutputView = new FunctionOutputIntegration();
                _mathFunctionViewModel.CalculationView = _functionOutputView;
            }

            _functionOutputView.AddListenerUpdatePlotter(UpdatePlotterView);

            if (_functionOutputView is IFunctionIntegrationOutputView)
            {
                var view = _functionOutputView as IFunctionIntegrationOutputView;
                view.AddListenerCalcCount(UpdateFunctionView);
                view.AddListenerRectangelIntegral(UpdateRectangelIntegration);
                view.AddListenerTrapecialIntegral(UpdateTrapecialIntegration);
                view.AddListenerSimpsonIntegral(UpdateSimpsonIntegration);
            }
        }

        private void UpdateRectangelIntegration()
        {
            var outputView = _functionOutputView as IFunctionIntegrationOutputView;
            double integralValue = NumericalIntegration.RectangleMethod(_calculation.Calculate, _functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.CountSteps);

            double incrementRate = double.Parse(_functionInputModel.PrecisionValue);
            string result;

            if (incrementRate < 0)
            {
                result = integralValue.ToString($"F{-1 * incrementRate}");
            }
            else
            {
                result = integralValue.ToString();
            }

            outputView.SetResult(TypeMathResult.IntegralRectangelValue, result);
        }

        private void UpdateTrapecialIntegration()
        {
            var outputView = _functionOutputView as IFunctionIntegrationOutputView;
            double integralValue = NumericalIntegration.TrapezoidMethod(_calculation.Calculate, _functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.CountSteps);

            double incrementRate = double.Parse(_functionInputModel.PrecisionValue);
            string result;
            if (incrementRate < 0)
            {
                result = integralValue.ToString($"F{-1 * incrementRate}");
            }
            else
            {
                result = integralValue.ToString();
            }
            outputView.SetResult(TypeMathResult.IntegralTrapezeValue, result);
        }

        private void UpdateSimpsonIntegration()
        {
            try
            {
                var outputView = _functionOutputView as IFunctionIntegrationOutputView;
                double integralValue = NumericalIntegration.SimpsonMethod(_calculation.Calculate, _functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.CountSteps);

                double incrementRate = double.Parse(_functionInputModel.PrecisionValue);
                string result;
                if (incrementRate < 0)
                {
                    result = integralValue.ToString($"F{-1 * incrementRate}");
                }
                else
                {
                    result = integralValue.ToString();
                }
                outputView.SetResult(TypeMathResult.IntegralSimpsonValue, result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InitBisectionMethod()
        {
            InitFunctionInputView();

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
            InitFunctionInputView();

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

        private void InitCoordinateDesent()
        {
            InitFunctionIntegrationInputView();

            _mathFunctionViewModel.TypeMethod = TypeMathMethod.CoordinateDesent;

            _mathFunctionViewModel.DescriptionView = new CoordinateDescentDescription();

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

        private void InitNewtonMethod()
        {
            InitFunctionIntegrationInputView();

            _mathFunctionViewModel.TypeMethod = TypeMathMethod.Newton;

            _mathFunctionViewModel.DescriptionView = new NewtonMethodDescription();

            if (_mathFunctionViewModel.CalculationView != null && _mathFunctionViewModel.CalculationView is FunctionOutputViewIntersection)
            {

                _functionOutputView = (FunctionOutputMinMaxIntersectView)_mathFunctionViewModel.CalculationView;
            }
            else
            {
                _functionOutputView = new FunctionOutputMinMaxIntersectView();
                _mathFunctionViewModel.CalculationView = _functionOutputView;
            }

            _functionOutputView.AddListenerUpdateFunction(UpdateFunctionView);
            _functionOutputView.AddListenerUpdatePlotter(UpdatePlotterView);
        }

        private void InitTest()
        {
            InitFunctionInputView();

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
                        _functionInputModel.Formula = oldName;
                        (_functionInputView as FunctionInputView).ReturnedFocus = textBox;
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

        private void UpdateCountStepsArg(TextBox textBox)
        {
            double newValue = 0;
            if (NumberFunctionHandler(textBox, _functionInputModel.XEnd, ref newValue))
            {
                _functionInputModel.CountSteps = newValue;
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
                    //_functionInputView as F.ReturnedFocus = textBox;
                    (_functionInputView as FunctionInputView).ReturnedFocus = textBox;

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

                    case TypeMathMethod.CoordinateDesent:
                        {
                            //double value = Dihtomia.Calc(func,_functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy);
                            double value = CoordinateDescent.Calc(_calculation, _functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy, (int) _functionInputModel.CountSteps);
                            _calculation.IsInverse = true;
                            double value2 = CoordinateDescent.Calc(_calculation, _functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy, (int) _functionInputModel.CountSteps);
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
                            double result = _calculation.Calculate(_functionInputModel.XStart);
                            double der1 = _calculation.CalculateDer1(_functionInputModel.XStart);
                            double der2 = _calculation.CalculateDer2(_functionInputModel.XStart);

                            _functionOutputView.SetResult(TypeMathResult.Derevative1, der1.ToString());
                            _functionOutputView.SetResult(TypeMathResult.Derevative2, der2.ToString());
                            _functionOutputView.SetResult(TypeMathResult.MinimumValue, result.ToString());

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

                    case TypeMathMethod.Integration:
                        {
                            double countIterations = NumericalIntegration.CalculateCountIterations(_calculation, _functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy);
                            MessageBox.Show($"Необходимо количество итераций {countIterations}");
                            break;
                        }

                    case TypeMathMethod.Newton:
                        {
                            try
                            {
                                double maxValue = NewtonMethod.CalcMax(_calculation, _functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy, (int)_functionInputModel.CountSteps);
                                double minValue = NewtonMethod.CalcMin(_calculation, _functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy, (int)_functionInputModel.CountSteps);
                                //MessageBox.Show($"Максимум: {maxValue.ToString()}");
                                //MessageBox.Show($"Максимум: {minValue.ToString()}");
                                string argValue, funcValue, argMinValue, funcMinValue, argMaxValue, funMaxValue;

                                double incrementRate = double.Parse(_functionInputModel.PrecisionValue);
                                double valueFuncMax = _calculation.Calculate(maxValue);
                                double valueFuncMin = _calculation.Calculate(minValue);

                                if (incrementRate < 0)
                                {
                                    argMinValue = minValue.ToString($"F{-1 * incrementRate}");
                                    funcMinValue = valueFuncMin.ToString($"F{-1 * incrementRate}");
                                    argMaxValue = maxValue.ToString($"F{-1 * incrementRate}");
                                    funMaxValue = valueFuncMax.ToString($"F{-1 * incrementRate}");
                                }
                                else
                                {
                                    argMinValue = minValue.ToString("F0");
                                    funcMinValue = valueFuncMin.ToString("F0");
                                    argMaxValue = maxValue.ToString("F0");
                                    funMaxValue = valueFuncMax.ToString("F0");
                                }


                                _functionOutputView.SetResult(TypeMathResult.MinimumArgument, argMinValue);
                                _functionOutputView.SetResult(TypeMathResult.MinimumValue, funcMinValue);
                                _functionOutputView.SetResult(TypeMathResult.MaximumArgument, argMaxValue);
                                _functionOutputView.SetResult(TypeMathResult.MaximumValue, funMaxValue);

                                try
                                {
                                    double value = NewtonMethod.Calc(_calculation, _functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy, (int)_functionInputModel.CountSteps);
                                    double valueFunc = _calculation.Calculate(value);

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
                                }
                                catch (Exception ex)
                                {
                                    _functionOutputView.SetResult(TypeMathResult.IntespectionArgument, ex.Message);
                                    _functionOutputView.SetResult(TypeMathResult.IntespectionValue, ex.Message);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
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

                pm.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Title = "Ось X",
                    Minimum = _functionInputModel.XStart, // Минимальное значение по оси X
                    Maximum = _functionInputModel.XEnd // Максимальное значение по оси X
                });

                //// Добавление оси Y
                //pm.Axes.Add(new LinearAxis
                //{
                //    Position = AxisPosition.Left,
                //    Title = "ОсьY",
                //    Minimum = -10, // Минимальное значение по оси Y
                //    Maximum = 10 // Максимальное значение по оси Y
                //});

                if (_mathFunctionViewModel.TypeMethod == TypeMathMethod.Test)
                {
                    var list = _calculation.FindDiscontinuities(_functionInputModel.XStart, _functionInputModel.XEnd);

                    if (list.Count > 0)
                    {
                        pm.Series.Add(new FunctionSeries(func, _functionInputModel.XStart, list[0], 0.1, _calculation.Formula));
                        for (int i = 1,end = list.Count -1; i < end; i += 2)
                        {
                            pm.Series.Add(new FunctionSeries(func, list[i], list[i+1], 0.1, _calculation.Formula));
                        }

                        pm.Series.Add(new FunctionSeries(func, list[list.Count-1], _functionInputModel.XEnd, 0.1, _calculation.Formula));

                    }
                    else
                    {
                        pm.Series.Add(new FunctionSeries(func, _functionInputModel.XStart, _functionInputModel.XEnd, 0.1, _calculation.Formula));
                    }
                }
                else
                {
                    pm.Series.Add(new FunctionSeries(func, _functionInputModel.XStart, _functionInputModel.XEnd, 0.1, _calculation.Formula));
                    pm.Series.Add(new FunctionSeries() { Points = { new DataPoint(_functionInputModel.XStart, 0), new DataPoint(_functionInputModel.XEnd, 0) } });
                }

                if (_mathFunctionViewModel.TypeMethod == TypeMathMethod.Test)
                {
                    //pm.Series.Add(new FunctionSeries(_calculation.CalculateDer1, _functionInputModel.XStart, _functionInputModel.XEnd, 0.1, "Производная1"));
                    //pm.Series.Add(new FunctionSeries(_calculation.CalculateDer2, _functionInputModel.XStart, _functionInputModel.XEnd, 0.1, "Производная2"));
                }
                _graphPlotter.SetPlotterModel(pm);
            }
        }
    }
}
