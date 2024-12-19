using MathFunctionWPF.SLAU.Controls;
using MathFunctionWPF.SLAU.ViewModels;
using MathFunctionWPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using MathFunctionWPF.MNK.views;
using MathFunctionWPF.MNK.viewmodels;
using OxyPlot;
using OxyPlot.Series;
using MathTableMatrix;
using Microsoft.Office.Interop.Excel;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using System.Runtime.InteropServices;
using System.Windows;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows.Documents;

namespace MathFunctionWPF.Controllers
{
    class MNKController : IBaseController
    {
        private TypeMathMethod _method;

        MKKMainControl _view = null;

        MKKMainControlModel _model;

        public Control View => _view;

        DynamicTableControl dynamicTable;
        DynamicTableController dynamicTableController;

        public MNKController(MKKMainControl view)
        {
            _view = view;
            _model = new MKKMainControlModel();

            _view.BCalc.Click += BCalc_Click;
            _view.BDraw.Click += BDraw_Click;
            _view.BResize.Click += BResize_Click;
            _view.BRand.Click += BRand_Click;

            _view.BSaveExcel.Click += BSaveExcel_Click;
            _view.BLoadExcel.Click += BLoadExcel_Click;


            _view.BSaveJson.Click += BSaveJson_Click;
            _view.BLoadJson.Click += BLoadJson_Click; ;

            //_view.B.Click += BRand_Click; ;

            dynamicTable = new MathTableMatrix.DynamicTableControl();
            dynamicTableController = new MathTableMatrix.DynamicTableController(dynamicTable);
            dynamicTableController.InitializeDynamicTable(0, 0);
            _method = new TypeMathMethod();

            _model.InputView = dynamicTable;
            _model.ListInputControl = new System.Collections.ObjectModel.ObservableCollection<MethodNameList>()
            {
                new MethodNameList("RowCount")
                {
                    TypeTitle = "Количество значений",
                    Value = "12",
                }
            };
            Reset();
        }

