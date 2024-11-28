using System;
using Oracle.ManagedDataAccess.Client;
using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Controls;
using BCSH2_BDAS2_Armadni_Informacni_System.Entities;
using System.Text;
using System.Windows;

namespace BCSH2_BDAS2_Armadni_Informacni_System
{
    public class UserAuthService
    {
        private readonly Database _database;

        public UserAuthService()
        {
            _database = new Database();
        }

        // Ověření uživatele na základě e-mailu a hesla
        public bool AuthenticateUser(string email, string heslo, out string userRole)
        {
            userRole = string.Empty;
            string storedHash;

            //Console.WriteLine($"pwdHash: {passwordHash}");
            Debug.WriteLine($"pwdHash: {heslo}");

            using (var con = _database.GetOpenConnection())
            {
                var command = new OracleCommand(
                    // "SELECT Heslo FROM Vojaci WHERE Email = :email",
                    "SELECT V.Heslo, R.Nazev AS RoleName " +
                    "FROM Vojaci V " +
                    "JOIN Hodnosti H ON V.Id_Hodnost = H.Id_Hodnost " +
                    "JOIN Role R ON H.Id_Role = R.Id_Role " +
                    "WHERE V.Email = :email",
                    con);

                command.Parameters.Add(new OracleParameter("email", email));

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        storedHash = reader["Heslo"].ToString();
                        userRole = reader["RoleName"].ToString(); 
                        // Porovnání hesla pomocí hashovacího algoritmu
                        return PasswordHasher.VerifyPassword(heslo, storedHash);
                    }
                }
            }
            return false; // Přihlášení selhalo
        }

        public void RegisterUser(string email, string heslo, string jmeno, string prijmeni)
        {
            string passwordHash = PasswordHasher.HashPassword(heslo); // Hashování hesla

            using (var con = _database.GetOpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = @"
                    INSERT INTO Vojaci (Jmeno, Prijmeni, Datum_Nastupu, Email, Heslo, Id_Jednotka, Id_Hodnost)
                    VALUES (:jmeno, :prijmeni, SYSDATE, :email, :passwordHash, :idJednotka, :idHodnost)";

                        // Přiřazení parametrů
                        cmd.Parameters.Add(new OracleParameter("jmeno", jmeno));
                        cmd.Parameters.Add(new OracleParameter("prijmeni", prijmeni));
                        cmd.Parameters.Add(new OracleParameter("email", email));
                        cmd.Parameters.Add(new OracleParameter("passwordHash", passwordHash));
                        cmd.Parameters.Add(new OracleParameter("idJednotka", 1)); // Pevná hodnota, upravit podle potřeby
                        cmd.Parameters.Add(new OracleParameter("idHodnost", 1));  // Pevná hodnota, upravit podle potřeby

                        // Spuštění dotazu
                        int rowsAffected = cmd.ExecuteNonQuery();
                        con.Commit(); // Potvrzení transakce
                        MessageBox.Show("Registrace úspěšná, můžete se přihlásit.");
                    }
                    catch (OracleException ex)
                    {
                        MessageBox.Show($"Chyba při registraci: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        // Obecné zpracování chyb
                        MessageBox.Show($"Neočekávaná chyba: {ex.Message}");
                    }
                }
            }
        }

    }
}