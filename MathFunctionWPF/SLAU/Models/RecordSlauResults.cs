using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathFunctionWPF.SLAU.Models
{
    public class RecordSlauResults : INotifyPropertyChanged
    {
        private string tile = "";
        private long time = 0;
        private object results = null;
        double[]? resultMatrix = null;

        public string Tile
        {
            get => tile;
            set
            {
                tile = value;
                OnPropertyChanged();
            }
        }
        public long Time
        {
            get => time;
            set
            {
                time = value;
                OnPropertyChanged();
            }
        }

        public object Results
        {
            get => ResultMatrix;
            set
            {
                results = value;
                OnPropertyChanged();
            } // Результаты на List<double>
        }



        public double[]? ResultMatrix
        {
            get => resultMatrix;
            set
            {
                resultMatrix = value;
                OnPropertyChanged();
                OnPropertyChanged("Results");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

}
