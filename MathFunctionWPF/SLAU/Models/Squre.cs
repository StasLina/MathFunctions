using MathFunctionWPF.SLAU.Interfaces;

namespace MathFunctionWPF.SLAU.Models
{
    public class Squre : MethodBase, IMatrixSolver
    {
        public Squre() : base(TypeMethod.Squre) { }

        public event EventHandler SLAUSolved;

        public double[] Solve(double[,] matrix, double[] vector)
        {
            int n = matrix.GetLength(0);
            double[,] L = new double[n, n];

            // Разложение Холецкого
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < j; k++)
                    {
                        sum += L[i, k] * L[j, k];
                    }

                    if (i == j)
                    {
                        L[i, j] = Math.Sqrt(matrix[i, i] - sum);
                    }
                    else
                    {
                        L[i, j] = (matrix[i, j] - sum) / L[j, j];
                    }
                }
            }

            // Прямой ход: решаем L * y = b
            double[] y = new double[n];
            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                for (int k = 0; k < i; k++)
                {
                    sum += L[i, k] * y[k];
                }
                y[i] = (vector[i] - sum) / L[i, i];
            }

            // Обратный ход: решаем L^T * x = y
            double[] x = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                double sum = 0;
                for (int k = i + 1; k < n; k++)
                {
                    sum += L[k, i] * x[k];
                }
                x[i] = (y[i] - sum) / L[i, i];
            }

            SLAUSolved?.Invoke(this, new EventArgs());
            return x;
        }
    }


}


