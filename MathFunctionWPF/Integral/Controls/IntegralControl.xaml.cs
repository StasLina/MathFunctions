using MathFunctionWPF.Models;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using MathFunctionWPF.Integral.ViewModels;
using MathFunctionWPF.Views;
using Microsoft.Office.Interop.Excel;
using MathFunctionWPF.MathMethods;
using System.Reflection.Metadata;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Annotations;
using System.Drawing;

namespace MathFunctionWPF.Integral.Controls

{
    /// <summary>
    /// Логика взаимодействия для IntegralControl.xaml
    /// </summary>
    /// 
    public class Pnl
    {
        public double X { get; set; }
        public double Y { get; set; }
    }


    public static class MathExtension
    {
        public static (double a, double b, double c) FindParabolaCoefficients(double x0, double x1, double x2, double f0, double f1, double f2)
        {
            // Проверка на совпадение координат, чтобы избежать деления на ноль
            if (x0 == x1 || x1 == x2 || x0 == x2)
            {
                throw new ArgumentException("Значения x0, x1, x2 не должны совпадать.");
            }

            // Формируем систему линейных уравнений для нахождения коэффициентов a, b, c
            // ax^2 + bx + c = f(x)

            // Решаем систему для a, b, c
            double A = x0 * x0; // x0^2
            double B = x1 * x1; // x1^2
            double C = x2 * x2; // x2^2

            double D = x0;  // x0
            double E = x1;  // x1
            double F = x2;  // x2

            double[,] matrix = {
                { A, D, 1 },
                { B, E, 1 },
                { C, F, 1 }
            };

            double[] results = { f0, f1, f2 };  // f(x0), f(x1), f(x2)

            // Решаем систему линейных уравнений с помощью метода Гаусса или другой подходящей библиотеки
            double[] coefficients = SolveLinearSystem(matrix, results);

            return (coefficients[0], coefficients[1], coefficients[2]);  // a, b, c
        }

        // Метод для решения системы линейных уравнений (например, методом Гаусса)
        public static double[] SolveLinearSystem(double[,] matrix, double[] results)
        {
            int n = results.Length;
            double[] x = new double[n];

            // Прямой ход Гаусса
            for (int i = 0; i < n; i++)
            {
                double pivot = matrix[i, i];
                if (Math.Abs(pivot) < 1e-10)  // Проверка на деление на ноль
                {
                    throw new InvalidOperationException("Матрица вырождена, деление на ноль.");
                }

                for (int j = 0; j < n; j++)
                    matrix[i, j] /= pivot;
                results[i] /= pivot;

                for (int j = i + 1; j < n; j++)
                {
                    double factor = matrix[j, i];
                    for (int k = 0; k < n; k++)
                        matrix[j, k] -= factor * matrix[i, k];
                    results[j] -= factor * results[i];
                }
            }

            // Обратный ход
            for (int i = n - 1; i >= 0; i--)
            {
                x[i] = results[i];
                for (int j = i + 1; j < n; j++)
                    x[i] -= matrix[i, j] * x[j];
            }

            return x;
        }
    }

    public partial class IntegralControl : UserControl
    {


        IntegralControlModel _model;

        public IntegralControl()
        {
            InitializeComponent();
            _model = new IntegralControlModel();

            _model.InputFields = new ObservableCollection<InputField>
                {
                    new InputField { Label = "Точность", ValidationType = ValidationType.Double, Key = "AccuracyText", Value = "0.01"},
                    new InputField { Label = "Кол-во знаков", ValidationType = ValidationType.Double, Key = "PrecisionText", Value = "-2"},
                    new InputField { Label = "Количество шагов", ValidationType = ValidationType.Double, Key="CountStepsText", Value = "4"}
                };

            _model.OutputFields = new ObservableCollection<InputField>
                {
                    new InputField { Label = "Прямоугольники лев.", ValidationType = ValidationType.Block, Key = "RectL"},
                    new InputField { Label = "Прямоугольники центр.", ValidationType = ValidationType.Block, Key = "RectC"},
                    new InputField { Label = "Прямоугольники прав.", ValidationType = ValidationType.Block, Key = "RectR"},
                    new InputField { Label = "Трапеций", ValidationType = ValidationType.Block, Key = "Trap"},
                    new InputField { Label = "Симпсона", ValidationType = ValidationType.Block, Key = "Simp"},
                };


            // Генерируем картинку

            // Создание MemoryStream из массива байтов
            byte[] svgData = MathFunctionWPF.Resources.Resource1.integral_svgrepo_com;
            SKBitmap bitMap;

            using (MemoryStream svgStream = new MemoryStream(svgData))
            {
                bitMap = Controllers.Drawing.LoaderIcon.LoadSvgIcon(svgStream, 200, 200);
            }

            Image icon = new Image
            {
                //Proper
                //Properties.Resources.
                //Source = new BitmapImage(new Uri("pack://application:,,,/MathFunctionWPF;component/Icons/SearchIcon.svg")), // Путь к вашему SVG или PNG изображению

                Source = Controllers.Drawing.LoaderIcon.ConvertToBitmapImage(bitMap),
                Width = 100, // Ширина иконки
                Height = 100 // Высота иконки
            };

            _model.IntegralImage = icon;
            DataContext = _model;
        }

