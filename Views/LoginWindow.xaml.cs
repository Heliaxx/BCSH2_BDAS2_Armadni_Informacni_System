using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;
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
        private MockDatabaseService mockDatabaseService;
        private bool isRegisterMode = false;

        public LoginWindow()
        {
            InitializeComponent();
            CheckDatabaseConnection();
            mockDatabaseService = new MockDatabaseService();
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
                stackfn.Visibility = Visibility.Visible;
                stackln.Visibility = Visibility.Visible;
                stackem.Visibility = Visibility.Collapsed;
                signInButton.Content = "Register";
                registerButton.Content = "Back to Login";
            }
            else
            {
                stackfn.Visibility = Visibility.Collapsed;
                stackln.Visibility = Visibility.Collapsed;
                stackem.Visibility = Visibility.Visible;
                signInButton.Content = "Sign in";
                registerButton.Content = "Register";
            }
        }

        private void LoginUser()
        {
            string emailInput = email.Text;
            string passwordInput = password.Password;

            UserAuthService authService = new UserAuthService();

            bool isAuthenticated = authService.AuthenticateUser(emailInput, passwordInput);
            Console.WriteLine($"Authenticated: {isAuthenticated}");
            if (isAuthenticated)
            {
                MessageBox.Show($"Přihlášení úspěšné. Role: {ProfilUzivateleManager.CurrentUser.Role}");
                MainWindow mainWindow = new MainWindow(ProfilUzivateleManager.CurrentUser.Role);
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
            string passwordInput = password.Password;
            string firstNameInput = firstName.Text;
            string lastNameInput = lastName.Text;

            if (string.IsNullOrEmpty(firstNameInput) || string.IsNullOrEmpty(lastNameInput))
            {
                MessageBox.Show("Prosím, vyplňte jméno a příjmení.");
                return;
            }

            UserAuthService authService = new UserAuthService();
           authService.RegisterUser(passwordInput, firstNameInput, lastNameInput);
        }
    }
}