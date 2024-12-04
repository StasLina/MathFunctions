// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnimationExtensions.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AnimationsDemo
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using OxyPlot;
    using OxyPlot.Series;

    public static partial class AnimationExtensions
    {
        public static AnimationFrame GetFinalAnimationFrame(DataPointSeries series)
        {
            var animationFrame = new AnimationFrame
            {
                Duration = TimeSpan.Zero
            };

            var points = series.GetAnimatablePoints();
            foreach (var point in points)
            {
                animationFrame.AnimationPoints.Add(new AnimationPoint
                {
                    X = point.FinalX,
                    Y = point.FinalY
                });
            }

            return animationFrame;
        }

        public static async Task AnimateSeriesAsync(
            this PlotModel plotModel,
            DataPointSeries series,
            List<AnimationFrame> animationFrames)
        {
            if (animationFrames.Count == 0)
            {
                return;
            }

            var finalAnimationFrame = GetFinalAnimationFrame(series);
            animationFrames.Add(finalAnimationFrame);

            var xAxis = plotModel.DefaultXAxis;
            var oldXAxisMinimum = xAxis.Minimum;
            var oldXAxisMaximum = xAxis.Maximum;

            xAxis.Minimum = xAxis.ActualMinimum;
            xAxis.Maximum = xAxis.ActualMaximum;

            var yAxis = plotModel.DefaultYAxis;
            var oldYAxisMinimum = yAxis.Minimum;
            var oldYAxisMaximum = yAxis.Maximum;

            yAxis.Minimum = yAxis.ActualMinimum;
            yAxis.Maximum = yAxis.ActualMaximum;

            var previousDataFieldX = series.DataFieldX;
            var previousDataFieldY = series.DataFieldY;

            // Always fix up the data fields (we are using IAnimatablePoint from now on)
            series.DataFieldX = "X";
            series.DataFieldY = "Y";

            var points = series.GetAnimatablePoints();

            foreach (var animationFrame in animationFrames)
            {
                // TODO: consider implementing the IsVisible feature

                var animationPoints = animationFrame.AnimationPoints;
                if (animationPoints.Count > 0)
                {
                    for (var j = 0; j < points.Count; j++)
                    {
                        var animatablePoint = points[j];
                        if (animatablePoint != null)
                        {
                            //if (j < animationPoints.Count)
                            {
                                var animationPoint = animationPoints[j];

                                animatablePoint.X = animationPoint.X;
                                animatablePoint.Y = animationPoint.Y;
                            }
                        }
                    }
                }

                plotModel.InvalidatePlot(true);
                await Task.Delay(10);
            }

            xAxis.Minimum = oldXAxisMinimum;
            xAxis.Maximum = oldXAxisMaximum;

            yAxis.Minimum = oldYAxisMinimum;
            yAxis.Maximum = oldYAxisMaximum;

            series.DataFieldX = previousDataFieldX;
            series.DataFieldY = previousDataFieldY;

            plotModel.InvalidatePlot(true);
        }


        public static async Task AnimateSeriesAsync(
    this PlotModel plotModel,
    DataPointSeries series,
    DataPointSeries series2,
    CancellationToken cancellationToken)
        {
            //var xAxis = plotModel.DefaultXAxis;
            //var oldXAxisMinimum = xAxis.Minimum;
            //var oldXAxisMaximum = xAxis.Maximum;

            //xAxis.Minimum = xAxis.ActualMinimum;
            //xAxis.Maximum = xAxis.ActualMaximum;

            //var yAxis = plotModel.DefaultYAxis;
            //var oldYAxisMinimum = yAxis.Minimum;
            //var oldYAxisMaximum = yAxis.Maximum;

            //yAxis.Minimum = yAxis.ActualMinimum;
            //yAxis.Maximum = yAxis.ActualMaximum;

            //var previousDataFieldX = series.DataFieldX;
            //var previousDataFieldY = series.DataFieldY;

            // Always fix up the data fields (we are using IAnimatablePoint from now on)
            series.DataFieldX = "X";
            series.DataFieldY = "Y";

            var points = series.GetAnimatablePoints();
            //var points2 = series2.GetAnimatablePoints();

            QuickSort quickSort = new QuickSort();
            List<double> values = new List<double>();
            quickSort.Data = values;

            // Обновляем содержимое
            quickSort.EventAtion += async (List<double> listItems) =>
            {
                List<Pnl> list = (List<Pnl>)series2.ItemsSource;
                list.Clear();

                bool isOld = true;
                for (int j = 0; j < listItems.Count; ++j)
                {
                    var currentBar = points[j];

                    if (currentBar.IsHide)
                    {
                        currentBar.IsHide = false;
                        currentBar.Y = currentBar.HideValue;
                    }

                    if (currentBar.Y != listItems[j])
                    {
                        currentBar.Y = 0;
                        currentBar.IsHide = true;
                        isOld = false;
                        currentBar.HideValue = listItems[j];
                        //points2[j].Y = listItems[j];
                        list.Add(new Pnl() { X = currentBar.X, Y = currentBar.HideValue });
                    }
                }

                if (isOld)
                {
                    return false;
                }

                // Invalidate the plot asynchronously without blocking the UI thread
                plotModel.InvalidatePlot(true);


                // Проверка отмены перед выполнением задержки
                try { 
                cancellationToken.ThrowIfCancellationRequested();
                //Task.Delay(200).Wait(); // асинхронная задержка
                Thread.Sleep(200);
                    cancellationToken.ThrowIfCancellationRequested(); // повторная проверка отмены
                }
                catch (OperationCanceledException)
                {
                    return true;
                }
                return false;
            };

            // Устанавливаем значения точек
            for (var j = 0; j < points.Count; j++)
            {
                var item = points[j];
                values.Add(item.Y);
            }

            try
            {
                // Сортировка в фоне
                Task.Run(() => quickSort.SortAsync(values, true,cancellationToken), cancellationToken).Wait();
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("Сортировка была отменена.");
                cancellationToken.ThrowIfCancellationRequested();
            }



            // Восстановление старых значений
            //xAxis.Minimum = oldXAxisMinimum;
            //xAxis.Maximum = oldXAxisMaximum;

            //yAxis.Minimum = oldYAxisMinimum;
            //yAxis.Maximum = oldYAxisMaximum;

            //series.DataFieldX = previousDataFieldX;
            //series.DataFieldY = previousDataFieldY;

            List<Pnl> list = (List<Pnl>)series2.ItemsSource;
            list.Clear();
            for (int j = 0; j < values.Count; ++j)
            {
                var currentBar = points[j];

                if (currentBar.IsHide)
                {
                    currentBar.IsHide = false;
                    currentBar.Y = currentBar.HideValue;
                }

                if (currentBar.Y != values[j])
                {
                    currentBar.Y = 0;
                    currentBar.Y = values[j];
                }
            }

            plotModel.InvalidatePlot(true); // Повторная отрисовка графика
        }



        public static async Task AnimateSeriesAsyncOld42(
    this PlotModel plotModel,
        DataPointSeries series,
    DataPointSeries series2, CancellationToken cancellationToken)
        {
            var xAxis = plotModel.DefaultXAxis;
            var oldXAxisMinimum = xAxis.Minimum;
            var oldXAxisMaximum = xAxis.Maximum;

            xAxis.Minimum = xAxis.ActualMinimum;
            xAxis.Maximum = xAxis.ActualMaximum;

            var yAxis = plotModel.DefaultYAxis;
            var oldYAxisMinimum = yAxis.Minimum;
            var oldYAxisMaximum = yAxis.Maximum;

            yAxis.Minimum = yAxis.ActualMinimum;
            yAxis.Maximum = yAxis.ActualMaximum;

            var previousDataFieldX = series.DataFieldX;
            var previousDataFieldY = series.DataFieldY;

            // Always fix up the data fields (we are using IAnimatablePoint from now on)
            series.DataFieldX = "X";
            series.DataFieldY = "Y";

            var points = series.GetAnimatablePoints();


            //var points2 = series2.GetAnimatablePoints();

            QuickSort quickSort = new QuickSort();

            List<double> values = new List<double>();
            quickSort.Data = values;

            // Обновляем содержимое
            quickSort.EventAtion +=  (List<double> listItems) =>
            {
                List<Pnl> list = (List<Pnl>)series2.ItemsSource;
                list.Clear();
                // Update the points asynchronously
                bool isOld = true;
                for (int j = 0; j < listItems.Count; ++j)
                {
                    var currentBar = points[j];

                    if (currentBar.IsHide)
                    {
                        currentBar.IsHide = false;
                        currentBar.Y = currentBar.HideValue;
                    }

                    if (currentBar.Y != listItems[j])
                    {

                        currentBar.Y = 0;
                        currentBar.IsHide = true;
                        isOld = false;
                        currentBar.HideValue = listItems[j];
                        //points2[j].Y = listItems[j];
                        list.Add(new Pnl() { X = currentBar.X, Y = currentBar.HideValue });
                    }
                    //else
                    //{
                    //    points[j].Y = listItems[j];
                    //}

                }

                // Invalidate the plot asynchronously without blocking the UI thread

                
                plotModel.InvalidatePlot(true);
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    Thread.Sleep(200);
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    Task.FromResult(true);
                }
                return Task.FromResult(false);
            };

            // Устанавливаем значения точек
            for (var j = 0; j < points.Count; j++)
            {
                var item = points[j];
                values.Add(item.Y);
            }
            try
            {
                // Сортировка в фоне
                await Task.Run(() => quickSort.Sort(values, true));

            }
            catch (OperationCanceledException ex)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
            //cancellationToken.ThrowIfCancellationRequested();

            // Восстановление старых значений
            xAxis.Minimum = oldXAxisMinimum;
            xAxis.Maximum = oldXAxisMaximum;

            yAxis.Minimum = oldYAxisMinimum;
            yAxis.Maximum = oldYAxisMaximum;

            series.DataFieldX = previousDataFieldX;
            series.DataFieldY = previousDataFieldY;

            plotModel.InvalidatePlot(true); // Повторная отрисовка графика
        }


        public static async Task AnimateSeriesAsync2(
            this PlotModel plotModel,
            DataPointSeries series)
        {

            var xAxis = plotModel.DefaultXAxis;
            var oldXAxisMinimum = xAxis.Minimum;
            var oldXAxisMaximum = xAxis.Maximum;

            xAxis.Minimum = xAxis.ActualMinimum;
            xAxis.Maximum = xAxis.ActualMaximum;

            var yAxis = plotModel.DefaultYAxis;
            var oldYAxisMinimum = yAxis.Minimum;
            var oldYAxisMaximum = yAxis.Maximum;

            yAxis.Minimum = yAxis.ActualMinimum;
            yAxis.Maximum = yAxis.ActualMaximum;

            var previousDataFieldX = series.DataFieldX;
            var previousDataFieldY = series.DataFieldY;

            // Always fix up the data fields (we are using IAnimatablePoint from now on)
            series.DataFieldX = "X";
            series.DataFieldY = "Y";

            var points = series.GetAnimatablePoints();

            QuickSort quickSort = new QuickSort();

            List<double> values = new List<double>();
            quickSort.Data = values;
            quickSort.EventAtion += async (List<double> listItems) =>
            {

                for (int j = 0; j < listItems.Count; ++j)
                {
                    points[j].Y = listItems[j];
                }

                plotModel.InvalidatePlot(true);

                await Task.Delay(10);
                return false;
            };

            // Устанавливаем значения точек
            for (var j = 0; j < points.Count; j++)
            {
                var item = points[j];
                values.Add(item.Y);
            }

            quickSort.Sort(values, true);


            xAxis.Minimum = oldXAxisMinimum;
            xAxis.Maximum = oldXAxisMaximum;

            yAxis.Minimum = oldYAxisMinimum;
            yAxis.Maximum = oldYAxisMaximum;

            series.DataFieldX = previousDataFieldX;
            series.DataFieldY = previousDataFieldY;

            plotModel.InvalidatePlot(true);
        }


    }

    public abstract class SortBase
    {
        //public RecordSortResults Results { get; set; }
        Stopwatch _stopwatch;
        public int InstructionCount { get; protected set; }

        // Метод для сортировки, будет реализован в подклассах
        //public abstract void Sort(List<double> data);
        public abstract void Sort(List<double> data, bool order);


        protected List<double> _data;


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








    public class QuickSort : SortBase
    {
        public List<double> Data
        {
            get => _data;
            set => _data = value;
        }
        public QuickSort() { }

        // Нужно ли прекратить 
        public event Func<List<double>, Task<bool>> EventAtion;


        public async Task SortAsync(List<double> data, bool ascending, CancellationToken cancellationToken)
        {
            try
            {
                // Сортировка с учетом отмены
                await QuickSortHelperAsync(data, 0, data.Count - 1, ascending, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Сортировка была отменена.");
            }
        }

        public async Task QuickSortHelperAsync(List<double> data, int low, int high, bool ascending, CancellationToken cancellationToken)
        {
            if (low < high)
            {
                // Отмена операции
                cancellationToken.ThrowIfCancellationRequested();

                int pi = await PartitionAsync(data, low, high, ascending, cancellationToken);

                // Рекурсивные вызовы для сортировки двух частей
                await QuickSortHelperAsync(data, low, pi - 1, ascending, cancellationToken);
                await QuickSortHelperAsync(data, pi + 1, high, ascending, cancellationToken);
            }
        }

        public async Task<int> PartitionAsync(List<double> data, int low, int high, bool ascending, CancellationToken cancellationToken)
        {
            double pivot = data[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                cancellationToken.ThrowIfCancellationRequested(); // Проверка отмены внутри цикла

                if ((ascending && data[j] <= pivot) || (!ascending && data[j] >= pivot))
                {
                    i++;
                    if (i != j)
                    {
                        double temp = data[i];
                        data[i] = data[j];
                        data[j] = temp;
                        await IncrementInstructionCount(); // Подсчёт инструкций для сравнения
                    }
                }
            }

            ++i;
            // Меняем элементы местами
            if (i != high)
            {
                double swapTemp = data[i];
                data[i] = data[high];
                data[high] = swapTemp;
                await IncrementInstructionCount(); // Подсчёт инструкций для присваивания
            }

            return i;
        }


        public async Task IncrementInstructionCount()
        {
            if (EventAtion != null)
            {
                // Ожидаем завершения асинхронных обработчиков
                bool result = await EventAtion.Invoke(_data); // Ждем завершения асинхронных обработчиков
                if (result)
                {
                    // Если условие выполнено, выбрасываем исключение
                    throw new OperationCanceledException("Операция отменена.");
                }
            }
        }
        
        //protected async void IncrementInstructionCount()
        //{
        //    if (EventAtion != null)
        //    {
        //        // Ожидаем завершения асинхронных обработчиков
        //        bool a = await EventAtion.Invoke(_data); // Важно использовать await
                
        //        if (a)
        //        {
        //            throw new OperationCanceledException();
        //        }
        //    }
        //    return;
        //}

        public override void Sort(List<double> data, bool asxending)
        {
            try
            {
                QuickSortHelper(data, 0, data.Count - 1, asxending);

            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Сортировка была отменена.");
            }
            return;
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
                if ((ascending && data[j] <= pivot) || (!ascending && data[j] >= pivot))
                {
                    i++;

                    if (i != j)
                    {
                        double temp = data[i];
                        data[i] = data[j];
                        data[j] = temp;
                        IncrementInstructionCount(); // Подсчёт инструкций для сравнения
                    }
                }

            }

            ++i;
            // Меняем элементы местами

            if (i != high)
            {
                double swapTemp = data[i];
                data[i] = data[high];
                data[high] = swapTemp;
                IncrementInstructionCount(); // Подсчёт инструкций для присваивания
            }

            return i;
        }

        // Метод для подсчёта инструкций (например, сравнений или присваиваний)

    }
}