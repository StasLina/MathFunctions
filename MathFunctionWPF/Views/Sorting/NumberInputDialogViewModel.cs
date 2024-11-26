using System.Collections.ObjectModel;

namespace MathFunctionWPF.Models
{
    public class InputField
    {
        public string Label { get; set; } // Название поля
        public string Value { get; set; } // Значение поля

        public ValidationType ValidationType { get; set; } = ValidationType.None;

    }

    public enum ValidationType
    {
        None,PositiveNumber, Number, Double
    }

    public class NumberInputDialogViewModel
    {
        public ObservableCollection<InputField> InputFields { get; set; }

        public NumberInputDialogViewModel()
        {
            InputFields = new ObservableCollection<InputField>
        {
            new InputField { Label = "Количество точек:", ValidationType = ValidationType.PositiveNumber},
            new InputField { Label = "От:" ,ValidationType = ValidationType.Double},
            new InputField { Label = "До:" ,ValidationType = ValidationType.Double}
        };
        }
    }
}
