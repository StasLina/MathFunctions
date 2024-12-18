using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static MathFunctionWPF.Views.FunctionOutputViewIntersection;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using SkiaSharp;
using MathFunctionWPF.Themes;
using MathFunctionWPF.Controllers.Drawing;

namespace MathFunctionWPF.Views
{
    /// <summary>
    /// Логика взаимодействия для MethodListControl.xaml
    /// </summary>
    public partial class MethodListControl : UserControl
    {
        public delegate void MethodChangeDelegate(TypeMathMethod typeMethod);
        public event MethodChangeDelegate MethodChanged;

        static readonly double ItemWidth = 40;
        static readonly double  ItemHeight = 40;
        public MethodListControl()
        {
            InitializeComponent();

            var listItems = ButtonPanel.Children;

            // Домашний экран
            listItems.Add(AddButtonIconSVG(MathFunctionWPF.Resources.Resource1.house_svgrepo_com, TypeMathMethod.MainMenu));
            
            // Поиск
            listItems.Add(AddButtonIconSVG(MathFunctionWPF.Resources.Resource1.search_svgrepo_com, TypeMathMethod.Search));

            ExpandableIconContainer chislMethods = AddExtandableButton(LoaderIcon.LoadPngImage(MathFunctionWPF.Resources.Resource1.chisl_methods));

            chislMethods.Panel.Children.Add(AddButtonIconSVG(MathFunctionWPF.Resources.Resource1.d_svgrepo_com, TypeMathMethod.Bisection));
            chislMethods.Panel.Children.Add(AddButtonIconSVG(MathFunctionWPF.Resources.Resource1.n_svgrepo_com, TypeMathMethod.Newton));
            chislMethods.Panel.Children.Add(AddButtonIconSVG(MathFunctionWPF.Resources.Resource1.s_svgrepo_com, TypeMathMethod.GoldenSearch));
            chislMethods.Panel.Children.Add(AddButtonIconSVG(MathFunctionWPF.Resources.Resource1.c_svgrepo_com, TypeMathMethod.CoordinateDesent));
            
            listItems.Add(chislMethods);


            ExpandableIconContainer sortMethod = AddExtandableButton(LoaderIcon.LoadPngImage(MathFunctionWPF.Resources.Resource1.alg_sortirovki));
            sortMethod.Panel.Children.Add(AddButtonIconSVG(MathFunctionWPF.Resources.Resource1.bubble_chart_svgrepo_com, TypeMathMethod.BubbleSort));
            listItems.Add(sortMethod);

            listItems.Add(AddButtonIconSVG(MathFunctionWPF.Resources.Resource1.integral_svgrepo_com, TypeMathMethod.Integral));
            listItems.Add(AddButtonIconSVG(MathFunctionWPF.Resources.Resource1.research_matrix_svgrepo_com, TypeMathMethod.SLAU));
            listItems.Add(AddButtonIconSVG(MathFunctionWPF.Resources.Resource1.z_svgrepo_com, TypeMathMethod.MNK));
            //ExpandableIconContainer integr = AddExtandableButton(LoaderIcon.LoadPngImage(MathFunctionWPF.Resources.Resource1.alg_sortirovki));
            //listItems.Add(AddButton("I", TypeMathMethod.Integral));
            //listItems.Add(AddButton("T", TypeMathMethod.Test));
            //listItems.Add(AddButton("I", TypeMathMethod.Integration));


        }

        private Button AddButton(string name, TypeMathMethod method)
        {
            Button button = new Button
            {
                Width = ItemWidth, // Ширина кнопки
                Height = ItemHeight, // Высота кнопки для создания квадратной формы
                Content = name, // Текст на кнопке
                Margin = new Thickness(5) // Отступы между кнопками
            };
            // Подписка на событие нажатия на кнопку
            button.Click += Button_Click;

            // Добавляем кнопку в StackPanel
            //ButtonPanel.Children.Add(button);

            MethodButtonModel methodButtonModel = new MethodButtonModel();
            button.DataContext = methodButtonModel;
            methodButtonModel.Method = method;

            return button;
        }
        
