using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace MathFunctionWPF.SLAU.Controls.Converters
{
    public class ResultsVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Если Results не null и не пустое, то показываем кнопку
            if (value != null && value is object results && results != DBNull.Value)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed; // Скрываем кнопку, если Results пустое
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Для ConvertBack мы не будем использовать
            return null;
        }
    }
}
