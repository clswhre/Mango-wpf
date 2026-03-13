using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Wpf.Ui.Appearance;
using OOPWPFProject.ViewModels;


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