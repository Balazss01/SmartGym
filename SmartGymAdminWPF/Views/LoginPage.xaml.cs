using SmartGymAdminWPF;
using SmartGymAdminWPF.Services;
using System;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SmartGymAdminWPF.Views
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginEnter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoginButton_Click(sender, e);
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ErrorText.Visibility = Visibility.Collapsed;
                ErrorText.Text = "";

                var api = new ApiService();

                var json = JsonSerializer.Serialize(new
                {
                    email = EmailTextBox.Text,
                    password = PasswordBox.Password
                });

                var response = await api.Post("api/Auth/login", json);

                using var doc = JsonDocument.Parse(response);
                var root = doc.RootElement;

                if (!root.TryGetProperty("token", out var tokenElement))
                {
                    ErrorText.Text = "Hibás email vagy jelszó!";
                    ErrorText.Visibility = Visibility.Visible;
                    return;
                }

                var token = tokenElement.GetString();

                if (string.IsNullOrWhiteSpace(token))
                {
                    ErrorText.Text = "Nincs token.";
                    ErrorText.Visibility = Visibility.Visible;
                    return;
                }

                bool admin = false;

                if (root.TryGetProperty("roles", out var roles))
                {
                    admin = roles.EnumerateArray()
                        .Select(x => x.GetString())
                        .Any(r => r == "Admin");
                }

                if (!admin)
                {
                    ErrorText.Text = "Csak admin léphet be!";
                    ErrorText.Visibility = Visibility.Visible;
                    return;
                }

                ApiService.Token = token;

                var main = Application.Current.MainWindow as MainWindow;
                main?.NavigateToDashboard();
            }
            catch
            {
                ErrorText.Text = "Hibás email vagy jelszó!";
                ErrorText.Visibility = Visibility.Visible;
            }
        }
    }
}