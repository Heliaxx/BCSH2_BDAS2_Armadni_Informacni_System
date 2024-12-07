using System.Collections.ObjectModel;
using System.ComponentModel;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;
using Oracle.ManagedDataAccess.Client;

namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
{
    internal class PrehledNadrizenychViewModel : INotifyPropertyChanged
    {
        private readonly Database _database;

        public ObservableCollection<PrehledNadrizenych> Nadrizeni { get; set; } = new ObservableCollection<PrehledNadrizenych>();

        public event PropertyChangedEventHandler PropertyChanged;

        public PrehledNadrizenychViewModel()
        {
            _database = new Database();
            LoadNadrizeni();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadNadrizeni()
        {
            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("SELECT * FROM PREHLED_NADRYZENYCH", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Nadrizeni.Add(new PrehledNadrizenych
                    {
                        LevelVojaka = reader.GetInt32(0),
                        Jmeno = reader.GetString(1),
                        Prijmeni = reader.GetString(2),
                        Hodnost = reader.GetString(3)
                    });
                }
            }
        }
    }
}
