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
using System.Reflection.Emit;

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

                //MethodChanged(TypeMathMethod.Newton);
                MethodChanged(TypeMathMethod.Bisection);
            }
        }

        private void MethodChanged(TypeMathMethod typeMethod)
        {
            _functionInputModel.ResetLabels();
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

        public static readonly int[] OneArg = new int[] { 1 };
        public static readonly int[] TwoArg = new int[] { 1, 2 };

        public int[] GetCountArgsForMethod(TypeMathMethod typeMethod)
        {
            _functionInputModel.ResetLabels();
            switch (typeMethod)
            {
                case TypeMathMethod.Bisection:
                case TypeMathMethod.GoldenSearch:
                case TypeMathMethod.Test:
                case TypeMathMethod.Integration:
                case TypeMathMethod.Newton:
                    {
                        return OneArg;
                    }
                case TypeMathMethod.CoordinateDesent:
                    {
                        return TwoArg;
                    }
                default:
                    {
                        return OneArg;
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
                _functionInputView = inputView;

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
                _functionInputView = inputView;

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

        private void InitCoordinateDesentLabels()
        {
            _functionInputModel.CountStepsLabel = "Количество итераций";

            if (_calculation != null)
            {
                if (_calculation.CountArgs() == 2)
                {
                    _functionInputModel.X1Label = "Y";
                    _functionInputModel.X0Label = "X";
                }
                else if (_calculation.CountArgs() == 1)
                {
                    _functionInputModel.X1Label = "X1";
                    _functionInputModel.X0Label = "X0";
                }

            }
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

            InitCoordinateDesentLabels();


            _functionOutputView.AddListenerUpdateFunction(UpdateFunctionView);
            _functionOutputView.AddListenerUpdatePlotter(UpdatePlotterViewCustom);
        }

        private void InitNewtonMethod()
        {
            InitFunctionIntegrationInputView();

            _mathFunctionViewModel.TypeMethod = TypeMathMethod.Newton;

            _mathFunctionViewModel.DescriptionView = new NewtonMethodDescription();

            if (_mathFunctionViewModel.CalculationView != null && _mathFunctionViewModel.CalculationView is FunctionOutputMinMaxIntersectView)
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

        public class PlotModel2Arg
        {
            public PlotModel PlotModel { get; private set; }
            double x0 = -10, x1 = 10, y0 = -10, y1 = 10;
            int xStep = 100, yStep = 100;
            public PlotModel2Arg()
            {
                PlotModel = new PlotModel { Title = "Depth Map of 3D Function" };

                // Настройка осей X и Y
                PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X" });
                PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Y" });

                // Создаем цветную карту глубины
                var heatMapSeries = new HeatMapSeries
                {
                    X0 = x0,
                    X1 = x1,
                    Y0 = y0,
                    Y1 = y1,
                    Interpolate = true,
                    RenderMethod = HeatMapRenderMethod.Bitmap,
                    Data = GenerateDepthData(x0, x1, y0, y1, xStep, yStep)
                };

                // Добавляем HeatMapSeries в модель графика
                PlotModel.Series.Add(heatMapSeries);

                // Настраиваем ось цвета
                var colorAxis = new LinearColorAxis
                {
                    Position = AxisPosition.Right,
                    Palette = OxyPalettes.Jet(200),
                    Title = "Depth (z)"
                };
                PlotModel.Axes.Add(colorAxis);
            }

            public PlotModel2Arg(FunctionCalculation functionCalculation)
            {
                PlotModel = new PlotModel { Title = functionCalculation.Formula };
                Init(functionCalculation);
            }

            private void Init(FunctionCalculation functionCalculation)
            {
                CalculateDepthFunction = functionCalculation.Calculate2Arg;
                // Настройка осей X и Y
                PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X" });
                PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Y" });

                // Создаем цветную карту глубины
                var heatMapSeries = new HeatMapSeries
                {
                    X0 = x0,
                    X1 = x1,
                    Y0 = y0,
                    Y1 = y1,
                    Interpolate = true,
                    RenderMethod = HeatMapRenderMethod.Bitmap,
                    Data = GenerateDepthData(x0, x1, y0, y1, xStep, yStep)

                };
                // Добавляем HeatMapSeries в модель графика
                PlotModel.Series.Add(heatMapSeries);

                // Настраиваем ось цвета
                var colorAxis = new LinearColorAxis
                {
                    Position = AxisPosition.Right,
                    Palette = OxyPalettes.Jet(200),
                    Title = "Depth (z)"
                };
                PlotModel.Axes.Add(colorAxis);
            }

            private double[,] GenerateDepthData(double x0, double x1, double y0, double y1, int xSteps, int ySteps)
            {
                double[,] data = new double[xSteps, ySteps];
                double dx = (x1 - x0) / (xSteps - 1);
                double dy = (y1 - y0) / (ySteps - 1);

                for (int i = 0; i < xSteps; i++)
                {
                    for (int j = 0; j < ySteps; j++)
                    {
                        double x = x0 + i * dx;
                        double y = y0 + j * dy;
                        data[i, j] = CalculateDepthFunction(x, y);
                    }
                }
                return data;
            }
            public delegate double Calculcate(double x, double y);
            public Calculcate CalculateDepthFunction { get; set; } = CalculateDepthFunc;

            private static double CalculateDepthFunc(double x, double y)
            {
                // Пример функции: z = sin(sqrt(x^2 + y^2))
                return Math.Sin(Math.Sqrt(x * x + y * y));
            }

            public PlotModel2Arg(FunctionCalculation functionCalculation, double x1, double x2, double y1, double y2, double width, double height)
            {
                PlotModel = new PlotModel { Title = functionCalculation.Formula };

                // Пример координат
                //double x1 = 2.0, y1 = 3.0; // Начальная точка
                //double x2 = 8.0, y2 = 6.0; // Конечная точка

                // Ширина и высота графика
                //double width = 800; // В пикселях
                //double height = 600; // В пикселях

                // Нахождение min и max для X и Y
                double minX = Math.Min(x1, x2);
                double maxX = Math.Max(x1, x2);
                double minY = Math.Min(y1, y2);
                double maxY = Math.Max(y1, y2);

                // Расчет диапазонов
                double rangeX = maxX - minX;
                double rangeY = maxY - minY;

                // Регулировка диапазонов на 5% с каждой стороны
                double adjustedMinX = minX - 0.05 * rangeX;
                double adjustedMaxX = maxX + 0.05 * rangeX;
                double adjustedMinY = minY - 0.05 * rangeY;
                double adjustedMaxY = maxY + 0.05 * rangeY;

                // Рассчитываем количество шагов
                double adjustedRangeX = adjustedMaxX - adjustedMinX;
                double adjustedRangeY = adjustedMaxY - adjustedMinY;

                int stepsX = Convert.ToInt32(width);
                int stepsY = Convert.ToInt32(height);

                this.x0 = adjustedMinX;
                this.x1 = adjustedMaxX;
                this.y0 = adjustedMinY;
                this.y1 = adjustedMaxY;
                this.xStep = stepsX;
                this.yStep = stepsY;





                Init(functionCalculation);

                var scatterSeries = new ScatterSeries
                {
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 10,
                    MarkerFill = OxyColor.FromRgb(255, 0, 0)
                };
                // Добавление одной точки
                scatterSeries.Points.Add(new ScatterPoint(x1, y1));
                scatterSeries.Points.Add(new ScatterPoint(x2, y2));
                //this.PlotModel = new PlotModel();
                // Добавление серии на график
                PlotModel.Series.Add(scatterSeries);

            }



        }
        private void UpdatePlotterViewCustom()
        {
            if (_calculation != null)
            {

                switch (_calculation.CountArgs())
                {
                    case 1:
                        UpdatePlotterView();
                        break;
                    case 2:
                        PlotModel2Arg v = new PlotModel2Arg(_calculation);
                        _graphPlotter.SetPlotterModel(v.PlotModel);
                        break;
                }

            }
            return;
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
            _functionOutputView.AddListenerUpdatePlotter(UpdatePlotterViewCustom);
        }

        static string FormatArgumentsExcpetion(int[] args)
        {
            string result = "Неверное количество аргументов. Возможное количество аргументов: ";
            for (int i = 0; i < args.Length; i++)
            {
                result += args[i].ToString();
                if (i < args.Length - 1)
                {
                    result += ", ";
                }
                else
                {
                    result += ".";
                }
            }
            throw new Exception(result);
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


                    int[] possibleCountArgsArr = GetCountArgsForMethod(_mathFunctionViewModel.TypeMethod);

                    if (possibleCountArgsArr.Contains(calculation.CountArgs()) == false)
                    {
                        // Если не содержит
                        FormatArgumentsExcpetion(possibleCountArgsArr);
                    }
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
                        ReturnFocus(_functionInputView, textBox);
                    }
                    else
                    {
                        _functionInputModel.Formula = oldName;
                        textBox.Text = oldName;
                    }
                }

                // Формула поменялась
                switch (_mathFunctionViewModel.TypeMethod)
                {
                    case TypeMathMethod.CoordinateDesent:
                        InitCoordinateDesentLabels();
                        break;
                }
            }
        }

        void ReturnFocus(object view, TextBox box)
        {
            if (view is FunctionInputView)
            {
                (view as FunctionInputView).ReturnedFocus = box;
            }
            else if (view is FunctionInputIntegralView)
            {
                (view as FunctionInputIntegralView).ReturnedFocus = box;
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
                    //if(_functionInputView is FunctionInputView)
                    //{
                    //    (_functionInputView as FunctionInputView).ReturnedFocus = textBox;
                    //}else if (_functionInputView is FunctionInputIntegralView)
                    //{
                    //    (_functionInputView as FunctionInputIntegralView).ReturnedFocus = textBox;
                    //}
                    ReturnFocus(_functionInputView, textBox);
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

                //Проверка на количество аргументов

                int[] possibleCountArgsArr = GetCountArgsForMethod(_mathFunctionViewModel.TypeMethod);

                if (possibleCountArgsArr.Contains(_calculation.CountArgs()) == false)
                {
                    // Если не содержит
                    FormatArgumentsExcpetion(possibleCountArgsArr);
                }

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
                            switch (_calculation.CountArgs())
                            {

                                case 1:
                                    {
                                        double value = CoordinateDescent.Calc1Arg(_calculation, _functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy, (int)_functionInputModel.CountSteps);
                                        _calculation.IsInverse = true;
                                        double value2 = CoordinateDescent.Calc1Arg(_calculation, _functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy, (int)_functionInputModel.CountSteps);
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
                                case 2:
                                    {
                                        double x = _functionInputModel.XStart;
                                        double y = _functionInputModel.XEnd;
                                        double[] minimum = CoordinateDescent.Calc2Arg(_calculation, x, y, _functionInputModel.Accuracy, (int)_functionInputModel.CountSteps);
                                        _calculation.IsInverse = true;
                                        double[] maximum = CoordinateDescent.Calc2Arg(_calculation, x, y, _functionInputModel.Accuracy, (int)_functionInputModel.CountSteps);

                                        _calculation.IsInverse = false;
                                        string minimalValue, maximalValue;

                                        double incrementRate = double.Parse(_functionInputModel.PrecisionValue);

                                        string minimalVuncValue, maximalFuncValue;
                                        if (incrementRate < 0)
                                        {
                                            minimalValue = $"{{{minimum[0].ToString($"F{-1 * incrementRate}")}; {minimum[1].ToString($"F{-1 * incrementRate}")}}} ";
                                            maximalValue = $"{{{maximum[0].ToString($"F{-1 * incrementRate}")}; {maximum[1].ToString($"F{-1 * incrementRate}")}}} ";

                                            minimalVuncValue = _calculation.Calculate2Arg(minimum[0], minimum[1]).ToString($"F{-1 * incrementRate}");
                                            maximalFuncValue = _calculation.Calculate2Arg(maximum[0], maximum[1]).ToString($"F{-1 * incrementRate}");
                                        }
                                        else
                                        {
                                            minimalValue = $"{{{minimum[0].ToString()}; {minimum[1].ToString()}}} ";
                                            maximalValue = $"{{{maximum[0].ToString()}; {maximum[1].ToString()}}} ";

                                            minimalVuncValue = _calculation.Calculate2Arg(minimum[0], minimum[1]).ToString($"F0");
                                            maximalFuncValue = _calculation.Calculate2Arg(maximum[0], maximum[1]).ToString($"F0");
                                        }

                                        _functionOutputView.SetResult(TypeMathResult.MinimumArgument, minimalValue);
                                        _functionOutputView.SetResult(TypeMathResult.MaximumArgument, maximalValue);

                                        _functionOutputView.SetResult(TypeMathResult.MinimumValue, minimalVuncValue);
                                        _functionOutputView.SetResult(TypeMathResult.MaximumValue, maximalFuncValue);

                                        double width = _graphPlotter.ActualWidth;
                                        double height = _graphPlotter.ActualHeight;
                                        PlotModel2Arg arg = new PlotModel2Arg(_calculation, minimum[0], minimum[1], maximum[0], maximum[1], width, height);

                                        _graphPlotter.SetPlotterModel(arg.PlotModel);


                                        break;
                                    }
                            }
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
                                //double maxValue = NewtonMethod.CalcMax(_calculation, _functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy, (int)_functionInputModel.CountSteps);
                                double maxValue = 0;
                                double minValue = NewtonMethod.CalcMin(_calculation, _functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy, (int)_functionInputModel.CountSteps);
                                if (_calculation.CalculateDer2(minValue) > 0)
                                {
                                    // Минимум
                                    _calculation.IsInverse = true;
                                    maxValue = NewtonMethod.CalcMin(_calculation, _functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy, (int)_functionInputModel.CountSteps);
                                    _calculation.IsInverse = false;
                                }
                                else
                                {
                                    // Максимум
                                    maxValue = minValue;
                                    _calculation.IsInverse = true;
                                    minValue = NewtonMethod.CalcMin(_calculation, _functionInputModel.XStart, _functionInputModel.XEnd, _functionInputModel.Accuracy, (int)_functionInputModel.CountSteps);
                                    _calculation.IsInverse = false;
                                }

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
                try
                {
                    int[] possibleCountArgsArr = GetCountArgsForMethod(_mathFunctionViewModel.TypeMethod);

                    if (possibleCountArgsArr.Contains(_calculation.CountArgs()) == false)
                    {
                        // Если не содержит
                        FormatArgumentsExcpetion(possibleCountArgsArr);
                    }

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
                            for (int i = 1, end = list.Count - 1; i < end; i += 2)
                            {
                                pm.Series.Add(new FunctionSeries(func, list[i], list[i + 1], 0.1, _calculation.Formula));
                            }

                            pm.Series.Add(new FunctionSeries(func, list[list.Count - 1], _functionInputModel.XEnd, 0.1, _calculation.Formula));

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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
