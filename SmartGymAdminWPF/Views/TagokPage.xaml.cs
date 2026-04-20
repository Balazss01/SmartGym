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
    public partial class TagokPage : Page
    {
        private List<TagDto> _osszesTag = new List<TagDto>();

        public TagokPage()
        {
            InitializeComponent();
            Loaded += TagokPage_Loaded;
        }

        private async void TagokPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ApiService.Token))
            {
                var main = (MainWindow)Application.Current.MainWindow;
                main.MainFrame.Navigate(new LoginPage());
                return;
            }

            await LoadTagok();
        }

        private async Task LoadTagok()
        {
            try
            {
                var api = new ApiService();
                var json = await api.Get("api/Tagok");

                _osszesTag = JsonSerializer.Deserialize<List<TagDto>>(json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<TagDto>();

                UpdateStats();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tagok betöltési hiba: " + ex.Message);
            }
        }

        private void UpdateStats()
        {
            var osszes = _osszesTag.Count;
            var aktiv = _osszesTag.Count(t => t.Aktiv);
            var inaktiv = osszes - aktiv;

            OsszesTagText.Text = osszes.ToString();
            AktivTagText.Text = aktiv.ToString();
            InaktivTagText.Text = inaktiv.ToString();
        }

        private void ApplyFilter()
        {
            var keresett = KeresesTextBox.Text?.Trim().ToLower() ?? "";
            var csakAktiv = CsakAktivCheckBox.IsChecked == true;

            var szurt = _osszesTag
                .Where(t =>
                    (!csakAktiv || t.Aktiv) &&
                    (
                        string.IsNullOrWhiteSpace(keresett) ||
                        (t.TeljesNev?.ToLower().Contains(keresett) ?? false) ||
                        (t.Vezeteknev?.ToLower().Contains(keresett) ?? false) ||
                        (t.Keresztnev?.ToLower().Contains(keresett) ?? false) ||
                        t.TagId.ToString().Contains(keresett)
                    ))
                .OrderBy(t => t.Vezeteknev)
                .ThenBy(t => t.Keresztnev)
                .ToList();

            TagokGrid.ItemsSource = szurt;
        }

        private async void AktivChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                var checkbox = sender as CheckBox;
                var tag = checkbox?.DataContext as TagDto;

                if (tag == null)
                    return;

                var api = new ApiService();
                var json = JsonSerializer.Serialize(tag.Aktiv);

                await api.Put($"api/Tagok/{tag.TagId}/aktiv", json);

                UpdateStats();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Státusz frissítési hiba: " + ex.Message);
                await LoadTagok();
            }
        }

        private void KeresesTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void CsakAktivCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private async void FrissitesButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadTagok();
        }
    }

    public class TagDto
    {
        public int TagId { get; set; }
        public string TeljesNev { get; set; }
        public string Vezeteknev { get; set; }
        public string Keresztnev { get; set; }
        public DateTime SzuletesiDatum { get; set; }
        public bool Aktiv { get; set; }
    }
}