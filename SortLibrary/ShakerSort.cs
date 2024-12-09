namespace SortLibrary
{
    public class ShakerSort : SortBase
    {
        public void Sort(List<double> data)
        {
            int left = 0;
            int right = data.Count - 1;
            bool swapped = true;

            while (left < right && swapped)
            {
                swapped = false;

                // Проходим слева направо
                for (int i = left; i < right; i++)
                {
                    IncrementInstructionCount(); // Подсчёт инструкции для сравнения
                    if (data[i] > data[i + 1])
                    {
                        // Обмен элементов
                        double temp = data[i];
                        data[i] = data[i + 1];
                        data[i + 1] = temp;
                        swapped = true;
                    }
                }

                // Уменьшаем правую границу
                right--;

                // Если элементы были перемещены, делаем обратный проход справа налево
                if (swapped)
                {
                    for (int i = right; i > left; i--)
                    {
                        IncrementInstructionCount(); // Подсчёт инструкции для сравнения
                        if (data[i] < data[i - 1])
                        {
                            // Обмен элементов
                            double temp = data[i];
                            data[i] = data[i - 1];
                            data[i - 1] = temp;
                            swapped = true;
                        }
                    }
                    // Увеличиваем левую границу
                    left++;
                }
            }
        }

        public override void Sort(List<double> data, bool ascending)
        {
            int left = 0;
            int right = data.Count - 1;
            bool swapped = true;

            while (left < right && swapped)
            {
                swapped = false;

                // Проход слева направо
                for (int i = left; i < right; i++)
                {
                    IncrementInstructionCount(); // Подсчёт инструкции для сравнения
                    if ((ascending && data[i] > data[i + 1]) || (!ascending && data[i] < data[i + 1]))
                    {
                        // Обмен элементов
                        double temp = data[i];
                        data[i] = data[i + 1];
                        data[i + 1] = temp;
                        swapped = true;
                    }
                }

                // Уменьшаем правую границу
                right--;

                // Если элементы были перемещены, делаем обратный проход справа налево
                if (swapped)
                {
                    for (int i = right; i > left; i--)
                    {
                        IncrementInstructionCount(); // Подсчёт инструкции для сравнения
                        if ((ascending && data[i] < data[i - 1]) || (!ascending && data[i] > data[i - 1]))
                        {
                            // Обмен элементов
                            double temp = data[i];
                            data[i] = data[i - 1];
                            data[i - 1] = temp;
                            swapped = true;
                        }
                    }
                    // Увеличиваем левую границу
                    left++;
                }
            }
        }



        public override async Task SortAsync(List<double> data, bool ascending, CancellationToken cancellationToken)
        {
            int left = 0;
            int right = data.Count - 1;
            bool swapped = true;

            while (left < right && swapped)
            {
                swapped = false;

                // Проход слева направо
                for (int i = left; i < right; i++)
                {
                    await IncrementInstructionCount(); // Подсчёт инструкции для сравнения
                    if ((ascending && data[i] > data[i + 1]) || (!ascending && data[i] < data[i + 1]))
                    {
                        // Обмен элементов
                        double temp = data[i];
                        data[i] = data[i + 1];
                        data[i + 1] = temp;
                        swapped = true;
                    }
                }

                // Уменьшаем правую границу
                right--;

                // Если элементы были перемещены, делаем обратный проход справа налево
                if (swapped)
                {
                    for (int i = right; i > left; i--)
                    {
                        await IncrementInstructionCount(); // Подсчёт инструкции для сравнения
                        if ((ascending && data[i] < data[i - 1]) || (!ascending && data[i] > data[i - 1]))
                        {
                            // Обмен элементов
                            double temp = data[i];
                            data[i] = data[i - 1];
                            data[i - 1] = temp;
                            swapped = true;
                        }
                    }
                    // Увеличиваем левую границу
                    left++;
                }
            }
        }


    }


}
