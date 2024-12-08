using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;
using System.ComponentModel;

namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
{
    public class ProfilUzivateleViewModel : INotifyPropertyChanged
    {
        private string _email;
        private string _role;
        private string _vojakJmeno;
        private string _vojakPrijmeni;
        private string _hodnost;

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public string Role
        {
            get => _role;
            set
            {
                if (_role != value)
                {
                    _role = value;
                    OnPropertyChanged(nameof(Role));
                }
            }
        }

        public string VojakJmeno
        {
            get => _vojakJmeno;
            set
            {
                if (_vojakJmeno != value)
                {
                    _vojakJmeno = value;
                    OnPropertyChanged(nameof(VojakJmeno));
                }
            }
        }

        public string VojakPrijmeni
        {
            get => _vojakPrijmeni;
            set
            {
                if (_vojakPrijmeni != value)
                {
                    _vojakPrijmeni = value;
                    OnPropertyChanged(nameof(VojakPrijmeni));
                }
            }
        }

        public string Hodnost
        {
            get => _hodnost;
            set
            {
                if (_hodnost != value)
                {
                    _hodnost = value;
                    OnPropertyChanged(nameof(Hodnost));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Konstruktor pro inicializaci a načtení uživatelského profilu
        public ProfilUzivateleViewModel()  
        {
            LoadUserProfile();
        }

        // Metoda pro načtení údajů o uživatelském profilu
        public void LoadUserProfile()
        {
            if (ProfilUzivateleManager.CurrentUser != null)
            {
                Email = ProfilUzivateleManager.CurrentUser.Email;
                Role = ProfilUzivateleManager.CurrentUser.Role;
                VojakJmeno = ProfilUzivateleManager.CurrentUser.Jmeno;
                VojakPrijmeni = ProfilUzivateleManager.CurrentUser.Prijmeni;
                Hodnost = ProfilUzivateleManager.CurrentUser.Hodnost;
            }
        }
    }
}
