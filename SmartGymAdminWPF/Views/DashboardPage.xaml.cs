using SmartGymAdminWPF.Services;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SmartGymAdminWPF.Views
{
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            Loaded += DashboardPage_Loaded;
        }

        private async void DashboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ApiService.Token))
            {
                var main = (MainWindow)Application.Current.MainWindow;
                main.MainFrame.Navigate(new LoginPage());
                return;
            }

            await LoadStats();
            await LoadUtolsoBelepesek();
        }

        private async Task LoadStats()
        {
            try
            {
                var api = new ApiService();
                var json = await api.Get("api/AdminDashboard/stats");

                var stats = JsonSerializer.Deserialize<DashboardStatsDto>(json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (stats == null)
                    return;

                OsszesTagText.Text = stats.OsszesTag.ToString();
                AktivTagText.Text = stats.AktivTag.ToString();
                OsszesBerletText.Text = stats.OsszesBerlet.ToString();
                AktivBerletText.Text = stats.AktivBerlet.ToString();
                MaiBelepesText.Text = stats.MaiBelepesek.ToString();
                BentLevokText.Text = stats.BentLevok.ToString();
                SzekrenyFoglalasText.Text = stats.AktivSzekrenyFoglalasok.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dashboard stat hiba: " + ex.Message);
            }
        }

        private async Task LoadUtolsoBelepesek()
        {
            try
            {
                var api = new ApiService();
                var json = await api.Get("api/AdminDashboard/utolso-belepesek");

                var lista = JsonSerializer.Deserialize<List<UtolsoBelepesDto>>(json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<UtolsoBelepesDto>();

                UtolsoBelepesekGrid.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Utolsó belépések hiba: " + ex.Message);
            }
        }
    }

    public class DashboardStatsDto
    {
        public int OsszesTag { get; set; }
        public int AktivTag { get; set; }
        public int OsszesBerlet { get; set; }
        public int AktivBerlet { get; set; }
        public int MaiBelepesek { get; set; }
        public int BentLevok { get; set; }
        public int AktivSzekrenyFoglalasok { get; set; }
    }

    public class UtolsoBelepesDto
    {
        public int BelepesId { get; set; }
        public string TeljesNev { get; set; }
        public DateTime BelepesIdopont { get; set; }
        public DateTime? KilepesIdopont { get; set; }
    }
}