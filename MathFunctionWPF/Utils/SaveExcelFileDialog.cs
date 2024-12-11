using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathFunctionWPF.Utils
{
    internal class SaveExcelFileDialog
    {   
        public bool Show(out string pathToSave)
        {
            var saveFileDialog = new SaveFileDialog()
            {
                Filter = "JSON Files (*.xlsx)|*.xlsx",//|All Files (*.*)|*.*", // Фильтр для файлов JSON
                Title = "Сохранить файл как JSON",
                DefaultExt = "xlsx", // Расширение по умолчанию
                FileName = "matrix" // Имя файла по умолчанию
            };

            if (saveFileDialog.ShowDialog() == true)
            {

                //File.WriteAllText(saveFileDialog.FileName, content);
                pathToSave = saveFileDialog.FileName;
                return true;
            }

            pathToSave = null;
            return false;
        }
    }
}
