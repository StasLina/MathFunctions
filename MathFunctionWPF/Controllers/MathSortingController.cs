using MathFunctionWPF.Models;
using MathFunctionWPF.Views;
using MathFunctionWPF.Views.Sorting;
using Microsoft.Win32;
using System.Windows;

using Newtonsoft.Json;
using System.Runtime.InteropServices;
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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using SortLibrary;
using MathData;
using AnimationsDemo;
using MathFunctionWPF.Utils;


namespace MathFunctionWPF.Models
{
    [Serializable]
    public class CompanionData : INotifyPropertyChanged
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
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public CompanionData()
        {
            PropertyChanged += PropertyChangedEventHandler;
        }

        private static void PropertyChangedEventHandler(object? sender, PropertyChangedEventArgs e)
        {
            CompanionData? data = sender as CompanionData;
            if (e.PropertyName == "ArrValues")
            {
                data?._eventDataChanged?.Invoke();
            }
        }
    }
}

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
        MathSortView _view = null;

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
                        //_view.SortView.BUpdateTable.Click += BUpdateTable_Click;
                        _view.SortView.CBViewResults.SelectionChanged += CBViewResults_SelectionChanged;

                        _view.SortView.BVizulizate.Click += BVizulizate_Click;
                        draw = new Draw(_view.SortView.Drawing);

                        _view.SortView.BSaveData.Click += BSaveData_Click;


                        _view.SortView.eventSaveClick += (List<double> listValues) =>
                        {
                            Save(listValues);
                        };
                        //BubleSortView
                    }
                    break;
            }
        }

        private void BVizulizate_Click(object sender, RoutedEventArgs e)
        {
            draw.SortAnimModel(_data.ArrValues, _view.SortView.CBViewResults.Text);
            //draw.DrawModel();
            //throw new NotImplementedException();
        }

        private void BSaveData_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void BRandInput_Click(object sender, RoutedEventArgs e)
        {
            NumberInputDialog numberInputDialog = new NumberInputDialog();
            numberInputDialog.EventInputNumber += (NumberInputDialog window) =>
            {
                var viewModel = window.DataContext as Models.NumberInputDialogViewModel;

                int count = int.Parse(viewModel.InputFields[0].Value);
                double min = double.Parse(viewModel.InputFields[1].Value);
                double max = double.Parse(viewModel.InputFields[2].Value);

                List<double> newValues = new List<double>();
                Random rnd = new Random();

                if (max < min)
                {
                    throw new Exception("Конец диапазона меньша чем начало");
                }
                double diap = max - min;
                for (int idx = 0; idx < count; ++idx)
                {
                    newValues.Add(rnd.NextDouble() * diap + min);
                }

                _data.ArrValues = newValues;
            };
            numberInputDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
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
            List<double> newValues = _data.ArrValues.ToList();
            MathFunctionWPF.Views.WInputNumbers wInputNumbers = new WInputNumbers(newValues);
            wInputNumbers.ShowDialog();
            _data.ArrValues = newValues;
        }


        public Control View
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
            return _view.SortView.OrderSort.Sorting == false ? SortOder.Asc : SortOder.Desc;
        }

        CompanionData _data = new CompanionData();

        void Save(List<double>? listValues = null)
        {
            if (listValues == null)
            {
                listValues = _data.ArrValues;
            }

            var saveFileDialog = new SaveFileDialog()
            {
                Filter = "JSON Files (*.json)|*.json",//|All Files (*.*)|*.*", // Фильтр для файлов JSON
                Title = "Сохранить файл как JSON",
                DefaultExt = "json", // Расширение по умолчанию
                FileName = "data" // Имя файла по умолчанию
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string json = JsonConvert.SerializeObject(listValues, Formatting.Indented);

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
                    //CompanionData? data = JsonConvert.DeserializeObject<CompanionData>(content);
                    List<double> data = JsonConvert.DeserializeObject<List<double>>(content);
                    if (data != null)
                    {
                        if (_data == null)
                        {
                            _data = new CompanionData();
                        }

                        _data.ArrValues = data;
                    }
                }
            }

        }
        private void BLoadFile_Click(object sender, RoutedEventArgs e)
        {

            OpenExcelFileDialog openExcelFileDialog = new OpenExcelFileDialog();
            try
            {
                if (openExcelFileDialog.Show(out var path))
                {
                    LoadExcelFile(path);
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
                    if (double.TryParse(text, out double value) == true)
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

        public class CONSTANTS
        {
            public const string BOGO = "Бого";
            public const string BUBLE = "Пузырьковая";
            public const string INSERT = "Вставками";
            public const string SHEIKER = "Шэйкерная";
            public const string QUICK= "Быстрая";
        }

        ObservableCollection<RecordSortResults>? _records = null;
        async void Sort()
        {
            if (_data.ArrValues.Count == 0)
            {
                return;
            }
            BubbleModel model = (BubbleModel)_view.SortView.DataContext;

            List<SortLibrary.SortBase> sortingBase = new List<SortLibrary.SortBase>();
            //            List<RecordSortResults> records = new List<RecordSortResults>();
            _records = new ObservableCollection<RecordSortResults>();

            //records[0].
            ;
            if (model.IsBogo)
            {
                var sort = new Bogosort() { Results = new RecordSortResults { Tile = CONSTANTS.BOGO, Time = 0, Iteration = 0, Results = null } };
                sortingBase.Add(sort);
                _records.Add(sort.Results);
            }//Results = new List<double> { 1.2, 2.4, 3.6 } }

            if (model.IsBuble)
            {
                var sort = new BubbleSort() { Results = new RecordSortResults { Tile = CONSTANTS.BUBLE, Time = 0, Iteration = 0, Results = null } };
                sortingBase.Add(sort);
                _records.Add(sort.Results);
            }//Results = new List<double> { 1.2, 2.4, 3.6 } }

            if (model.IsInserter)
            {
                var sort = new InsertionSort() { Results = new RecordSortResults { Tile = CONSTANTS.INSERT, Time = 0, Iteration = 0, Results = null } };
                sortingBase.Add(sort);
                _records.Add(sort.Results);
            }

            if (model.IsSheikernay)
            {
                var sort = new ShakerSort() { Results = new RecordSortResults { Tile = CONSTANTS.SHEIKER, Time = 0, Iteration = 0, Results = null } };
                sortingBase.Add(sort);
                _records.Add(sort.Results);
            }

            if (model.IsFaster)
            {
                var sort = new SortLibrary.QuickSort() { Results = new RecordSortResults { Tile = CONSTANTS.QUICK, Time = 0, Iteration = 0, Results = null } };
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

            bool isCycleUpdate = true;


            //            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    // Код, который будет выполнен в главном потоке
            //    // например, обновление UI
            //}));


            //Task taskWait = Task.Run(async () =>
            //{

            //    isCycleUpdate = false;
            //});


            foreach (var a in tasks)
            {
                await a;
            }
            // Ожидаем завершения всех задач

            isCycleUpdate = false;

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
            _view.SortView.BVizulizate.Visibility = Visibility.Hidden;
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
            draw.DrawModel((List<double>)_records[index].Results);
        }

        private void CBViewResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (updatingCBViewResults) return;
            UpdateResults(_view.SortView.CBViewResults.SelectedIndex);
            _view.SortView.BVizulizate.Visibility = Visibility.Visible;
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
            OxyPlot.Wpf.PlotView _drawing;

            // Конструктор класса Draw, инициализируем _drawing
            public Draw(OxyPlot.Wpf.PlotView drawing)
            {
                _drawing = drawing;
            }

            // Метод для построения диаграммы

            AnimationsDemo.LinearBarViewModel model = null;

            CancellationTokenSource cancellationTokenSource;

            public async void SortAnimModel(List<double> listValues, string name = "")
            {
                //if (model == null)
                //{
                //}
                List<double> data = listValues.ToList();
                model = new AnimationsDemo.LinearBarViewModel(data);

                AnimationSettings animationSettings = new AnimationSettings();
                switch (name)
                {
                    case CONSTANTS.BOGO:
                        animationSettings.SortBase = new Bogosort();
                        break;
                    case CONSTANTS.SHEIKER:
                        animationSettings.SortBase = new ShakerSort();
                        break;
                    case CONSTANTS.BUBLE:
                        animationSettings.SortBase = new BubbleSort();
                        break;
                    case CONSTANTS.INSERT:
                        animationSettings.SortBase = new InsertionSort();
                        break;
                    case CONSTANTS.QUICK:
                        animationSettings.SortBase = new SortLibrary.QuickSort();
                        break;

                }

                var plotView = _drawing;
                plotView.Model = model.PlotModel;

                //var vm = this.DataContext as IAnimationViewModel;
                var vm = model;
                if (vm != null)
                {
                    if (vm.IsAnimationRuning == false)
                    {
                        cancellationTokenSource = new CancellationTokenSource();
                        var cancellationToken = cancellationTokenSource.Token;
                        vm.IsAnimationRuning = true;

                        try
                        {
                            // Ожидаем завершение задачи
                            await Task.Run(async () =>
                            {
                                vm.AnimateAsync2(cancellationToken,animationSettings).Wait();
                                vm.IsAnimationRuning = false;
                            });
                        }
                        catch (OperationCanceledException)
                        {
                        }

                    }
                    else
                    {
                        cancellationTokenSource.Cancel();
                    }
                }
            }
            public void DrawModel(List<double> listValues)
            {

                double[] data = listValues.ToArray();
                var plotView = _drawing;

                //plotView.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                //plotView.HorizontalAlignment = System.Windows.VerticalAlignment.Stretch;


                var plotModel = new PlotModel { Title = "Столбчатая диаграмма" };
                var itemsSource = new List<BarItem>();

                double min = double.MaxValue, max = -double.MaxValue;
                foreach (var item in data)
                {
                    itemsSource.Add(new BarItem() { Value = item });
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
                    Maximum = max + 5,
                    // StringFormat = "0" // Формат отображения чисел (без научной нотации)
                };

                plotModel.Axes.Add(linearAxis);


                plotModel.Series.Add(barSeries);

                plotView.Model = plotModel;
            }

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


//public class RecordSortResults
//{
//    public CancellationTokenSource cts = new CancellationTokenSource();
//    public object _lock = new object();
//    public bool isPaused = false;
//    public string Tile { get; set; } = ""; // Тип
//    public long Time { get; set; } = 0; // Время мс
//    public int Iteration { get; set; } = 0; // Кол-во итераций
//    public object Results { get; set; } // Результаты на List<double>
//}
