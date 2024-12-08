using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;
using BCSH2_BDAS2_Armadni_Informacni_System.Entities;
using System.Windows;
using System;
using System.Windows.Input;
using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;

namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
{
    public class PrehledZbranViewModel : INotifyPropertyChanged
    {
        private readonly Database _database;
        private PrehledZbran _selectedZbran;

        public ObservableCollection<PrehledZbran> Zbrane { get; set; } = new ObservableCollection<PrehledZbran>();
        public ObservableCollection<Utvary> Utvary { get; set; } = new ObservableCollection<Utvary>();
        public ObservableCollection<Vojak> Vojaci { get; set; } = new ObservableCollection<Vojak>();

        public PrehledZbran SelectedZbran
        {
            get => _selectedZbran;
            set
            {
                _selectedZbran = value;
                OnPropertyChanged(nameof(SelectedZbran));
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

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddCommand { get; }

        // Konstruktor
        public PrehledZbranViewModel()
        {
            _database = new Database();
            LoadUtvary();
            LoadVojaci();
            LoadZbrane();
            SaveCommand = new RelayCommand(SaveZbran);
            DeleteCommand = new RelayCommand(DeleteZbran);
            AddCommand = new RelayCommand(AddZbran);
            SetUserRolePermissions();
        }

        private void SetUserRolePermissions()
        {
            string userRole = ProfilUzivateleManager.CurrentUser?.Role;
            CanEdit = !(userRole == "Vojáci");
        }

        // Načítání útvarů z databáze
        private void LoadUtvary()
        {
            Utvary.Clear();
            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("SELECT * FROM UTVARY", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Utvary.Add(new Utvary
                    {
                        IdUtvar = reader.GetInt32(0),
                        Nazev = reader.GetString(1),
                        Typ = reader.GetString(2)
                    });
                }
            }
        }

        // Načítání vojáků z databáze
        private void LoadVojaci()
        {
            Vojaci.Clear();
            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("SELECT * FROM VOJACI", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Vojaci.Add(new Vojak
                    {
                        IdVojak = reader.GetInt32(0),
                        Jmeno = reader.GetString(1),
                        Prijmeni = reader.GetString(2)
                    });
                }
            }
        }

        private void LoadZbrane()
        {
            Zbrane.Clear();
            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("SELECT * FROM PREHLED_ZBRAN", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Zbrane.Add(new PrehledZbran
                    {
                        IdZbran = reader.GetInt32(0),
                        NazevZbrane = reader.GetString(1),
                        DatumPorizeni = reader.GetDateTime(2),
                        Typ = reader.GetString(3),
                        Kalibr = reader.GetString(4),
                        IdUtvar = reader.GetInt32(5),
                        NazevUtvaru = reader.GetString(6),
                        IdVojak = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),
                        VojakJmeno = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                        VojakPrijmeni = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                    });
                }
            }

            // Pokud není vybrána žádná zbraň, nastaví se na první prvek
            if (SelectedZbran == null && Zbrane.Count > 0)
            {
                SelectedZbran = Zbrane[0];
            }
        }

        // Uložení vybrané zbraně
        public void SaveZbran()
        {
            if (SelectedZbran == null) return;
            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("edit_zbrane", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.Add("p_id_zbran", OracleDbType.Int32).Value = SelectedZbran.IdZbran;
                    command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = SelectedZbran.NazevZbrane;
                    command.Parameters.Add("p_datum", OracleDbType.Date).Value = SelectedZbran.DatumPorizeni;
                    command.Parameters.Add("p_typ", OracleDbType.Varchar2).Value = SelectedZbran.Typ;
                    command.Parameters.Add("p_kalibr", OracleDbType.Varchar2).Value = SelectedZbran.Kalibr;
                    command.Parameters.Add("p_id_utvar", OracleDbType.Int32).Value = SelectedZbran.IdUtvar;
                    command.Parameters.Add("p_id_vojak", OracleDbType.Int32).Value = SelectedZbran.IdVojak;

                    command.ExecuteNonQuery();
                }
                LoadZbrane();  // Reload after saving
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při ukládání zbraně: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddZbran()
        {
            if (SelectedZbran == null) return;
            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("edit_zbrane", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.Add("p_id_zbran", OracleDbType.Int32).Value = DBNull.Value;
                    command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = SelectedZbran.NazevZbrane;
                    command.Parameters.Add("p_datum", OracleDbType.Date).Value = SelectedZbran.DatumPorizeni;
                    command.Parameters.Add("p_typ", OracleDbType.Varchar2).Value = SelectedZbran.Typ;
                    command.Parameters.Add("p_kalibr", OracleDbType.Varchar2).Value = SelectedZbran.Kalibr;
                    command.Parameters.Add("p_id_utvar", OracleDbType.Int32).Value = SelectedZbran.IdUtvar;
                    command.Parameters.Add("p_id_vojak", OracleDbType.Int32).Value = SelectedZbran.IdVojak;

                    command.ExecuteNonQuery();
                }
                LoadZbrane();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při ukládání zbraně: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteZbran()
        {
            if (SelectedZbran == null)
            {
                return;
            }

            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    // Vytvoření SQL dotazu pro smazání záznamu z tabulky ZBRANE
                    var command = new OracleCommand("DELETE FROM ZBRANE WHERE ID_ZBRAN = :idZbran", connection);

                    command.Parameters.Add("idZbran", OracleDbType.Int32).Value = SelectedZbran.IdZbran;

                    command.ExecuteNonQuery();
                }

                LoadZbrane();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při mazání zbraně: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
