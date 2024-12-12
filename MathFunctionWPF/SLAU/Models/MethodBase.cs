using MathFunctionWPF.SLAU.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathFunctionWPF.SLAU.Models
{

    
    public enum TypeMethod
    {
        Gaus,          // Гауса
        GaussJordan, // Гауса Жордана
        Kramera, // Крамера
        Squre,         // Квадратных корней
        Progonki,      // Прогонки
        SimpleIter,    // Простой итерации
        HirestDown,    // Наискорейшего спуска
        ComplexGraident // Сопряжённых градиентов
    }

    static class TypeMethodExtension
    {
        public static string ToDescription(this TypeMethod method)
        {
            return method switch
            {
                TypeMethod.Gaus => "Гауcса",
                TypeMethod.GaussJordan => "Гауcса-Жордана",
                TypeMethod.Kramera => "Крамера",
                TypeMethod.Squre => "Квадратных корней",
                TypeMethod.Progonki => "Прогонки",
                TypeMethod.SimpleIter => "Простой итерации",
                TypeMethod.HirestDown => "Наискорейшего спуска",
                TypeMethod.ComplexGraident => "Сопряжённых градиентов",
                _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
            };
        }
    }

    public class MethodBase : INotifyPropertyChanged
    {
        private TypeMethod _type;
        private bool _isChecked;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MethodBase(TypeMethod type)
        {
            _type = type;
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        TypeMethod Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged();
                OnPropertyChanged("TypeTitle ");
            }
        }
        public string TypeTitle { get => _type.ToDescription(); }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        public RecordSlauResults Result;
    }

}


