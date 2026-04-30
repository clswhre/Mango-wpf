using OOPWPFProject.ViewModels;

using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace OOPWPFProject;

public partial class MainWindow : FluentWindow
{
    public MainWindow ()
    {
        InitializeComponent();
        ApplicationThemeManager.Apply( this );
        DataContext = new MainViewModel();
    }
}