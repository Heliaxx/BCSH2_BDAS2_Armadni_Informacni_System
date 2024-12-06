using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;

namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
{
    internal class PrehledUzivateleViewModel : INotifyPropertyChanged
    {
        private readonly Database _database;
        public ObservableCollection<Uzivatel> Uzivatele { get; set; } = new ObservableCollection<Uzivatel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public PrehledUzivateleViewModel()
        {
            _database = new Database();
            LoadUzivatele(); 
        }

        // Metoda pro načítání uživatelů z databáze
        private void LoadUzivatele()
        {
            Uzivatele.Clear();
            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("SELECT * FROM PREHLED_UZIVATELE", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Uzivatele.Add(new Uzivatel
                    {
                        id_vojak = reader.GetInt32(0), 
                        jmeno = reader.GetString(1),
                        prijmeni = reader.GetString(2),
                        email = reader.GetString(3),
                        nazev_hodnosti = reader.GetString(4),
                        nazev_role = reader.GetString(5)
                    }); 
                }
            }
        }
    }
}
