using OOPWPFProject.ViewModels;
using System.Windows;
using Wpf.Ui.Appearance;

namespace OOPWPFProject;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ApplicationThemeManager.Apply(this);
        DataContext = new MainViewModel();
    }
}