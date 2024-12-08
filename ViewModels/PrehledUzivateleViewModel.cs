using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Oracle.ManagedDataAccess.Client;
using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;
using System.Windows;
using BCSH2_BDAS2_Armadni_Informacni_System.Entities;
using System.Windows.Controls;

namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
{
    internal class PrehledUzivateleViewModel : INotifyPropertyChanged
    {
        private readonly Database _database;
        public ObservableCollection<PrehledUzivatele> Uzivatele { get; set; } = new ObservableCollection<PrehledUzivatele>();

        private PrehledUzivatele _selectedUzivatel;
        public PrehledUzivatele SelectedUzivatel
        {
            get => _selectedUzivatel;
            set
            {
                _selectedUzivatel = value;
                OnPropertyChanged(nameof(SelectedUzivatel));
            }
        }

        private bool _canEdit = false;
        public bool CanEdit
        {
            get => _canEdit;
            set
            {
                _canEdit = value;
                OnPropertyChanged(nameof(CanEdit));
            }
        }

        public ICommand EmulateCommand { get; }

        private Page _currentPage;

        public PrehledUzivateleViewModel(Page currentPage)
        {
            _currentPage = currentPage;
            _database = new Database();
            LoadUzivatele();
            EmulateCommand = new RelayCommand(Emulate);
            SetUserRolePermissions();
        }


        private void SetUserRolePermissions()
        {
            // Nastavení práv na základě role uživatele
            string userRole = ProfilUzivateleManager.CurrentUser?.Role;
            CanEdit = !(userRole == "Vojáci" || userRole == "Poddůstojníci" || userRole == "Důstojníci");
        }

        private void LoadUzivatele()
        {
            Uzivatele.Clear();
            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("SELECT * FROM PREHLED_UZIVATELE", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var email = ProfilUzivateleManager.CurrentUser.Role.Equals("Generálové", StringComparison.OrdinalIgnoreCase)
                        ? reader.GetString(3) : "*****";  // Maskování emailu na základě role

                    Uzivatele.Add(new PrehledUzivatele
                    {
                        id_vojak = reader.GetInt32(0),
                        jmeno = reader.GetString(1),
                        prijmeni = reader.GetString(2),
                        email = email,  // Maskovaný email nebo zobrazený podle role
                        heslo = reader.GetString(4),
                        nazev_hodnosti = reader.GetString(5),
                        nazev_role = reader.GetString(6)
                    });
                }
            }
        }

        private void Emulate()
        {
            if (SelectedUzivatel == null)
            {
                MessageBox.Show("Vyberte platného uživatele pro emulaci.");
                return;
            }

            ProfilUzivateleManager.OriginalUser = ProfilUzivateleManager.CurrentUser;

            ProfilUzivateleManager.CurrentUser = new ProfilUzivatele
            {
                Email = SelectedUzivatel.email,
                Role = SelectedUzivatel.nazev_role,
                IdVojak = SelectedUzivatel.id_vojak,
                Jmeno = SelectedUzivatel.jmeno,
                Prijmeni = SelectedUzivatel.prijmeni,
                Hodnost = SelectedUzivatel.nazev_hodnosti
            };

            MessageBox.Show($"Emulace byla úspěšná. Role: {ProfilUzivateleManager.CurrentUser.Role}");

            // Otevři nové hlavní okno
            MainWindow mainWindow = new MainWindow(ProfilUzivateleManager.CurrentUser.Role);
            mainWindow.Show();

            // Zavři rodičovské okno obsahující tuto stránku
            Window parentWindow = Window.GetWindow(_currentPage);
            parentWindow?.Close();
        }

        // PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
