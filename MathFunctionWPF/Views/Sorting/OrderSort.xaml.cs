﻿using System;
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

namespace MathFunctionWPF.Views.Sorting
{
    /// <summary>
    /// Логика взаимодействия для OrderSort.xaml
    /// </summary>
    public partial class OrderSort : UserControl
    {
        public bool Sorting { get => CBSortDesc.IsChecked == true ? false : true; }
        public OrderSort()
        {
            InitializeComponent();
            CBSortAsc.IsChecked = true;
        }

        private void CBSortDesc_Checked(object sender, RoutedEventArgs e)
        {
            CBSortAsc.IsChecked = false;
        }

        private void CBSortAsc_Checked(object sender, RoutedEventArgs e)
        {
            CBSortDesc.IsChecked = false;
        }
    }
}
