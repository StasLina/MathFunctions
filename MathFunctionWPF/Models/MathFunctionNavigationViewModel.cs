using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathFunctionWPF.Models
{
    class MathFunctionNavigationViewModel : INotifyPropertyChanged
    {
        private object _selectionView;

        public MathFunctionNavigationViewModel()
        {
            // Изначально показываем первое представление
            //CurrentView = new MathFunctionView();
        }

        public object SelectionView
        {
            get { return _selectionView; }
            set
            {
                _selectionView = value;
                OnPropertyChanged();
            }
        }

        private object _listMethodsView;
        public object ListMethodsView
        {
            get { return _listMethodsView; }
            set
            {
                _listMethodsView = value;
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
