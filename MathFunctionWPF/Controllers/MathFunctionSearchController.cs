using MathFunctionWPF.Models;
using MathFunctionWPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MathFunctionWPF.Controllers
{
    internal class MathFunctionSearchController : IBaseController
    {
        TypeMathMethod _currentType = TypeMathMethod.MainMenu;
        MathFunctionSearchView _view;

        public MathFunctionSearchController(MathFunctionSearchView view)
        {
            _view = view;
            _view.AddHandler(StringFilterElements);

            // Добавляем список
            FillElemets(SearchModel.ListItems.ToArray());
        }

        void FillElemets(Item[] arrItems)
        {
            _view.ListItems.Children.Clear();
            for (int idx=0,idxEnd=arrItems.Length;idx != idxEnd; ++idx) {
            Themes.ShortFunctionalityDescription description = new Themes.ShortFunctionalityDescription();
                //description

                Item refrenceItem = arrItems[idx];

                Image icon = new Image
                {
                    //Proper
                    //Properties.Resources.
                    //Source = new BitmapImage(new Uri("pack://application:,,,/MathFunctionWPF;component/Icons/SearchIcon.svg")), // Путь к вашему SVG или PNG изображению

                    Source = refrenceItem.Image,
                    //Source = Drawing.LoaderIcon.GetImageSVG(MathFunctionWPF.Resources.Resource1.search_svgrepo_com),
                    Width = 40,
                    Height = 40
                };
                
                description.MethodImage.Content = icon;
                description.MethodDescription.Text = refrenceItem.ShortDescription;
                description.MethodName.Text = refrenceItem.Title;

                _view.ListItems.Children.Add(description);
            }
        }

        public void MethodChanged(TypeMathMethod newMethod)
        {
            //throw new NotImplementedException();
        }

        public Control View { get => _view; }

        void StringFilterElements(string searchingString)
        {
            if (searchingString != "")
            {
                //SearchModel.ListItems.ToArray()
                SearchModel model = new SearchModel(SearchModel.ListItems);
                var items = model.Search(searchingString);
                FillElemets(items.ToArray());
            }
            else
            {
                FillElemets(SearchModel.ListItems.ToArray());
            }
        }
    }
}
