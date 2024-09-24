using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using MathFunctionWPF.Views;

namespace MathFunctionWPF.Models
{
    class MainViewModel : INotifyPropertyChanged
    {
        private object _currentView;

        public MainViewModel()
        {
            // Изначально показываем первое представление
            //CurrentView = new MathFunctionView();
        }

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
