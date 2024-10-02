using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathFunctionWPF
{
    internal class App : Application
    {
        [STAThread]
        public static int Main(string[] args)
        {
            // Устанавливаем культуру с десятичной запятой
            CultureInfo culture = CultureInfo.InvariantCulture; // Французская культура использует запятую
            Thread.CurrentThread.CurrentCulture = culture;

            App app = new App();
            // Setup your application as you want before running it
            MainWindow mainWindow = new MainWindow();
            Controllers.MainWindowController controller = new Controllers.MainWindowController(mainWindow);
            return app.Run(mainWindow);
        }

        public App()
        {
            // (Optional) Load your application resources file (which has a "Page" build action, not "ApplicationDefinition",
            // and a root node of type "ResourceDictionary", not "Application")
            //Resources = (ResourceDictionary)Application.LoadComponent(new Uri("/MyAssemblyName;component/Resources.xaml", UriKind.Relative));
        }
    }
}