        private void BLoadJson_Click(object sender, RoutedEventArgs e)
        {
            var save = new Utils.OpenJsonFileDialog();
            if (save.Show(out var pathToLoad))
            {
                string content = File.ReadAllText(pathToLoad);
                try
                {
                    List<List<double>> dataList= Newtonsoft.Json.JsonConvert.DeserializeObject<List < List<double> >> (content);

                    double[,] data = new double[dataList.Count, dataList[0].Count];

                    for (int i = 0; i < dataList.Count; i++)
                    {
                        for (int j = 0; j < dataList[0].Count; j++)
                        {
                            data[i,j] = dataList[i][j];
                        }
                    }


                    dynamicTableController.SetData(data);

                    if (data.GetLength(1) == 2)
                    {
                        dynamicTableController.SetData(data);
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private void BSaveJson_Click(object sender, RoutedEventArgs e)
        {
            var save = new Utils.SaveJsonFileDialog();
            if (save.Show(out var pathToSave))
            {
                var array = (dynamicTableController.GetData());
                List<List<double>> list = new List<List<double>>();
                for (int i = 0; i < array.GetLength(0); i++)
                {
                    List<double> row = new List<double>();
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        row.Add(array[i, j]);
                    }
                    list.Add(row);
                }

                string contrnet = Newtonsoft.Json.JsonConvert.SerializeObject(array);
                File.WriteAllText(pathToSave, contrnet);
            }
        }

        private void BLoadExcel_Click(object sender, RoutedEventArgs e)
        {
            var save = new Utils.OpenExcelFileDialog();
            if (save.Show(out var pathToLoad))
            {
                LoadExcelFile(pathToLoad);
            }
        }

        private void BSaveExcel_Click(object sender, RoutedEventArgs e)
        {
            var save = new Utils.SaveExcelFileDialog();
            if (save.Show(out var pathToSave))
            {
                SaveExcelFile(pathToSave, dynamicTableController.GetData());
            }
        }

        private void BRand_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Random rand = new Random();

            int rows = GetRowCount();

            if (rows > 0)
            {
                double[,] data = new double[rows, 2];

                for (int idxRow = 0; idxRow < rows; idxRow++)
                {
                    data[idxRow, 1] = rand.NextDouble() * 20 - 10;
                    data[idxRow, 0] = rand.NextDouble() * 100 - 50;
                }

                dynamicTableController.SetData(data);
            }

        }

        private string GetValue(string key)
        {
            foreach (var item in _model.ListInputControl)
            {
                if (item.Key == key)
                {
                    return item.Value;
                }
            }
            return "";
        }

        private int GetRowCount()
        {
            string value = GetValue("RowCount");

            if (int.TryParse(value, out var rowCount))
            {
                if (rowCount > 0)
                    return rowCount;
            }
            return 0;
        }

        private void BResize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            dynamicTableController.InitializeDynamicTable(GetRowCount(), 2);
        }

        class DotaSeriesItem
        {
            public double X { get; set; } = 0;
            public double Y { get; set; } = 0;
        }

        private List<DotaSeriesItem> SourceData()
        {
            List<DotaSeriesItem> dotaSeriesses = new List<DotaSeriesItem>();
            var data = dynamicTableController.GetData();


            for (int rowIdx = 0, rowIdxMax = data.GetLength(0); rowIdx < rowIdxMax; ++rowIdx)
            {
                dotaSeriesses.Add(new DotaSeriesItem()
                {
                    X = data[rowIdx, 0],
                    Y = data[rowIdx, 1],
                });
            }
            return dotaSeriesses;
        }

        class FunCalc
        {
            double[] _args;

            public FunCalc(double[] args)
            {
                _args = args;
            }

            public double Calc(double arg)
            {
                double result = 0;

                for (int idx = 0, idxMax = _args.Length; idx < idxMax; ++idx)
                {
                    result += _args[idx] * Math.Pow(arg, idx);
                }

                return result;
            }
        }

        double[] GetMinMax(double[] data)
        {
            double min = double.MaxValue, max = -double.MaxValue;

            foreach (var item in data)
            {
                if (item > max)
                {
                    max = item;
                }

                if (item < min)
                {
                    min = item;
                }
            }
            return new double[] { min, max };
        }
        private void BDraw_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            List<DotaSeriesItem> list = SourceData();
            var pm = new PlotModel
            {
                Title = "МНК",
                Subtitle = "",
                PlotType = PlotType.Cartesian,
                Background = OxyColors.White
            };


            var dots = new LineSeries()
            {
                Title = "SourceData",
                ItemsSource = list,
                DataFieldX = "X",
                DataFieldY = "Y",
                StrokeThickness = 0,
                MarkerSize = 3,
                MarkerStroke = OxyColors.ForestGreen,
                MarkerType = MarkerType.Plus
            };


            var dataColumnX = dynamicTableController.GetDataColumn(0);
            if (dataColumnX.Length > 0)
            {
                var dataColumnY = dynamicTableController.GetDataColumn(1);


                double[] args = MNK.models.LeastSquares.Calculate(dataColumnX, dataColumnY, 1);
                double[] args2 = MNK.models.LeastSquares.Calculate(dataColumnX, dataColumnY, 2);
                FunCalc funCalc = new FunCalc(args);
                FunCalc funCalc2 = new FunCalc(args2);

                _model.Func1Result = GetFunction(args);
                _model.Func2Result = GetFunction(args2);

                var minMax = GetMinMax(dataColumnX);
                var lines = new FunctionSeries(funCalc.Calc, minMax[0], minMax[1], 0.1);
                pm.Series.Add(lines);

                var lines2 = new FunctionSeries(funCalc2.Calc, minMax[0], minMax[1], 0.1);
                pm.Series.Add(lines2);

                var annotationY = new LineAnnotation
                {
                    Type = LineAnnotationType.Horizontal,
                    Y = 0
                };

                var annotationX = new LineAnnotation
                {
                    Type = LineAnnotationType.Vertical,
                    X = 0
                };


                pm.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Title = "Ось X",
                    Minimum = minMax[0], // Минимальное значение по оси X
                    Maximum = minMax[1]// Максимальное значение по оси X
                });
                pm.Annotations.Add(annotationX);
                pm.Annotations.Add(annotationY);
            }

            pm.Series.Add(dots);

