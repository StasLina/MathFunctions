﻿using MathFunctionWPF.Models;
using MathFunctionWPF.Views;
using MathFunctionWPF.Views.Sorting;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;
using Microsoft.Office;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Globalization;
using System.Windows.Data;
using System.Diagnostics;
using System.Collections.ObjectModel;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Windows.Controls;
using System.Windows.Media;

namespace MathFunctionWPF.Views
{

    public class ResultsVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Если Results не null и не пустое, то показываем кнопку
            if (value != null && value is object results && results != DBNull.Value)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed; // Скрываем кнопку, если Results пустое
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Для ConvertBack мы не будем использовать
            return null;
        }
    }
}

namespace MathFunctionWPF.Controllers
{

    class MathSortingController : IBaseController
    {
        TypeMathMethod _method;
        public void MethodChanged(TypeMathMethod newMethod)
        {
            //throw new NotImplementedException();
            switch (newMethod)
            {
                case TypeMathMethod.BubbleSort:
                    {
                        _method = TypeMathMethod.BubbleSort;

                        _view.SortView.BManualInput.Click += BManualInput_Click;
                        _view.SortView.BFileIput.Click += BFileIput_Click;
                        _view.SortView.BExcelInput.Click += BExcelInput_Click;
                        _view.SortView.BSortData.Click += BSortData_Click;
                        _view.SortView.BRandInput.Click += BRandInput_Click;
                        _view.SortView.BUpdateTable.Click += BUpdateTable_Click;
                        _view.SortView.CBViewResults.SelectionChanged += CBViewResults_SelectionChanged; ;
                        draw = new Draw(_view.SortView.Drawing);
                    }
                    break;
            }
        }


        private void BRandInput_Click(object sender, RoutedEventArgs e)
        {
            NumberInputDialog numberInputDialog = new NumberInputDialog();
            numberInputDialog.EventInputNumber += (double value) =>
            {
                int count = (int)double.Ceiling(value);

                List<double> newValues = new List<double>();
                Random rnd = new Random();

                for (int idx = 0; idx < count; ++idx)
                {
                    newValues.Add(rnd.Next());
                }

                _data.ArrValues = newValues;
            };
            numberInputDialog.ShowDialog();
        }

        private void BSortData_Click(object sender, RoutedEventArgs e)
        {
            Sort();
        }

        private void BExcelInput_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FileSelectionForm fileSelectionForm = new FileSelectionForm();
            fileSelectionForm.BLoadFile.Click += BLoadFile_Click;
            fileSelectionForm.ShowDialog();
        }

