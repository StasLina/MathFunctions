using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathFunctionWPF.SLAU.Interfaces
{

    public interface IMatrixSolver
    {
        double[] Solve(double[,] matrix, double[] vector);
        public event EventHandler SLAUSolved;

    }
}
