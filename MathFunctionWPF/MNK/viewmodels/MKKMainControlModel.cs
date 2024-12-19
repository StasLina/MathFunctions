using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private object inputView;
        private string func1Result;
        private string func2Result;
        private ObservableCollection<MethodNameList> methodNameList;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public object InputView
        {
            get => inputView;
            set
            {
                inputView = value;
                OnPropertyChanged();
            }
        }
        public string Func1Result
        {
            get => func1Result;
            set
            {
                func1Result = value;
                OnPropertyChanged();
            }
        }
        public string Func2Result
        {
            get => func2Result;
            set
            {
                func2Result = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MethodNameList> ListInputControl
        {
            get => methodNameList;
            set
            {
                methodNameList = value;
                OnPropertyChanged();
            }
        }
    }
}
