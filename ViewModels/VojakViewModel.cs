using BCSH2_BDAS2_Armadni_Informacni_System.Entities;
using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                connection.Open();
                var command = new OracleCommand("SELECT * FROM Vojaci", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Vojaci.Add(new Vojak
                    {
                        IdVojak = reader.GetInt32(0),
                        Jmeno = reader.GetString(1),
                        Prijmeni = reader.GetString(2),
                        DatumNastupu = reader.GetDateTime(3),
                        DatumPropusteni = reader.GetDateTime(4),
                        IdHodnost = reader.GetInt32(5),
                        IdJednotka = reader.GetInt32(6),
                        IdZbran = reader.GetInt32(7)
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
                command.Parameters.Add("IdHodnost", SelectedVojak.IdHodnost);
                command.Parameters.Add("IdJednotka", SelectedVojak.IdJednotka);
                command.Parameters.Add("IdZbran", SelectedVojak.IdZbran);
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
                command.Parameters.Add("IdHodnost", SelectedVojak.IdHodnost);
                command.Parameters.Add("IdJednotka", SelectedVojak.IdJednotka);
                command.Parameters.Add("IdZbran", SelectedVojak.IdZbran);
                command.ExecuteNonQuery();
            }
            LoadVojaci();
        }

        private void DeleteVojak()
        {
            using (var connection = _database.GetOpenConnection())
            {
                connection.Open();
                var command = new OracleCommand("DELETE FROM Vojaci WHERE IdVojak = :IdVojak", connection);
                command.Parameters.Add("IdVojak", SelectedVojak.IdVojak);
                command.ExecuteNonQuery();
            }
            LoadVojaci();
        }
    }
}
