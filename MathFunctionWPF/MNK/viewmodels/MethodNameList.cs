using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathFunctionWPF.MNK.viewmodels
{
    internal class MethodNameList : INotifyPropertyChanged
    {
        private string typeTitle;
        private string _value;

        public delegate void DataCahangeEvent();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string TypeTitle
        {
            get => typeTitle;
            set
            {
                typeTitle = value;
            }
        }
        public string Value
        {
            get => _value;
            set
            {
                _value = value;
            }
        }
        public string Key;

        public MethodNameList(string key)
        {
            Key = key;
        }
    }
}
