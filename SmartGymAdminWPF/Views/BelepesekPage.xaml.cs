using SmartGymAdminWPF.Services;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SmartGymAdminWPF.Views
{
    public partial class BelepesekPage : Page
    {
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

            await LoadBelepesek();
        }

        private async Task LoadBelepesek()
        {
            try
            {
                var api = new ApiService();
                var json = await api.Get("api/Belepesek");

                var data = JsonSerializer.Deserialize<List<BelepesDto>>(json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<BelepesDto>();

                BelepesekGrid.ItemsSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Belépések betöltési hiba: " + ex.Message);
            }
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