using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;
using BCSH2_BDAS2_Armadni_Informacni_System.Views;
using System.Windows;

namespace BCSH2_BDAS2_Armadni_Informacni_System
{
    public partial class MainWindow : Window
    {
        private readonly string _userRole;

        public MainWindow(string userRole)
        {
            InitializeComponent();
            _userRole = userRole;

            ConfigureAccessBasedOnRole();
        }

        private void ConfigureAccessBasedOnRole()
        {
            switch (_userRole)
            {
                case "Generálové":
                    // Nastavíme možnosti pouze pro administrátora
                    VojakViewButton.IsEnabled = true;
                    Logy.IsEnabled = true;
                    break;

                case "Důstojníci":
                    // Nastavíme možnosti pro běžného uživatele
                    VojakViewButton.IsEnabled = true;
                    Logy.Visibility = Visibility.Collapsed;
                    break;

                case "Poddůstojníci":
                    // Nastavíme možnosti pro běžného uživatele
                    VojakViewButton.IsEnabled = true;
                    Logy.Visibility = Visibility.Collapsed;
                    break;

                case "Vojáci":
                    // Nastavíme možnosti pro běžného uživatele
                    VojakViewButton.IsEnabled = true;
                    Logy.Visibility = Visibility.Collapsed;
                    break;

                default:
                    // Uživatel nemá žádnou roli nebo nemá oprávnění
                    VojakViewButton.IsEnabled = false;
                    Logy.Visibility = Visibility.Collapsed;
                    Uzivatele.Visibility = Visibility.Collapsed;
                    MessageBox.Show("Nemáte oprávnění k přístupu do této aplikace.");
                    Close();
                    break;
            }
        }

        private void VojakViewButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PrehledVojaciView();
        }

        private void Logy_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PrehledLogyView();
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void Uzivatele_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PrehledUzivateleView();
        }

        private void Jednotky_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PrehledJednotkyView();
        }

        private void Skoleni_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PrehledSkoleniView();
        }

        private void Role_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PrehledRoleView();
        }

        private void Nadrizeni_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PrehledNadrizenychView();
        }
    }
}
