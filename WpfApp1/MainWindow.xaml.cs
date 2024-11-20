using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
    public class MyData
    {
        public double Value { get; set; }
    }

    public partial class MainWindow : Window
    {
        List<double> listItems = new List<double>();

        public MainWindow()
        {
            InitializeComponent();

            WInputNumbers inputNumbers = new WInputNumbers(listItems);
            inputNumbers.Show();
        }

    }
}
