using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace MathFunctionWPF.Utils
{
    internal class Utils
    {
        static private T GetParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            // Если родитель не найден, возвращаем null
            if (parent == null)
                return null;

            // Если это нужный тип, возвращаем его
            if (parent is T parentT)
                return parentT;

            // Иначе рекурсивно ищем дальше
            return GetParent<T>(parent);
        }
    }
}