        private Button AddButtonIconSVG(byte[] svgData, TypeMathMethod method)
        {
            Button button = new Button
            {
                Width = ItemWidth, // Ширина кнопки
                Height = ItemHeight, // Высота кнопки для создания квадратной формы
                Margin = new Thickness(5) // Отступы между кнопками
            };

            // Создаем элемент Image и загружаем изображение (например, в формате SVG)
            //Uri uri = new Uri("pack://application:,,,/Resources/searchicon.svg", UriKind.Absolute);
            //var stream = Application.GetResourceStream(uri)?.Stream;
            //System.IO.Stream stream = new Stream();
            //MathFunctionWPF.Resources.Resource1.search_svgrepo_com;'

            // Создание MemoryStream из массива байтов
            SKBitmap bitMap;
            using (MemoryStream svgStream = new MemoryStream(svgData))
            {
                bitMap= Controllers.Drawing.LoaderIcon.LoadSvgIcon(svgStream, 200, 200);
            }

            Image icon = new Image
            {
                //Proper
                //Properties.Resources.
                //Source = new BitmapImage(new Uri("pack://application:,,,/MathFunctionWPF;component/Icons/SearchIcon.svg")), // Путь к вашему SVG или PNG изображению
                
                Source = Controllers.Drawing.LoaderIcon.ConvertToBitmapImage(bitMap), 
                Width = ItemWidth-10, // Ширина иконки
                Height = ItemHeight-10 // Высота иконки
            };
            
            // Устанавливаем изображение как содержимое кнопки
            button.Content = icon;

            // Подписка на событие нажатия на кнопку
            button.Click += Button_Click;

            // Добавляем кнопку в S tackPanel
            //ButtonPanel.Children.Add(button);

            // Если есть необходимость, можно использовать DataContext
            MethodButtonModel methodButtonModel = new MethodButtonModel();
            button.DataContext = methodButtonModel;
            methodButtonModel.Method = method;

            return button;
        }


        private Button AddButtonIconRastr(byte[] restData, TypeMathMethod method)
        {
            Button button = new Button
            {
                Width = ItemWidth, // Ширина кнопки
                Height = ItemHeight, // Высота кнопки для создания квадратной формы
                Margin = new Thickness(5) // Отступы между кнопками
            };

            Image icon = new Image
            {
                //Proper
                //Properties.Resources.
                //Source = new BitmapImage(new Uri("pack://application:,,,/MathFunctionWPF;component/Icons/SearchIcon.svg")), // Путь к вашему SVG или PNG изображению

                Source = Controllers.Drawing.LoaderIcon.LoadPngImage(restData),
                Width = ItemWidth, // Ширина иконки
                Height = ItemHeight // Высота иконки
            };

            // Устанавливаем изображение как содержимое кнопки
            button.Content = icon;

            // Подписка на событие нажатия на кнопку
            button.Click += Button_Click;

            // Добавляем кнопку в S tackPanel
            //ButtonPanel.Children.Add(button);

            // Если есть необходимость, можно использовать DataContext
            MethodButtonModel methodButtonModel = new MethodButtonModel();
            button.DataContext = methodButtonModel;
            methodButtonModel.Method = method;

            return button;
        }

        private ExpandableIconContainer AddExtandableButton(BitmapImage image)
        {
            ExpandableIconContainer iconContainer = new ExpandableIconContainer()
            {
                Margin = new Thickness(5)
            };

            Image icon = new Image
            {
                //Proper
                //Properties.Resources.
                //Source = new BitmapImage(new Uri("pack://application:,,,/MathFunctionWPF;component/Icons/SearchIcon.svg")), // Путь к вашему SVG или PNG изображению

                Source = image,
                Width = ItemWidth, // Ширина иконки
                Height = ItemHeight // Высота иконки
            };


            iconContainer.Button.Width = ItemWidth;
            iconContainer.Button.Height = ItemHeight;
            iconContainer.Button.Content = icon;
            //ButtonPanel.Children.Add(icon);

            return iconContainer;
        }

        // Обработчик нажатия на кнопку
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            if (button != null)
            {
                MethodButtonModel? dataContext = button.DataContext as MethodButtonModel;
                if (dataContext != null)
                {
                    MethodChanged(dataContext.Method);
                }
            }

            // Показываем модальное окно при нажатии на кнопку
            //MessageBox.Show($"You clicked {button.Content}", "Button Clicked", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public enum TypeMathMethod
    {
        Bisection, GoldenSearch, Test, Integration, Newton, CoordinateDesent, MainMenu, Search, BubbleSort, Integral, SLAU, MNK
    };


    public class MethodButtonModel : INotifyPropertyChanged
    {

        public TypeMathMethod Method
        {
            get; set;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

}
