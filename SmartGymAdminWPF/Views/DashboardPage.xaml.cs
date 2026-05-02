using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SmartGymAdminWPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SmartGymAdminWPF.Views
{
    public partial class DashboardPage : Page, INotifyPropertyChanged
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        private ISeries[] _chartSeries = Array.Empty<ISeries>();
        private Axis[] _chartXAxes = Array.Empty<Axis>();
        private Axis[] _chartYAxes = Array.Empty<Axis>();

        // A stats végpontból betöltött valódi mai belépésszám
        private int _valodiBelepesSzam = 0;

        public ISeries[] ChartSeries
        {
            get => _chartSeries;
            set { _chartSeries = value; OnPropertyChanged(nameof(ChartSeries)); }
        }

        public Axis[] ChartXAxes
        {
            get => _chartXAxes;
            set { _chartXAxes = value; OnPropertyChanged(nameof(ChartXAxes)); }
        }

        public Axis[] ChartYAxes
        {
            get => _chartYAxes;
            set { _chartYAxes = value; OnPropertyChanged(nameof(ChartYAxes)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public DashboardPage()
        {
            InitializeComponent();
            DataContext = this;
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

        private void DashboardPage_Unloaded(object sender, RoutedEventArgs e) => _timer.Stop();
        private async void Timer_Tick(object? sender, EventArgs e) => await ReloadAll();

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

                // Eltároljuk a valódi mai belépésszámot a chart számára
                _valodiBelepesSzam = stats.MaiBelepesek;
            }
            catch (Exception ex) { MessageBox.Show("Dashboard stat hiba: " + ex.Message); }
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
            catch (Exception ex) { MessageBox.Show("Bent lévők hiba: " + ex.Message); }
        }

        private async Task LoadUtolsoBelepesek()
        {
            try
            {
                var api = new ApiService();
                var json = await api.Get("api/AdminDashboard/utolso-belepesek");
                var lista = JsonSerializer.Deserialize<List<UtolsoBelepesDto>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                // Átadjuk a stats-ból betöltött valódi számot is
                BuildAktivitasChart(lista, _valodiBelepesSzam);
            }
            catch (Exception ex) { MessageBox.Show("Utolsó belépések hiba: " + ex.Message); }
        }

        // FIX: valodiBelepesSzam paraméter hozzáadva, hogy a stats végpont
        // helyes értékét használjuk az összefoglaló kártyákban — az utolso-belepesek
        // lista limitált lehet (pl. top 10/50), ezért tért el a két szám.
        private void BuildAktivitasChart(List<UtolsoBelepesDto> lista, int valodiBelepesSzam)
        {
            // Csak a mai belépések, időpont szerint rendezve
            var mai = lista
                .Where(b => b.BelepesIdopont.Date == DateTime.Today)
                .OrderBy(b => b.BelepesIdopont)
                .ToList();

            // Kumulált lépcsős vonal: X = percek éjféltől, Y = kumulált belépésszám
            var pontok = new List<ObservablePoint>();
            pontok.Add(new ObservablePoint(0, 0));

            for (int i = 0; i < mai.Count; i++)
            {
                double percEjfeltol = mai[i].BelepesIdopont.Hour * 60 + mai[i].BelepesIdopont.Minute;
                if (i > 0)
                    pontok.Add(new ObservablePoint(percEjfeltol, i));
                pontok.Add(new ObservablePoint(percEjfeltol, i + 1));
            }

            double mostPerc = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
            pontok.Add(new ObservablePoint(mostPerc, mai.Count));

            ChartSeries = new ISeries[]
            {
                new LineSeries<ObservablePoint>
                {
                    Name           = "Belépések",
                    Values         = new ObservableCollection<ObservablePoint>(pontok),
                    Fill           = new LinearGradientPaint(
                                         new[] { SKColor.Parse("#3300f5c4"), SKColor.Parse("#0000f5c4") },
                                         new SKPoint(0, 0), new SKPoint(0, 1)),
                    Stroke         = new SolidColorPaint(SKColor.Parse("#00f5c4")) { StrokeThickness = 2 },
                    GeometrySize   = 0,
                    LineSmoothness = 0,
                }
            };

            ChartXAxes = new Axis[]
            {
                new Axis
                {
                    MinLimit        = 0,
                    MaxLimit        = 1440,
                    MinStep         = 60,
                    Labeler         = val => $"{(int)(val / 60):D2}:00",
                    LabelsRotation  = 0,
                    TextSize        = 10,
                    LabelsPaint     = new SolidColorPaint(SKColor.Parse("#4a4740")),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#1a1a28")),
                    TicksPaint      = null,
                    DrawTicksPath   = false,
                }
            };

            ChartYAxes = new Axis[]
            {
                new Axis
                {
                    Name            = "belépés",
                    NamePaint       = new SolidColorPaint(SKColor.Parse("#3a3a4a")),
                    LabelsPaint     = new SolidColorPaint(SKColor.Parse("#4a4740")),
                    TextSize        = 11,
                    MinStep         = 1,
                    MinLimit        = 0,
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#1f1f2e")),
                    TicksPaint      = null,
                    DrawTicksPath   = false,
                }
            };

            // FIX: valodiBelepesSzam-ot használjuk mai.Count helyett,
            // mert az utolso-belepesek lista limitált lehet
            AtlagBentletText.Text = $"{valodiBelepesSzam} fő";
            OsszesBelepesHintText.Text = $"{valodiBelepesSzam} belépés ma";

            if (mai.Any())
            {
                var csucsOra = mai
                    .GroupBy(b => b.BelepesIdopont.Hour)
                    .OrderByDescending(g => g.Count())
                    .First();
                MaxBentletText.Text = $"{csucsOra.Key:D2}:00";

                var utolso = mai.Last();
                var percElt = (int)(DateTime.Now - utolso.BelepesIdopont).TotalMinutes;
                FolyamatbanText.Text = percElt < 60
                    ? $"{percElt} perce"
                    : $"{utolso.BelepesIdopont:HH:mm}";
            }
            else
            {
                MaxBentletText.Text = "—";
                FolyamatbanText.Text = "—";
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
                foreach (var item in lista) item.MaxErtek = max;

                HetiBelepesekItemsControl.ItemsSource = lista;
            }
            catch (Exception ex) { MessageBox.Show("Heti belépések hiba: " + ex.Message); }
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
            catch (Exception ex) { MessageBox.Show("Bérlet megoszlás hiba: " + ex.Message); }
        }

        private async void FrissitesButton_Click(object sender, RoutedEventArgs e) => await ReloadAll();
        private void AutoRefreshCheckBox_Checked(object sender, RoutedEventArgs e) => _timer.Start();
        private void AutoRefreshCheckBox_Unchecked(object sender, RoutedEventArgs e) => _timer.Stop();
    }

    // ── DTOs ──

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
        public string TeljesNev { get; set; } = "";
        public DateTime BelepesIdopont { get; set; }
        public DateTime? KilepesIdopont { get; set; }
    }

    public class BentLevoDto
    {
        public int BelepesId { get; set; }
        public int TagId { get; set; }
        public string TeljesNev { get; set; } = "";
        public DateTime BelepesIdopont { get; set; }
    }

    public class HetiBelepesDto
    {
        public string Datum { get; set; } = "";
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