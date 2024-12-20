﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MathFunctionWPF.SLAU.ViewModels
{
    public class ResultViewModels : INotifyPropertyChanged
    {
        private object table;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public object Table
        {
            get => table;
            set
            {
                table = value;
                OnPropertyChanged();
            }
        }
    }
}
