using System;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace BCSH2_BDAS2_Armadni_Informacni_System
{
    internal class Database
    {
        private string connectionString;

        public Database()
        {
            // Načtení connection stringu z app.config
            connectionString = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
        }

        public string TestConnection()
        {
            using (var connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    return "Připojeno k databázi.";
                }
                catch (Exception ex)
                {
                    return "Chyba připojení: " + ex.Message;
                }
            }
        }
    }
}
