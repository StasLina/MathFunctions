using MathFunctionWPF.SLAU.Events;
using MathFunctionWPF.SLAU.Interfaces;

namespace MathFunctionWPF.SLAU.Models
{
    public class Gaus : MethodBase, IMatrixSolver
    {
        public Gaus() : base(TypeMethod.Gaus)
        {
        }

        public event EventHandler SLAUSolved;

        public double[] Solve(double[,] matrix, double[] vector, EventHandler action = null)
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

                action?.Invoke(this, null);
            }

            // Прямой ход: Приведение к треугольному виду
            for (int row = 0; row < n; row++)
            {
                // Нормализация текущей строки
                double pivot = augmentedMatrix[row, row];
                if (Math.Abs(pivot) < 1e-10)
                    throw new InvalidOperationException("Матрица вырожденная или плохо обусловлена.");

                // Делим всю строку на одно и тоже число
                for (int column = row; column <= n; column++)
                {
                    augmentedMatrix[row, column] /= pivot;
                }
                // pivot элемент 1 или -1

                // Обнуление элементов ниже текущей строки
                for (int ceilRow = row + 1; ceilRow < n; ceilRow++)
                {
                    // Все строки ниже текущией
                    double factor = augmentedMatrix[ceilRow, row];
                    // factor - диагональный элемент матрицы столбца
                    for (int ceilColumn = row; ceilColumn <= n; ceilColumn++)
                    {
                        // Начинает столбцы со столбца дигонального элемента
                        augmentedMatrix[ceilRow, ceilColumn] -= // Для каждого из жлементов отнимаем
                            
                            factor * augmentedMatrix[row, ceilColumn];
                        
                    }
                }
                action?.Invoke(this, null);
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
                action?.Invoke(this, null);
            }

            SLAUSolved?.Invoke(this, new Results() { Result = result });
            return result;
        }

        public async Task<double[]> SolveAsync(double[,] matrix, double[] vector, CancellationToken token)
        {

            return await Task.Run(() =>
            {
                double[] result = new double[] { 0 };
                DateTime time = DateTime.Now;
                try
                {
                    result = Solve(matrix, vector, (obk, args) =>
                   {
                       if (this.Result != null)
                       {
                           this.Result.Time = (DateTime.Now - time).Seconds;
                           //Thread.Sleep(1000);
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
            return await Task.Run(() =>
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

            return result;
        });
        }

    }
}


