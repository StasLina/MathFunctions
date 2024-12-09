using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MathFunctionWPF.Models
{
    public class InputField : INotifyPropertyChanged
    {
        public string _key = "";
        private string _value = "";
        private bool isChecked;

        public string Key
        {
            get
            {
                if (_key == "")
                {
                    return Label;
                }
                return _key;
            }

            set
            {
                _key = value;
            }
        }

        public string Label
        {
            get;
            set;
        } // Название поля

        public string Value
        {
            get => _value;
            set
            {
                this._value = value;
                OnPropertyChanged();
            }
        } // Значение поля

        public bool IsChecked
        {
            get => isChecked; 
            set
            {
                isChecked = value;
                OnPropertyChanged();
            }
        }
        public ValidationType ValidationType { get; set; } = ValidationType.None;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }

    public enum ValidationType
    {
        None, PositiveNumber, Number, Double, Function, Block
    }

    public class NumberInputDialogViewModel
    {
        public ObservableCollection<InputField> InputFields { get; set; }

        public NumberInputDialogViewModel()
        {
            InputFields = new ObservableCollection<InputField>
            {
                new InputField { Label = "Количество точек:", ValidationType = ValidationType.PositiveNumber },
                new InputField { Label = "От:", ValidationType = ValidationType.Double },
                new InputField { Label = "До:", ValidationType = ValidationType.Double }
            };
        }
    }
}
