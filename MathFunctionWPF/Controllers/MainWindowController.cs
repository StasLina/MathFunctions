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
using MathFunctionWPF.SLAU.Controls;
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
                    {
                        if (_navNodel.SelectionView is MathFunctionSearchView)
                        {
                            isNewWindow = false;
                            return _navNodel.SelectionView;
                        }
                        else
                        {
                            isNewWindow = true;
                            MathSortingController controller = new MathSortingController(new MathSortView());
                            _contrllerSelectionView = controller;
                            return controller.View;
                        }

      
                    }
                    break;

                case TypeMathMethod.Integral:
                    {
                        if (_navNodel.SelectionView is Integral.Controls.IntegralControl)
                        {
                            isNewWindow = false;
                            return _navNodel.SelectionView;
                        }
                        else
                        {
                            isNewWindow = true;
                            IntegralController controller = new IntegralController(new Integral.Controls.IntegralControl());
                            _contrllerSelectionView = controller;
                            return controller.View;
                        }


                    }
                    break;

                case TypeMathMethod.SLAU:
                    {
                        if (_navNodel.SelectionView is SLAU.Controls.SLAUMainControl)
                        {
                            isNewWindow = false;
                            return _navNodel.SelectionView;
                        }
                        else
                        {
                            isNewWindow = true;
                            SLAUController controller = new SLAUController(new SLAUMainControl());
                            _contrllerSelectionView = controller;
                            return controller.View;
                        }


                    }
                    break;

                case TypeMathMethod.MNK:
                    {
                        if (_navNodel.SelectionView is MNK.views.MKKMainControl)
                        {
                            isNewWindow = false;
                            return _navNodel.SelectionView;
                        }
                        else
                        {
                            isNewWindow = true;
                            MNKController controller = new MNKController(new MNK.views.MKKMainControl());
                            _contrllerSelectionView = controller;
                            return controller.View;
                        }
                    }
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
