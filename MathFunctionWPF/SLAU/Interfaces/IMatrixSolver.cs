using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathFunctionWPF.SLAU.Interfaces
{

    public interface IMatrixSolver
    {
        double[] Solve(double[,] matrix, double[] vector, EventHandler action = null);
        //double[] SolveAsync(double[,] matrix, double[] vector, CancellationToken token);
        Task<double[]> SolveAsync(double[,] matrix, double[] vector, CancellationToken token);


        public event EventHandler SLAUSolved;


    }
}
