using MathFunctionWPF.SLAU.Interfaces;

namespace MathFunctionWPF.SLAU.Models
{
    public class ComplexGradient : MethodBase, IMatrixSolver
    {
        public ComplexGradient() : base(TypeMethod.ComplexGraident) { }

        public event EventHandler SLAUSolved;

        public double[] Solve(double[,] matrix, double[] vector)
        {
            int n = vector.Length;
            double[] x = new double[n];
            double[] r = Subtract(vector, Multiply(matrix, x)); // Начальный остаток
            double[] p = (double[])r.Clone();
            double rsOld = Dot(r, r);

            for (int i = 0; i < n; i++)
            {
                double[] Ap = Multiply(matrix, p);
                double alpha = rsOld / Dot(p, Ap);

                for (int j = 0; j < n; j++)
                {
                    x[j] += alpha * p[j];
                    r[j] -= alpha * Ap[j];
                }

                double rsNew = Dot(r, r);
                if (Math.Sqrt(rsNew) < 1e-6)
                    break;

                for (int j = 0; j < n; j++)
                {
                    p[j] = r[j] + (rsNew / rsOld) * p[j];
                }
                rsOld = rsNew;
            }

            SLAUSolved?.Invoke(this, new EventArgs());
            return x;
        }

        private double[] Multiply(double[,] matrix, double[] vector)
        {
            int n = vector.Length;
            double[] result = new double[n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[i] += matrix[i, j] * vector[j];
                }
            }
            return result;
        }

        private double Dot(double[] v1, double[] v2)
        {
            double result = 0;
            for (int i = 0; i < v1.Length; i++)
            {
                result += v1[i] * v2[i];
            }
            return result;
        }

        private double[] Subtract(double[] v1, double[] v2)
        {
            double[] result = new double[v1.Length];
            for (int i = 0; i < v1.Length; i++)
            {
                result[i] = v1[i] - v2[i];
            }
            return result;
        }
    }

}


