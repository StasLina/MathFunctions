using MathFunctionWPF.SLAU.Events;
using MathFunctionWPF.SLAU.Interfaces;

namespace MathFunctionWPF.SLAU.Models
{
    public class SimpleIter : MethodBase, IMatrixSolver
    {
        public SimpleIter() : base(TypeMethod.SimpleIter) { }

        public event EventHandler SLAUSolved;


        // Метод для решения системы линейных уравнений методом Якоби

        double tolerance = 1e-6;
        int maxIterations = 1000;
        // Метод Якоби
        public double[] Solve(double[,] matrix, double[] vector, EventHandler action = null)
        {
            int n = matrix.GetLength(0); // Размерность системы
            double[] x = new double[n]; // Вектор решений
            double[] prevX = new double[n]; // Вектор предыдущих приближений

            // Инициализация вектора решений начальными значениями (например, нулями)
            for (int i = 0; i < n; i++)
            {
                x[i] = 0;
                prevX[i] = 0;
            }

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                // Процесс итерации для каждого элемента x[i]
                for (int i = 0; i < n; i++)
                {
                    double sum = 0;
                    // Суммируем все элементы A_ij * x_j, где j != i
                    for (int j = 0; j < n; j++)
                    {
                        if (j != i)
                        {
                            sum += matrix[i, j] * prevX[j];
                        }
                    }
                    // Вычисление нового значения x[i]
                    x[i] = (vector[i] - sum) / matrix[i, i];
                }
                action?.Invoke(this, null);

                // Проверка на сходимость: вычисление ошибки как максимального изменения вектора
                double error = 0;
                for (int i = 0; i < n; i++)
                {
                    error = Math.Max(error, Math.Abs(x[i] - prevX[i]));
                }

                // Если погрешность меньше заданного порога, алгоритм завершает работу
                if (error < tolerance)
                {
                    break;
                }
                action?.Invoke(this, null);

                // Копируем новые значения в prevX для следующей итерации
                Array.Copy(x, prevX, n);
            }

            return x; // Возвращаем вектор решений
        }
        
        // Метод Гаусса-Зейделя
        public double[] Solve3(double[,] matrix, double[] vector, EventHandler action = null)
        {
            int n = matrix.GetLength(0); // Размерность системы
            double[] x = new double[n];  // Вектор решений
            double[] prevX = new double[n];  // Вектор предыдущих приближений
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
                            sum += matrix[i, j] * x[j];  // Используем x, а не prevX
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

                // Копируем новые значения в prevX для следующей итерации
                Array.Copy(x, prevX, n);
            }

            return x;  // Возвращаем вектор решений
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


