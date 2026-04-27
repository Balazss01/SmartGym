using SmartGymAdminWPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SmartGymAdminWPF.Views
{
    public partial class DashboardPage : Page
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        public DashboardPage()
        {
            InitializeComponent();
            Loaded += DashboardPage_Loaded;
            Unloaded += DashboardPage_Unloaded;
            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += Timer_Tick;
        }

        private async void DashboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ApiService.Token))
            {
                var main = (MainWindow)Application.Current.MainWindow;
                main.MainFrame.Navigate(new LoginPage());
                return;
            }

            await ReloadAll();

            _timer.Start();
            AutoRefreshCheckBox.IsChecked = true;
        }

        private void DashboardPage_Unloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }

        private async void Timer_Tick(object? sender, EventArgs e)
        {
            await ReloadAll();
        }

        private async Task ReloadAll()
        {
            await LoadStats();
            await LoadBentLevok();
            await LoadUtolsoBelepesek();
            await LoadHetiBelepesek();
            await LoadBerletMegoszlas();

            FrissitveText.Text = $"Utolsó frissítés: {DateTime.Now:yyyy.MM.dd HH:mm:ss}";
        }

        private async Task LoadStats()
        {
            try
            {
                var api = new ApiService();
                var json = await api.Get("api/AdminDashboard/stats");

                var stats = JsonSerializer.Deserialize<DashboardStatsDto>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (stats == null) return;

                OsszesTagText.Text = stats.OsszesTag.ToString();
                AktivTagText.Text = stats.AktivTag.ToString();
                OsszesBerletText.Text = stats.OsszesBerlet.ToString();
                AktivBerletText.Text = stats.AktivBerlet.ToString();
                EloreMegvasaroltBerletText.Text = stats.EloreMegvasaroltBerlet.ToString();
                MaiBelepesText.Text = stats.MaiBelepesek.ToString();
                BentLevokText.Text = stats.BentLevok.ToString();
                SzekrenyFoglalasText.Text = stats.AktivSzekrenyFoglalasok.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dashboard stat hiba: " + ex.Message);
            }
        }

        private async Task LoadBentLevok()
        {
            try
            {
                var api = new ApiService();
                var json = await api.Get("api/AdminDashboard/bent-levok");

                var lista = JsonSerializer.Deserialize<List<BentLevoDto>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                BentLevokGrid.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bent lévők hiba: " + ex.Message);
            }
        }

        private async Task LoadUtolsoBelepesek()
        {
            try
            {
                var api = new ApiService();
                var json = await api.Get("api/AdminDashboard/utolso-belepesek");

                var lista = JsonSerializer.Deserialize<List<UtolsoBelepesDto>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                UtolsoBelepesekGrid.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Utolsó belépések hiba: " + ex.Message);
            }
        }

        private async Task LoadHetiBelepesek()
        {
            try
            {
                var api = new ApiService();
                var json = await api.Get("api/AdminDashboard/heti-belepesek");

                var lista = JsonSerializer.Deserialize<List<HetiBelepesDto>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                var max = lista.Any() ? Math.Max(lista.Max(x => x.Darab), 1) : 1;

                foreach (var item in lista)
                {
                    item.MaxErtek = max;
                }

                HetiBelepesekItemsControl.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Heti belépések hiba: " + ex.Message);
            }
        }

        private async Task LoadBerletMegoszlas()
        {
            try
            {
                var api = new ApiService();
                var json = await api.Get("api/AdminDashboard/berlet-megoszlas");

                var adat = JsonSerializer.Deserialize<BerletMegoszlasDto>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (adat == null) return;

                var total = Math.Max(adat.Aktiv + adat.EloreMegvasarolt + adat.Lejart, 1);

                AktivBerletProgress.Maximum = total;
                AktivBerletProgress.Value = adat.Aktiv;
                AktivBerletAranyText.Text = $"{adat.Aktiv} aktív";

                EloreMegvasaroltProgress.Maximum = total;
                EloreMegvasaroltProgress.Value = adat.EloreMegvasarolt;
                EloreMegvasaroltAranyText.Text = $"{adat.EloreMegvasarolt} előre megvásárolt";

                LejartBerletProgress.Maximum = total;
                LejartBerletProgress.Value = adat.Lejart;
                LejartBerletAranyText.Text = $"{adat.Lejart} lejárt / inaktív";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bérlet megoszlás hiba: " + ex.Message);
            }
        }

        private async void FrissitesButton_Click(object sender, RoutedEventArgs e)
        {
            await ReloadAll();
        }

        private void AutoRefreshCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _timer.Start();
        }

        private void AutoRefreshCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }
    }

    public class DashboardStatsDto
    {
        public int OsszesTag { get; set; }
        public int AktivTag { get; set; }
        public int OsszesBerlet { get; set; }
        public int AktivBerlet { get; set; }
        public int EloreMegvasaroltBerlet { get; set; }
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

    public class BentLevoDto
    {
        public int BelepesId { get; set; }
        public int TagId { get; set; }
        public string TeljesNev { get; set; }
        public DateTime BelepesIdopont { get; set; }
    }

    public class HetiBelepesDto
    {
        public string Datum { get; set; }
        public int Darab { get; set; }
        public int MaxErtek { get; set; }
    }

    public class BerletMegoszlasDto
    {
        public int Aktiv { get; set; }
        public int Lejart { get; set; }
        public int EloreMegvasarolt { get; set; }
    }
}