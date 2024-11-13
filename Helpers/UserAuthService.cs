using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;

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
        public bool AuthenticateUser(string email, string password, out string userRole)
        {
            userRole = string.Empty;
            string storedHash;

            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("SELECT PasswordHash, Role.Nazev AS RoleName FROM Vojak JOIN Role ON Vojak.IdRole = Role.IdRole WHERE Email = :email", connection);
                command.Parameters.Add(new OracleParameter("email", email));

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        storedHash = reader["PasswordHash"].ToString();
                        userRole = reader["RoleName"].ToString();

                        return PasswordHasher.VerifyPassword(password, storedHash); // Porovnání hesla
                    }
                }
            }
            return false;
        }

        public bool RegisterUser(string email, string password, string firstName, string lastName)
        {
            string passwordHash = PasswordHasher.HashPassword(password);

            using (var connection = _database.GetOpenConnection())
            {
                try
                {
                    var command = new OracleCommand(
                        "INSERT INTO Vojak (Email, PasswordHash, Jmeno, Prijmeni, IdRole) VALUES (:email, :passwordHash, :firstName, :lastName, :defaultRoleId)",
                        connection);

                    command.Parameters.Add(new OracleParameter("email", email));
                    command.Parameters.Add(new OracleParameter("passwordHash", passwordHash));
                    command.Parameters.Add(new OracleParameter("firstName", firstName));
                    command.Parameters.Add(new OracleParameter("lastName", lastName));
                    command.Parameters.Add(new OracleParameter("defaultRoleId", 2)); // 2 = ID role pro běžného uživatele

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (OracleException)
                {
                    // Ošetření výjimek, např. když je email již zaregistrován
                    return false;
                }
            }
        }
    }
}