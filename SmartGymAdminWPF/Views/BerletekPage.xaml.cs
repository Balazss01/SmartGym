using SmartGymAdminWPF.Services;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SmartGymAdminWPF.Views
{
    public partial class BerletekPage : Page
    {
        public BerletekPage()
        {
            InitializeComponent();
            Loaded += BerletekPage_Loaded;
        }

        private async void BerletekPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ApiService.Token))
            {
                var main = (MainWindow)Application.Current.MainWindow;
                main.MainFrame.Navigate(new LoginPage());
                return;
            }

            await LoadBerletek();
        }

        private async Task LoadBerletek()
        {
            try
            {
                var api = new ApiService();
                var json = await api.Get("api/Berletek");

                var berletek = JsonSerializer.Deserialize<List<BerletListDto>>(json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<BerletListDto>();

                BerletekGrid.ItemsSource = berletek;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bérletek betöltési hiba: " + ExtractMessage(ex.Message));
            }
        }

        private string ExtractMessage(string raw)
        {
            try
            {
                using var doc = JsonDocument.Parse(raw);

                if (doc.RootElement.TryGetProperty("message", out var msg))
                    return msg.GetString() ?? raw;
            }
            catch
            {
            }

            return raw;
        }
    }

    public class BerletListDto
    {
        public int BerletId { get; set; }
        public int TagId { get; set; }
        public string TeljesNev { get; set; }
        public DateTime KezdetDatum { get; set; }
        public DateTime VegeDatum { get; set; }
        public bool Aktiv { get; set; }
        public string BerletTipusNev { get; set; }
    }
}