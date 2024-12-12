using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathFunctionWPF.SLAU.Events
{
    internal class ArgsResultSave : EventArgs
    {
        public MathTableMatrix.DynamicTableController Controller { get; set; }
    }

    internal class Results : EventArgs
    {
        public double[] Result { get; set; }
    }
}
