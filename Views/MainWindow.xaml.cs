using BCSH2_BDAS2_Armadni_Informacni_System.Models;
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
            if (ProfilUzivateleManager.OriginalUser != null)
            {
                UkoncitEmulaciButton.Visibility = Visibility.Visible;
            }
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
                    Utvary.Visibility = Visibility.Collapsed;
                    Role.Visibility = Visibility.Collapsed;
                    break;

                case "Poddůstojníci":
                    // Nastavíme možnosti pro běžného uživatele
                    VojakViewButton.IsEnabled = true;
                    Logy.Visibility = Visibility.Collapsed;
                    Utvary.Visibility = Visibility.Collapsed;
                    Role.Visibility = Visibility.Collapsed;
                    Technika.Visibility = Visibility.Collapsed;
                    break;

                case "Vojáci":
                    // Nastavíme možnosti pro běžného uživatele
                    VojakViewButton.IsEnabled = true;
                    Logy.Visibility = Visibility.Collapsed;
                    Utvary.Visibility = Visibility.Collapsed;
                    Role.Visibility = Visibility.Collapsed;
                    Specializace.Visibility = Visibility.Collapsed;
                    Uzivatele.Visibility = Visibility.Collapsed;
                    Technika.Visibility = Visibility.Collapsed;
                    Jednotky.Visibility = Visibility.Collapsed;
                    Zbrane.Visibility = Visibility.Collapsed;
                    break;

                default:
                    // Uživatel nemá žádnou roli nebo nemá oprávnění
                    VojakViewButton.IsEnabled = false;
                    Logy.Visibility = Visibility.Collapsed;
                    Uzivatele.Visibility = Visibility.Collapsed;
                    Utvary.Visibility = Visibility.Collapsed;
                    Role.Visibility = Visibility.Collapsed;
                    Specializace.Visibility = Visibility.Collapsed;
                    Uzivatele.Visibility = Visibility.Collapsed;
                    Technika.Visibility = Visibility.Collapsed;
                    Jednotky.Visibility = Visibility.Collapsed;
                    Zbrane.Visibility = Visibility.Collapsed;
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

        private void Soubory_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PrehledSouboryView();
        }

        private void Specializace_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PrehledSpecializaceView();
        }

        private void Systemovy_Katalog_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DatabaseObjectsView();
        }

        private void Technika_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PrehledTechnikaView();
        }

        private void Zbrane_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PrehledZbranView();
        }

        private void Utvary_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PrehledUtvarView();
        }

        private void Konec_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Profil_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new BCSH2_BDAS2_Armadni_Informacni_System.Views.ProfilUzivatele();
        }

        private void UkoncitEmulaciButton_Click(object sender, RoutedEventArgs e)
        {
            // Zkontroluj, zda je původní uživatel uložen
            if (ProfilUzivateleManager.OriginalUser == null)
            {
                MessageBox.Show("Nebyl nalezen původní uživatel pro ukončení emulace.");
                return;
            }

            ProfilUzivateleManager.CurrentUser = ProfilUzivateleManager.OriginalUser;
            ProfilUzivateleManager.OriginalUser = null;

            MessageBox.Show($"Emulace ukončena. Přihlášen jako role: {ProfilUzivateleManager.CurrentUser.Role}");

            // Otevři nové hlavní okno s původními oprávněními
            MainWindow mainWindow = new MainWindow(ProfilUzivateleManager.CurrentUser.Role);
            mainWindow.Show();

            // Zavři aktuální okno
            this.Close();
        }

        private void Dovolenky_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PrehledDovolenkyView();
        }

        private void Hodnosti_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PrehledHodnostView();
        }

        private void Dovolenky_Vojaci_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PrehledDovolenkyVojaciView();
        }

        private void Skoleni_Ucastnici_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PrehledSkoleniUcastniciView();
        }

        private void Specializace_Vojaci_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PrehledSpecializaceVojaciView();
        }
    }
}