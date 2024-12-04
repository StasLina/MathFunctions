using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathFunctionsWPF.Interfaces
{
    public interface IFormula
    {
        public string Formula { get; set; }
    }

    public interface IFormulaDimension
    {
        public double Accuracy { get; set; }
        public double XStart { get; set; }
        public double XEnd { get; set; }
        public double CalcIncrementRate();

    }

    public static class FormulaDimensionExtensions
    {
        // Метод расширения для вычисления величины инкремента
        public static double CalcIncrementRate(this IFormulaDimension formulaDimension)
        {
            double eN = 1;
            if (formulaDimension.Accuracy >= 1)
            {
                while (Math.Pow(10, eN) < formulaDimension.Accuracy)
                {
                    ++eN;
                }
                --eN;
                return eN;
            }
            else
            {
                while (Math.Pow(10, -eN) > formulaDimension.Accuracy)
                {
                    ++eN;
                }
                ++eN;
                return -eN;
            }
        }
    }

    public interface IFunctionCalculationData : IFormula, IFormulaDimension
    {
        public string XStartText { get; set; }
        public string XEndText {  get; set; }
        public string AccuracyText {  get; set; }
        public string PrecisionText {  get; set; }
        public string CountStepsText { get; set; }

    }


}