        private void InputField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //e.Handled = true;
            if (sender is System.Windows.Controls.TextBox textBox)
            {

                if (textBox.DataContext is InputField)
                {
                    InputField inputField = (InputField)textBox.DataContext;
                    e.Handled = true;

                    // Выбираем правило проверки на основе ValidationType
                    switch (inputField.ValidationType)
                    {
                        case ValidationType.Double:
                            if (Common.IsDouble(e.Text, textBox.CaretIndex, textBox.Text))
                            {
                                e.Handled = false;
                            }
                            break;

                        case ValidationType.Number:
                            if (Common.IsNumber(e.Text, textBox.CaretIndex))
                                e.Handled = false;
                            break;

                        case ValidationType.PositiveNumber:
                            if (Common.IsNumberPositive(e.Text))
                                e.Handled = false;
                            break;

                        case ValidationType.None:
                            e.Handled = false;
                            break;

                        case ValidationType.Block:
                            e.Handled = true;
                            break;
                        default:
                            e.Handled = false;
                            break;
                    }
                }
                else if (textBox.DataContext is IntegralControlModel)
                {
                    switch (textBox.Name)
                    {
                        case "ArgA":
                        case "ArgB":
                            e.Handled = true;
                            if (Common.IsDouble(e.Text, textBox.CaretIndex, textBox.Text))
                                e.Handled = false;
                            break;
                    }
                }
            }
        }

