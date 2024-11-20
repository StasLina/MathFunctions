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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для InputNumbers.xaml
    /// </summary>
    public partial class WInputNumbers : Window
    {
        public WInputNumbers(List<double> items)
        {
            InitializeComponent();

            UserControl1 userControl1 = new UserControl1(items);
            Content = userControl1;
            Show();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            SizeToContent = SizeToContent.WidthAndHeight;
            ResizeMode = ResizeMode.NoResize;

            Closing += (object? sender, System.ComponentModel.CancelEventArgs e) => {
                if (userControl1.IsDataChange)
                {
                    // Спрашиваем пользователя
                    MessageBoxResult result = MessageBox.Show(
                        "Данные не сохранены, сохранить?", // Текст сообщения
                        "Сохранение", // Заголовок окна
                        MessageBoxButton.YesNo, // Кнопки "Да" и "Нет"
                        MessageBoxImage.Question // Иконка вопроса
                    );

                    // Обработка результата
                    if (result == MessageBoxResult.Yes)
                    {
                        userControl1.Save();
                    }
                    else
                    {
                    }
                }
            };
        }
    }
}
