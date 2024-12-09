using MathFunctionWPF.SLAU.Interfaces;

namespace MathFunctionWPF.SLAU.Models
{
    public class Progonki : MethodBase, IMatrixSolver
    {
        public Progonki() : base(TypeMethod.Progonki) { }

        public event EventHandler SLAUSolved;

        public double[] Solve(double[,] matrix, double[] vector)
        {
            int n = matrix.GetLength(0);
            double[] alpha = new double[n];
            double[] beta = new double[n];

            // Прямой ход
            alpha[0] = -matrix[0, 1] / matrix[0, 0];
            beta[0] = vector[0] / matrix[0, 0];

            for (int i = 1; i < n - 1; i++)
            {
                double denominator = matrix[i, i] + matrix[i, i - 1] * alpha[i - 1];
                alpha[i] = -matrix[i, i + 1] / denominator;
                beta[i] = (vector[i] - matrix[i, i - 1] * beta[i - 1]) / denominator;
            }

            beta[n - 1] = (vector[n - 1] - matrix[n - 1, n - 2] * beta[n - 2]) /
                          (matrix[n - 1, n - 1] + matrix[n - 1, n - 2] * alpha[n - 2]);

            // Обратный ход
            double[] x = new double[n];
            x[n - 1] = beta[n - 1];
            for (int i = n - 2; i >= 0; i--)
            {
                x[i] = alpha[i] * x[i + 1] + beta[i];
            }
            SLAUSolved?.Invoke(this, new EventArgs());
            return x;
        }
    }

}


