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

namespace MathFunctionWPF.Views
{
    /// <summary>
    /// Логика взаимодействия для MathFunctionSearchView.xaml
    /// </summary>
    public partial class MathFunctionSearchView : UserControl
    {
        public delegate void RouteFilterElementHandlers(string searchingString);

        RouteFilterElementHandlers ?handler;
        public MathFunctionSearchView()
        {
            InitializeComponent();
            this.BSearch.Click += SearchEventHandler;
        }

        void SearchEventHandler(object sender, RoutedEventArgs e)
        {
            handler.Invoke(TextInput.Text);
        }

        public void AddHandler(RouteFilterElementHandlers e)
        {
            handler += e;
        }


    }
}
