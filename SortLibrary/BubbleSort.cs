namespace SortLibrary
{
    public class BubbleSort : SortBase
    {
        public void Sort(List<double> data)
        {
            int n = data.Count;
            bool swapped;

            for (int i = 0; i < n - 1; i++)
            {
                swapped = false;

                // Проходим по массиву, и находим элементы, которые нужно обменять
                for (int j = 0; j < n - i - 1; j++)
                {
                    IncrementInstructionCount(); // Подсчёт инструкции для сравнения
                    if (data[j] > data[j + 1])
                    {
                        // Обмен элементов
                        double temp = data[j];
                        data[j] = data[j + 1];
                        data[j + 1] = temp;
                        swapped = true;
                    }
                }

                // Если в этом проходе не было обменов, то массив уже отсортирован
                if (!swapped)
                    break;
            }
        }

        public override void Sort(List<double> data, bool ascending)
        {
            int n = data.Count;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    IncrementInstructionCount();

                    bool condition = ascending ? data[j] > data[j + 1] : data[j] < data[j + 1];

                    if (condition)
                    {
                        // Обмен элементов
                        double temp = data[j];
                        data[j] = data[j + 1];
                        data[j + 1] = temp;
                    }
                }
            }
        }



        public override async Task SortAsync(List<double> data, bool ascending, CancellationToken cancellationToken)
        {
            try
            {

                int n = data.Count;
                for (int i = 0; i < n - 1; i++)
                {
                    for (int j = 0; j < n - i - 1; j++)
                    {

                        bool condition = ascending ? data[j] > data[j + 1] : data[j] < data[j + 1];

                        if (condition)
                        {
                            // Обмен элементов
                            double temp = data[j];
                            data[j] = data[j + 1];
                            data[j + 1] = temp;

                            await IncrementInstructionCount();

                        }
                    }
                }

            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Сортировка была отменена.");
            }
        }

        public async Task QuickSortHelperAsync(List<double> data, int low, int high, bool ascending, CancellationToken cancellationToken)
        {
            int n = data.Count;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    await IncrementInstructionCount();

                    bool condition = ascending ? data[j] > data[j + 1] : data[j] < data[j + 1];

                    if (condition)
                    {
                        // Обмен элементов
                        double temp = data[j];
                        data[j] = data[j + 1];
                        data[j + 1] = temp;
                    }
                }
            }
        }




    }


}
