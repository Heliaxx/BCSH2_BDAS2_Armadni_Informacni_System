using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BCSH2_BDAS2_Armadni_Informacni_System
{
    /// <summary>
    /// Interakční logika pro LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool isRegisterMode = false;

        public LoginWindow()
        {
            InitializeComponent();
            CheckDatabaseConnection();
        }

        private void CheckDatabaseConnection()
        {
            Database db = new Database();
            dbStatus.Text = db.TestConnection();
        }

        private void Sign_In_Click(object sender, RoutedEventArgs e)
        {
            if (isRegisterMode)
            {
                RegisterUser();
            }
            else
            {
                LoginUser();
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            isRegisterMode = !isRegisterMode;

            if (isRegisterMode)
            {
                firstName.Visibility = Visibility.Visible;
                lastName.Visibility = Visibility.Visible;
                signInButton.Content = "Register";
                registerButton.Content = "Back to Login";
            }
            else
            {
                firstName.Visibility = Visibility.Collapsed;
                lastName.Visibility = Visibility.Collapsed;
                signInButton.Content = "Sign in";
                registerButton.Content = "Register";
            }
        }

        private void LoginUser()
        {
            string emailInput = email.Text;
            string passwordInput = password.Text;

            UserAuthService authService = new UserAuthService();
            string userRole;

            bool isAuthenticated = authService.AuthenticateUser(emailInput, passwordInput, out userRole);

            if (isAuthenticated)
            {
                MessageBox.Show($"Přihlášení úspěšné. Role: {userRole}");
                MainWindow mainWindow = new MainWindow(userRole);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Nesprávný e-mail nebo heslo. Zkuste to znovu.");
            }
        }

        private void RegisterUser()
        {
            string emailInput = email.Text;
            string passwordInput = password.Text;
            string firstNameInput = firstName.Text;
            string lastNameInput = lastName.Text;

            if (string.IsNullOrEmpty(firstNameInput) || string.IsNullOrEmpty(lastNameInput))
            {
                MessageBox.Show("Prosím, vyplňte jméno a příjmení.");
                return;
            }

            UserAuthService authService = new UserAuthService();
            bool isRegistered = authService.RegisterUser(emailInput, passwordInput, firstNameInput, lastNameInput);

            if (isRegistered)
            {
                MessageBox.Show("Registrace byla úspěšná. Nyní se můžete přihlásit.");
                Register_Click(null, null); // Přepnutí zpět na login
            }
            else
            {
                MessageBox.Show("Registrace se nezdařila.");
            }
        }
    }
}