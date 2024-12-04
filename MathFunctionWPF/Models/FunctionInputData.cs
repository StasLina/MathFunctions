using MathFunctionsWPF.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;



namespace MathFunctionWPF.Models
{
    class FunctionInputData : INotifyPropertyChanged, IFunctionCalculationData
    {
        public string Formula
        {
            get; set;
        }
        //= "F(x)=x^2+2";
        //= "F(x)=1/tg(x)";
        //= "F(x)=sin(x)";
        //= "F(x)=x^4+2*x^2+3*x^3+10-x";
        //= "F(x)=2*x^4-3*x^2+5*x^3+10";
        //= "F(x)=sin(x)+x";
        = "F(x)=(x+2)^2+2";
        //= "F(x,y)=(x+2)^2+2+y";


        public double XStart { get; set; } = -5;
        public double XEnd { get; set; } = 10;

        public double Accuracy { get; set; } = 0.1;

        private string _precisionValue = "-5";

        public string PrecisionText
        {
            get
            {
                return _precisionValue;
            }
            set
            {
                _precisionValue = value;
                OnPropertyChanged();
            }

        }

        string
            _functionText = "",
            _xStartText = "",
            _x1EndText = "",
            _accuracyText = "";
        public string FunctionText
        {
            get
            {
                return _functionText;
            }
            set
            {
                _functionText = value;
                OnPropertyChanged();
            }
        }

        public string XStartText
        {
            get
            {
                return _xStartText;
            }
            set
            {
                _xStartText = value;
                OnPropertyChanged();
            }
        }

        public string XEndText
        {
            get
            {
                return _x1EndText;
            }
            set
            {
                _x1EndText = value;
                OnPropertyChanged();
            }
        }

        public string XStartText1
        {
            get
            {
                return _xStartText;
            }
            set
            {
                _xStartText = value;
                OnPropertyChanged();
            }
        }

        public string X1EndText1
        {
            get
            {
                return _x1EndText;
            }
            set
            {
                _x1EndText = value;
                OnPropertyChanged();
            }
        }

        public string AccuracyText
        {
            get
            {
                return _accuracyText;
            }
            set
            {
                _accuracyText = value;
                OnPropertyChanged();
            }
        }

        public double CountSteps = 1;
        string _countStepsText = "";
        public string CountStepsText
        {
            get
            {
                return _countStepsText;
            }
            set
            {
                _countStepsText = value;
                OnPropertyChanged();
            }
        }

        public void UpdateWithData()
        {
            FunctionText = Formula;
            XStartText = XStart.ToString();
            XEndText = XEnd.ToString();
            AccuracyText = Accuracy.ToString();
            ((IFormulaDimension)this).CalcIncrementRate();
            PrecisionText = FormulaDimensionExtensions.CalcIncrementRate(this).ToString();
            CountStepsText = CountSteps.ToString();
        }

        public string? CountStepsLabel
        {
            get => _countStepsLabel;
            set
            {
                _countStepsLabel = value;
                OnPropertyChanged();
            }
        }

        string? _countStepsLabel = null;

        public string? PrecisionLabel
        {
            get => _precisionLabel;
            set
            {
                _precisionLabel = value;
                OnPropertyChanged();
            }
        }

        string? _precisionLabel = null;
        public string? AccuracyLabel
        {
            get => _accuracyLabel;
            set
            {
                _accuracyLabel = value;
                OnPropertyChanged();
            }
        }

        string? _accuracyLabel = null;

        public string X1Label
        {
            get => _X1Label;
            set
            {
                _X1Label = value;
                OnPropertyChanged();
            }
        }
        string _X1Label = null;

        public string? X0Label
        {
            get => _X0Label;
            set
            {
                _X0Label = value;
                OnPropertyChanged();
            }
        }
        string? _X0Label = null;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public void ResetLabels()
        {
            X1Label = null;
            X0Label = null;
            AccuracyLabel = null;
            PrecisionLabel = null;
            CountStepsLabel = null;
        }

        public double CalcIncrementRate()
        {
            return FormulaDimensionExtensions.CalcIncrementRate(this);
        }
    }

}
