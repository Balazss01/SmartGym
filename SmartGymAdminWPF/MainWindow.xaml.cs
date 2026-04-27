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
            HideSidebar();
            MainFrame.Navigate(new LoginPage());
        }

        public void NavigateToDashboard()
        {
            ShowSidebar();
            ClearFrameHistory();
            MainFrame.Navigate(new DashboardPage());
        }

        private void ShowSidebar()
        {
            SidebarColumn.Width = new GridLength(260);
            SidebarBorder.Visibility = Visibility.Visible;
        }

        private void HideSidebar()
        {
            SidebarColumn.Width = new GridLength(0);
            SidebarBorder.Visibility = Visibility.Collapsed;
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(ApiService.Token);
        }

        // 🔥 EZ HIÁNYZOTT → FRAME TAKARÍTÁS
        private void ClearFrameHistory()
        {
            MainFrame.Content = null;

            while (MainFrame.CanGoBack)
            {
                MainFrame.RemoveBackEntry();
            }
        }

        private void RequireLogin()
        {
            ClearFrameHistory();
            HideSidebar();
            MainFrame.Navigate(new LoginPage());
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLoggedIn())
            {
                RequireLogin();
                return;
            }

            ClearFrameHistory();
            MainFrame.Navigate(new DashboardPage());
        }

        private void Tagok_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLoggedIn())
            {
                RequireLogin();
                return;
            }

            ClearFrameHistory();
            MainFrame.Navigate(new TagokPage());
        }

        private void Berletek_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLoggedIn())
            {
                RequireLogin();
                return;
            }

            ClearFrameHistory();
            MainFrame.Navigate(new BerletekPage());
        }

        private void Belepesek_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLoggedIn())
            {
                RequireLogin();
                return;
            }

            ClearFrameHistory();
            MainFrame.Navigate(new BelepesekPage());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            ClearFrameHistory();

            ApiService.Token = "";
            HideSidebar();

            MainFrame.Navigate(new LoginPage());
        }
    }
}