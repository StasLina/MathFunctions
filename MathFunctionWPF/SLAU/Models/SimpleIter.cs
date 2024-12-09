using MathFunctionWPF.SLAU.Interfaces;

namespace MathFunctionWPF.SLAU.Models
{
    public class SimpleIter : MethodBase, IMatrixSolver
    {
        public SimpleIter() : base(TypeMethod.SimpleIter) { }

        public event EventHandler SLAUSolved;

        public double[] Solve(double[,] matrix, double[] vector)
        {
            int n = matrix.GetLength(0);
            double[] x = new double[n];
            double[] prevX = new double[n];
            double tolerance = 1e-6;
            int maxIterations = 1000;

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                for (int i = 0; i < n; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < n; j++)
                    {
                        if (j != i)
                            sum += matrix[i, j] * prevX[j];
                    }
                    x[i] = (vector[i] - sum) / matrix[i, i];
                }

                // Проверка на сходимость
                double error = 0;
                for (int i = 0; i < n; i++)
                {
                    error = Math.Max(error, Math.Abs(x[i] - prevX[i]));
                }
                if (error < tolerance)
                    break;

                Array.Copy(x, prevX, n);
            }

            SLAUSolved?.Invoke(this, new EventArgs());
            return x;
        }
    }

}


