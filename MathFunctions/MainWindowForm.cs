

using MathFunctions.Views;

namespace MathFunctions
{
    public partial class MainWindowForm : Form
    {
        public MainWindowForm()
        {
            InitializeComponent();
        }


    }

    public class MainWindowFormController
    {
        MainWindowForm _form;

        Graph graph;
        graph = new Graph();
        GBGraph.Controls.Add(graph);
        public MainWindowFormController(MainWindowForm form)
        {
            _form = form;
        }
        
        private void Init()
        {

        }


        void SetFuncition()
        {
            var pm = new PlotModel
            {
                Title = "Trigonometric functions",
                Subtitle = "Example using the FunctionSeries",
                PlotType = PlotType.Cartesian,
                Background = OxyColors.White
            };
            pm.Series.Add(new FunctionSeries(Math.Sin, -10, 10, 0.1, "sin(x)"));
            pm.Series.Add(new FunctionSeries(Math.Cos, -10, 10, 0.1, "cos(x)"));
            pm.Series.Add(new FunctionSeries(t => 5 * Math.Cos(t), t => 5 * Math.Sin(t), 0, 2 * Math.PI, 0.1, "cos(t),sin(t)"));
            plot1.Model = pm;
        }

        class Calculation
        {
            Function f;
            public double Calc(double x)
            {
                return f.calculate(x);
            }
        }

        public void Foo()
        {
            // Создание функции f(x)
            Function f = new Function("f(x) = x^2 + 2*x + 1");

            // Пример использования функции
            double result = f.calculate(3); // Подставляем x = 3

            // Вывод результата
            Console.WriteLine("f(3) = " + result);
            Calculation c = new Calculation();
            Func<double, double> foo = c.Calc;
        }
    }
}
