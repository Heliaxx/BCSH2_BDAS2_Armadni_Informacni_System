using BCSH2_BDAS2_Armadni_Informacni_System.Models;
using BCSH2_BDAS2_Armadni_Informacni_System;
using Oracle.ManagedDataAccess.Client;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System;
using BCSH2_BDAS2_Armadni_Informacni_System.Entities;
using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;
using System.Linq;
using System.Windows;
using System.Data;
using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;
using BCSH2_BDAS2_Armadni_Informacni_System.Views;
using System.Windows.Input;
using Oracle.ManagedDataAccess.Types;

public class PrehledVojaciViewModel : INotifyPropertyChanged
{
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private readonly Database _database;

    // Přidání privátní proměnné pro aktuálně vybraného vojáka
    private PrehledVojaci _selectedVojak;

    // Kolekce pro hodnosti a jednotky
    public ObservableCollection<Hodnost> Hodnosti { get; set; } = new ObservableCollection<Hodnost>();
    public ObservableCollection<Jednotka> Jednotky { get; set; } = new ObservableCollection<Jednotka>();

    public RelayCommand AddCommand { get; set; }
    public RelayCommand SaveCommand { get; set; }
    public RelayCommand DeleteCommand { get; set; }
    public RelayCommand ShowCommand { get; set; }
    public RelayCommand AdeptCommand { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public ObservableCollection<PrehledVojaci> Vojaci { get; set; } = new ObservableCollection<PrehledVojaci>();
    public PrehledVojaci SelectedVojak
    {
        get { return _selectedVojak; }
        set
        {
            _selectedVojak = value;
            OnPropertyChanged(nameof(SelectedVojak));

            if (_selectedVojak != null)
            {
                // Nastavíme hodnoty pro ComboBoxy podle ID
                HodnostId = _selectedVojak.HodnostId;
                JednotkaId = _selectedVojak.JednotkaId ?? 0;
            }

            // Aktualizace tlačítek a jiných vlastností
            DeleteCommand.RaiseCanExecuteChanged();
        }
    }

    private bool _canEdit = false;
    public bool CanEdit
    {
        get => _canEdit;
        set
        {
            _canEdit = value;
            OnPropertyChanged(nameof(CanEdit));
        }
    }

    // Vlastnosti pro výběr hodnosti a jednotky
    public int HodnostId { get; set; }
    public int JednotkaId { get; set; }

    public PrehledVojaciViewModel()
    {
        _database = new Database();
        LoadHodnosti();
        LoadJednotky();
        LoadVojaci();
        AddCommand = new RelayCommand(AddVojak);
        SaveCommand = new RelayCommand(SaveVojak);
        DeleteCommand = new RelayCommand(DeleteVojak);
        ShowCommand = new RelayCommand(ShowNadrizeny);
        AdeptCommand = new RelayCommand(Adept);
        SetUserRolePermissions();
    }

    private void Adept()
    {
        
    }

    private void SetUserRolePermissions()
    {
        string userRole = ProfilUzivateleManager.CurrentUser?.Role;
        CanEdit = !(userRole == "Vojáci" || userRole == "Poddůstojníci" || userRole == "Důstojníci");
    }

    private void ShowNadrizeny()
    {
        if (SelectedVojak == null) return;

        try
        {
            // SQL příkaz s dynamickou hodnotou
            string createViewQuery = $@"
            CREATE OR REPLACE VIEW prehled_nadryzenych AS
            SELECT 
                LEVEL AS LEVEL_VOJAKA, 
                v.JMENO, 
                v.PRIJMENI, 
                h.NAZEV AS HODNOST
            FROM 
                VOJACI v
            JOIN 
                HODNOSTI h ON v.ID_HODNOST = h.ID_HODNOST
            START WITH v.ID_VOJAK = {SelectedVojak.IdVojak}
            CONNECT BY PRIOR v.ID_PRIMY_NADRIZENY = v.ID_VOJAK  
            AND LEVEL <= 4";

            using (var connection = _database.GetOpenConnection())
            {
                using (var createCommand = new OracleCommand(createViewQuery, connection))
                {
                    createCommand.ExecuteNonQuery();
                }
            }

            // Otevření okna pro zobrazení pohledu
            var nadrizeniWindow = new PrehledNadrizenychWindow();
            nadrizeniWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Chyba: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SaveVojak()
    {
        if (SelectedVojak == null) return;

        try
        {

            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("edit_vojaci", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                string email = EmailParser.GenerateEmail(SelectedVojak.Jmeno, SelectedVojak.Prijmeni);

                command.Parameters.Add("p_id_vojak", OracleDbType.Int32).Value = SelectedVojak.IdVojak;
                command.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = SelectedVojak.Jmeno;
                command.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = SelectedVojak.Prijmeni;
                command.Parameters.Add("p_datum_nastupu", OracleDbType.Date).Value = SelectedVojak.DatumNastupu;
                command.Parameters.Add("p_datum_propusteni", OracleDbType.Date).Value = SelectedVojak.DatumPropusteni ?? (object)DBNull.Value;
                command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = email;
                command.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = SelectedVojak.Heslo;
                command.Parameters.Add("p_id_hodnost", OracleDbType.Int32).Value = SelectedVojak.HodnostId;
                command.Parameters.Add("p_id_jednotka", OracleDbType.Int32).Value = SelectedVojak.JednotkaId ?? (object)DBNull.Value;
                command.Parameters.Add("p_id_primy_nadrizeny", OracleDbType.Int32).Value = SelectedVojak.PrimyNadrizenyId ?? (object)DBNull.Value;

                // Spuštění procedury
                command.ExecuteNonQuery();
            }

            // Aktualizace seznamu vojáků
            LoadVojaci();

        }
        catch (OracleException ex)
        {
            // Zpracování chyby při ukládání
            MessageBox.Show($"Chyba při ukládání vojáka: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
    }

    private void AddVojak()
    {
        if (SelectedVojak == null) return;

        try
        {

            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("edit_vojaci", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                string email = EmailParser.GenerateEmail(SelectedVojak.Jmeno, SelectedVojak.Prijmeni);

                command.Parameters.Add("p_id_vojak", OracleDbType.Int32).Value = DBNull.Value;
                command.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = SelectedVojak.Jmeno;
                command.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = SelectedVojak.Prijmeni;
                command.Parameters.Add("p_datum_nastupu", OracleDbType.Date).Value = SelectedVojak.DatumNastupu;
                command.Parameters.Add("p_datum_propusteni", OracleDbType.Date).Value = SelectedVojak.DatumPropusteni ?? (object)DBNull.Value;
                command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = email;
                command.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = SelectedVojak.Heslo ?? "placeholder";
                command.Parameters.Add("p_id_hodnost", OracleDbType.Int32).Value = SelectedVojak.HodnostId;
                command.Parameters.Add("p_id_jednotka", OracleDbType.Int32).Value = SelectedVojak.JednotkaId ?? (object)DBNull.Value;
                command.Parameters.Add("p_id_primy_nadrizeny", OracleDbType.Int32).Value = SelectedVojak.PrimyNadrizenyId ?? (object)DBNull.Value;

                // Spuštění procedury
                command.ExecuteNonQuery();
            }

            // Aktualizace seznamu vojáků
            LoadVojaci();

        }
        catch (OracleException ex)
        {
            // Zpracování chyby při ukládání
            MessageBox.Show($"Chyba při ukládání vojáka: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
    }

    private void DeleteVojak()
    {
        if (SelectedVojak == null)
        {
            return;
        }

        var result = MessageBox.Show($"Opravdu chcete smazat vojáka {SelectedVojak.Jmeno} {SelectedVojak.Prijmeni}?",
                                     "Potvrzení mazání",
                                     MessageBoxButton.YesNo,
                                     MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        using (var connection = _database.GetOpenConnection())
        {
            try
            {
                var command = new OracleCommand("BEGIN smazat_vojaka(:idVojak); END;", connection);
                command.Parameters.Add(new OracleParameter(":idVojak", SelectedVojak.IdVojak));

                command.ExecuteNonQuery();
            }
            catch (OracleException ex)
            {
                // Zpracování chyby při mazání
                MessageBox.Show($"Chyba při mazání vojáka: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        LoadVojaci();

        SelectedVojak = null;
        OnPropertyChanged(nameof(SelectedVojak));
    }


    // Načítání hodností z databáze
    private void LoadHodnosti()
    {
        Hodnosti.Clear();
        using (var connection = _database.GetOpenConnection())
        {
            // Načítáme všechny potřebné sloupce z tabulky hodnosti
            var command = new OracleCommand("SELECT id_hodnost, nazev, odmeny, potrebny_stupen_vzdelani, potrebny_pocet_let_v_praxi, vaha_hodnosti, id_role FROM hodnosti ORDER BY id_hodnost", connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                Hodnosti.Add(new Hodnost
                {
                    IdHodnost = reader.GetInt32(0),
                    Nazev = reader.GetString(1),
                    Odmeny = reader.IsDBNull(2) ? "Tato hodnost nemá odměny" : reader.GetString(2),
                    PotrebnyStupenVzdelani = reader.GetString(3),
                    PotrebnyPocetLetVPraxi = reader.GetInt32(4),
                    VahaHodnosti = reader.GetInt32(5),
                    IdRole = reader.GetInt32(6)
                });
            }
        }
    }

    // Načítání jednotek z databáze
    private void LoadJednotky()
    {
        Jednotky.Clear();

        // Přidání možnosti "Voják zatím nepatří do žádné jednotky"
        Jednotky.Add(new Jednotka
        {
            IdJednotka = -1,
            Nazev = "Voják zatím nepatří do žádné jednotky",
            Typ = "",
            Velikost = 0,
            IdUtvar = -1
        });

        using (var connection = _database.GetOpenConnection())
        {
            var command = new OracleCommand("SELECT id_jednotka, nazev, typ, velikost, id_utvar FROM jednotky", connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                Jednotky.Add(new Jednotka
                {
                    IdJednotka = reader.GetInt32(0),
                    Nazev = reader.GetString(1),
                    Typ = reader.GetString(2),
                    Velikost = reader.GetInt32(3),
                    IdUtvar = reader.GetInt32(4)
                });
            }
        }
    }

    // Načítání vojáků z databáze
    private void LoadVojaci()
    {
        Vojaci.Clear();

        using (var connection = _database.GetOpenConnection())
        {
            var command = new OracleCommand("SELECT * FROM Prehled_Vojaci", connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var voj = new PrehledVojaci
                {
                    IdVojak = reader.GetInt32(0),
                    Jmeno = reader.GetString(1),
                    Prijmeni = reader.GetString(2),
                    DatumNastupu = reader.GetDateTime(3),
                    DatumPropusteni = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                    Email = reader.GetString(5),
                    Heslo = reader.GetString(6),
                    Hodnost = reader.GetString(7),
                    Jednotka = reader.IsDBNull(8) ? "Voják zatím nepatří do žádné jednotky" : reader.GetString(8),
                    HodnostId = reader.GetInt32(9),
                    JednotkaId = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10),
                    PrimyNadrizeny = reader.IsDBNull(11) ? "Voják nemá přímého nadřízeného" : reader.GetString(11),
                    PrimyNadrizenyId = reader.IsDBNull(12) ? (int?)null : reader.GetInt32(12)
                };

                // Při načítání vojáka také aktualizujeme hodnoty pro comboBoxy
                if (_selectedVojak != null && voj.IdVojak == _selectedVojak.IdVojak)
                {
                    HodnostId = voj.HodnostId;
                    JednotkaId = voj.JednotkaId ?? -1;
                }

                Vojaci.Add(voj);
            }
        }
    }
}