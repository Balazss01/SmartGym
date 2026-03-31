using System.Data;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace SmartGymAdminWPF.Views
{
    public partial class TagokPage : Page
    {
        private DataTable table = new DataTable();

        public TagokPage()
        {
            InitializeComponent();
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var json = await MainWindow.Api.Get("/api/tagok");

                table = new DataTable();

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

                UsersGrid.ItemsSource = table.DefaultView;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Search(object sender, TextChangedEventArgs e)
        {
            var text = SearchBox.Text.ToLower();
            var filtered = table.Clone();

            foreach (DataRow r in table.Rows)
            {
                if (r.ItemArray.Any(x => x != null && x.ToString()!.ToLower().Contains(text)))
                    filtered.ImportRow(r);
            }

            UsersGrid.ItemsSource = filtered.DefaultView;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var row = UsersGrid.SelectedItem as DataRowView;
                if (row == null)
                {
                    MessageBox.Show("Válassz ki egy sort.");
                    return;
                }

                if (!table.Columns.Contains("Id"))
                {
                    MessageBox.Show("Nincs Id oszlop.");
                    return;
                }

                var id = row["Id"]?.ToString();

                if (string.IsNullOrWhiteSpace(id))
                {
                    MessageBox.Show("Érvénytelen azonosító.");
                    return;
                }

                var confirm = MessageBox.Show(
                    $"Biztos törlöd ezt a rekordot? (Id: {id})",
                    "Megerősítés",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirm != MessageBoxResult.Yes)
                    return;

                await MainWindow.Api.Delete($"/api/tagok/{id}");
                LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}