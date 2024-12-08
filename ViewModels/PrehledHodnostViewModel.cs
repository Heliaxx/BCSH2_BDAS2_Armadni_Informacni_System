using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;
using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;
using System.Windows;
using System.Windows.Input;

namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
{
    internal class PrehledHodnostViewModel : INotifyPropertyChanged
    {
        private readonly Database _database;
        private PrehledHodnost _selectedHodnost;
        private bool _canEdit;

        public ObservableCollection<PrehledHodnost> Hodnosti { get; set; } = new ObservableCollection<PrehledHodnost>();

        public PrehledHodnost SelectedHodnost
        {
            get => _selectedHodnost;
            set
            {
                _selectedHodnost = value;
                OnPropertyChanged(nameof(SelectedHodnost));
            }
        }

        public bool CanEdit
        {
            get => _canEdit;
            set
            {
                _canEdit = value;
                OnPropertyChanged(nameof(CanEdit));
            }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public PrehledHodnostViewModel()
        {
            _database = new Database();
            AddCommand = new RelayCommand(AddHodnost);
            DeleteCommand = new RelayCommand(DeleteHodnost);
            SaveCommand = new RelayCommand(SaveHodnost);
            SetUserPermissions();
            LoadHodnosti();
        }

        private void SetUserPermissions()
        {
            string userRole = ProfilUzivateleManager.CurrentUser?.Role;
            CanEdit = !(userRole == "Vojáci" || userRole == "Poddůstojníci" || userRole == "Důstojníci");
        }

        // Načítání hodností z databáze
        private void LoadHodnosti()
        {
            Hodnosti.Clear();
            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("SELECT * FROM PREHLED_HODNOSTI", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Hodnosti.Add(new PrehledHodnost
                    {
                        IdHodnost = reader.GetInt32(0),
                        Nazev = reader.GetString(1),
                        Odmeny = reader.IsDBNull(2) ? null : reader.GetString(2),
                        PotrebnyStupenVzdelani = reader.GetString(3),
                        PotrebnyPocetLetVPraxi = reader.GetInt32(4),
                        VahaHodnosti = reader.GetDecimal(5),
                        IdRole = reader.GetInt32(6),
                        NazevRole = reader.GetString(7)
                    });
                }
            }
        }

        // Uložení hodnosti
        private void SaveHodnost()
        {
            if (SelectedHodnost == null) return;
            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("edit_hodnost", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.Add("p_id_hodnost", OracleDbType.Int32).Value = SelectedHodnost.IdHodnost;
                    command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = SelectedHodnost.Nazev;
                    command.Parameters.Add("p_odmeny", OracleDbType.Varchar2).Value = (object)SelectedHodnost.Odmeny ?? DBNull.Value;
                    command.Parameters.Add("p_potrebny_stupen_vzdelani", OracleDbType.Varchar2).Value = SelectedHodnost.PotrebnyStupenVzdelani;
                    command.Parameters.Add("p_potrebny_pocet_let_v_praxi", OracleDbType.Int32).Value = SelectedHodnost.PotrebnyPocetLetVPraxi;
                    command.Parameters.Add("p_vaha_hodnosti", OracleDbType.Decimal).Value = SelectedHodnost.VahaHodnosti;
                    command.Parameters.Add("p_id_role", OracleDbType.Int32).Value = SelectedHodnost.IdRole;

                    command.ExecuteNonQuery();
                }
                LoadHodnosti(); // Reload after saving
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při ukládání hodnosti: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Přidání nové hodnosti
        private void AddHodnost()
        {
            if (SelectedHodnost == null) return; // Pokud není vybrána žádná hodnost, neprovádí se nic

            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("edit_hodnost", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Předání parametrů pro novou hodnost
                    command.Parameters.Add("p_id_hodnost", OracleDbType.Int32).Value = DBNull.Value; // Nová hodnost, tedy ID bude NULL
                    command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = SelectedHodnost.Nazev;
                    command.Parameters.Add("p_odmeny", OracleDbType.Varchar2).Value = (object)SelectedHodnost.Odmeny ?? DBNull.Value; // Odměny, pokud nejsou, použije se NULL
                    command.Parameters.Add("p_potrebny_stupen_vzdelani", OracleDbType.Varchar2).Value = SelectedHodnost.PotrebnyStupenVzdelani;
                    command.Parameters.Add("p_potrebny_pocet_let_v_praxi", OracleDbType.Int32).Value = SelectedHodnost.PotrebnyPocetLetVPraxi;
                    command.Parameters.Add("p_vaha_hodnosti", OracleDbType.Decimal).Value = SelectedHodnost.VahaHodnosti;
                    command.Parameters.Add("p_id_role", OracleDbType.Int32).Value = SelectedHodnost.IdRole;

                    // Spuštění příkazu
                    command.ExecuteNonQuery();
                }

                // Načtení hodností znovu po přidání nové
                LoadHodnosti();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při přidávání hodnosti: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Smazání vybrané hodnosti
        private void DeleteHodnost()
        {
            if (SelectedHodnost == null)
            {
                return;
            }

            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("smazat_hodnost", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.Add("p_id_hodnost", OracleDbType.Int32).Value = SelectedHodnost.IdHodnost;

                    command.ExecuteNonQuery();
                }

                LoadHodnosti();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při mazání hodnosti: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event pro PropertyChanged
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
