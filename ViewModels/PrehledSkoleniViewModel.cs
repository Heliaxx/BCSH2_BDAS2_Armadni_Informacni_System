using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Windows;
using System.Windows.Input;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;
using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;
using System;

namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
{
    public class PrehledSkoleniViewModel : INotifyPropertyChanged
    {
        private readonly Database _database;
        private PrehledSkoleni _selectedSkoleni;

        public ObservableCollection<PrehledSkoleni> Skoleni { get; set; } = new ObservableCollection<PrehledSkoleni>();

        public PrehledSkoleni SelectedSkoleni
        {
            get => _selectedSkoleni;
            set
            {
                _selectedSkoleni = value;
                OnPropertyChanged(nameof(SelectedSkoleni));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddCommand { get; set; }

        // Konstruktor
        public PrehledSkoleniViewModel()
        {
            _database = new Database();
            LoadSkoleni();
            SaveCommand = new RelayCommand(SaveSkoleni);
            DeleteCommand = new RelayCommand(DeleteSkoleni);
            AddCommand = new RelayCommand(AddSkoleni);
        }

        // Načítání školení z databáze
        private void LoadSkoleni()
        {
            Skoleni.Clear();
            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("SELECT * FROM PREHLED_SKOLENI", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Skoleni.Add(new PrehledSkoleni
                    {
                        IdSkoleni = reader.GetInt32(0),
                        Nazev = reader.GetString(1),
                        DatumOd = reader.GetDateTime(2),
                        DatumDo = reader.GetDateTime(3),
                        Misto = reader.GetString(4),
                        PocetUcastniku = reader.GetInt32(5)
                    });
                }
            }

            // Pokud není vybrán žádný prvek, nastaví se na první prvek v seznamu
            if (SelectedSkoleni == null && Skoleni.Count > 0)
            {
                SelectedSkoleni = Skoleni[0];
            }
        }

        // Uložení školení
        public void SaveSkoleni()
        {
            if (SelectedSkoleni == null) return;
            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("edit_skoleni", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.Add("p_id_skoleni", OracleDbType.Int32).Value = SelectedSkoleni.IdSkoleni;
                    command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = SelectedSkoleni.Nazev;
                    command.Parameters.Add("p_datum_od", OracleDbType.Date).Value = SelectedSkoleni.DatumOd;
                    command.Parameters.Add("p_datum_do", OracleDbType.Date).Value = SelectedSkoleni.DatumDo;
                    command.Parameters.Add("p_misto", OracleDbType.Varchar2).Value = SelectedSkoleni.Misto;
                    command.Parameters.Add("p_pocet_ucastniku", OracleDbType.Int32).Value = SelectedSkoleni.PocetUcastniku;

                    command.ExecuteNonQuery();
                }
                LoadSkoleni();  // Reload after saving
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při ukládání školení: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Vymazání výběru
        private void AddSkoleni()
        {
            if (SelectedSkoleni == null) return;
            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("edit_skoleni", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.Add("p_id_skoleni", OracleDbType.Int32).Value = DBNull.Value;
                    command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = SelectedSkoleni.Nazev;
                    command.Parameters.Add("p_datum_od", OracleDbType.Date).Value = SelectedSkoleni.DatumOd;
                    command.Parameters.Add("p_datum_do", OracleDbType.Date).Value = SelectedSkoleni.DatumDo;
                    command.Parameters.Add("p_misto", OracleDbType.Varchar2).Value = SelectedSkoleni.Misto;
                    command.Parameters.Add("p_pocet_ucastniku", OracleDbType.Int32).Value = SelectedSkoleni.PocetUcastniku;

                    command.ExecuteNonQuery();
                }

                LoadSkoleni();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při ukládání školení: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Smazání školení
        public void DeleteSkoleni()
        {
            if (SelectedSkoleni == null)
            {
                return;
            }

            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("smazat_skoleni", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.Add("p_id_skoleni", OracleDbType.Int32).Value = SelectedSkoleni.IdSkoleni;

                    command.ExecuteNonQuery();
                }

                LoadSkoleni();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při mazání školení: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
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