            _view.Drawing.Model = pm;
        }

        private void BCalc_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dataColumnX = dynamicTableController.GetDataColumn(0);
            if (dataColumnX.Length > 0)
            {
                var dataColumnY = dynamicTableController.GetDataColumn(1);

                double[] args = MNK.models.LeastSquares.Calculate(dataColumnX, dataColumnY, 1);
                double[] args2 = MNK.models.LeastSquares.Calculate(dataColumnX, dataColumnY, 2);

                _model.Func1Result = GetFunction(args);
                _model.Func2Result = GetFunction(args2);
            }
        }

        string GetFunction(double[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("F(x)=");

            double arg = args[0];

            sb.Append(arg.ToString("F2"));

            for (int idx = 1; idx < args.Length; idx++)
            {
                arg = args[idx];
                if (arg == 0)
                {
                    continue;
                }

                if (arg > 0)
                {
                    sb.Append('+');
                }

                sb.Append(arg.ToString("F2"));
                sb.Append($"*x^{idx}");
            }

            return sb.ToString();
        }

        void Reset()
        {
            _model.Func1Result = "Не определено";
            _model.Func2Result = "Не определено";
        }

        public void MethodChanged(TypeMathMethod newMethod)
        {
            switch (newMethod)
            {
                case TypeMathMethod.MNK:
                    {
                        _method = TypeMathMethod.MNK;
                        _view.DataContext = _model;
                    }
                    break;
            }
        }


        void SaveExcelFile(string filePath, double[,] arrValues)
        {
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            Workbook workbook = null;
            Worksheet worksheet = null;
            List<List<string>>? tableData = null;
            bool isNewFile = false;
            try
            {
                // Открываем книгу Excel
                if (File.Exists(filePath))
                {
                    workbook = excelApp.Workbooks.Open(filePath);
                }
                else
                {
                    workbook = excelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                    isNewFile = true;
                }
                worksheet = workbook.Sheets[1]; // Выбираем первый лист

                // Очищаем старые данные
                if (isNewFile == false)
                {
                    // Получаем диапазон всех используемых ячеек на листе
                    Microsoft.Office.Interop.Excel.Range usedRange = worksheet.UsedRange;

                    // Очищаем форматирование и содержимое всех ячеек
                    usedRange.Clear();
                }

                Microsoft.Office.Interop.Excel.Range range = worksheet.UsedRange; // Диапазон используемых ячеек

                for (int idxRow = 0, idxRowEnd = arrValues.GetLength(0); idxRow < idxRowEnd; idxRow++)
                {
                    for (int idxColumn = 0, idxColumnEnd = arrValues.GetLength(1); idxColumn < idxColumnEnd; ++idxColumn)
                    {
                        range.Cells[idxRow + 1, idxColumn + 1].Value = arrValues[idxRow, idxColumn];
                    }
                }

                if (isNewFile)
                {
                    workbook.SaveAs(filePath);
                }
                else
                {
                    workbook.Save();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Закрытие книги и приложения Excel
                if (workbook != null)
                {
                    workbook.Close();
                    Marshal.ReleaseComObject(workbook);
                }

                if (excelApp != null)
                {
                    excelApp.Quit();
                    Marshal.ReleaseComObject(excelApp);
                }
            }
        }

        void LoadExcelFile(string filePath)
        {

            // Создаем приложение Excel
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            Workbook workbook = null;
            Worksheet worksheet = null;
            List<List<string>>? tableData = null;

            int maxColumn = 0;
            List<List<double>>? doublesMatrix = null;
            List<double>? doublesVector = null;
            try
            {
                // Открываем книгу Excel
                workbook = excelApp.Workbooks.Open(filePath);
                worksheet = workbook.Sheets[1]; // Выбираем первый лист

                Microsoft.Office.Interop.Excel.Range range = worksheet.UsedRange; // Диапазон используемых ячеек
                int row = 1; // Начинаем с первой строки
                int column = 1; // Начинаем с первой строки

                int maxColumnText = 1;
                // Массив для хранения данных столбца A
                tableData = new List<List<string>>();

                // Чтение значений из столбца A до первой пустой строки
                while (range.Cells[row, column].Value != null)
                {
                    List<string> values = new List<string>();

                    while (range.Cells[row, column].Value != null)
                    {
                        values.Add(range.Cells[row, column].Value.ToString()); // Добавляем значение в список
                        ++column;
                    }

                    if (maxColumnText < column)
                    {
                        maxColumnText = column;
                    }
                    tableData.Add(values);
                    ++row;
                    column = 1;
                }


                // Показать загруженные данные
                //string result = string.Join("\n", tableData);
                //MessageBox.Show($"Загруженные данные:\n{result}", "Данные из Excel", MessageBoxButton.OK, MessageBoxImage.Information);


                if (tableData != null)
                {
                    doublesMatrix = new List<List<double>>();
                    doublesVector = new List<double>();

                    bool needBreak = false;

                    for (int rowIdx = 0; rowIdx < tableData.Count; rowIdx++)
                    {
                        List<string>? RowData = tableData[rowIdx];
                        List<double> values = new List<double>();
                        int columnIdx = 0;

                        for (columnIdx = 0; columnIdx < RowData.Count; columnIdx++)
                        {
                            string? text = RowData[columnIdx];
                            if (double.TryParse(text, out double value) == true)
                            {
                                values.Add(value);
                            }
                            else
                            {
                                if (columnIdx == 0)
                                {
                                    needBreak = true;
                                }
                                break;
                            }
                        }

                        // Запоминаем максимальный тсобец
                        if (columnIdx > maxColumn)
                        {
                            maxColumn = columnIdx;
                        }

                        if (needBreak)
                        {
                            break;
                        }
                        else
                        {
                            doublesMatrix.Add(values);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Закрытие книги и приложения Excel
                if (workbook != null)
                {
                    workbook.Close(false);
                    Marshal.ReleaseComObject(workbook);
                }

                if (excelApp != null)
                {
                    excelApp.Quit();
                    Marshal.ReleaseComObject(excelApp);
                }
            }

            // Заполняем недостоющие столбцы нулями
            if (maxColumn > 0)
            {
                var resultLoadMatrix = new double[doublesMatrix.Count, 2];

                for (int rowIdx = 0; rowIdx < doublesMatrix.Count; ++rowIdx)
                {
                    int columnIdx = 0;
                    for (; columnIdx < doublesMatrix[rowIdx].Count && columnIdx < resultLoadMatrix.GetLength(1); ++columnIdx)
                    {
                        resultLoadMatrix[rowIdx, columnIdx] = doublesMatrix[rowIdx][columnIdx];
                    }
                }

                dynamicTableController.SetData(resultLoadMatrix);
            }
            //} // if (tableData != null) end

        }
    }
}
