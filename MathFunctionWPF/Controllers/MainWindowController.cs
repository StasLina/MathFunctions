using MathFunctionWPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathFunctionWPF.Views;
using MathFunctionWPF.Models;

namespace MathFunctionWPF.Controllers
{
    
    class MainWindowController
    {
        private MainViewModel mainViewModel;
        private MainWindow _window;

        public MainWindowController(MainWindow window)
        {
            _window = window;
            mainViewModel = _window.DataContext as MainViewModel;
            Init();
        }

        void Init()
        {
            MathFunctionController controller = new MathFunctionController(new MathFunctionView());
            mainViewModel.CurrentView = controller.View;
        }

    }
}
