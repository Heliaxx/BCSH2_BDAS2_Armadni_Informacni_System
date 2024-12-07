using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;

namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
{
    internal class PrehledRoleViewModel : INotifyPropertyChanged
    {
        private readonly Database _database;

        public ObservableCollection<PrehledRole> Role { get; set; } = new ObservableCollection<PrehledRole>();

        public event PropertyChangedEventHandler PropertyChanged;

        // Konstruktor
        public PrehledRoleViewModel()
        {
            _database = new Database();
            LoadRole();
        }

        // Načítání Rolí z databáze
        private void LoadRole()
        {
            Role.Clear();
            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("SELECT * FROM PREHLED_ROLE", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Role.Add(new PrehledRole
                    {
                        id_role = reader.GetInt32(0),
                        nazev_role = reader.GetString(1)
                    });
                }
            }
        }
    }
}
