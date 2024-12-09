using System.Windows;

namespace SortLibrary
{
    public class Bogosort : SortBase
    {
        // Свойство, которое определяет порядок сортировки: по возрастанию (true) или по убыванию (false)

        public void Sort(List<double> data)
        {
            Random rand = new Random();
            InstructionCount = 0;  // Сбрасываем счетчик перед началом сортировки

            // Продолжаем случайно перемешивать элементы, пока они не окажутся отсортированными
            while (!IsSorted(data))
            {
                Shuffle(data, rand);  // Перемешиваем элементы
                IncrementInstructionCount();  // Увеличиваем счетчик инструкций за перемешивание
                if (InstructionCount > 5000)
                {
                    MessageBox.Show("Количество итераций больше 5000 сортировка завершено досрочна");
                    break;
                }
            }

        }

        // Метод для выполнения сортировки
        public override void Sort(List<double> data, bool isAscending)
        {
            Random rand = new Random();
            InstructionCount = 0;  // Сбрасываем счетчик перед началом сортировки

            // Продолжаем случайно перемешивать элементы, пока они не окажутся отсортированными
            while (!IsSorted(data, isAscending))
            {
                Shuffle(data, rand);  // Перемешиваем элементы
                IncrementInstructionCount();  // Увеличиваем счетчик инструкций за перемешивание

                if (InstructionCount > 5000)
                {
                    MessageBox.Show("Количество итераций больше 5000 сортировка завершено досрочна");
                    break;
                }
            }
        }

        public override async Task SortAsync(List<double> data, bool ascending, CancellationToken cancellationToken)
        {
            int InstructionCountMax = 200;
            try
            {
                Random rand = new Random();
                InstructionCount = 0;  // Сбрасываем счетчик перед началом сортировки

                // Продолжаем случайно перемешивать элементы, пока они не окажутся отсортированными
                while (!IsSorted(data, ascending))
                {
                    Shuffle(data, rand);  // Перемешиваем элементы
                    await IncrementInstructionCount();  // Увеличиваем счетчик инструкций за перемешивание

                    if (InstructionCount > InstructionCountMax)
                    {
                        MessageBox.Show($"Количество итераций больше {InstructionCountMax} сортировка завершено досрочна");
                        break;
                    }
                }
                // Сортировка с учетом отмены
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Сортировка была отменена.");
            }
        }


        // Метод для проверки, отсортирован ли список
        private bool IsSorted(List<double> data, bool isAscending = true)
        {
            for (int i = 1; i < data.Count; i++)
            {
                IncrementInstructionCount();  // Подсчитываем инструкции на каждой проверке
                if ((isAscending && data[i - 1] > data[i]) || (!isAscending && data[i - 1] < data[i]))
                {
                    // Если элементы не отсортированы в нужном порядке
                    return false;
                }
            }
            return true;
        }

        // Метод для случайного перемешивания элементов
        private void Shuffle(List<double> data, Random rand)
        {
            int n = data.Count;
            for (int i = 0; i < n; i++)
            {
                int j = rand.Next(i, n);  // Генерируем случайный индекс для обмена
                double temp = data[i];
                data[i] = data[j];
                data[j] = temp;
            }
        }
    }


}