        private void BFileIput_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            LoadFile();
        }

        private void BManualInput_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MathFunctionWPF.Views.WInputNumbers wInputNumbers = new WInputNumbers(_data.ArrValues);
            wInputNumbers.ShowDialog();
        }

        MathSortView _view = null;

        public MathSortView View
        {
            get { return _view; }
        }
        public MathSortingController(MathSortView sortView)
        {
            _view = sortView;
            _data.AddHandlerDataChanged(DataValuesChanged);

        }
        void DataValuesChanged()
        {
            if (_data.ArrValues.Count > 0)
            {
                _view.SortView.LIsDataSet.Content = "Данные установлены";
            }
            else
            {
                _view.SortView.LIsDataSet.Content = "Данные не установлены";

            }
        }

        enum SortOder
        {
            Asc, Desc
        };

        SortOder GetSortOder()
        {
            return _view.SortView.CBOrder.IsChecked == true ? SortOder.Asc : SortOder.Desc;
        }

        [Serializable]
        class CompanionData
        {
            public delegate void DataCahangeEvent();
            DataCahangeEvent? _eventDataChanged;
            private List<double> arrValues = new List<double>();

            public void AddHandlerDataChanged(DataCahangeEvent e)
            {
                _eventDataChanged = e;
            }
            public List<double> ArrValues
            {
                get => arrValues;
                set
                {
                    arrValues = value;
                    _eventDataChanged?.Invoke();
                }
            }
        }

        CompanionData _data = new CompanionData();



        void Save()
        {
            var saveFileDialog = new SaveFileDialog()
            {
                Filter = "JSON Files (*.json)|*.json",//|All Files (*.*)|*.*", // Фильтр для файлов JSON
                Title = "Сохранить файл как JSON",
                DefaultExt = "json", // Расширение по умолчанию
                FileName = "data" // Имя файла по умолчанию
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string json = JsonConvert.SerializeObject(_data, Formatting.Indented);

                try
                {
                    File.WriteAllText(saveFileDialog.FileName, json);
                }
                catch (Exception ex)
                {
                    // Обработка ошибок
                    MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }

        }

        void LoadFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files (*.json)|*.json", // |All Files (*.*)|*.*
                Title = "Выберите файл .json",
                Multiselect = false

            };
            if (openFileDialog.ShowDialog() == true) // В WPF это возвращает true/false
            {

                using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                using (StreamReader reader = new StreamReader(fs))
                {
                    // Чтение всего содержимого файла
                    string content = reader.ReadToEnd();
                    CompanionData? data = JsonConvert.DeserializeObject<CompanionData>(content);

                    if (data != null)
                    {
                        _data = data;
                    }
                }
            }

        }
        private void BLoadFile_Click(object sender, RoutedEventArgs e)
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
                    LoadExcelFile(openFileDialog.FileName);
                    // Показать путь к выбранному файлу (можно заменить на сохранение в переменную)
                    //MessageBox.Show($"Выбран файл: {openFileDialog.FileName}", "Файл выбран", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void LoadExcelFile(string filePath)
        {

            // Создаем приложение Excel
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            Workbook workbook = null;
            Worksheet worksheet = null;
            List<String>? columnData = null;
            try
            {
                // Открываем книгу Excel
                workbook = excelApp.Workbooks.Open(filePath);
                worksheet = workbook.Sheets[1]; // Выбираем первый лист

                Microsoft.Office.Interop.Excel.Range range = worksheet.UsedRange; // Диапазон используемых ячеек
                int row = 1; // Начинаем с первой строки

                // Массив для хранения данных столбца A
                columnData = new System.Collections.Generic.List<string>();

                // Чтение значений из столбца A до первой пустой строки
                while (range.Cells[row, 1].Value != null)
                {
                    columnData.Add(range.Cells[row, 1].Value.ToString()); // Добавляем значение в список
                    row++;
                }

                // Показать загруженные данные
                string result = string.Join("\n", columnData);
                //MessageBox.Show($"Загруженные данные:\n{result}", "Данные из Excel", MessageBoxButton.OK, MessageBoxImage.Information);
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

            if (columnData != null)
            {
                List<double> doubles = new List<double>();

                foreach (var text in columnData)
                {
                    if (double.TryParse(text, out double value) == false)
                    {
                        doubles.Add(value);
                    }
                    else
                    {
                        break;
                    }
                }
                _data.ArrValues = doubles;

            }

        }

        bool IsAscending
        {
            get
            {
                switch (GetSortOder())
                {
                    case SortOder.Asc:
                        return false;
                    case SortOder.Desc:
                        return true;
                    default:
                        return false;
                }
            }
        }

        ObservableCollection<RecordSortResults>? _records = null;
        async void Sort()
        {
            if (_data.ArrValues.Count == 0)
            {
                return;
            }
            BubbleModel model = (BubbleModel)_view.SortView.DataContext;

            List<SortBase> sortingBase = new List<SortBase>();
            //            List<RecordSortResults> records = new List<RecordSortResults>();
            _records = new ObservableCollection<RecordSortResults>();

            //records[0].

            if (model.IsBogo)
            {
                var sort = new Bogosort() { Results = new RecordSortResults { Tile = "Бого", Time = 0, Iteration = 0, Results = null } };
                sortingBase.Add(sort);
                _records.Add(sort.Results);
            }//Results = new List<double> { 1.2, 2.4, 3.6 } }

            if (model.IsBuble)
            {
                var sort = new BubbleSort() { Results = new RecordSortResults { Tile = "Болотная", Time = 0, Iteration = 0, Results = null } };
                sortingBase.Add(sort);
                _records.Add(sort.Results);
            }//Results = new List<double> { 1.2, 2.4, 3.6 } }

            if (model.IsInserter)
            {
                var sort = new InsertionSort() { Results = new RecordSortResults { Tile = "Вставками", Time = 0, Iteration = 0, Results = null } };
                sortingBase.Add(sort);
                _records.Add(sort.Results);
            }

            if (model.IsSheikernay)
            {
                var sort = new ShakerSort() { Results = new RecordSortResults { Tile = "Шэйкерная", Time = 0, Iteration = 0, Results = null } };
                sortingBase.Add(sort);
                _records.Add(sort.Results);
            }

            if (model.IsFaster)
            {
                var sort = new QuickSort() { Results = new RecordSortResults { Tile = "Быстрая", Time = 0, Iteration = 0, Results = null } };
                sortingBase.Add(sort);
                _records.Add(sort.Results);
            }
            _view.SortView.DataGrid.ItemsSource = _records;


            List<List<double>> rows = new List<List<double>>();
            List<Task> tasks = new List<Task>();

            bool order = IsAscending;
            foreach (var sort in sortingBase)
            {
                List<double> newList = _data.ArrValues.ToList();
                rows.Add(newList);


                Task a = Task.Run(
                    () =>
                    {
                        sort.TimingSort(newList, order);
                    });


                //a.Wait();
                //a.Start();
                //TaskStatus status = a.Status;
                tasks.Add(a);
            }

            // Ожидаем завершения всех задач
            foreach (var a in tasks)
            {
                await a;
            }

            // Обновляем результаты

            UpdateTalbeResults();
            UpdateCBViewResults();
            //_view.SortView.DataGrid.ItemsSource;

        }

        Draw draw;

        bool updatingCBViewResults = false;

        void UpdateCBViewResults()
        {
            updatingCBViewResults = true;
            _view.SortView.CBViewResults.Items.Clear();
            

            if (_records == null)
            {
                
            }
            else
            {
                var items = _view.SortView.CBViewResults.Items;
                foreach (var item in _records)
                {
                    items.Add(item.Tile);
                }
            }
            updatingCBViewResults = false;
        }

        void UpdateResults(int index = 0)
        {
            draw.DrawModel((List<double>) _records[index].Results);
        }

        private void CBViewResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (updatingCBViewResults) return;
            UpdateResults(_view.SortView.CBViewResults.SelectedIndex);
        }


        void UpdateTalbeResults()
        {
            _view.SortView.DataGrid.ItemsSource = null;
            _view.SortView.DataGrid.ItemsSource = _records;
        }
        private void BUpdateTable_Click(object sender, RoutedEventArgs e)
        {
            UpdateTalbeResults();
        }


        class Draw
        {
            ContentControl _drawing;

            // Конструктор класса Draw, инициализируем _drawing
            public Draw(ContentControl drawing)
            {
                _drawing = drawing;
            }

            // Метод для построения диаграммы
            public void DrawModel(List<double> listValues)
            {

                double[] data = listValues.ToArray();
                var plotView = new OxyPlot.Wpf.PlotView();
                _drawing.Content = plotView;

                var plotModel = new PlotModel { Title = "Столбчатая диаграмма" };

                //            var barSeries = new BarSeries
                //            {
                //                ItemsSource = new List<BarItem>
                //{
                //    new BarItem { Value = 10 },
                //    new BarItem { Value = 20 },
                //    new BarItem { Value = 30 }
                //}
                //            };

                var itemsSource = new List<BarItem>();

                double min = double.MaxValue, max = -double.MaxValue;
                foreach(var item in data) {
                    itemsSource.Add(new BarItem() { Value = item});
                    min = double.Min(min, item);
                    max = double.Max(min, item);
                }

                var barSeries = new BarSeries()
                {
                    ItemsSource = itemsSource
                };

                var linearAxis = new LinearAxis
                {
                    Position = AxisPosition.Bottom,  // Устанавливаем ось значений по горизонтали (снизу)
                    Title = "Значение",
                    Minimum = 0,
                    Maximum = max+5,
                   // StringFormat = "0" // Формат отображения чисел (без научной нотации)
                };

                plotModel.Axes.Add(linearAxis);


                plotModel.Series.Add(barSeries);


                //// Создаем ось категорий (по оси X)
                //plotModel.Axes.Add(new CategoryAxis
                //{
                //    Position = AxisPosition.Bottom,
                //    Key = "CategoryAxis",
                //    //ItemsSource = new[] { "A", "B", "C", "D", "E" },  // Категории GetIndexes(data)
                //});

                //// Создаем ось значений (по оси Y)
                //plotModel.Axes.Add(new LinearAxis
                //{
                //    Position = AxisPosition.Left,
                //    Title = "Значения",
                //    Minimum = 0,
                //    Maximum = 10
                //});



                //// Создаем серию столбцов
                //var barSeries = new BarSeries
                //{

                //    ItemsSource = new[]
                //    {
                //        new BarItem { Value = 5 },
                //        new BarItem { Value = 3 },
                //        new BarItem { Value = 8 },
                //        new BarItem { Value = 6 },
                //        new BarItem { Value = 4 }
                //    },
                //    LabelPlacement = LabelPlacement.Outside, // Размещение меток
                //    LabelFormatString = "{0:0.00}" // Формат метки
                //};

                //barSeries.YAxisKey = "CategoryAxis";


                //// Добавляем серию в модель
                //plotModel.Series.Add(barSeries);

                //// Привязываем модель к PlotView
                plotView.Model = plotModel;
            }

            //    try
            //    {
            //        double[] data = { 3.5, 7.2, 1.8, 4.6, 5.9 };

            //        // Создаем модель графика
            //        var plotModel = new PlotModel { Title = "Пример столбчатой диаграммы" };

            //        // Создаем оси
            //        plotModel.Axes.Add(new CategoryAxis
            //        {
            //            Position = AxisPosition.Bottom,
            //            Key = "CategoryAxis",
            //            ItemsSource = GetIndexes(data),  // Индексы массива в качестве категорий
            //            LabelField = "Index",  // Названия категорий
            //        });

            //        plotModel.Axes.Add(new LinearAxis
            //        {
            //            Position = AxisPosition.Left,
            //            Title = "Значение",
            //            Minimum = 0,
            //            Maximum = 10
            //        });

            //        // Добавляем столбчатую серию
            //        var barSeries = new BarSeries
            //        {
            //            ItemsSource = GetBarItems(data), // Список столбцов
            //            LabelPlacement = LabelPlacement.Outside, // Размещение меток
            //            LabelFormatString = "{0:0.00}" // Формат метки
            //        };

            //        barSeries.YAxisKey = "CategoryAxis";

            //        plotModel.Series.Add(barSeries);

            //        // Привязка модели к графику
            //        plotView.Model = plotModel;

            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }
            //}

            // Метод для создания списка индексов (категорий)
            private List<KeyValuePair<string, double>> GetIndexes(double[] data)
            {
                var list = new List<KeyValuePair<string, double>>();
                for (int i = 0; i < data.Length; i++)
                {
                    list.Add(new KeyValuePair<string, double>(i.ToString(), data[i])); // Используем индекс как ключ
                }
                return list;
            }

            // Метод для создания списка данных для BarSeries
            private List<BarItem> GetBarItems(double[] data)
            {
                var list = new List<BarItem>();
                for (int i = 0; i < data.Length; i++)
                {
                    list.Add(new BarItem { Value = data[i] });
                }
                return list;
            }
        }



    }

}


