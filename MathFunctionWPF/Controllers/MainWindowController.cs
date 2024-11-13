using MathFunctionWPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathFunctionWPF.Views;
using MathFunctionWPF.Models;
using MathFunctionWPF.MathMethods;
using Microsoft.VisualBasic;
using static System.Net.Mime.MediaTypeNames;

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
        IBaseController _contrllerSelectionView;
        MathFunctionNavigationViewModel _navNodel;
        void Init()
        {
            //MathFunctionController controller = new MathFunctionController(new MathFunctionView());
            //mainViewModel
            MathFunctionNavigationPanel panel = new MathFunctionNavigationPanel();
            _navNodel = panel.DataContext as MathFunctionNavigationViewModel;

            // Окно смены 
            MethodListControl methodListControl = new MethodListControl();
            _navNodel.ListMethodsView = methodListControl;
            methodListControl.MethodChanged += MethodChanged; //controller.MethodChanged;

            // Иницилизируем SelectionView
            // Иницилизируем контроллер 

            //_navNodel.SelectionView = controller.View;

            // Отображаем окошко
            MethodChanged(TypeMathMethod.MainMenu);

            mainViewModel.CurrentView = panel;
        }

        public void MethodChanged(TypeMathMethod typeMethod)
        {
            var view = GetSelectionView(typeMethod, out var isNewWindow);

            if (isNewWindow)
            {
                _navNodel.SelectionView = view;
            }
            _contrllerSelectionView.MethodChanged(typeMethod);
        }

        public object GetSelectionView(TypeMathMethod typeMethod, out bool isNewWindow)
        {
            switch (typeMethod)
            {
                case TypeMathMethod.Bisection:
                case TypeMathMethod.GoldenSearch:
                case TypeMathMethod.Test:
                case TypeMathMethod.Integration:
                case TypeMathMethod.Newton:
                case TypeMathMethod.CoordinateDesent:
                    if(_navNodel.SelectionView is MathFunctionView)
                    {
                        isNewWindow = false;
                        return _navNodel.SelectionView;
                    }
                    else
                    {
                        MathFunctionController controller = new MathFunctionController(new MathFunctionView());
                        _contrllerSelectionView = controller;
                        isNewWindow = true;
                        return controller.View;
                    }
                case TypeMathMethod.Search:
                    if (_navNodel.SelectionView is MathFunctionSearchView)
                    {
                        isNewWindow = false;
                        return _navNodel.SelectionView;
                    }
                    else
                    {
                        MathFunctionSearchController controller = new MathFunctionSearchController(new MathFunctionSearchView());
                        _contrllerSelectionView = controller;
                        isNewWindow = true;
                        return controller.View;
                    }
                case TypeMathMethod.BubbleSort:
                    isNewWindow = true;
                    //return _contrllerSelectionView;
                    break;
                case TypeMathMethod.MainMenu:
                default:

                    if (_navNodel.SelectionView is MathFunctionHomeView)
                    {
                        isNewWindow = false;
                        return _navNodel.SelectionView;
                    }
                    else
                    {
                        MathFunctionHomeController controller = new MathFunctionHomeController(new MathFunctionHomeView());
                        _contrllerSelectionView = controller;
                        isNewWindow = true;
                        return controller.View;
                    }
            }
        }
    }
}
