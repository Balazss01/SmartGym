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
    public partial class BerletekPage : Page
    {
        private List<BerletListDto> _osszesBerlet = new List<BerletListDto>();

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

                _osszesBerlet = JsonSerializer.Deserialize<List<BerletListDto>>(json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<BerletListDto>();

                LoadTipusok();
                UpdateStats();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bérletek betöltési hiba: " + ExtractMessage(ex.Message));
            }
        }

        private void LoadTipusok()
        {
            if (BerletTipusComboBox == null)
                return;

            var aktualis = (BerletTipusComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

            var tipusok = _osszesBerlet
                .Select(b => b.BerletTipusNev)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            BerletTipusComboBox.Items.Clear();
            BerletTipusComboBox.Items.Add(new ComboBoxItem { Content = "Összes típus" });

            foreach (var tipus in tipusok)
            {
                BerletTipusComboBox.Items.Add(new ComboBoxItem { Content = tipus });
            }

            if (!string.IsNullOrWhiteSpace(aktualis))
            {
                foreach (ComboBoxItem item in BerletTipusComboBox.Items)
                {
                    if ((item.Content?.ToString() ?? "") == aktualis)
                    {
                        BerletTipusComboBox.SelectedItem = item;
                        return;
                    }
                }
            }

            BerletTipusComboBox.SelectedIndex = 0;
        }

        private void UpdateStats()
        {
            if (OsszesBerletText == null || AktivBerletText == null || LejartBerletText == null)
                return;

            var most = DateTime.Now;
            var osszes = _osszesBerlet.Count;
            var aktiv = _osszesBerlet.Count(b => b.Aktiv && b.VegeDatum > most);
            var lejart = _osszesBerlet.Count(b => b.VegeDatum <= most || !b.Aktiv);

            OsszesBerletText.Text = osszes.ToString();
            AktivBerletText.Text = aktiv.ToString();
            LejartBerletText.Text = lejart.ToString();
        }

        private void ApplyFilter()
        {
            if (BerletekGrid == null ||
                KeresesTextBox == null ||
                CsakAktivCheckBox == null ||
                CsakLejartCheckBox == null ||
                BerletTipusComboBox == null)
                return;

            var keresett = KeresesTextBox.Text?.Trim().ToLower() ?? "";
            var csakAktiv = CsakAktivCheckBox.IsChecked == true;
            var csakLejart = CsakLejartCheckBox.IsChecked == true;
            var selectedTipus = (BerletTipusComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Összes típus";
            var most = DateTime.Now;

            var szurt = _osszesBerlet
                .Where(b =>
                    (!csakAktiv || (b.Aktiv && b.VegeDatum > most)) &&
                    (!csakLejart || (b.VegeDatum <= most || !b.Aktiv)) &&
                    (selectedTipus == "Összes típus" || b.BerletTipusNev == selectedTipus) &&
                    (
                        string.IsNullOrWhiteSpace(keresett) ||
                        b.BerletId.ToString().Contains(keresett) ||
                        b.TagId.ToString().Contains(keresett) ||
                        (b.TeljesNev?.ToLower().Contains(keresett) ?? false) ||
                        (b.BerletTipusNev?.ToLower().Contains(keresett) ?? false)
                    ))
                .OrderByDescending(b => b.KezdetDatum)
                .ToList();

            BerletekGrid.ItemsSource = szurt;
        }

        private void KeresesTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private async void FrissitesButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadBerletek();
        }

        private async void ToggleBerlet_Click(object sender, RoutedEventArgs e)
        {
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
        public int BerletTipusId { get; set; }
    }

    public class UpdateBerletDto
    {
        public int BerletTipusId { get; set; }
        public DateTime KezdetDatum { get; set; }
        public DateTime VegeDatum { get; set; }
        public bool Aktiv { get; set; }
    }
}