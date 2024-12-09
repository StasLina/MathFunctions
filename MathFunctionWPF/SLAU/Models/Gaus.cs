using MathFunctionWPF.SLAU.Interfaces;

namespace MathFunctionWPF.SLAU.Models
{
    public class Gaus : MethodBase, IMatrixSolver
    {
        public Gaus() : base(TypeMethod.Gaus)
        {
        }

        public event EventHandler SLAUSolved;

        public double[] Solve(double[,] matrix, double[] vector)
        {
            int n = matrix.GetLength(0);

            // Создаем расширенную матрицу
            double[,] augmentedMatrix = new double[n, n + 1];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    augmentedMatrix[i, j] = matrix[i, j];
                }
                augmentedMatrix[i, n] = vector[i];
            }

            // Прямой ход: Приведение к треугольному виду
            for (int i = 0; i < n; i++)
            {
                // Нормализация текущей строки
                double pivot = augmentedMatrix[i, i];
                if (Math.Abs(pivot) < 1e-10)
                    throw new InvalidOperationException("Матрица вырожденная или плохо обусловлена.");

                for (int j = i; j <= n; j++)
                {
                    augmentedMatrix[i, j] /= pivot;
                }

                // Обнуление элементов ниже текущей строки
                for (int k = i + 1; k < n; k++)
                {
                    double factor = augmentedMatrix[k, i];
                    for (int j = i; j <= n; j++)
                    {
                        augmentedMatrix[k, j] -= factor * augmentedMatrix[i, j];
                    }
                }
            }

            // Обратный ход: Нахождение решений
            double[] result = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                result[i] = augmentedMatrix[i, n];
                for (int j = i + 1; j < n; j++)
                {
                    result[i] -= augmentedMatrix[i, j] * result[j];
                }
            }

            SLAUSolved?.Invoke(this, new EventArgs());
            return result;
        }
    }

}


