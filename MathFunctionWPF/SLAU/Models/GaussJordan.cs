using MathFunctionWPF.SLAU.Events;
using MathFunctionWPF.SLAU.Interfaces;

namespace MathFunctionWPF.SLAU.Models
{
    class GaussJordan : MethodBase, IMatrixSolver
    {
        public GaussJordan() : base(TypeMethod.GaussJordan) { }

        public event EventHandler SLAUSolved;

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
        }

        public double[] Solve(double[,] A, double[] b, EventHandler action = null)
        {
            int n = A.GetLength(0);
            double[,] augmentedMatrix = new double[n, n + 1];

            // Создаем расширенную матрицу [A | b]
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    augmentedMatrix[i, j] = A[i, j];
                }
                augmentedMatrix[i, n] = b[i];
            }
            action?.Invoke(this, null);


            // Применение метода Гаусса-Жордана
            for (int i = 0; i < n; i++)
            {
                // Находим максимальный элемент в текущем столбце
                int maxRow = i;
                for (int j = i + 1; j < n; j++)
                {
                    if (Math.Abs(augmentedMatrix[j, i]) > Math.Abs(augmentedMatrix[maxRow, i]))
                    {
                        maxRow = j;
                    }
                }

                // Меняем текущую строку с строкой с максимальным элементом
                for (int j = 0; j <= n; j++)
                {
                    double temp = augmentedMatrix[i, j];
                    augmentedMatrix[i, j] = augmentedMatrix[maxRow, j];
                    augmentedMatrix[maxRow, j] = temp;
                }

                // Нормализуем текущую строку (делим на ведущий элемент)
                double pivot = augmentedMatrix[i, i];
                for (int j = 0; j <= n; j++)
                {
                    augmentedMatrix[i, j] /= pivot;
                }

                // Обнуляем все элементы в текущем столбце, кроме диагонали
                for (int j = 0; j < n; j++)
                {
                    if (j != i)
                    {
                        double factor = augmentedMatrix[j, i];
                        for (int k = 0; k <= n; k++)
                        {
                            augmentedMatrix[j, k] -= factor * augmentedMatrix[i, k];
                        }
                    }
                }
                action?.Invoke(this, null);
            }

            // Извлекаем решение из последней колонки
            double[] solution = new double[n];
            for (int i = 0; i < n; i++)
            {
                solution[i] = augmentedMatrix[i, n];
            }

            SLAUSolved?.Invoke(this, new Results() { Result = solution });

            return solution;
        }
    }
}


