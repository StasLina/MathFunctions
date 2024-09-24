namespace MathFunctions
{
    internal static class Programm
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            MainWindowForm mainWindowForm = new MainWindowForm();
            MainWindowFormController controller = new MainWindowFormController(mainWindowForm);
            Application.Run(mainWindowForm);
        }
    }
}