        private void InputField_FocusLeave(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox textBox && textBox.DataContext is IntegralControlModel _model)
            {
                // Выбираем правило проверки на основе ValidationType
                if (textBox.Name == "ArgB")
                {
                    e.Handled = true;

                    if (_model.XEnd < _model.XStart)
                    {
                        MessageBoxResult result = MessageBox.Show(
                            ". Конец интервала больше начала, изменить?", // Текст сообщения
                            "Ошибка ввода формулы", // Заголовок окна
                            MessageBoxButton.YesNo, // Кнопки "Да" и "Нет"
                            MessageBoxImage.Question // Иконка вопроса
                        );

                        // Обработка результата
                        if (result == MessageBoxResult.Yes)
                        {
                            e.Handled = true;
                        }
                        else
                        {
                            e.Handled = false;
                            _model.XEnd = _model.XStart;
                        }
                    }
                    else
                    {
                        e.Handled = false;
                    }
                }
                else
                if (textBox.Name == "ArgA")
                {
                    e.Handled = true;

                    if (_model.XEnd < _model.XStart)
                    {
                        MessageBoxResult result = MessageBox.Show(
                            "Начало интервала должно быть меньше конца, изменить?", // Текст сообщения
                            "Ошибка ввода формулы", // Заголовок окна
                            MessageBoxButton.YesNo, // Кнопки "Да" и "Нет"
                            MessageBoxImage.Question // Иконка вопроса
                        );

                        // Обработка результата
                        if (result == MessageBoxResult.Yes)
                        {
                            e.Handled = true;
                        }
                        else
                        {
                            e.Handled = false;
                            _model.XEnd = _model.XStart;
                        }
                    }
                    else
                    {
                        e.Handled = false;
                    }
                }
                else if (textBox.Name == "FunctionInput")
                {
                    e.Handled = true;

                    if (_model.VerifyedFormula == textBox.Text)
                    {
                        e.Handled = false;
                        return;
                    }


                    FunctionCalculation calculation;
                    try
                    {
                        _model.Formula = textBox.Text;
                        calculation = new FunctionCalculation(_model);


                        if (calculation.ArgsCount != 1)
                        {
                            throw new Exception("Количество аргументов не равно 1");
                        }

                        _model.VerifyedFormula = _model.Formula;
                        e.Handled = false;
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
                            e.Handled = true;
                        }
                        else
                        {
                            e.Handled = false;
                            textBox.Text = _model.VerifyedFormula;
                        }
                    }
                    return;
                }
            }
        }

        private void AutoDetectCount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FunctionCalculation _calculation = new FunctionCalculation(_model);

                // Рассчёт количества итераций
                double countIterations = NumericalIntegration.CalculateCountIterations(_calculation, _model.XStart, _model.XEnd, _model.Accuracy);
                SetInput("CountStepsText", countIterations.ToString("F0"));
                MessageBox.Show($"Необходимо количество итераций {countIterations}");

                //_model.CountStepsText = FormatValue();
                //outputView.SetResult(TypeMathResult.IntegralRectangelValue, result);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private string FormatValue(double value)
        {
            string result = "";
            var incrementRate = _model.IncrementRate;

            if (incrementRate < 0)
            {
                result = value.ToString($"F{-1 * incrementRate}");
            }
            else
            {
                result = value.ToString();
            }

            return result;
        }

        void SetResults(string key, double value)
        {
            foreach (var elm in _model.OutputFields)
            {
                if (elm.Key == key)
                {
                    elm.Value = FormatValue(value);
                    break;
                }
            }
        }

        void SetResults(string key, string text = "")
        {
            foreach (var elm in _model.OutputFields)
            {
                if (elm.Key == key)
                {
                    elm.Value = text;
                    break;
                }
            }
        }
        void SetInput(string key, string value)
        {
            foreach (var elm in _model.InputFields)
            {
                if (elm.Key == key)
                {
                    elm.Value = value;
                    break;
                }
            }
        }


        private void DrawClick(object sender, RoutedEventArgs e)
        {
            FunctionCalculation _calculation = new FunctionCalculation(_model);

            try
            {
                Func<double, double> func = _calculation.Calculate;

                double step = NumericalIntegration.GetStep(_model.XStart, _model.XEnd, _model.CountSteps);

                var pm = new PlotModel
                {
                    Title = _calculation.Formula,
                    Subtitle = "",
                    PlotType = PlotType.Cartesian,
                    Background = OxyColors.White
                };
                double incrementRate = _model.Accuracy;

                var serFunc = new FunctionSeries(func, _model.XStart, _model.XEnd, 0.1, _calculation.Formula);
                serFunc.Color = OxyColor.Parse("#000000");

                serFunc.StrokeThickness = 4;
                var annotation = new LineAnnotation
                {
                    Type = LineAnnotationType.Horizontal,
                    Y = 0
                };

                pm.Annotations.Add(annotation);

                // Нкаходим минимум\максимум функции

                double yMin = double.MaxValue, yMax = -double.MaxValue;

                foreach (var item in serFunc.Points)
                {
                    if (item.Y < yMin)
                    {
                        yMin = item.Y;
                    }

                    if (item.Y > yMax)
                    {
                        yMax = item.Y;
                    }
                }

                var maximum = yMax;
                var minimum = yMin;

                var margin = (maximum - minimum) * 0.05;

                var valueAxis = new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Minimum = minimum - margin,
                    Maximum = maximum + margin,
                };


                //PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X" });
                //PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Y" });
                pm.Axes.Add(valueAxis);

                var listPanels = new List<Pnl>();
                var listPanels2 = new List<Pnl>();
                var listPanels3 = new List<Pnl>();
                var serBar = new FunctionSeries(func, _model.XStart, _model.XEnd, step, _calculation.Formula);

                foreach (var point in serBar.Points)
                {
                    listPanels.Add(new Pnl() { X = point.X, Y = point.Y });
                }
                listPanels2.Add(new Pnl() { X = serBar.Points[0].X, Y = serBar.Points[0].Y });

                double multipl = 1;
                for (int idxPoint = 1; idxPoint < serBar.Points.Count; ++idxPoint)
                {
                    DataPoint point = serBar.Points[idxPoint];
                    DataPoint point2 = serBar.Points[idxPoint - 1];
                    listPanels2.Add(new Pnl() { X = point.X, Y = point2.Y });
                    listPanels2.Add(new Pnl() { X = point.X, Y = point.Y });


                    //if (idxPoint % 2 == 1)
                    //{
                    //multipl = 1;
                    //double x2 = point.X + step;
                    double x2 = (point.X + point2.X) / 2;
                    double y2 = func(x2);

                    listPanels3.AddRange(FillParabolaPoints(point2.X, x2, point.X, point2.Y, y2, point.Y, multipl));
                    //}
                    //else
                    //{
                    //    multipl = -1;
                    //}

                }
                //{
                //    new Pnl() { X = 3, Y = 4 },
                //    new Pnl() {X = 4, Y = 3}
                //};
                var paraolSwries = new LineSeries
                {
                    Title = "P & L",
                    ItemsSource = listPanels3,
                    DataFieldX = "X",
                    DataFieldY = "Y",
                    StrokeThickness = 2 //140000FF
                };

                paraolSwries.Color = OxyColor.Parse("#FF0000");

                // Добавляем Прямоугольники
                var seriesBar = new LinearBarSeries
                {
                    Title = "P & L",
                    ItemsSource = listPanels,
                    DataFieldX = "X",
                    DataFieldY = "Y",
                    //FillColor = OxyColor.Parse("#14FF0000"),//14FF0000
                    StrokeColor = OxyColor.Parse("#000000"),
                    StrokeThickness = 1,
                    BarWidth = 0
                };


                var seriesBar2 = new AreaSeries
                {
                    Title = "P & L",
                    ItemsSource = listPanels2,
                    DataFieldX = "X",
                    DataFieldY = "Y",
                    Fill = OxyColor.Parse("#10FF0000"),//14FF0000
                    StrokeThickness = 0,
                };

                var seriesArea = new AreaSeries
                {
                    Title = "P & L",
                    ItemsSource = listPanels,
                    DataFieldX = "X",
                    DataFieldY = "Y",
                    Color = OxyColor.Parse("#4CAF50"),
                    Fill = OxyColor.Parse("#454CAF50"),
                    MarkerSize = 3,
                    MarkerFill = OxyColor.Parse("#FFFFFFFF"),
                    MarkerStroke = OxyColor.Parse("#4CAF50"),
                    MarkerStrokeThickness = 1.5,
                    MarkerType = MarkerType.Circle,
                    StrokeThickness = 1,
                };

                pm.Series.Add(serFunc);
                pm.Series.Add(paraolSwries);

                pm.Series.Add(seriesBar);
                pm.Series.Add(seriesBar2);
                pm.Series.Add(seriesArea);

                Drawing.Model = pm;
                pm.InvalidatePlot(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private List<Pnl> FillParabolaPoints(double x0, double x1, double x2, double y0, double y1, double y2, double multipl = 1)
        {
            List<Pnl> listResults = new List<Pnl>();


            var coefficients = MathExtension.FindParabolaCoefficients(x0, x1, x2, y0, y1, y2);
            double a = coefficients.a;
            double b = coefficients.b;
            double c = coefficients.c;


            // Рисуем параболу
            for (double x = x0; x <= x2; x += 0.01)
            {
                double y = multipl * a * x * x + b * x + c;
                listResults.Add(new Pnl() { X = x, Y = y });
                //int screenX = offsetX + (int)(x * scale);
                //int screenY = offsetY - (int)(y * scale);
                //g.FillEllipse(Brushes.Red, screenX, screenY, 2, 2);  // Отображаем параболу красным
            }
            return listResults;
        }


        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FunctionCalculation _calculation = new FunctionCalculation(_model);

                // Рассчёт количества итераций
                //double countIterations = NumericalIntegration.CalculateCountIterations(_calculation, _model.XStart, _model.XEnd, _model.Accuracy);
                //MessageBox.Show($"Необходимо количество итераций {countIterations}");

                //var outputView = _functionOutputView as IFunctionIntegrationOutputView;




                if (IsChecked("RectL"))
                {
                    double integralValueRectL = NumericalIntegration.RectangleMethodLeft(_calculation.Calculate, _model.XStart, _model.XEnd, _model.CountSteps);
                    SetResults("RectL", integralValueRectL);
                }
                else
                {
                    SetResults("RectL");
                }

                if (IsChecked("RectC"))
                {
                    double integralValueRectC = NumericalIntegration.RectangleMethodCenter(_calculation.Calculate, _model.XStart, _model.XEnd, _model.CountSteps);
                    SetResults("RectC", integralValueRectC);
                    SetResults("RectC");
                }

                if (IsChecked("RectR"))
                {
                    double integralValueRectR = NumericalIntegration.RectangleMethodRight(_calculation.Calculate, _model.XStart, _model.XEnd, _model.CountSteps);
                    SetResults("RectR", integralValueRectR);
                    SetResults("RectR");
                }

                if (IsChecked("Trap"))
                {
                    double integralValueTrap = NumericalIntegration.TrapezoidMethod(_calculation.Calculate, _model.XStart, _model.XEnd, _model.CountSteps);
                    SetResults("Trap", integralValueTrap);
                    SetResults("Trap");
                }

                if (IsChecked("Simp"))
                {
                    double integralValueSim = NumericalIntegration.SimpsonMethod(_calculation.Calculate, _model.XStart, _model.XEnd, _model.CountSteps);
                    SetResults("Simp", integralValueSim);
                    SetResults("Simp");
                }


                //outputView.SetResult(TypeMathResult.IntegralRectangelValue, result);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        bool IsChecked(string key)
        {
            var result = from num in _model.OutputFields
                         where num.Key == key
                         select num;

            if (result.Count() > 0)
            {
                return result.First().IsChecked;
            }

            return false;
        }
    }
}
