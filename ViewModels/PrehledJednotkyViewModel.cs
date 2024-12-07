using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;
using BCSH2_BDAS2_Armadni_Informacni_System.Entities;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;

namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
{
    public class PrehledJednotkyViewModel : INotifyPropertyChanged
    {
        private readonly Database _database;
        private PrehledJednotky _selectedJednotka;

        public ObservableCollection<PrehledJednotky> Jednotky { get; set; } = new ObservableCollection<PrehledJednotky>();
        public ObservableCollection<Utvary> Utvary { get; set; } = new ObservableCollection<Utvary>();

        public PrehledJednotky SelectedJednotka
        {
            get => _selectedJednotka;
            set
            {
                _selectedJednotka = value;
                OnPropertyChanged(nameof(SelectedJednotka));
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
        public PrehledJednotkyViewModel()
        {
            _database = new Database();
            LoadUtvary();
            LoadJednotky();
            SaveCommand = new RelayCommand(SaveJednotka);
            DeleteCommand = new RelayCommand(DeleteJednotka);
            AddCommand = new RelayCommand(AddJednotka);
            SetUserRolePermissions();
        }

        private void SetUserRolePermissions()
        {
            string userRole = ProfilUzivateleManager.CurrentUser?.Role;
            CanEdit = !(userRole == "Vojáci" || userRole == "Poddůstojníci" || userRole == "Důstojníci");
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

        // Načítání jednotek z databáze
        private void LoadJednotky()
        {
            Jednotky.Clear();
            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("SELECT * FROM PREHLED_JEDNOTKY", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Jednotky.Add(new PrehledJednotky
                    {
                        IdJednotka = reader.GetInt32(0),
                        Nazev = reader.GetString(1),
                        Typ = reader.GetString(2),
                        Velikost = reader.GetInt32(3),
                        IdUtvar = reader.GetInt32(4),
                        PatriPodUtvar = reader.GetString(5)
                    });
                }
            }

            // Pokud není vybrána žádná jednotka, nastaví se na první prvek
            if (SelectedJednotka == null && Jednotky.Count > 0)
            {
                SelectedJednotka = Jednotky[0];
            }
        }

        // Uložení vybrané jednotky
        public void SaveJednotka()
        {
            if (SelectedJednotka == null) return;
            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("edit_jednotky", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.Add("p_id_jednotka", OracleDbType.Int32).Value = SelectedJednotka.IdJednotka;
                    command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = SelectedJednotka.Nazev;
                    command.Parameters.Add("p_typ", OracleDbType.Varchar2).Value = SelectedJednotka.Typ;
                    command.Parameters.Add("p_velikost", OracleDbType.Int32).Value = SelectedJednotka.Velikost;
                    command.Parameters.Add("p_id_utvar", OracleDbType.Int32).Value = SelectedJednotka.IdUtvar;

                    command.ExecuteNonQuery();
                }
                LoadJednotky();  // Reload after saving
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při ukládání jednotky: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddJednotka()
        {
            if (SelectedJednotka == null) return;
            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("edit_jednotky", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.Add("p_id_jednotka", OracleDbType.Int32).Value = DBNull.Value;
                    command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = SelectedJednotka.Nazev;
                    command.Parameters.Add("p_typ", OracleDbType.Varchar2).Value = SelectedJednotka.Typ;
                    command.Parameters.Add("p_velikost", OracleDbType.Int32).Value = SelectedJednotka.Velikost;
                    command.Parameters.Add("p_id_utvar", OracleDbType.Int32).Value = SelectedJednotka.IdUtvar;

                    command.ExecuteNonQuery();
                }
                LoadJednotky();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při ukládání jednotky: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteJednotka()
        {
            if (SelectedJednotka == null)
            {
                return;
            }

            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("smazat_jednotku", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Přidání parametru pro ID jednotky
                    command.Parameters.Add("p_id_jednotka", OracleDbType.Int32).Value = SelectedJednotka.IdJednotka;

                    // Vykonání procedury
                    command.ExecuteNonQuery();
                }

                // Po úspěšném smazání načíst znovu seznam jednotek
                LoadJednotky();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při mazání jednotky: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
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