public class RecordSortResults
{
    public CancellationTokenSource cts = new CancellationTokenSource();
    public object _lock = new object();
    public bool isPaused = false;
    public string Tile { get; set; } = ""; // Тип
    public long Time { get; set; } = 0; // Время мс
    public int Iteration { get; set; } = 0; // Кол-во итераций
    public object Results { get; set; } // Результаты на List<double>
}

public abstract class SortBase
{
    public RecordSortResults Results { get; set; }
    Stopwatch _stopwatch;
    public int InstructionCount { get; protected set; }

    // Метод для сортировки, будет реализован в подклассах
    //public abstract void Sort(List<double> data);
    public abstract void Sort(List<double> data, bool order);


    List<double> _data;
    public void TimingSort(List<double> data, bool order)
    {
        _stopwatch = new Stopwatch();
        _data = data;
        // Запускаем таймер
        _stopwatch.Start();
        Sort(data, order);
        _stopwatch.Stop();
        long elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            Results.Time = _stopwatch.ElapsedMilliseconds;
            Results.Results = _data;
            Results.Iteration = InstructionCount;

        }
        );
    }

    // Метод для увеличения счетчика инструкций
    protected void IncrementInstructionCount()
    {
        InstructionCount++;

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            Results.Time = _stopwatch.ElapsedMilliseconds;
            Results.Iteration = InstructionCount;
        }
        );
        //Results.Time = _stopwatch.ElapsedMilliseconds;


        if (Results.cts.Token.IsCancellationRequested)
        {
            Results.cts.Token.ThrowIfCancellationRequested();
        }
        else
        {
            // Проверяем  на паузы

            while (Results.isPaused)
            {
                Monitor.Wait(Results._lock); // Ждём, пока приостановка не будет снята
            }
        }
    }


    void Pause()
    {
        lock (Results._lock)
        {
            Results.isPaused = true;
        }
    }

    void Resume()
    {
        lock (Results._lock)
        {
            Results.isPaused = false;
            Monitor.PulseAll(Results._lock); // Уведомляем все ожидающие потоки
        }
    }


    //class Program
    //    {
    //        private static CancellationTokenSource cts = new CancellationTokenSource();
    //        private static object _lock = new object();
    //        private static bool isPaused = false;

    //        var task = Task.Run(() => DoWork(cts.Token));

    //static void DoWork(CancellationToken token)
    //{
    //    cts.Cancel();
    //    int i = 0;

    //    while (!token.IsCancellationRequested)
    //    {
    //        lock (_lock)
    //        {
    //            while (isPaused)
    //            {
    //                Monitor.Wait(_lock); // Ждём, пока приостановка не будет снята
    //            }
    //        }

    //        //Console.WriteLine($"Работаем... {++i}");
    //        //Thread.Sleep(1000); // Симуляция работы
    //    }
    //}






}

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

}

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

}


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

}


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

}

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
        }
    }

    // Метод для выполнения сортировки
    public override void Sort(List<double> data, bool isAscending)
    {
        Random rand = new Random();
        InstructionCount = 0;  // Сбрасываем счетчик перед началом сортировки

        // Продолжаем случайно перемешивать элементы, пока они не окажутся отсортированными
        while (!IsSorted(data))
        {
            Shuffle(data, rand);  // Перемешиваем элементы
            IncrementInstructionCount();  // Увеличиваем счетчик инструкций за перемешивание
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

