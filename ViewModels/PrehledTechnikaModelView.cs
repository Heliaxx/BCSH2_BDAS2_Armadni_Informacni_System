using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;
using BCSH2_BDAS2_Armadni_Informacni_System.Entities;
using System.Windows;
using System.Windows.Input;
using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;
using System.Collections.Generic;

namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
{
    public class PrehledTechnikaViewModel : INotifyPropertyChanged
    {
        private readonly Database _database;
        private PrehledTechnika _selectedTechnika;
        private string _searchText;

        public ObservableCollection<PrehledTechnika> Technika { get; set; } = new ObservableCollection<PrehledTechnika>();
        public ObservableCollection<PrehledTechnika> FilteredTechnika { get; set; } = new ObservableCollection<PrehledTechnika>();
        public ObservableCollection<Utvary> Utvary { get; set; } = new ObservableCollection<Utvary>();

        public PrehledTechnika SelectedTechnika
        {
            get => _selectedTechnika;
            set
            {
                _selectedTechnika = value;
                OnPropertyChanged(nameof(SelectedTechnika));
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

        private List<string> _puvody;
        public List<string> Puvody
        {
            get { return _puvody; }
            set
            {
                _puvody = value;
                OnPropertyChanged(nameof(Puvody));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddCommand { get; }

        // Konstruktor
        public PrehledTechnikaViewModel()
        {
            _database = new Database();
            LoadUtvary();
            LoadTechnika();
            SaveCommand = new RelayCommand(SaveTechnika);
            DeleteCommand = new RelayCommand(DeleteTechnika);
            AddCommand = new RelayCommand(AddTechnika);
            Puvody = new List<string> { "I", "D" };
            SetUserRolePermissions();
        }

        private void SetUserRolePermissions()
        {
            string userRole = ProfilUzivateleManager.CurrentUser?.Role;
            CanEdit = !(userRole == "Vojáci" || userRole == "Poddůstojníci");
        }

        private void ApplyFilter()
        {
            FilteredTechnika.Clear();

            foreach (var technika in Technika)
            {
                if (string.IsNullOrWhiteSpace(SearchText) ||
                    technika.MestoVyroby.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    technika.PatriPodUtvar.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    technika.ZemeImportu.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    technika.Puvod.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    technika.Vyrobce.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    technika.RegistracniCislo.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    technika.CisloVyrobnichPlanu.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    technika.Typ.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    FilteredTechnika.Add(technika);
                }
            }
        }

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

        private void LoadTechnika()
        {
            Technika.Clear();
            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("SELECT * FROM PREHLED_TECHNIKA_UTVARY", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var technika = new PrehledTechnika
                    {
                        IdTechnika = reader.GetInt32(0),
                        Typ = reader.GetString(1),
                        RegistracniCislo = reader.GetString(2),
                        DatumPorizeni = reader.GetDateTime(3),
                        Vyrobce = reader.GetString(4),
                        Puvod = reader.GetString(5),
                        MestoVyroby = reader.IsDBNull(6) ? "Technika není domácí" : reader.GetString(6),
                        CisloVyrobnichPlanu = reader.IsDBNull(7) ? "Technika není domácí" : reader.GetString(7),
                        ZemeImportu = reader.IsDBNull(8) ? "Technika není importovaná" : reader.GetString(8),
                        IdUtvar = reader.GetInt32(9),
                        PatriPodUtvar = reader.GetString(10)
                    };
                    Technika.Add(technika);
                    FilteredTechnika.Add(technika);
                }
            }

            if (SelectedTechnika == null && Technika.Count > 0)
            {
                SelectedTechnika = Technika[0];
            }
        }

        public void SaveTechnika()
        {
            if (SelectedTechnika == null) return;

            if (SelectedTechnika.Puvod == "I" && string.IsNullOrEmpty(SelectedTechnika.ZemeImportu))
            {
                MessageBox.Show("Pro techniku s původem 'I' musíte vyplnit Zemi importu.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SelectedTechnika.Puvod == "D" && (string.IsNullOrEmpty(SelectedTechnika.MestoVyroby) || string.IsNullOrEmpty(SelectedTechnika.CisloVyrobnichPlanu)))
            {
                MessageBox.Show("Pro techniku s původem 'D' musíte vyplnit Město výroby a Číslo výrobních plánů.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("edit_technika", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.Add("p_id_technika", OracleDbType.Int32).Value = SelectedTechnika.IdTechnika;
                    command.Parameters.Add("p_typ", OracleDbType.Varchar2).Value = SelectedTechnika.Typ;
                    command.Parameters.Add("p_registracni_cislo", OracleDbType.Varchar2).Value = SelectedTechnika.RegistracniCislo;
                    command.Parameters.Add("p_datum_porizeni", OracleDbType.Date).Value = SelectedTechnika.DatumPorizeni;
                    command.Parameters.Add("p_vyrobce", OracleDbType.Varchar2).Value = SelectedTechnika.Vyrobce;
                    command.Parameters.Add("p_puvod", OracleDbType.Varchar2).Value = SelectedTechnika.Puvod;
                    command.Parameters.Add("p_id_utvar", OracleDbType.Int32).Value = SelectedTechnika.IdUtvar;

                    command.ExecuteNonQuery();
                }

                // Zavoláme pouze metodu, která odpovídá původu
                if (SelectedTechnika.Puvod == "I")
                {
                    SaveImportovana();
                }
                else if (SelectedTechnika.Puvod == "D")
                {
                    SaveDomaci();
                }

                LoadTechnika();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při ukládání techniky: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AddTechnika()
        {
            if (SelectedTechnika == null) return;

            if (SelectedTechnika.Puvod == "I" && string.IsNullOrEmpty(SelectedTechnika.ZemeImportu))
            {
                MessageBox.Show("Pro techniku s původem 'I' musíte vyplnit Zemi importu.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SelectedTechnika.Puvod == "D" && (string.IsNullOrEmpty(SelectedTechnika.MestoVyroby) || string.IsNullOrEmpty(SelectedTechnika.CisloVyrobnichPlanu)))
            {
                MessageBox.Show("Pro techniku s původem 'D' musíte vyplnit Město výroby a Číslo výrobních plánů.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("edit_technika", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.Add("p_id_technika", OracleDbType.Int32).Value = DBNull.Value;
                    command.Parameters.Add("p_typ", OracleDbType.Varchar2).Value = SelectedTechnika.Typ;
                    command.Parameters.Add("p_registracni_cislo", OracleDbType.Varchar2).Value = SelectedTechnika.RegistracniCislo;
                    command.Parameters.Add("p_datum_porizeni", OracleDbType.Date).Value = SelectedTechnika.DatumPorizeni;
                    command.Parameters.Add("p_vyrobce", OracleDbType.Varchar2).Value = SelectedTechnika.Vyrobce;
                    command.Parameters.Add("p_puvod", OracleDbType.Varchar2).Value = SelectedTechnika.Puvod;
                    command.Parameters.Add("p_id_utvar", OracleDbType.Int32).Value = SelectedTechnika.IdUtvar;

                    command.ExecuteNonQuery();
                }

                SaveImportovana(); 
                SaveDomaci();  
                LoadTechnika();  
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při přidávání techniky: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveImportovana()
        {
            if (SelectedTechnika == null) return;

            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("edit_importovana", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    if (SelectedTechnika.Puvod == "I")
                    {
                        command.Parameters.Add("p_id_technika", OracleDbType.Int32).Value = SelectedTechnika.IdTechnika;
                        command.Parameters.Add("p_zeme_importu", OracleDbType.Varchar2).Value = SelectedTechnika.ZemeImportu;
                    } else
                    {
                        command.Parameters.Add("p_id_technika", OracleDbType.Int32).Value = DBNull.Value;
                        command.Parameters.Add("p_zeme_importu", OracleDbType.Varchar2).Value = DBNull.Value;
                    }

                    command.ExecuteNonQuery();
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při ukládání do tabulky Importovaná: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveDomaci()
        {
            if (SelectedTechnika == null) return;

            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("edit_domaci", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    if (SelectedTechnika.Puvod == "D")
                    {
                        command.Parameters.Add("p_id_technika", OracleDbType.Int32).Value = SelectedTechnika.IdTechnika;
                        command.Parameters.Add("p_mesto_vyroby", OracleDbType.Varchar2).Value = SelectedTechnika.MestoVyroby;
                        command.Parameters.Add("p_cislo_vyrobnich_planu", OracleDbType.Varchar2).Value = SelectedTechnika.CisloVyrobnichPlanu;
                    }
                    else
                    {
                        command.Parameters.Add("p_id_technika", OracleDbType.Int32).Value = DBNull.Value;
                        command.Parameters.Add("p_mesto_vyroby", OracleDbType.Varchar2).Value = DBNull.Value;
                        command.Parameters.Add("p_cislo_vyrobnich_planu", OracleDbType.Varchar2).Value = DBNull.Value;
                    }

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při ukládání do tabulky Domácí: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void DeleteTechnika()
        {
            if (SelectedTechnika == null) return;
            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("smazat_techniku", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.Add("p_id_technika", OracleDbType.Int32).Value = SelectedTechnika.IdTechnika;

                    command.ExecuteNonQuery();
                }
                LoadTechnika();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při mazání techniky: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
