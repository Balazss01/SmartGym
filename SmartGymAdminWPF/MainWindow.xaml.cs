using System.Windows;
using SmartGymAdminWPF.Services;
using SmartGymAdminWPF.Views;

namespace SmartGymAdminWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new LoginPage());
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(ApiService.Token);
        }

        private void RequireLogin()
        {
            MessageBox.Show("Először jelentkezz be adminnal.");
            MainFrame.Navigate(new LoginPage());
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LoginPage());
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLoggedIn())
            {
                RequireLogin();
                return;
            }

            MainFrame.Navigate(new DashboardPage());
        }

        private void Berletek_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLoggedIn())
            {
                RequireLogin();
                return;
            }

            MainFrame.Navigate(new BerletekPage());
        }

        private void Belepesek_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLoggedIn())
            {
                RequireLogin();
                return;
            }

            MainFrame.Navigate(new BelepesekPage());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            ApiService.Token = "";
            MessageBox.Show("Kijelentkeztél.");
            MainFrame.Navigate(new LoginPage());
        }
    }
}