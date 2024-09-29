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
    class FunctionInputData : INotifyPropertyChanged
    {
        public string Formula
        {
            get; set;
        } = "F(x)=x-4";

        public double XStart { get; set; } = 0;
        public double XEnd { get; set; } = 10;
        public double Accuracy { get; set; } = 0.0001;
        public double IncrementRate
        {
            get
            {
                double eN = 1;
                if (Accuracy >= 1)
                {
                    while(Math.Pow(10,eN) < Accuracy)
                    {
                        ++eN;
                    }
                    --eN;
                    return eN;
                }
                else
                {
                    while (Math.Pow(10, -eN) > Accuracy )
                    {
                        ++eN;
                    }
                    ++eN;
                    return -eN;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

}
