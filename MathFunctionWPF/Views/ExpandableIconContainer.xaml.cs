using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MathFunctionWPF.Views
{
    /// <summary>
    /// Логика взаимодействия для ExpandableIconContainer.xaml
    /// </summary>
    public partial class ExpandableIconContainer : UserControl
    {
        public ExpandableIconContainer()
        {
            InitializeComponent();
        }

        private void FinishedLoad(object? sender, EventArgs e)
        {
            var expandAnimation = new DoubleAnimation
            {
                From = IconContainer.Height,
                To = IconContainer.ActualHeight,
                Duration = new Duration(TimeSpan.FromSeconds(0.1))
            };

            IconContainer.BeginAnimation(HeightProperty, expandAnimation);
        }

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            if (IconContainer.Visibility == Visibility.Collapsed)
            {
                // Show the icon container and play the expand animation
                IconContainer.Visibility = Visibility.Visible;
                Storyboard expandStoryboard = (Storyboard)FindResource("ExpandAnimation");
                expandStoryboard.Completed += FinishedLoad;
                expandStoryboard.Begin();



                //ExpandButton.Content = "Collapse";
            }
            else
            {
                // Play the collapse animation and hide the icon container after it finishes
                //Storyboard collapseStoryboard = (Storyboard)FindResource("CollapseAnimation");
                //collapseStoryboard.Completed += (s, args) =>
                //{
                //    IconContainer.Visibility = Visibility.Collapsed;
                //};


                var expandAnimation = new DoubleAnimation
                {
                    From = IconContainer.ActualHeight,
                    To = 0,
                    Duration = new Duration(TimeSpan.FromSeconds(0.1))
                };

                expandAnimation.Completed += (s, args) =>
                {
                    IconContainer.Visibility = Visibility.Collapsed;
                };
                IconContainer.BeginAnimation(HeightProperty, expandAnimation);

                //IconContainer.Visibility = Visibility.Collapsed;
                //ExpandButton.Content = "Expand";
            }
        }

        public StackPanel Panel { get => IconContainer; }
        public ToggleButton Button { get => ExpandButton; }
    }
}
