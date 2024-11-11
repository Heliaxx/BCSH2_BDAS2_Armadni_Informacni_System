using System;
using System.Configuration;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;
using Oracle.ManagedDataAccess.Client;

namespace BCSH2_BDAS2_Armadni_Informacni_System
{
    internal class Database
    {
        private static string connectionString;

        public Database()
        {
            // Load connection string from app.config
            connectionString = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
        }

        public OracleConnection GetOpenConnection()
        {
            var connection = new OracleConnection(connectionString);
            connection.Open();
            return connection;
        }

        public string TestConnection()
        {
            using (var connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    return "Database connection established.";
                }
                catch (Exception ex)
                {
                    return "Connection error: " + ex.Message;
                }
            }
        }
    }
}