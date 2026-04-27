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
    public partial class BerletekPage : Page
    {
        private List<BerletListDto> _osszesBerlet = new();
        private readonly DispatcherTimer _timer = new();
        private bool _isUnloading = false;

        public BerletekPage()
        {
            InitializeComponent();

            Loaded += BerletekPage_Loaded;
            Unloaded += BerletekPage_Unloaded;

            _timer.Interval = TimeSpan.FromSeconds(20);
            _timer.Tick += async (_, __) => await LoadBerletek();
        }

        private async void BerletekPage_Loaded(object sender, RoutedEventArgs e)
        {
            _isUnloading = false;

            if (string.IsNullOrWhiteSpace(ApiService.Token))
                return;

            _timer.Start();
            await LoadBerletek();
        }

        private void BerletekPage_Unloaded(object sender, RoutedEventArgs e)
        {
            _isUnloading = true;
            _timer.Stop();
        }

        private async Task LoadBerletek()
        {
            if (_isUnloading || string.IsNullOrWhiteSpace(ApiService.Token))
                return;

            try
            {
                var api = new ApiService();
                var json = await api.Get("api/Berletek");

                if (_isUnloading || string.IsNullOrWhiteSpace(ApiService.Token))
                    return;

                _osszesBerlet = JsonSerializer.Deserialize<List<BerletListDto>>(json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new();

                LoadTipusok();
                UpdateStats();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                if (_isUnloading || string.IsNullOrWhiteSpace(ApiService.Token))
                    return;

                MessageBox.Show("Bérletek betöltési hiba: " + ExtractMessage(ex.Message));
            }
        }

        private void LoadTipusok()
        {
            if (BerletTipusComboBox == null)
                return;

            var tipusok = _osszesBerlet
                .Select(b => b.BerletTipusNev)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            BerletTipusComboBox.Items.Clear();
            BerletTipusComboBox.Items.Add(new ComboBoxItem { Content = "Összes típus" });

            foreach (var tipus in tipusok)
                BerletTipusComboBox.Items.Add(new ComboBoxItem { Content = tipus });

            BerletTipusComboBox.SelectedIndex = 0;
        }

        private void UpdateStats()
        {
            var most = DateTime.Now;

            OsszesBerletText.Text = _osszesBerlet.Count.ToString();

            AktivBerletText.Text = _osszesBerlet.Count(b =>
                b.Aktiv && b.KezdetDatum <= most && b.VegeDatum > most).ToString();

            LejartBerletText.Text = _osszesBerlet.Count(b =>
                b.VegeDatum <= most || !b.Aktiv).ToString();
        }

        private void ApplyFilter()
        {
            if (_isUnloading) return;

            var keresett = KeresesTextBox.Text?.Trim().ToLower() ?? "";
            var csakAktiv = CsakAktivCheckBox.IsChecked == true;
            var csakLejart = CsakLejartCheckBox.IsChecked == true;

            var selectedTipus =
                (BerletTipusComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString()
                ?? "Összes típus";

            var szurt = _osszesBerlet
                .Where(b =>
                    (!csakAktiv || b.Statusz == "AKTÍV") &&
                    (!csakLejart || b.Statusz == "LEJÁRT") &&
                    (selectedTipus == "Összes típus" || b.BerletTipusNev == selectedTipus) &&
                    (
                        string.IsNullOrWhiteSpace(keresett) ||
                        b.BerletId.ToString().Contains(keresett) ||
                        b.TagId.ToString().Contains(keresett) ||
                        (b.TeljesNev?.ToLower().Contains(keresett) ?? false) ||
                        (b.BerletTipusNev?.ToLower().Contains(keresett) ?? false) ||
                        b.Statusz.ToLower().Contains(keresett)
                    ))
                .OrderByDescending(b => b.KezdetDatum)
                .ToList();

            BerletekGrid.ItemsSource = szurt;
        }

        private void KeresesTextBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilter();

        private void FilterChanged(object sender, RoutedEventArgs e) => ApplyFilter();

        private void FilterChanged(object sender, SelectionChangedEventArgs e) => ApplyFilter();

        private async void FrissitesButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadBerletek();
        }

        private async void ToggleBerlet_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ApiService.Token))
                return;

            try
            {
                var button = sender as Button;
                var berlet = button?.DataContext as BerletListDto;

                if (berlet == null)
                    return;

                var api = new ApiService();

                var json = JsonSerializer.Serialize(new UpdateBerletDto
                {
                    BerletTipusId = berlet.BerletTipusId,
                    KezdetDatum = berlet.KezdetDatum,
                    VegeDatum = berlet.VegeDatum,
                    Aktiv = !berlet.Aktiv
                });

                await api.Put($"api/Berletek/{berlet.BerletId}", json);
                await LoadBerletek();
            }
            catch (Exception ex)
            {
                if (string.IsNullOrWhiteSpace(ApiService.Token))
                    return;

                MessageBox.Show("Toggle hiba: " + ex.Message);
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
            catch { }

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
        public int BerletTipusId { get; set; }

        public string Statusz
        {
            get
            {
                var most = DateTime.Now;

                if (Aktiv && KezdetDatum > most)
                    return "ELŐRE MEGVÁSÁROLT";

                if (Aktiv && KezdetDatum <= most && VegeDatum > most)
                    return "AKTÍV";

                return "LEJÁRT";
            }
        }
    }

    public class UpdateBerletDto
    {
        public int BerletTipusId { get; set; }
        public DateTime KezdetDatum { get; set; }
        public DateTime VegeDatum { get; set; }
        public bool Aktiv { get; set; }
    }
}