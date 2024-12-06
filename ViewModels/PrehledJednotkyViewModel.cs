using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;
using BCSH2_BDAS2_Armadni_Informacni_System.Entities;
using System.Windows;
using System;
using System.Windows.Controls;

namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
{
    internal class PrehledJednotkyViewModel : INotifyPropertyChanged
    {
        private readonly Database _database;
        private PrehledJednotky _selectedJednotka;

        // ObservableCollection pro jednotky
        public ObservableCollection<PrehledJednotky> Jednotky { get; set; } = new ObservableCollection<PrehledJednotky>();

        // ObservableCollection pro útvary
        public ObservableCollection<Utvary> Utvary { get; set; } = new ObservableCollection<Utvary>();

        // Property pro vybranou jednotku
        public PrehledJednotky SelectedJednotka
        {
            get => _selectedJednotka;
            set
            {
                _selectedJednotka = value;
                OnPropertyChanged(nameof(SelectedJednotka));
            }
        }

        // Událost pro PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public PrehledJednotkyViewModel()
        {
            _database = new Database();
            LoadUtvary();
            LoadJednotky();
        }

        // Načte útvary z databáze
        private void LoadUtvary()
        {
            // Vymazání předchozích dat
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

        // Načte jednotky z databáze
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
        }

        // Uloží změny na vybrané jednotce do databáze
        public void SaveJednotka()
        {
            if (SelectedJednotka == null)
            {

            }

            try
            {
                // Příprava příkazu pro volání procedury edit_jednotky
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("edit_jednotky", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Parametry pro proceduru
                    command.Parameters.Add("p_id_jednotka", OracleDbType.Int32).Value = SelectedJednotka.IdJednotka;
                    command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = SelectedJednotka.Nazev;
                    command.Parameters.Add("p_typ", OracleDbType.Varchar2).Value = SelectedJednotka.Typ;
                    command.Parameters.Add("p_velikost", OracleDbType.Int32).Value = SelectedJednotka.Velikost;
                    command.Parameters.Add("p_id_utvar", OracleDbType.Int32).Value = SelectedJednotka.IdUtvar;  

                    // Provést příkaz
                    command.ExecuteNonQuery();
                }

                // Po úspěšné aktualizaci, reload jednotek
                LoadJednotky();
            }
            catch (Exception ex)
            {
                // Zpracování chyby (např. logování nebo zobrazení chybové hlášky)
                MessageBox.Show($"Chyba při ukládání jednotky: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Smaže vybranou jednotku z databáze
        public void DeleteJednotka()
        {

        }

        // Metoda pro PropertyChanged
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
