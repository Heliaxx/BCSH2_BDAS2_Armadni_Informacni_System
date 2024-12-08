using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;
using BCSH2_BDAS2_Armadni_Informacni_System;
using Oracle.ManagedDataAccess.Client;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows.Input;
using System.Windows;
using System;

public class PrehledDovolenkyViewModel : INotifyPropertyChanged
{
    private readonly Database _database;
    private PrehledDovolenky _selectedDovolenka;

    public ObservableCollection<PrehledDovolenky> Dovolene { get; set; } = new ObservableCollection<PrehledDovolenky>();
    public PrehledDovolenky SelectedDovolenka
    {
        get => _selectedDovolenka;
        set
        {
            _selectedDovolenka = value;
            OnPropertyChanged(nameof(SelectedDovolenka));
        }
    }

    private bool _canEdit = false;
    public bool CannEdit
    {
        get => _canEdit;
        set
        {
            _canEdit = value;
            OnPropertyChanged(nameof(CannEdit));
        }
    }

    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand AddCommand { get; }

    public PrehledDovolenkyViewModel()
    {
        _database = new Database();
        LoadDovolene();
        SaveCommand = new RelayCommand(SaveDovolenka);
        DeleteCommand = new RelayCommand(DeleteDovolenka);
        AddCommand = new RelayCommand(AddDovolenka);
        SetUserRolePermissions();
    }

    private void SetUserRolePermissions()
    {
        string userRole = ProfilUzivateleManager.CurrentUser?.Role;
        _canEdit = !(userRole == "Vojáci");
    }

    // Načítání dovolenek z databáze
    private void LoadDovolene()
    {
        Dovolene.Clear();
        try
        {
            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("SELECT * FROM PREHLED_DOVOLENKY", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var dovolenka = new PrehledDovolenky
                    {
                        IdDovolenka = reader.GetInt32(0),
                        DatumOd = reader.GetDateTime(1),
                        DatumDo = reader.GetDateTime(2),
                        Duvod = reader.GetString(3)
                    };
                    Dovolene.Add(dovolenka);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Chyba při načítání dovolenek: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        if (SelectedDovolenka == null && Dovolene.Count > 0)
        {
            SelectedDovolenka = Dovolene[0];
        }
    }

    // Uložení vybrané dovolenky
    public void SaveDovolenka()
    {
        if (SelectedDovolenka == null) return;
        try
        {
            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("edit_dovolenky", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add("p_id_dovolenka", OracleDbType.Int32).Value = SelectedDovolenka.IdDovolenka;
                command.Parameters.Add("p_datum_od", OracleDbType.Date).Value = SelectedDovolenka.DatumOd;
                command.Parameters.Add("p_datum_do", OracleDbType.Date).Value = SelectedDovolenka.DatumDo;
                command.Parameters.Add("p_duvod", OracleDbType.Varchar2).Value = SelectedDovolenka.Duvod;

                command.ExecuteNonQuery();
            }
            LoadDovolene();  // Reload after saving
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Chyba při ukládání dovolenky: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // Přidání nové dovolenky
    private void AddDovolenka()
    {
        if (SelectedDovolenka == null) return;
        try
        {
            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("edit_dovolenky", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add("p_id_dovolenka", OracleDbType.Int32).Value = DBNull.Value;
                command.Parameters.Add("p_datum_od", OracleDbType.Date).Value = SelectedDovolenka.DatumOd;
                command.Parameters.Add("p_datum_do", OracleDbType.Date).Value = SelectedDovolenka.DatumDo;
                command.Parameters.Add("p_duvod", OracleDbType.Varchar2).Value = SelectedDovolenka.Duvod;

                command.ExecuteNonQuery();
            }
            LoadDovolene();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Chyba při přidávání dovolenky: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // Smazání vybrané dovolenky
    public void DeleteDovolenka()
    {
        if (SelectedDovolenka == null)
        {
            return;
        }

        try
        {
            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("smazat_dovolenku", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add("p_id_dovolenka", OracleDbType.Int32).Value = SelectedDovolenka.IdDovolenka;
                command.ExecuteNonQuery();
            }

            LoadDovolene();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Chyba při mazání dovolenky: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
