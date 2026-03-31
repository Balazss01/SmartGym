using System.Data;
using System.Text.Json;
using System.Windows.Controls;

namespace SmartGymAdminWPF.Views
{
    public partial class BelepesekPage : Page
    {
        public BelepesekPage()
        {
            InitializeComponent();
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var json = await MainWindow.Api.Get("/api/belepesek");
                var table = new DataTable();

                using var doc = JsonDocument.Parse(json);
                var arr = doc.RootElement.EnumerateArray();

                foreach (var item in arr)
                {
                    foreach (var p in item.EnumerateObject())
                    {
                        if (!table.Columns.Contains(p.Name))
                            table.Columns.Add(p.Name);
                    }

                    var row = table.NewRow();

                    foreach (var p in item.EnumerateObject())
                        row[p.Name] = p.Value.ToString();

                    table.Rows.Add(row);
                }

                Grid.ItemsSource = table.DefaultView;
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}