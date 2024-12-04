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

namespace MathFunctionWPF.Integral.Controls

{
    /// <summary>
    /// Логика взаимодействия для IntegralControl.xaml
    /// </summary>
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
                    new InputField { Label = "Количество шагов", ValidationType = ValidationType.Double, Key="CountStepsText", Value = "0"}
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
                            if (Common.IsNumber(e.Text, textBox.CaretIndex))
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
                e.Handled = true;
                // Выбираем правило проверки на основе ValidationType

                if (textBox.Name != "FunctionInput")
                {
                    return;
                }

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
            }
        }

        private void AutoDetectCount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FunctionCalculation _calculation = new FunctionCalculation(_model);

                // Рассчёт количества итераций
                double countIterations = NumericalIntegration.CalculateCountIterations(_calculation, _model.XStart, _model.XEnd, _model.Accuracy);
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


        private void DrawClick(object sender, RoutedEventArgs e)
        {
            FunctionCalculation _calculation = new FunctionCalculation(_model);

            try
            {
                Func<double, double> func = _calculation.Calculate;
                
                var pm = new PlotModel
                {
                    Title = _calculation.Formula,
                    Subtitle = "",
                    PlotType = PlotType.Cartesian,
                    Background = OxyColors.White
                };
                double incrementRate = _model.Accuracy;

                pm.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Title = "Ось X",
                    Minimum = _model.XStart, // Минимальное значение по оси X
                    Maximum = _model.XEnd // Максимальное значение по оси X
                });

                pm.Series.Add(new FunctionSeries(func, _model.XStart, _model.XEnd, 0.1, _calculation.Formula));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            double integralValueRectL = NumericalIntegration.RectangleMethodLeft(_calculation.Calculate, _model.XStart, _model.XEnd, _model.CountSteps);
            double integralValueRectC = NumericalIntegration.RectangleMethodCenter(_calculation.Calculate, _model.XStart, _model.XEnd, _model.CountSteps);
            double integralValueRectR = NumericalIntegration.RectangleMethodRight(_calculation.Calculate, _model.XStart, _model.XEnd, _model.CountSteps);
            double integralValueTrap = NumericalIntegration.TrapezoidMethod(_calculation.Calculate, _model.XStart, _model.XEnd, _model.CountSteps);
            double integralValueSim = NumericalIntegration.SimpsonMethod(_calculation.Calculate, _model.XStart, _model.XEnd, _model.CountSteps);

            SetResults("RectL", integralValueRectL);
            SetResults("RectC", integralValueRectC);
            SetResults("RectR", integralValueRectR);
            SetResults("Trap", integralValueTrap);
            SetResults("Simp", integralValueSim);
            //outputView.SetResult(TypeMathResult.IntegralRectangelValue, result);


        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка");
        }
    }
}
}
