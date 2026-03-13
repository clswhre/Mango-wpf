using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Wpf.Ui.Appearance;
using OOPWPFProject.ViewModels;


namespace OOPWPFProject;

public partial class MainWindow : Window
{
    //public class Place(string nameOfPlace, string country, string description, double? rating, DateOnly? dateOfVisiting)
    //{
    //    public string NameOfPlace { get; set; } = nameOfPlace;
    //    public string Country { get; set; } = country;
    //    public string Description { get; set; } = description;
    //    public double? Rating { get; set; } = rating;
    //    public DateOnly? DateOfVisiting { get; set; } = dateOfVisiting;
    //}
    //public ObservableCollection<Place> Places { get; set; } = [];

    public MainWindow()
    {
        InitializeComponent();
        ApplicationThemeManager.Apply(this);
        DataContext = new MainViewModel();
    }
    /**
    private async void AddPlacePressed(object sender, RoutedEventArgs e)
    {

        DateOnly? visitDate = DatePickerBox.SelectedDate.HasValue ? DateOnly.FromDateTime(DatePickerBox.SelectedDate.Value) : null;

        double? rating = Rating.Value > 0 ? Rating.Value : null;


        var newPlace = new Place(CityTextBox.Text,
                                CountryTextBox.Text,
                                DescriptionTextBox.Text,
                                rating,
                                visitDate);

        Places.Add(newPlace);

        StringBuilder messageBuilder = new();
        messageBuilder.AppendLine($"Місто: {CityTextBox.Text}");
        messageBuilder.AppendLine($"Країна: {CountryTextBox.Text}");
        messageBuilder.AppendLine($"Опис: {DescriptionTextBox.Text}");

        if (visitDate.HasValue)
        {
            messageBuilder.AppendLine($"Дата: {visitDate.Value:dd.MM.yyyy}");
        }

        if (rating.HasValue)
        {
            messageBuilder.AppendLine($"Рейтинг: {rating.Value}");
        }

        var successDialog = new Wpf.Ui.Controls.MessageBox
        {
            Title = "Успіх",
            Content = messageBuilder.ToString().TrimEnd(),
            CloseButtonText = "ОК"
        };

        await successDialog.ShowDialogAsync();

        ClearForm();
    }

    private void ClearFormPressed(object sender, RoutedEventArgs e)
    {
        ClearForm();
    }

    private async void DeletePlacePressed(object sender, RoutedEventArgs e)
    {
        if (PlacesDataGrid.SelectedItem is Place selectedPlace)
        {
            Places.Remove(selectedPlace);
        }
        else
        {
            var warningDialog = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Увага",
                Content = "Виберіть запис у таблиці для видалення.",
                CloseButtonText = "ОК"
            };
            await warningDialog.ShowDialogAsync();
        }
    }

    private void ClearForm()
    {
        CityTextBox.Clear();
        CountryTextBox.Clear();
        DescriptionTextBox.Clear();
        DatePickerBox.SelectedDate = null;
        Rating.Value = 0;
    }
    */
}