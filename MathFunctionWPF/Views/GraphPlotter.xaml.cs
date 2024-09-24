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
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using MathFunctionWPF.Models;

namespace MathFunctionWPF.Controls
{
    /// <summary>
    /// Логика взаимодействия для GraphPlotter.xaml
    /// </summary>
    public partial class GraphPlotter : UserControl
    {
        PlotterModel _model;
        public GraphPlotter()
        {
            InitializeComponent();
            _model = new PlotterModel();
            DataContext = _model;
        }

        public void SetPlotterModel(PlotModel model)
        {
            PlotView.Model = model;
        }
    }
}
