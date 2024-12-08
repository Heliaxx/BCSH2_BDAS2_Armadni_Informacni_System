using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;
using System.Windows;
using System.Windows.Input;
using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;
using System;

namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
{
    public class PrehledUtvarViewModel : INotifyPropertyChanged
    {
        private readonly Database _database;
        private PrehledUtvar _selectedUtvar;
        private string _searchText;

        public ObservableCollection<PrehledUtvar> Utvary { get; set; } = new ObservableCollection<PrehledUtvar>();
        public ObservableCollection<PrehledUtvar> FilteredUtvary { get; set; } = new ObservableCollection<PrehledUtvar>();

        public PrehledUtvar SelectedUtvar
        {
            get => _selectedUtvar;
            set
            {
                _selectedUtvar = value;
                OnPropertyChanged(nameof(SelectedUtvar));
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                ApplyFilter();
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
        public PrehledUtvarViewModel()
        {
            _database = new Database();
            LoadUtvary();
            SaveCommand = new RelayCommand(SaveUtvar);
            DeleteCommand = new RelayCommand(DeleteUtvar);
            AddCommand = new RelayCommand(AddUtvar);
            SetUserRolePermissions();
        }

        private void ApplyFilter()
        {
            FilteredUtvary.Clear();

            foreach (var utvar in Utvary)
            {
                if (string.IsNullOrWhiteSpace(SearchText) ||
                    utvar.Nazev.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    utvar.Typ.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    FilteredUtvary.Add(utvar);
                }
            }
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
            FilteredUtvary.Clear();
            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("SELECT * FROM PREHLED_UTVAR", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var utvar = new PrehledUtvar
                    {
                        IdUtvar = reader.GetInt32(0),
                        Nazev = reader.GetString(1),
                        Typ = reader.GetString(2),
                        IdVelikost = reader.GetInt32(3),
                        PocetVojaku = reader.GetInt32(4),
                        PocetJednotek = reader.GetInt32(5)
                    };
                    Utvary.Add(utvar);
                    FilteredUtvary.Add(utvar);
                }
            }

            // Pokud není vybrán žádný útvar, nastaví se na první prvek
            if (SelectedUtvar == null && Utvary.Count > 0)
            {
                SelectedUtvar = Utvary[0];
            }
        }

        // Uložení vybraného útvaru
        public void SaveUtvar()
        {
            if (SelectedUtvar == null) return;
            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("edit_utvary", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.Add("p_id_utvar", OracleDbType.Int32).Value = SelectedUtvar.IdUtvar;
                    command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = SelectedUtvar.Nazev;
                    command.Parameters.Add("p_typ", OracleDbType.Varchar2).Value = SelectedUtvar.Typ;
                    command.Parameters.Add("p_id_velikost", OracleDbType.Int32).Value = SelectedUtvar.IdVelikost;

                    command.ExecuteNonQuery();
                }
                LoadUtvary();  // Reload after saving
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při ukládání útvaru: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddUtvar()
        {
            if (SelectedUtvar == null) return;
            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("edit_utvary", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.Add("p_id_utvar", OracleDbType.Int32).Value = DBNull.Value;
                    command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = SelectedUtvar.Nazev;
                    command.Parameters.Add("p_typ", OracleDbType.Varchar2).Value = SelectedUtvar.Typ;
                    command.Parameters.Add("p_id_velikost", OracleDbType.Int32).Value = DBNull.Value;

                    command.ExecuteNonQuery();
                }

                // Načtení aktualizovaných údajů
                LoadUtvary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při přidávání útvaru: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void DeleteUtvar()
        {
            if (SelectedUtvar == null)
            {
                return;
            }

            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("smazat_utvar", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Přidání parametru pro ID útvaru
                    command.Parameters.Add("p_id_utvar", OracleDbType.Int32).Value = SelectedUtvar.IdUtvar;

                    // Vykonání procedury
                    command.ExecuteNonQuery();
                }

                // Po úspěšném smazání načíst znovu seznam útvarů
                LoadUtvary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při mazání útvaru: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
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
