namespace SortLibrary
{
    public class InsertionSort : SortBase
    {
        public void Sort(List<double> data)
        {
            int n = data.Count;
            for (int i = 1; i < n; i++)
            {
                double key = data[i];
                int j = i - 1;

                // Перемещаем элементы массива, которые больше ключа
                while (j >= 0 && data[j] > key)
                {
                    IncrementInstructionCount(); // Подсчёт инструкции для сравнения
                    data[j + 1] = data[j];
                    j--;
                }

                // Вставляем ключ на нужную позицию
                data[j + 1] = key;
                IncrementInstructionCount(); // Подсчёт инструкции для присваивания
            }
        }

        public override void Sort(List<double> data, bool ascending)
        {
            int n = data.Count;

            for (int i = 1; i < n; i++)
            {
                double key = data[i];
                int j = i - 1;

                // Сортировка по возрастанию или убыванию в зависимости от флага ascending
                while (j >= 0 && (ascending ? data[j] > key : data[j] < key))
                {
                    IncrementInstructionCount();
                    data[j + 1] = data[j];
                    j--;
                }

                data[j + 1] = key; // Вставка ключа на правильную позицию
                IncrementInstructionCount();
            }
        }


        public override async Task SortAsync(List<double> data, bool ascending, CancellationToken cancellationToken)
        {
            int n = data.Count;

            for (int i = 1; i < n; i++)
            {
                double key = data[i];
                int j = i - 1;

                // Сортировка по возрастанию или убыванию в зависимости от флага ascending
                while (j >= 0 && (ascending ? data[j] > key : data[j] < key))
                {
                    data[j + 1] = data[j];
                    await IncrementInstructionCount();
                    j--;
                }

                data[j + 1] = key; // Вставка ключа на правильную позицию
                await IncrementInstructionCount();
            }
        }

    }


}
