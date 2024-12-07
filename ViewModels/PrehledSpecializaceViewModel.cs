using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
{
    public class PrehledSpecializaceViewModel : INotifyPropertyChanged
    {
        private readonly Database _database;
        private PrehledSpecializace _selectedSpecializace;
        private string _searchText;

        public ObservableCollection<PrehledSpecializace> Specializace { get; set; } = new ObservableCollection<PrehledSpecializace>();
        public ObservableCollection<PrehledSpecializace> FilteredSpecializace { get; private set; } = new ObservableCollection<PrehledSpecializace>();
        public ObservableCollection<Utvary> Utvary { get; set; } = new ObservableCollection<Utvary>();

        public PrehledSpecializace SelectedSpecializace
        {
            get => _selectedSpecializace;
            set
            {
                _selectedSpecializace = value;
                OnPropertyChanged(nameof(SelectedSpecializace));
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
        public PrehledSpecializaceViewModel()
        {
            _database = new Database();
            LoadSpecializace();
            SaveCommand = new RelayCommand(SaveSpecializace);
            DeleteCommand = new RelayCommand(DeleteSpecializace);
            AddCommand = new RelayCommand(AddSpecializace);
            SetUserRolePermissions();
        }

        private void SetUserRolePermissions()
        {
            string userRole = ProfilUzivateleManager.CurrentUser?.Role;
            CanEdit = !(userRole == "Vojáci" || userRole == "Poddůstojníci" || userRole == "Důstojníci");
        }

        private void ApplyFilter()
        {
            FilteredSpecializace.Clear();

            foreach (var specializace in Specializace)
            {
                if (string.IsNullOrWhiteSpace(SearchText) ||
                    specializace.nazev.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    specializace.popis.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    FilteredSpecializace.Add(specializace);
                }
            }
        }

        private void LoadSpecializace()
        {
            Specializace.Clear();
            FilteredSpecializace.Clear();
            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("SELECT * FROM PREHLED_SPECIALIZACE", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var specializace = new PrehledSpecializace
                    {
                        id_specializace = reader.GetInt32(0),
                        nazev = reader.GetString(1),
                        stupen_narocnosti = reader.GetInt32(2),
                        popis = reader.GetString(3)
                    };
                    Specializace.Add(specializace);
                    FilteredSpecializace.Add(specializace);
                }
            }

            // Pokud není vybrána žádná jednotka, nastaví se na první prvek
            if (SelectedSpecializace == null && Specializace.Count > 0)
            {
                SelectedSpecializace = Specializace[0];
            }
        }

        // Uložení vybrané jednotky
        public void SaveSpecializace()
        {
            if (SelectedSpecializace == null) return;
            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("edit_specializace", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.Add("p_id_specializace", OracleDbType.Int32).Value = SelectedSpecializace.id_specializace;
                    command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = SelectedSpecializace.nazev;
                    command.Parameters.Add("p_stupen_narocnosti", OracleDbType.Int32).Value = SelectedSpecializace.stupen_narocnosti;
                    command.Parameters.Add("p_popis", OracleDbType.Varchar2).Value = SelectedSpecializace.popis;

                    command.ExecuteNonQuery();
                }
                LoadSpecializace();  // Reload after saving
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při ukládání jednotky: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddSpecializace()
        {
            if (SelectedSpecializace == null) return;
            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("edit_specializace", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.Add("p_id_specializace", OracleDbType.Int32).Value = DBNull.Value;
                    command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = SelectedSpecializace.nazev;
                    command.Parameters.Add("p_stupen_narocnosti", OracleDbType.Int32).Value = SelectedSpecializace.stupen_narocnosti;
                    command.Parameters.Add("p_popis", OracleDbType.Varchar2).Value = SelectedSpecializace.popis;

                    command.ExecuteNonQuery();
                }
                LoadSpecializace();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při ukládání jednotky: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteSpecializace()
        {
            if (SelectedSpecializace == null)
            {
                return;
            }

            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("smazat_specializaci", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Přidání parametru pro ID jednotky
                    command.Parameters.Add("p_id_specializace", OracleDbType.Int32).Value = SelectedSpecializace.id_specializace;

                    // Vykonání procedury
                    command.ExecuteNonQuery();
                }

                // Po úspěšném smazání načíst znovu seznam jednotek
                LoadSpecializace();
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
