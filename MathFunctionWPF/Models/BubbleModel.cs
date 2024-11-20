using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathFunctionWPF.Models
{
    internal class BubbleModel : INotifyPropertyChanged
    {
        bool _isBuble = true; 
        bool _isFaster = true; 
        bool _isInserter = true; 
        bool _isSheikernay = true; 
        bool _isBogo = true; 

        public bool IsBuble
        {
            get => _isBuble;
            set
            {
                _isBuble = value;
                OnPropertyChanged();
            }
        }

        public bool IsFaster
        {
            get => _isFaster;
            set
            {
                _isFaster = value;
                OnPropertyChanged();
            }
        }

        public bool IsInserter
        {
            get => _isInserter;
            set
            {
                _isInserter = value;
                OnPropertyChanged();
            }
        }

        public bool IsSheikernay
        {
            get => _isSheikernay;
            set
            {
                _isSheikernay = value;
                OnPropertyChanged();
            }
        }

        public bool IsBogo
        {
            get => _isBogo;
            set
            {
                _isBogo = value;
                OnPropertyChanged();
            }
        }

        public string OrderLabel { get => _orderChecked ? "По убыванию": "По возрастанию"; }
        public bool OrderChecked
        {
            get => _orderChecked;
            set
            {
                _orderChecked = value;
                OnPropertyChanged();
                OnPropertyChanged("OrderLabel");
            }
        }


        bool _orderChecked = false; // По возвранию, true - убывание


        public event PropertyChangedEventHandler? PropertyChanged;


        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
