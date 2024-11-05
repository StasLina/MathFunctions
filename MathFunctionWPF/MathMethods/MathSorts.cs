using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathFunctionWPF.MathMethods
{
    internal class MathSorts
    {
        // Пузырьковая сортировка
        public static void BubbleSort(int[] array)
        {
            int n = array.Length;

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (array[j] > array[j + 1])
                    {
                        int temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                    }
                }
            }
        }

        // Сортировка вставками

        public static void InsertionSort(int[] array)
        {
            int n = array.Length;

            for (int i = 1; i < n; ++i)
            {
                int key = array[i];
                int j = i - 1;

                while (j >= 0 && array[j] > key)
                {
                    array[j + 1] = array[j];
                    j--;
                }

                array[j + 1] = key;
            }
        }

        // Шэйкерная сортировка

        public static void ShakerSort(int[] array)
        {
            int left = 0;
            int right = array.Length - 1;

            while (left <= right)
            {
                for (int i = left; i < right; i++)
                {
                    if (array[i] > array[i + 1])
                    {
                        int temp = array[i];
                        array[i] = array[i + 1];
                        array[i + 1] = temp;
                    }
                }

                --right;

                for (int i = right; i > left; --i)
                {
                    if (array[i] < array[i - 1])
                    {
                        int temp = array[i];
                        array[i] = array[i - 1];
                        array[i - 1] = temp;
                    }
                }

                ++left;
            }
        }

        // Быстрая ортировка
        public static void QuickSort(int[] array, int low, int high)
        {
            if (low < high)
            {
                // Разделяем массив
                int pivotIndex = Partition(array, low, high);

                // Рекурсивно сортируем левую и правую части массива
                QuickSort(array, low, pivotIndex - 1);
                QuickSort(array, pivotIndex + 1, high);
            }
        }

        private static int Partition(int[] array, int low, int high)
        {
            // Выбираем последний элемент в качестве опорного
            int pivot = array[high];

            // Индекс для меньших элементов
            int i = (low - 1);

            // Перебираем все элементы от low до high-1
            for (int j = low; j < high; j++)
            {
                if (array[j] <= pivot)
                {
                    ++i;

                    int temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }

            // Помещаем опорный элемент на правильную позицию
            int temp2 = array[i + 1];
            array[i + 1] = array[high];
            array[high] = temp2;

            return i + 1;
        }

        // Болотная сортировка
        public static void BogoSort(int[] array)
        {
            Random random = new Random();

            while (!IsSorted(array))
            {
                // Перемешиваем массив случайным образом
                for (int i = 0; i < array.Length; ++i)
                {
                    int index = random.Next(i, array.Length);
                    Swap(ref array[i], ref array[index]);
                }
            }
        }

        // Проверка, отсортирован ли массив
        public static bool IsSorted(int[] array)
        {
            for (int i = 1; i < array.Length; ++i)
            {
                if (array[i - 1] > array[i])
                {
                    return false;
                }
            }

            return true;
        }

        // Обмен значений двух переменных
        private static void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }
    }
}
