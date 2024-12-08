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
using BCSH2_BDAS2_Armadni_Informacni_System.Entities;
using System.Linq;

public class PrehledDovolenkyVojaciViewModel : INotifyPropertyChanged
{
    private readonly Database _database;
    private string _searchText;

    public ObservableCollection<PrehledDovolenkyVojaci> DovolenkyVojaci { get; set; } = new ObservableCollection<PrehledDovolenkyVojaci>();
    public ObservableCollection<PrehledDovolenkyVojaci> FilteredDovolenkyVojaci { get; set; } = new ObservableCollection<PrehledDovolenkyVojaci>();

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged(nameof(SearchText));
            ApplyFilter();
        }
    }

    // Konstruktor
    public PrehledDovolenkyVojaciViewModel()
    {
        _database = new Database();
        LoadDovolenkyVojaci();
    }

    private void ApplyFilter()
    {
        FilteredDovolenkyVojaci.Clear();

        foreach (var dovolenkaVojak in DovolenkyVojaci)
        {
            if (string.IsNullOrWhiteSpace(SearchText) ||
                dovolenkaVojak.Jmeno.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                dovolenkaVojak.Prijmeni.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                dovolenkaVojak.Duvod.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                FilteredDovolenkyVojaci.Add(dovolenkaVojak);
            }
        }
    }

    // Načítání dovolenek vojáků z databáze
    private void LoadDovolenkyVojaci()
    {
        DovolenkyVojaci.Clear();
        FilteredDovolenkyVojaci.Clear();
        using (var connection = _database.GetOpenConnection())
        {
            var command = new OracleCommand("SELECT * FROM PREHLED_DOVOLENKY_VOJACI", connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var dovolenka = new PrehledDovolenkyVojaci
                {
                    IdDovolenka = reader.GetInt32(0),
                    DatumOd = reader.GetDateTime(1),
                    DatumDo = reader.GetDateTime(2),
                    Duvod = reader.GetString(3),
                    IdVojak = reader.GetInt32(4),
                    Jmeno = reader.GetString(5),
                    Prijmeni = reader.GetString(6)
                };
                DovolenkyVojaci.Add(dovolenka);
                FilteredDovolenkyVojaci.Add(dovolenka);
            }
        }
    }

    // PropertyChanged event
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}