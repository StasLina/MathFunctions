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
            _view.BRand.Click += BRand_Click; ;



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
                double[] args2 = MNK.models.LeastSquares.Calculate(dataColumnX, dataColumnY, 5);
                FunCalc funCalc = new FunCalc(args);
                FunCalc funCalc2 = new FunCalc(args2);

                _model.Func1Result = GetFunction(args);
                _model.Func2Result = GetFunction(args2);

                var minMax = GetMinMax(dataColumnX);
                var lines = new FunctionSeries(funCalc.Calc, minMax[0], minMax[1], 0.1);
                pm.Series.Add(lines);

                var lines2 = new FunctionSeries(funCalc2.Calc, minMax[0], minMax[1], 0.1);
                pm.Series.Add(lines2);
            }


            pm.Series.Add(dots);

            _view.Drawing.Model = pm;

            //pm.Axes.Add(new LinearAxis
            //{
            //    Position = AxisPosition.Bottom,
            //    Title = "Ось X",
            //    Minimum = _functionInputModel.XStart, // Минимальное значение по оси X
            //    Maximum = _functionInputModel.XEnd // Максимальное значение по оси X
            //});
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
    }
}
