using MathFunctionWPF.SLAU.Events;
using MathFunctionWPF.SLAU.Interfaces;

namespace MathFunctionWPF.SLAU.Models
{
    public class Squre : MethodBase, IMatrixSolver
    {
        public Squre() : base(TypeMethod.Squre) { }

        public event EventHandler SLAUSolved;

        // Определён как разложение Холецкого
        public double[] Solve(double[,] matrix, double[] vector, EventHandler action = null)
        {
            int n = matrix.GetLength(0);
            double[,] L = new double[n, n];

            action?.Invoke(this, null);
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
                action?.Invoke(this, null);
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
                action?.Invoke(this, null);
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
                action?.Invoke(this, null);
            }

            SLAUSolved?.Invoke(this, new Results() { Result = x });
            return x;
        }


        public async Task<double[]> SolveAsync(double[,] matrix, double[] vector, CancellationToken token)
        {
            DateTime time = DateTime.Now;
            return await Task.Run(() =>
            {
                double[] result = new double[] { 0 };
                try
                {
                    result = Solve(matrix, vector, (obk, args) =>
                    {
                        if (this.Result != null)
                        {
                            this.Result.Time = (DateTime.Now - time).Seconds;
                        }
                        token.ThrowIfCancellationRequested();
                    });
                }
                catch (Exception ex)
                {

                }
                return result;

            }
            );
        }
    }


}


