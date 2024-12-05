using MathFunctionsWPF.Interfaces;
using MathFunctionWPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MathFunctionsWPF.Interfaces;

namespace MathFunctionWPF.Integral.ViewModels
{

    internal class IntegralControlModel : INotifyPropertyChanged, IFunctionCalculationData
    {
        private string xStartText = "1";
        private string xEndText = "20";
        private string formula = "F(x)=sin(x)";
        public string VerifyedFormula { get; set; } = "F(x)=sin(x)";

        public double XStart
        {
            get
            {
                return double.Parse(XStartText);
            }
            set
            {
                XStartText = value.ToString();
            }
        }

        public double XEnd
        {
            get
            {
                return double.Parse(XEndText);
            }
            set
            {
                XEndText = value.ToString();
            }
        }

        public string Formula
        {
            get => formula;
            set
            {
                formula = value;
                OnPropertyChanged();
            }
        }

        public object IntegralImage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public double CalcIncrementRate()
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<InputField> InputFields { get; set; }
        public ObservableCollection<InputField> OutputFields { get; set; }

        string getFiled([CallerMemberName] string key = null)
        {
            foreach (var field in InputFields)
            {
                if (field.Key == key)
                {
                    return field.Value;
                }
            }
            return "";
        }

        public string XStartText
        {
            get => xStartText;
            set
            {
                xStartText = value;
                OnPropertyChanged();
            }
        }

        public string XEndText
        {
            get => xEndText;
            set
            {
                xEndText = value;
                OnPropertyChanged();
            }
        }



        public string AccuracyText { get => getFiled(); set { return; } }
        public string PrecisionText { get => getFiled(); set { return; } }
        public string CountStepsText { get => getFiled(); set { return; } }

        public double Accuracy { get => double.Parse(AccuracyText); set { return; } }


        public double CountSteps
        {
            get
            {
                double value = 0;
                try
                {
                    value = Math.Ceiling(double.Parse(CountStepsText));
                }
                catch
                {

                }
                return value;
            }
        }

        public double IncrementRate
        {
            get
            {
                return double.Parse(PrecisionText);
            }
        }

        string IFunctionCalculationData.CountStepsText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
