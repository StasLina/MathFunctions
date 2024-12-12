using MathFunctionWPF.Integral.ViewModels;
using MathFunctionWPF.Models;
using MathFunctionWPF.SLAU.ViewModels;
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

namespace MathFunctionWPF.LeastSquares.Controls
{
    /// <summary>
    /// Логика взаимодействия для SLAUMainCOntrol.xaml
    /// </summary>
    public partial class LeastSquaresMainControl : UserControl
    {
        public LeastSquaresMainControl()
        {
            InitializeComponent();
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

                else if (textBox.DataContext is SLAUMainControlModel)
                {
                    switch (textBox.Name)
                    {
                        case "RowField":
                        case "ColumnField":
                            e.Handled = true;
                            if (Common.IsNumberPositive(e.Text))
                                e.Handled = false;
                            break;
                    }
                }
            }
        }
    }
}
