// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinearBarView.xaml.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AnimationsDemo
{
    using System.Threading.Tasks;
    using System.Windows;

    public partial class AnimationSettingsControl
    {
        public AnimationSettingsControl()
        {
            this.InitializeComponent();
        }

        CancellationTokenSource cancellationTokenSource;
        private async void OnAnimateClick(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as IAnimationViewModel;
            if (vm != null)
            {
                if (vm.IsAnimationRuning == false)
                {
                    cancellationTokenSource = new CancellationTokenSource();
                    var cancellationToken = cancellationTokenSource.Token;
                    vm.IsAnimationRuning = true;

                    try
                    {
                        // Ожидаем завершение задачи
                       
                        await Task.Run(async () =>
                        {
                            vm.AnimateAsync(cancellationToken).Wait();
                            vm.IsAnimationRuning = false;
                        });
                    }
                    catch (OperationCanceledException)
                    {
                    }
     
                }
                else
                {
                    cancellationTokenSource.Cancel();
                }
            }
        }
    }
}