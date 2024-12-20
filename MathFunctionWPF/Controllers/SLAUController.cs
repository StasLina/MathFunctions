﻿using MathFunctionWPF.Models;
using MathFunctionWPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using MathFunctionWPF.SLAU.Controls;
using MathFunctionWPF.SLAU.ViewModels;
using MathFunctionWPF.SLAU.Models;
using System.Windows;
using System.ComponentModel;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Data.Common;
using System.Net.WebSockets;
using System.Windows.Media;
using System.IO;
using System.Runtime.CompilerServices;
using MathFunctionWPF.SLAU.Interfaces;
using System.Windows.Threading;




namespace MathFunctionWPF.Controllers
{

    class SLAUController : IBaseController
    {
        private TypeMathMethod _method;

        SLAUMainControl _view = null;

        SLAUMainControlModel _model;

        Gaus gaus = new Gaus() { IsChecked = true };
        GaussJordan gaussJordan = new GaussJordan() { IsChecked = true };
        KramerMethod kramera = new KramerMethod() { IsChecked = true };
        Squre squre = new Squre() { IsChecked = false };
        Progonki progonki = new Progonki() { IsChecked = false };
        SimpleIter simpleIter = new SimpleIter() { IsChecked = false };
        HirestDown hirestDown = new HirestDown() { IsChecked = false };
        ComplexGradient complexGradient = new ComplexGradient() { IsChecked = false };
        ObservableCollection<MethodBase> _dataListMethod;

        MathTableMatrix.DynamicTableController _matrixGrid, _vectorGrid;


        // Используется при загрузке
        double[,]? resultLoadMatrix;
        double[]? resultLoadVector;

        public SLAUController(SLAUMainControl view)
        {
            _view = view;
            _model = new SLAUMainControlModel();

            _dataListMethod = new ObservableCollection<MethodBase>()
            {
                gaus,gaussJordan,kramera, squre, progonki, simpleIter, hirestDown, complexGradient
            };

            _model.ListMethodsControl = _dataListMethod;

            // Создаём таблицы


            _matrixGrid = new MathTableMatrix.DynamicTableController(
            new MathTableMatrix.DynamicTableControl(), new MathTableMatrix.DynamicTableControlModel());
            _matrixGrid.InitializeDynamicTable(_model.Rows, _model.Columns);

            _vectorGrid = new MathTableMatrix.DynamicTableController(
            new MathTableMatrix.DynamicTableControl(), new MathTableMatrix.DynamicTableControlModel());
            _vectorGrid.InitializeDynamicTable(_model.Rows, 1);

            _model.MatrixDataContent = _matrixGrid.View;
            _model.VectorDataContent = _vectorGrid.View;
            _view.DataContext = _model;

            _view.BChangeSize.Click += CreateMatrixButton_Click;
            _view.BFillRand.Click += BFillRand_Click;
            _view.BLoadExcel.Click += BLoadExcel_Click;
            _view.BSaveExcel.Click += BSaveExcel_Click;
            _view.BCalcculcate.Click += BCalcculcate_Click;
        }

        CancellationTokenSource? cancelTokenSource = null;

        struct A
        {
            public MethodBase _method;
            public Task<double[]> _task;
            public A(MethodBase method, Task<double[]> task)
            {
                _method = method; _task = task;
            }
        };

