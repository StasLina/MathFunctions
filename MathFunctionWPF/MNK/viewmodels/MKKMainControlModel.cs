using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathFunctionWPF.MNK.viewmodels
{
    class MKKMainControlModel : INotifyPropertyChanged
    {
        public delegate void DataCahangeEvent();
        DataCahangeEvent? _eventDataChanged;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
