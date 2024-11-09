using System.ComponentModel;
using System.Runtime.CompilerServices;
using MathFunctionWPF.Views;

namespace MathFunctionWPF.Models
{
    class MathFunctionViewModel : INotifyPropertyChanged
    {
        private object _descriptionView;
        private object _graphPlotter;
        private object _sourceDataView;
        private object _calculationView;
        private object _listMethods;
        //private object _slideBarButton;

        public TypeMathMethod TypeMethod;
        public MathFunctionViewModel()
        {
            //FunctionInputView functionInput = new FunctionInputView();
            //SourceDataView = functionInput;
        }
        public object DescriptionView
        {
            get { return _descriptionView; }
            set
            {
                _descriptionView = value;
                OnPropertyChanged();
            }
        }

        //public object ListMethods
        //{
        //    get { return _listMethods; }
        //    set
        //    {
        //        _listMethods = value; OnPropertyChanged();
        //    }
        //}

        public object GraphPlotterView
        {
            get { return _graphPlotter; }
            set
            {
                _graphPlotter = value;
                OnPropertyChanged();
            }
        }

        public object SourceDataView
        {
            get
            {
                return _sourceDataView;
            }

            set
            {
                _sourceDataView = value;
                OnPropertyChanged();
            }
        }

        public object CalculationView
        {
            get { return _calculationView; }
            set
            {
                _calculationView = value;
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
