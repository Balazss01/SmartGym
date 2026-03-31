using System.Windows;
using SmartGymAdminWPF.Services;
using SmartGymAdminWPF.Views;

namespace SmartGymAdminWPF
{
    public partial class MainWindow : Window
    {
        public static ApiService Api = new ApiService();

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private async void Init()
        {
            try
            {
                await Api.Login();
                MainFrame.Navigate(new DashboardPage());
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardPage());
        }

        private void Tagok_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TagokPage());
        }

        private void Berletek_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new BerletekPage());
        }

        private void Belepesek_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new BelepesekPage());
        }
    }
}