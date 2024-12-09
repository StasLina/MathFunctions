namespace SortLibrary
{
    public class QuickSort : SortBase
    {
        public void Sort(List<double> data)
        {
            QuickSortHelper(data, 0, data.Count - 1);
        }

        public override void Sort(List<double> data, bool asxending)
        {
            QuickSortHelper(data, 0, data.Count - 1, asxending);
        }

        private void QuickSortHelper(List<double> data, int low, int high)
        {
            if (low < high)
            {
                int pi = Partition(data, low, high);

                // Рекурсивный вызов для сортировки двух частей
                QuickSortHelper(data, low, pi - 1);
                QuickSortHelper(data, pi + 1, high);
            }
        }

        private int Partition(List<double> data, int low, int high)
        {
            double pivot = data[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                IncrementInstructionCount(); // Подсчёт инструкции для сравнения
                if (data[j] <= pivot)
                {
                    i++;
                    double temp = data[i];
                    data[i] = data[j];
                    data[j] = temp;
                }
            }

            double swapTemp = data[i + 1];
            data[i + 1] = data[high];
            data[high] = swapTemp;
            IncrementInstructionCount(); // Подсчёт инструкции для присваивания

            return i + 1;
        }


        // Метод сортировки QuickSort с параметром ascending
        public void QuickSortHelper(List<double> data, int low, int high, bool ascending)
        {
            if (low < high)
            {
                int pi = Partition(data, low, high, ascending);

                // Рекурсивные вызовы для сортировки двух частей
                QuickSortHelper(data, low, pi - 1, ascending);
                QuickSortHelper(data, pi + 1, high, ascending);
            }
        }

        // Метод для разделения данных в зависимости от порядка сортировки (ascending или descending)
        private int Partition(List<double> data, int low, int high, bool ascending)
        {
            double pivot = data[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                IncrementInstructionCount(); // Подсчёт инструкций для сравнения
                if ((ascending && data[j] <= pivot) || (!ascending && data[j] >= pivot))
                {
                    i++;
                    double temp = data[i];
                    data[i] = data[j];
                    data[j] = temp;
                }
            }

            // Меняем элементы местами
            double swapTemp = data[i + 1];
            data[i + 1] = data[high];
            data[high] = swapTemp;
            IncrementInstructionCount(); // Подсчёт инструкций для присваивания

            return i + 1;
        }

        // Метод для подсчёта инструкций (например, сравнений или присваиваний)

        /// Асинхронные сортировки
        /// 
        public override async Task SortAsync(List<double> data, bool ascending, CancellationToken cancellationToken)
        {
            try
            {
                // Сортировка с учетом отмены
                await QuickSortHelperAsync(data, 0, data.Count - 1, ascending, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Сортировка была отменена.");
            }
        }

        public async Task QuickSortHelperAsync(List<double> data, int low, int high, bool ascending, CancellationToken cancellationToken)
        {
            if (low < high)
            {
                // Отмена операции
                cancellationToken.ThrowIfCancellationRequested();

                int pi = await PartitionAsync(data, low, high, ascending, cancellationToken);

                // Рекурсивные вызовы для сортировки двух частей
                await QuickSortHelperAsync(data, low, pi - 1, ascending, cancellationToken);
                await QuickSortHelperAsync(data, pi + 1, high, ascending, cancellationToken);
            }
        }

        public async Task<int> PartitionAsync(List<double> data, int low, int high, bool ascending, CancellationToken cancellationToken)
        {
            double pivot = data[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                cancellationToken.ThrowIfCancellationRequested(); // Проверка отмены внутри цикла

                if ((ascending && data[j] <= pivot) || (!ascending && data[j] >= pivot))
                {
                    i++;
                    if (i != j)
                    {
                        double temp = data[i];
                        data[i] = data[j];
                        data[j] = temp;
                        await IncrementInstructionCount(); // Подсчёт инструкций для сравнения
                    }
                }
            }

            ++i;
            // Меняем элементы местами
            if (i != high)
            {
                double swapTemp = data[i];
                data[i] = data[high];
                data[high] = swapTemp;
                await IncrementInstructionCount(); // Подсчёт инструкций для присваивания
            }

            return i;
        }
    }





}
