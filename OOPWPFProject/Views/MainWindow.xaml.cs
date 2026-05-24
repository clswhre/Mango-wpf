using OOPWPFProject.ViewModels;
using Wpf.Ui.Controls;

namespace OOPWPFProject;

public partial class MainWindow : FluentWindow
{
	public MainWindow() => InitializeComponent();
    protected override void OnClosed( EventArgs e )
    {
        base.OnClosed( e );
        if ( DataContext is MainViewModel vm )
        {
            vm.DetailsTabViewModel.Dispose();
            vm.StatisticTabViewModel.Dispose();
            vm.WeatherTabViewModel.Dispose();

            vm.MainContentViewModel.Dispose();
            vm.LeftPanelViewModel.Dispose();
        }
    }
}
