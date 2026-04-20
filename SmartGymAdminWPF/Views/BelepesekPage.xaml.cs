using SmartGymAdminWPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SmartGymAdminWPF.Views
{
    public partial class BelepesekPage : Page
    {
        private List<BelepesDto> _osszes = new();

        public BelepesekPage()
        {
            InitializeComponent();
            Loaded += BelepesekPage_Loaded;
        }

        private async void BelepesekPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ApiService.Token))
            {
                var main = (MainWindow)Application.Current.MainWindow;
                main.MainFrame.Navigate(new LoginPage());
                return;
            }

            await Load();
        }

        private async Task Load()
        {
            try
            {
                var api = new ApiService();
                var json = await api.Get("api/Belepesek");

                _osszes = JsonSerializer.Deserialize<List<BelepesDto>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                UpdateStats();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba: " + ex.Message);
            }
        }

        private void UpdateStats()
        {
            var now = DateTime.Now.Date;

            OsszesText.Text = _osszes.Count.ToString();
            MaiText.Text = _osszes.Count(x => x.BelepesIdopont.Date == now).ToString();
            BentText.Text = _osszes.Count(x => x.KilepesIdopont == null).ToString();
        }

        private void ApplyFilter()
        {
            var keres = KeresesTextBox.Text?.ToLower() ?? "";
            var csakMa = CsakMaCheckBox.IsChecked == true;
            var csakBent = CsakBentCheckBox.IsChecked == true;
            var today = DateTime.Now.Date;

            var lista = _osszes
                .Where(x =>
                    (!csakMa || x.BelepesIdopont.Date == today) &&
                    (!csakBent || x.KilepesIdopont == null) &&
                    (
                        string.IsNullOrWhiteSpace(keres) ||
                        x.BelepesId.ToString().Contains(keres) ||
                        x.TagId.ToString().Contains(keres) ||
                        (x.TeljesNev?.ToLower().Contains(keres) ?? false)
                    ))
                .OrderByDescending(x => x.BelepesIdopont)
                .ToList();

            BelepesekGrid.ItemsSource = lista;
        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private async void FrissitesButton_Click(object sender, RoutedEventArgs e)
        {
            await Load();
        }
    }

    public class BelepesDto
    {
        public int BelepesId { get; set; }
        public int TagId { get; set; }
        public string TeljesNev { get; set; }
        public DateTime BelepesIdopont { get; set; }
        public DateTime? KilepesIdopont { get; set; }
    }
}