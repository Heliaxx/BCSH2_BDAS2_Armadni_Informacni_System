using BCSH2_BDAS2_Armadni_Informacni_System;
using BCSH2_BDAS2_Armadni_Informacni_System.Entities;
using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
{
    public class VojakViewModel : INotifyPropertyChanged
    {
        private readonly Database _database;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Vojak> Vojaci { get; set; } = new ObservableCollection<Vojak>();
        public Vojak SelectedVojak { get; set; }

        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }

        public VojakViewModel()
        {
            _database = new Database();
            LoadCommand = new RelayCommand(LoadVojaci);
            AddCommand = new RelayCommand(AddVojak);
            UpdateCommand = new RelayCommand(UpdateVojak);
            DeleteCommand = new RelayCommand(DeleteVojak);
        }

        private void LoadVojaci()
        {
            Vojaci.Clear();
            using (var connection = _database.GetOpenConnection())
            {
                //connection.Open();
                var command = new OracleCommand("SELECT * FROM Prehled_Vojaci", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Vojaci.Add(new Vojak
                    {
                        //IdVojak = reader.GetInt32(0),            // ID vojáka
                        Jmeno = reader.GetString(1),            // Jméno
                        Prijmeni = reader.GetString(2),         // Příjmení
                        DatumNastupu = reader.GetDateTime(3),   // Datum nástupu
                        DatumPropusteni = reader.GetDateTime(4),// Datum propuštění
                        Hodnost = reader.GetString(5),          // Hodnost (přidat do modelu)
                        Jednotka = reader.GetString(6)
                    });
                }
            }
        }

        private void AddVojak()
        {
            using (var connection = _database.GetOpenConnection())
            {
                connection.Open();
                var command = new OracleCommand("INSERT INTO Vojaci (IdVojak, Jmeno, Prijmeni, DatumNastupu, DatumPropusteni, IdHodnost, IdJednotka, IdZbran) VALUES (:IdVojak, :Jmeno, :Prijmeni, :DatumNastupu, :DatumPropusteni, :IdHodnost, :IdJednotka, :IdZbran)", connection);
                command.Parameters.Add("IdVojak", SelectedVojak.IdVojak);
                command.Parameters.Add("Jmeno", SelectedVojak.Jmeno);
                command.Parameters.Add("Prijmeni", SelectedVojak.Prijmeni);
                command.Parameters.Add("DatumNastupu", SelectedVojak.DatumNastupu);
                command.Parameters.Add("DatumPropusteni", SelectedVojak.DatumPropusteni);
                //command.Parameters.Add("IdHodnost", SelectedVojak.IdHodnost);
                //command.Parameters.Add("IdJednotka", SelectedVojak.IdJednotka);
                //command.Parameters.Add("IdZbran", SelectedVojak.IdZbran);
                command.ExecuteNonQuery();
            }
            LoadVojaci();
        }

        private void UpdateVojak()
        {
            using (var connection = _database.GetOpenConnection())
            {
                connection.Open();
                var command = new OracleCommand("UPDATE Vojaci SET Jmeno = :Jmeno, Prijmeni = :Prijmeni, DatumNastupu = :DatumNastupu, DatumPropusteni = :DatumPropusteni, IdHodnost = :IdHodnost, IdJednotka = :IdJednotka, IdZbran = :IdZbran WHERE IdVojak = :IdVojak", connection);
                command.Parameters.Add("IdVojak", SelectedVojak.IdVojak);
                command.Parameters.Add("Jmeno", SelectedVojak.Jmeno);
                command.Parameters.Add("Prijmeni", SelectedVojak.Prijmeni);
                command.Parameters.Add("DatumNastupu", SelectedVojak.DatumNastupu);
                command.Parameters.Add("DatumPropusteni", SelectedVojak.DatumPropusteni);
                //command.Parameters.Add("IdHodnost", SelectedVojak.IdHodnost);
                //command.Parameters.Add("IdJednotka", SelectedVojak.IdJednotka);
                //command.Parameters.Add("IdZbran", SelectedVojak.IdZbran);
                command.ExecuteNonQuery();
            }
            LoadVojaci();
        }

        private void DeleteVojak()
        {
            using (var connection = _database.GetOpenConnection())
            {
                connection.Open();
                try
                {
                    var command = new OracleCommand("smazat_vojaka", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.Add("p_id_vojak", OracleDbType.Int32).Value = SelectedVojak.IdVojak;
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while deleting soldier: {ex.Message}");
                }
            }
            LoadVojaci(); // Refresh the list
        }
    }
}

//namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
//{
//    public class VojakViewModel
//    {
//        private MockDatabaseService mockDatabaseService;

//        public ObservableCollection<Vojak> Vojaci { get; set; }

//        public VojakViewModel()
//        {
//            mockDatabaseService = new MockDatabaseService();
//            LoadVojaci();
//        }

//        private void LoadVojaci()
//        {
//            Vojaci = new ObservableCollection<Vojak>(mockDatabaseService.GetAllVojaci());
//        }
//    }
//}


//namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
//{
//    public class VojakViewModel
//    {
//        private MockDatabaseService mockDatabaseService;

//        public ObservableCollection<Vojak> Vojaci { get; set; }

//        public Vojak SelectedVojak { get; set; }

//        public ICommand DeleteCommand { get; }

//        public VojakViewModel()
//        {
//            mockDatabaseService = new MockDatabaseService();
//            LoadVojaci();
//            DeleteCommand = new RelayCommand<int>(DeleteVojak);
//        }

//        // Načte všechny vojáky do ObservableCollection, aby bylo UI aktualizováno automaticky
//        private void LoadVojaci()
//        {
//            Vojaci = new ObservableCollection<Vojak>(mockDatabaseService.GetAllVojaci());
//        }

//        // Přidání nového vojáka
//        public void AddVojak(Vojak newVojak)
//        {
//            mockDatabaseService.AddVojak(newVojak);  // Přidá vojáka do mock databáze
//            Vojaci.Add(newVojak); // Přidá vojáka do ObservableCollection, UI se automaticky aktualizuje
//        }

//        // Aktualizace existujícího vojáka
//        public void UpdateVojak(Vojak updatedVojak)
//        {
//            var existingVojak = Vojaci.FirstOrDefault(v => v.IdVojak == updatedVojak.IdVojak);
//            if (existingVojak != null)
//            {
//                existingVojak.Jmeno = updatedVojak.Jmeno;
//                existingVojak.Prijmeni = updatedVojak.Prijmeni;
//                existingVojak.Email = updatedVojak.Email;
//                existingVojak.DatumNastupu = updatedVojak.DatumNastupu;
//                existingVojak.DatumPropusteni = updatedVojak.DatumPropusteni;

//                mockDatabaseService.UpdateVojak(updatedVojak); // Aktualizuje vojáka v mock databázi
//            }
//        }

//        // Odstranění vojáka
//        public void DeleteVojak(int idVojak)
//        {
//            var vojakToDelete = Vojaci.FirstOrDefault(v => v.IdVojak == idVojak);
//            if (vojakToDelete != null)
//            {
//                Vojaci.Remove(vojakToDelete); // Odebere vojáka z ObservableCollection, UI se automaticky aktualizuje
//                mockDatabaseService.DeleteVojak(idVojak); // Odebere vojáka z mock databáze
//            }
//        }

//        // Kontrola, zda je voják, kterého je možné smazat, vybrán
//        private bool CanDeleteVojak(int idVojak)
//        {
//            return idVojak > 0; // Pokud ID vojáka existuje, umožníme smazání
//        }
//    }
//}