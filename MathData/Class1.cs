
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MathData
{
    public class RecordSortResults : INotifyPropertyChanged
    {
        public CancellationTokenSource cts = new CancellationTokenSource();
        public object _lock = new object();
        public bool isPaused = false;
        private string tile = "";
        private long time = 0;
        private int iteration = 0;
        private object results = null;

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
        public int Iteration
        {
            get => iteration;
            set
            {
                iteration = value;
                OnPropertyChanged();
            }
        }

        public object Results
        {
            get => results;
            set
            {
                results = value;
                OnPropertyChanged();
            } // Результаты на List<double>
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }


}
