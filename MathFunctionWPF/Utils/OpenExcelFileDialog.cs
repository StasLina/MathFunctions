using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathFunctionWPF.Utils
{
    internal class OpenExcelFileDialog
    {
        public bool Show(out string result)
        {
            try
            {
                // Создание и конфигурация диалога для выбора файла
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx", // |All Files (*.*)|*.*
                    Title = "Выберите файл .xlsx",
                    Multiselect = false
                };

                // Открытие диалога и проверка, был ли выбран файл
                if (openFileDialog.ShowDialog() == true) // В WPF это возвращает true/false
                {
                    result = openFileDialog.FileName;
                    // Показать путь к выбранному файлу (можно заменить на сохранение в переменную)
                    //MessageBox.Show($"Выбран файл: {openFileDialog.FileName}", "Файл выбран", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            result = null;
            return false;
        }
    }

    internal class OpenJsonFileDialog
    {
        public bool Show(out string result)
        {
            try
            {
                // Создание и конфигурация диалога для выбора файла
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Excel Files (*.json)|*.json", // |All Files (*.*)|*.*
                    Title = "Выберите файл .json",
                    Multiselect = false
                };

                // Открытие диалога и проверка, был ли выбран файл
                if (openFileDialog.ShowDialog() == true) // В WPF это возвращает true/false
                {
                    result = openFileDialog.FileName;
                    // Показать путь к выбранному файлу (можно заменить на сохранение в переменную)
                    //MessageBox.Show($"Выбран файл: {openFileDialog.FileName}", "Файл выбран", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            result = null;
            return false;
        }
    }
}
