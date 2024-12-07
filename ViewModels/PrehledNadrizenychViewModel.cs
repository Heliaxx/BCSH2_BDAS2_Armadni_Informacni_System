using BCSH2_BDAS2_Armadni_Informacni_System.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
{
    public class PrehledNadrizeniViewModel : INotifyPropertyChanged
    {
        private readonly Database _database;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Nadrizeni> Nadrizeni { get; set; } = new ObservableCollection<Nadrizeni>();

        public PrehledNadrizeniViewModel()
        {
            _database = new Database();
            LoadNadrizeni();
        }

        public void LoadNadrizeni()
        {
            Nadrizeni.Clear();

            string query = "SELECT LEVEL_VOJAKA, ID_VOJAK, JMENO, PRIJMENI, HODNOST, ID_PRIMY_NADRIZENY FROM PREHLED_NADRYZENYCH";

            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand(query, connection);

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Nadrizeni.Add(new Nadrizeni
                    {
                        LevelVojaka = reader.GetInt32(0),
                        IdVojak = reader.GetInt32(1),
                        Jmeno = reader.GetString(2),
                        Prijmeni = reader.GetString(3),
                        Hodnost = reader.GetString(4),
                        IdPrimyNadrizeny = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5)
                    });
                }
            }
        }

    }
}