        private async void BCalcculcate_Click(object sender, RoutedEventArgs e)
        {
            if (cancelTokenSource != null)
            {
                cancelTokenSource.Cancel();
            }
            cancelTokenSource = new CancellationTokenSource();

            try
            {
                var resultTableContent = new SLAU.Controls.ResultTableControl();

                ObservableCollection<SLAU.Models.RecordSlauResults> results = new ObservableCollection<RecordSlauResults>();


                List<A> tasks = new List<A>();

                foreach (var sort in (ObservableCollection<MethodBase>)_model.ListMethodsControl)
                {
                    if (sort.IsChecked)
                    {

                        var newRow = new RecordSlauResults();
                        sort.Result = newRow;
                        newRow.Tile = sort.TypeTitle;
                        newRow.Time = 0;
                        results.Add(newRow);


                        if (sort is IMatrixSolver solver)
                        {
                            //var task = solver.SolveAsync(_matrixGrid.GetData(), _vectorGrid.GetDataColumn(), cancelTokenSource.Token);
                            //newRow.ResultMatrix = await task;
                            //bool isInit = false;

                            //var ta = Task.Run(async () =>
                            //{
                            //MethodBase method = sort;
                            //isInit = true;
                            solver.SLAUSolved += SLAUSolved;
                            var task = solver.SolveAsync(_matrixGrid.GetData(), _vectorGrid.GetDataColumn(), cancelTokenSource.Token);
                            tasks.Add(new A(sort, task));
                        }
                    }
                }

                resultTableContent.DataGrid.ItemsSource = results;
                _model.ResultsTableContent = resultTableContent;

                foreach (var task in tasks)
                {
                    task._method.Result.ResultMatrix = await task._task;
                }

                //foreach (var task in tasks)
                //{
                //    await Task.Run(async () =>
                //    {
                //        task._method.Result.ResultMatrix = await task._task;
                //    });
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SLAUSolved(object sender, EventArgs e)
        {
            if (sender is IMatrixSolver solver)
            {
                if (sender is MethodBase basee)
                {
                    if (e is SLAU.Events.Results res)
                    {
                        basee.Result.ResultMatrix = res.Result;
                    }
                }
            }

        }
        private void BSaveExcel_Click(object sender, RoutedEventArgs e)
        {
            var save = new Utils.SaveExcelFileDialog();
            if (save.Show(out var pathToSave))
            {
                SaveExcelFile(pathToSave, _matrixGrid.GetData(), _matrixGrid.GetDataColumn());
            }
        }

        private void BLoadExcel_Click(object sender, RoutedEventArgs e)
        {
            var open = new Utils.OpenExcelFileDialog();
            if (open.Show(out var pathToOpen))
            {
                resultLoadMatrix = null;
                resultLoadVector = null;

                LoadExcelFile(pathToOpen);

                if (resultLoadVector != null)
                {
                    _matrixGrid.SetData(resultLoadMatrix);

                    if (resultLoadVector != null)
                    {
                        _vectorGrid.SetData(resultLoadVector);
                    }

                    // Обновляем числа
                    _model.Rows = _matrixGrid.RowCount;
                    _model.Columns = _matrixGrid.ColumnCount;
                }


            }
        }

        public Control View { get => _view; }

        public void MethodChanged(TypeMathMethod newMethod)
        {
            switch (newMethod)
            {
                case TypeMathMethod.SLAU:
                    {
                        _method = TypeMathMethod.SLAU;
                    }
                    break;
            }
        }
        private void CreateMatrixButton_Click(object sender, RoutedEventArgs e)
        {
            int rows = _model.Rows, cols = _model.Columns;
            // Создаём таблицу
            {
                if (rows > 0 && cols > 0)
                {
                    _matrixGrid.InitializeDynamicTable(rows, cols);
                    _vectorGrid.InitializeDynamicTable(rows, 1);
                }
                else
                {
                    MessageBox.Show("Введите корректные размеры матрицы.");
                }
            }
        }

        private void BFillRand_Click(object sender, RoutedEventArgs e)
        {
            int colCount = _matrixGrid.ColumnCount;
            int rowCount = _matrixGrid.RowCount;

            double[,] values = new double[rowCount, colCount];
            double[] valuesVector = new double[rowCount];
            Random rnd = new Random();

            for (int idxRow = 0; idxRow < rowCount; ++idxRow)
            {
                for (int idxColumn = 0; idxColumn < colCount; ++idxColumn)
                {
                    values[idxRow, idxColumn] = rnd.NextDouble() * 100 - 50;
                }
                valuesVector[idxRow] = rnd.NextDouble() * 100 - 50;
            }

            _matrixGrid.SetData(values);
            _vectorGrid.SetData(valuesVector);
        }

        void SaveExcelFile(string filePath, double[,] arrValues, double[] arrVector)
        {
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            Workbook workbook = null;
            Worksheet worksheet = null;
            List<List<String>>? tableData = null;
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

                int LastColumn = arrValues.GetLength(1) + 3; // 1 смещение excel 2 смещение от правой границы массива
                for (int idxRow = 0, idxRowEnd = arrVector.GetLength(0); idxRow < idxRowEnd; idxRow++)
                {
                    range.Cells[idxRow + 1, LastColumn].Value = arrVector[idxRow];
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
            List<List<String>>? tableData = null;
            List<String>? vectorData = null;

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
                vectorData = new List<string>();

                // Чтение значений из столбца A до первой пустой строки
                while (range.Cells[row, column].Value != null)
                {
                    List<String> values = new List<String>();

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

                maxColumnText += 2;
                int rowVector = 1;
                // Собиираем вектор
                while (range.Cells[rowVector, maxColumnText].Value != null && rowVector < row)
                {
                    vectorData.Add(range.Cells[rowVector, maxColumnText].Value.ToString());
                    ++rowVector;
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

                    for (int rowIdx = 0; rowIdx < vectorData.Count; ++rowIdx)
                    {
                        doublesVector.Add(double.Parse(vectorData[rowIdx]));
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
                resultLoadMatrix = new double[doublesMatrix.Count, maxColumn];

                for (int rowIdx = 0; rowIdx < doublesMatrix.Count; ++rowIdx)
                {
                    int columnIdx = 0;
                    for (; columnIdx < doublesMatrix[rowIdx].Count; ++columnIdx)
                    {
                        resultLoadMatrix[rowIdx, columnIdx] = doublesMatrix[rowIdx][columnIdx];
                    }
                }

                resultLoadVector = new double[doublesMatrix.Count];

                for (int rowIdx = 0; rowIdx < doublesVector.Count; ++rowIdx)
                {
                    resultLoadVector[rowIdx] = doublesVector[rowIdx];
                }
            }
            //} // if (tableData != null) end

        }
    }

    //public class DynamicRow : INotifyPropertyChanged
    //{
    //    private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

    //    public object this[string propertyName]
    //    {
    //        get => _values.ContainsKey(propertyName) ? _values[propertyName] : null;
    //        set
    //        {
    //            if (_values.ContainsKey(propertyName))
    //            {
    //                _values[propertyName] = value;
    //            }
    //            else
    //            {
    //                _values.Add(propertyName, value);
    //            }
    //            OnPropertyChanged(propertyName);
    //        }
    //    }

    //    public event PropertyChangedEventHandler? PropertyChanged;

    //    protected void OnPropertyChanged(string propertyName)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }

    //    double[,]? resultLoadMatrix = null;
    //    double[]? resultLoadVector = null;

    //}
}
