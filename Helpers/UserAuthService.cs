using System;
using Oracle.ManagedDataAccess.Client;
using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Controls;
using BCSH2_BDAS2_Armadni_Informacni_System.Entities;
using System.Text;
using System.Windows;
using System.Linq;
using System.Globalization;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;

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
        public bool AuthenticateUser(string email, string heslo)
        {
            string storedHash;

            using (var con = _database.GetOpenConnection())
            {
                var command = new OracleCommand(
                    "SELECT V.Heslo, R.Nazev AS RoleName, V.Id_Vojak, V.Jmeno, V.Prijmeni, H.Nazev AS Hodnost " +
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
                        string userRole = reader["RoleName"].ToString();

                        // Načteme ID vojáka, jméno, příjmení a hodnost
                        int idVojak = Convert.ToInt32(reader["Id_Vojak"]);
                        string jmeno = reader["Jmeno"].ToString();
                        string prijmeni = reader["Prijmeni"].ToString();
                        string hodnost = reader["Hodnost"].ToString();

                        // Porovnání hesla pomocí hashovacího algoritmu
                        if (PasswordHasher.VerifyPassword(heslo, storedHash))
                        {
                            // Pokud je přihlášení úspěšné, uložíme informace o uživateli
                            ProfilUzivateleManager.CurrentUser = new ProfilUzivatele
                            {
                                Email = email,
                                Role = userRole,
                                IdVojak = idVojak,
                                Jmeno = jmeno,
                                Prijmeni = prijmeni,
                                Hodnost = hodnost
                            };
                            return true;
                        }
                    }
                }
            }
            return false; // Přihlášení selhalo
        }

        public void RegisterUser(string heslo, string jmeno, string prijmeni)
        {
            // Funkce pro odstranění diakritiky
            string RemoveDiacritics(string text)
            {
                return string.Concat(text.Normalize(NormalizationForm.FormD)
                                   .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
            }

            // Odstranění diakritiky z jména a příjmení
            string jmenoBezDiakritiky = RemoveDiacritics(jmeno).ToLower();
            string prijmeniBezDiakritiky = RemoveDiacritics(prijmeni).ToLower();

            // Generování emailu podle pravidla jmeno.prijmeni@vojak.com
            string email = EmailParser.GenerateEmail(jmenoBezDiakritiky, prijmeniBezDiakritiky);

            // Hashování hesla
            string passwordHash = PasswordHasher.HashPassword(heslo);

            using (var con = _database.GetOpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    try
                    {
                        // Příkaz pro volání procedury
                        cmd.CommandText = @"
    BEGIN 
        edit_vojaci(NULL, :jmeno, :prijmeni, SYSDATE, NULL, :email, :passwordHash, :idHodnost, NULL, NULL);
    END;";

                        // Přiřazení parametrů
                        cmd.Parameters.Add(new OracleParameter("jmeno", jmeno));
                        cmd.Parameters.Add(new OracleParameter("prijmeni", prijmeni));
                        cmd.Parameters.Add(new OracleParameter("email", email)); // Používáme automaticky generovaný e-mail
                        cmd.Parameters.Add(new OracleParameter("passwordHash", passwordHash));
                        cmd.Parameters.Add(new OracleParameter("idHodnost", 1));  // Pevná hodnota, upravit podle potřeby

                        // Spuštění procedury
                        cmd.ExecuteNonQuery();
                        con.Commit(); // Potvrzení transakce
                        MessageBox.Show($"Registrace úspěšná, můžete se přihlásit.\nVáš email: {email}\nPokud se vám nepodaří přihlásit, kontaktujte prosím vašeho nadřízeného");

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