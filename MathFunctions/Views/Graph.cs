using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot.WindowsForms;
namespace MathFunctions.Views
{
    public partial class GraphControl : Control
    {
        PlotView funcPlotter;

        public Graph()
        {
            InitializeComponent();
            funcPlotter = new PlotView();
            funcPlotter.Dock = DockStyle.Fill;
            Controls.Add(funcPlotter);
            this.Dock = DockStyle.Fill;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}
