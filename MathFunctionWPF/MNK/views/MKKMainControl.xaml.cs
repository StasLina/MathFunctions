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

namespace MathFunctionWPF.MNK.views
{
    /// <summary>
    /// Логика взаимодействия для MKKMainControl.xaml
    /// </summary>
    public partial class MKKMainControl : UserControl
    {
        public MKKMainControl()
        {
            InitializeComponent();
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Drawing.Width = e.NewSize.Width;
            Drawing.Height = e.NewSize.Height;

            // Если нужно оповестить OxyPlot о перерисовке
            if (Drawing.Model != null)
            {
                Drawing.Model.InvalidatePlot(true);
            }
        }
    }
}
