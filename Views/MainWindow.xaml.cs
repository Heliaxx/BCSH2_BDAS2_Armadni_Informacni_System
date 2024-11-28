using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;
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
                    Page2Button.IsEnabled = true;
                    break;

                case "Důstojníci":
                    // Nastavíme možnosti pro běžného uživatele
                    VojakViewButton.IsEnabled = true;
                    Page2Button.IsEnabled = false;
                    break;

                case "Poddůstojníci":
                    // Nastavíme možnosti pro běžného uživatele
                    VojakViewButton.IsEnabled = true;
                    Page2Button.IsEnabled = false;
                    break;

                case "Vojáci":
                    // Nastavíme možnosti pro běžného uživatele
                    VojakViewButton.IsEnabled = true;
                    Page2Button.IsEnabled = false;
                    break;

                default:
                    // Uživatel nemá žádnou roli nebo nemá oprávnění
                    VojakViewButton.IsEnabled = false;
                    Page2Button.IsEnabled = false;
                    MessageBox.Show("Nemáte oprávnění k přístupu do této aplikace.");
                    Close();
                    break;
            }
        }

        private void VojakViewButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new VojakView();
        }

        private void Page2Button_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Page2();
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }

}
