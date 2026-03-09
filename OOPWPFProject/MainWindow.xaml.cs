using System.Collections.Generic;
using System.Windows;
using Wpf.Ui.Appearance;

namespace OOPWPFProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary> 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ApplicationThemeManager.Apply(this);
        }

        private async void AddPlacePressed(object sender, RoutedEventArgs e)
        {

            List<string> missingFields = [];

            if (string.IsNullOrWhiteSpace(CityTextBox.Text))
                missingFields.Add(" Назва міста");

            if (string.IsNullOrWhiteSpace(CountryTextBox.Text))
                missingFields.Add(" Країна");

            if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
                missingFields.Add(" Опис");

            if (!DatePickerBox.SelectedDate.HasValue)
                missingFields.Add(" Дата відвідування");

            if (Rating.Value == 0)
                missingFields.Add(" Рейтинг");

            if (missingFields.Count > 0)
            {
                string errorMessage = "Неможливо додати запис. Не задані поля:\n\n" + string.Join("\n", missingFields);

                var errorDialog = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "Помилка запису!",
                    Content = errorMessage,
                    CloseButtonText = "ОК"
                };

                /// <summary>
                ///  Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                /// </summary> 
                errorDialog.ShowDialogAsync();
                

                return;
            }

            string city = CityTextBox.Text;
            string country = CountryTextBox.Text;
            string description = DescriptionTextBox.Text;
            string visitDate = DatePickerBox.SelectedDate.Value.ToString("dd.MM.yyyy");
            double rating = Rating.Value; 

            string message = $"Місто: {city}\nКраїна: {country}\nОпис: {description}\nДата: {visitDate}\nРейтинг: {rating}";

            var successDialog = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Успіх",
                Content = message,
                CloseButtonText = "ОК"
            };

            /// <summary>
            /// 'MessageBox.ShowDialog()' is obsolete: 'Use ShowDialogAsync instead'
            /// <summary/>
            await successDialog.ShowDialogAsync();
        }

        private void ClearFormPressed(object sender, RoutedEventArgs e)
        {
            CityTextBox.Clear();
            CountryTextBox.Clear();
            DescriptionTextBox.Clear();
            DatePickerBox.SelectedDate = null;
            Rating.Value = 0;
        }
    }
}