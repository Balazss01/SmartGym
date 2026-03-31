using System.Text.Json;
using System.Windows.Controls;

namespace SmartGymAdminWPF.Views
{
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var tagokJson = await MainWindow.Api.Get("/api/tagok");
                var belepesekJson = await MainWindow.Api.Get("/api/belepesek");
                var berletekJson = await MainWindow.Api.Get("/api/berletek");

                int tagok = JsonDocument.Parse(tagokJson).RootElement.GetArrayLength();
                int belepesek = JsonDocument.Parse(belepesekJson).RootElement.GetArrayLength();
                int berletek = JsonDocument.Parse(berletekJson).RootElement.GetArrayLength();

                TagCountText.Text = tagok.ToString();
                BelepesCountText.Text = belepesek.ToString();
                BerletCountText.Text = berletek.ToString();

                StatsText.Text = $"Tagok: {tagok} | Belépések: {belepesek} | Bérletek: {berletek}";
            }
            catch (System.Exception ex)
            {
                StatsText.Text = "Hiba: " + ex.Message;
            }
        }
    }
}