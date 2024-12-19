using System.Windows;
using System.Windows.Input;
using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;

namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly UserAuthService _authService;
        private readonly Database _databaseService;
        private bool _isRegisterMode;
        private string _dbStatus;
        private string _email;
        private string _password;
        private string _firstName;
        private string _lastName;

        public LoginViewModel()
        {
            _authService = new UserAuthService();
            _databaseService = new Database();
            SignInCommand = new RelayCommand(SignIn);
            ToggleRegisterModeCommand = new RelayCommand(ToggleRegisterMode);
            CheckDatabaseConnection();
        }

        public string DbStatus
        {
            get => _dbStatus;
            set => SetProperty(ref _dbStatus, value);
        }
        public bool IsEmailVisible => !IsRegisterMode;
        public bool IsRegisterMode
        {
            get => _isRegisterMode;
            set
            {
                SetProperty(ref _isRegisterMode, value);
                OnPropertyChanged(nameof(SignInButtonText));
                OnPropertyChanged(nameof(RegisterButtonText));
                OnPropertyChanged(nameof(IsEmailVisible));
            }
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public string SignInButtonText => IsRegisterMode ? "Register" : "Sign in";
        public string RegisterButtonText => IsRegisterMode ? "Back to Login" : "Register";

        public ICommand SignInCommand { get; }
        public ICommand ToggleRegisterModeCommand { get; }

        private void CheckDatabaseConnection()
        {
            DbStatus = _databaseService.TestConnection();
        }

        private void ToggleRegisterMode()
        {
            IsRegisterMode = !IsRegisterMode;
        }

        private void SignIn()
        {
            if (IsRegisterMode)
            {
                if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
                {
                    MessageBox.Show("Please fill out all fields.");
                    return;
                }

                _authService.RegisterUser(Password, FirstName, LastName);
            }
            else
            {
                bool success = _authService.AuthenticateUser(Email, Password);
                if (success)
                {
                    MessageBox.Show($"Login successful! Role: {ProfilUzivateleManager.CurrentUser.Role}");
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MainWindow mainWindow = new MainWindow(ProfilUzivateleManager.CurrentUser.Role);
                        mainWindow.Show();
                        Application.Current.MainWindow.Close();
                    });
                }
                else
                {
                    MessageBox.Show("Invalid email or password.");
                }
            }
        }
    }
}