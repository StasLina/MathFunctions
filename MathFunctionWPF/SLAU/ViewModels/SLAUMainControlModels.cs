using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathFunctionWPF.SLAU.ViewModels
{
    class SLAUMainControlModels : INotifyPropertyChanged
    {
        private object listMethodsControl;
        private object resultsTableContent;
        private object matrixDataContent;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public object ListMethodsControl
        {
            get => listMethodsControl;
            set
            {
                listMethodsControl = value;
                OnPropertyChanged();
            }
        }

        public object ResultsTableContent
        {
            get => resultsTableContent;
            set
            {
                resultsTableContent = value;
                OnPropertyChanged();
            }
        }
        public object MatrixDataContent
        {
            get => matrixDataContent;
            set
            {
                matrixDataContent = value;
                OnPropertyChanged();
            }
        }



    }
}
