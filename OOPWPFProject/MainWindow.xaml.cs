using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using Wpf.Ui.Appearance;

namespace OOPWPFProject;

public partial class MainWindow : Window
{
    public class Place(string nameOfPlace, string country, string description, double? rating, DateOnly? dateOfVisiting)
    {
        public string NameOfPlace { get; set; } = nameOfPlace;
        public string Country { get; set; } = country;
        public string Description { get; set; } = description;
        public double? Rating { get; set; } = rating;
        public DateOnly? DateOfVisiting { get; set; } = dateOfVisiting;
    }
    public ObservableCollection<Place> Places { get; set; } = [];

    public MainWindow()
    {
        InitializeComponent();
        ApplicationThemeManager.Apply(this);
        DataContext = this;
    }
    private async void AddPlacePressed(object sender, RoutedEventArgs e)
    {
        List<string> missingFields = [];

        if (string.IsNullOrWhiteSpace(CityTextBox.Text))
        {
            missingFields.Add("Назва міста");
        }

        if (string.IsNullOrWhiteSpace(CountryTextBox.Text))
        {
            missingFields.Add("Країна");
        }

        if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
        {
            missingFields.Add("Опис");
        }

        if (missingFields.Count > 0)
        {
            var errorDialog = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Помилка запису!",
                Content = "Неможливо додати запис. Не задані обов'язкові поля:\n\n" + string.Join("\n", missingFields),
                CloseButtonText = "ОК"
            };
            await errorDialog.ShowDialogAsync();
            return;
        }

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

}