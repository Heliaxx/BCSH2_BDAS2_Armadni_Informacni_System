using BCSH2_BDAS2_Armadni_Informacni_System.Models;
using BCSH2_BDAS2_Armadni_Informacni_System;
using Oracle.ManagedDataAccess.Client;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
{
    public class PrehledLogyViewModel : INotifyPropertyChanged
    {
        private readonly Database _database;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<PrehledLogy> Logy { get; set; } = new ObservableCollection<PrehledLogy>();

        public PrehledLogyViewModel()
        {
            _database = new Database();
            LoadLogy();
        }

        private void LoadLogy()
        {
            Logy.Clear();

            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("SELECT id_logy, datum_a_cas, zaznam FROM prehled_logy", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Logy.Add(new PrehledLogy
                    {
                        IdLogy = reader.GetInt32(0),
                        DatumACas = reader.GetString(1),
                        Zaznam = reader.GetString(2)
                    });
                }
            }
        }
    }
}