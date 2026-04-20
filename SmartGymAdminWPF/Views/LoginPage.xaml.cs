using SmartGymAdminWPF.Services;
using System;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace SmartGymAdminWPF.Views
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var api = new ApiService();

                var json = JsonSerializer.Serialize(new
                {
                    email = EmailTextBox.Text,
                    password = PasswordBox.Password
                });

                var response = await api.Post("api/Auth/login", json);

                using var doc = JsonDocument.Parse(response);
                var root = doc.RootElement;

                var token = root.GetProperty("token").GetString();

                if (string.IsNullOrWhiteSpace(token))
                {
                    MessageBox.Show("Nincs token.");
                    return;
                }

                // 🔥 ADMIN CHECK
                bool admin = false;

                if (root.TryGetProperty("roles", out var roles))
                {
                    admin = roles.EnumerateArray()
                        .Select(x => x.GetString())
                        .Any(r => r == "Admin");
                }

                if (!admin)
                {
                    MessageBox.Show("Csak admin léphet be!");
                    return;
                }

                ApiService.Token = token;

                MessageBox.Show("Sikeres admin login!");

                var main = (MainWindow)Application.Current.MainWindow;
                main.MainFrame.Navigate(new DashboardPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}