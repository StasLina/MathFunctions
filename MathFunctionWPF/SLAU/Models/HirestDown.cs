using MathFunctionWPF.SLAU.Interfaces;

namespace MathFunctionWPF.SLAU.Models
{
    public class HirestDown : MethodBase, IMatrixSolver
    {
        public HirestDown() : base(TypeMethod.HirestDown) { }

        public event EventHandler SLAUSolved;

        public double[] Solve(double[,] matrix, double[] vector)
        {
            int n = vector.Length;
            double[] x = new double[n];
            double tolerance = 1e-6;

            while (true)
            {
                double[] r = Subtract(vector, Multiply(matrix, x)); // r = b - Ax
                double rr = Dot(r, r);
                if (Math.Sqrt(rr) < tolerance)
                    break;

                double[] Ar = Multiply(matrix, r);
                double alpha = rr / Dot(r, Ar);

                for (int i = 0; i < n; i++)
                {
                    x[i] += alpha * r[i];
                }
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


