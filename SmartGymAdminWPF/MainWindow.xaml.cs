using SmartGymAdminWPF.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace SmartGymAdminWPF
{
    public partial class MainWindow : Window
    {
        HttpClient client = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
            _ = LoadUsers();
        }

        private async Task LoadUsers()
        {
            try
            {
                var response = await client.GetAsync("https://localhost:5001/api/users");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    var users = JsonSerializer.Deserialize<List<User>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    UsersGrid.ItemsSource = users;
                }
                else
                {
                    MessageBox.Show("API hiba");
                }
            }
            catch
            {
                MessageBox.Show("Backend nem fut!");
            }
        }
    }
}