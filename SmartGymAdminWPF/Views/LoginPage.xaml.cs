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
        private bool _isPasswordVisible = false;

        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginEnter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoginButton_Click(sender, e);
        }

        private void TogglePassword_Click(object sender, RoutedEventArgs e)
        {
            if (_isPasswordVisible)
            {
                PasswordBox.Password = PasswordVisibleTextBox.Text;
                PasswordBox.Visibility = Visibility.Visible;
                PasswordVisibleTextBox.Visibility = Visibility.Collapsed;

                PasswordBox.Focus();
            }
            else
            {
                PasswordVisibleTextBox.Text = PasswordBox.Password;
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordVisibleTextBox.Visibility = Visibility.Visible;

                PasswordVisibleTextBox.Focus();
                PasswordVisibleTextBox.CaretIndex = PasswordVisibleTextBox.Text.Length;
            }

            _isPasswordVisible = !_isPasswordVisible;
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorBorder.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            ErrorText.Text = "";
            ErrorBorder.Visibility = Visibility.Collapsed;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HideError();

                var api = new ApiService();

                var json = JsonSerializer.Serialize(new
                {
                    email = EmailTextBox.Text,
                    password = _isPasswordVisible
                        ? PasswordVisibleTextBox.Text
                        : PasswordBox.Password
                });

                var response = await api.Post("api/Auth/login", json);

                using var doc = JsonDocument.Parse(response);
                var root = doc.RootElement;

                if (!root.TryGetProperty("token", out var tokenElement))
                {
                    ShowError("Hibás email vagy jelszó!");
                    return;
                }

                var token = tokenElement.GetString();

                if (string.IsNullOrWhiteSpace(token))
                {
                    ShowError("Nincs token.");
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
                    ShowError("Csak admin léphet be!");
                    return;
                }

                ApiService.Token = token;

                var main = Application.Current.MainWindow as MainWindow;
                main?.NavigateToDashboard();
            }
            catch
            {
                ShowError("Hibás email vagy jelszó!");
            }
        }
    }
}