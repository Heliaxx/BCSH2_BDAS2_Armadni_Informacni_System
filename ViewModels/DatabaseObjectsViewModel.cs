using System;
using System.Collections.ObjectModel;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Windows;
using System.ComponentModel;

namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
{
    public class DatabaseObjectsViewModel : INotifyPropertyChanged
    {
        private readonly Database _database;

        public ObservableCollection<Models.DatabaseObject> DatabaseObjects { get; set; } = new ObservableCollection<Models.DatabaseObject>();

        public event PropertyChangedEventHandler PropertyChanged;

        public DatabaseObjectsViewModel()
        {
            _database = new Database();
            LoadDatabaseObjects();
        }

        // Načítání databázových objektů
        public void LoadDatabaseObjects()
        {
            string query = @"
                SELECT OBJECT_NAME, OBJECT_TYPE
                FROM USER_OBJECTS
                WHERE OBJECT_TYPE IN ('TABLE', 'VIEW', 'INDEX', 'SYNONYM', 'PROCEDURE', 'FUNCTION', 'TRIGGER')";

            try
            {
                using (var connection = _database.GetOpenConnection())
                {
                    using (var command = new OracleCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            DatabaseObjects.Clear();
                            while (reader.Read())
                            {
                                DatabaseObjects.Add(new Models.DatabaseObject
                                {
                                    Name = reader.GetString(0),
                                    Type = reader.GetString(1)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při načítání databázových objektů: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
