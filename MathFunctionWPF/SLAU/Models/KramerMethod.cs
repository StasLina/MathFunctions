using MathFunctionWPF.SLAU.Interfaces;

namespace MathFunctionWPF.SLAU.Models
{
    class KramerMethod : MethodBase, IMatrixSolver
    {
        public KramerMethod() : base(TypeMethod.Kramera)
        {

        }

        public event EventHandler SLAUSolved;

        // Метод для вычисления определителя матрицы
        public double Determinant(double[,] matrix)
        {
            int n = matrix.GetLength(0); // размерность матрицы (предполагаем квадратную матрицу)
            double det = 0;

            if (n == 1)
            {
                return matrix[0, 0];
            }

            if (n == 2)
            {
                return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
            }

            // Для больших матриц используем разложение по строкам или столбцам
            for (int i = 0; i < n; i++)
            {
                double[,] subMatrix = GetSubMatrix(matrix, 0, i);
                det += matrix[0, i] * Math.Pow(-1, i) * Determinant(subMatrix);
            }

            return det;
        }

        // Метод для получения подматрицы после удаления строки и столбца
        public double[,] GetSubMatrix(double[,] matrix, int row, int col)
        {
            int n = matrix.GetLength(0);
            double[,] subMatrix = new double[n - 1, n - 1];
            int subRow = 0, subCol;

            for (int i = 0; i < n; i++)
            {
                if (i == row) continue;
                subCol = 0;

                for (int j = 0; j < n; j++)
                {
                    if (j == col) continue;
                    subMatrix[subRow, subCol] = matrix[i, j];
                    subCol++;
                }
                subRow++;
            }

            return subMatrix;
        }

        // Метод для решения системы линейных уравнений методом Крамера

        public double[] Solve(double[,] A, double[] B, EventHandler action = null)
        {
            // n - количество строк
            int n = A.GetLength(0); 
            double detA = Determinant(A);

            if (detA == 0)
            {
                throw new InvalidOperationException("Определитель матрицы равен нулю, система не имеет уникального решения.");
            }

            double[] result = new double[n];

            // Для каждого неизвестного x_i
            for (int row = 0; row < n; row++)
            {
                double[,] Ai = (double[,])A.Clone();  // Копируем матрицу A
                for (int column = 0; column < n; column++)
                {
                    Ai[column, row] = B[column];  // Заменяем i-й столбец на вектор B
                }

                // Решение для x_i
                result[row] = Determinant(Ai) / detA;

                action?.Invoke(this, null);
            }

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
        }
    }
}


