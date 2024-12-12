using BCSH2_BDAS2_Armadni_Informacni_System.Entities;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;
using BCSH2_BDAS2_Armadni_Informacni_System;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PrehledSkoleniUcastniciViewModel : INotifyPropertyChanged
{
    private readonly Database _database;
    private string _searchText;


    // Změna na public pro binding
    public ObservableCollection<PrehledSkoleniUcastnici> SkoleniUcastnici { get; set; } = new ObservableCollection<PrehledSkoleniUcastnici>();
    public ObservableCollection<PrehledSkoleniUcastnici> FilteredSkoleniUcastnici { get; set; } = new ObservableCollection<PrehledSkoleniUcastnici>();

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
    public PrehledSkoleniUcastniciViewModel()
    {
        _database = new Database();
        LoadSkoleniUcastnici();
    }

    private void ApplyFilter()
    {
        FilteredSkoleniUcastnici.Clear();

        foreach (var skoleniUcastnik in SkoleniUcastnici)
        {
            if (string.IsNullOrWhiteSpace(SearchText) ||
                skoleniUcastnik.Skoleni.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                skoleniUcastnik.Misto.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                skoleniUcastnik.Jmeno.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                skoleniUcastnik.Prijmeni.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                FilteredSkoleniUcastnici.Add(skoleniUcastnik);
            }
        }
    }

    // Načítání dovolenek vojáků z databáze
    private void LoadSkoleniUcastnici()
    {
        SkoleniUcastnici.Clear();
        FilteredSkoleniUcastnici.Clear();

        try
        {
            using (var connection = _database.GetOpenConnection())
        {
            var command = new OracleCommand("SELECT * FROM PREHLED_SKOLENI_UCASTNICI", connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var skoleni = new PrehledSkoleniUcastnici
                {
                    IdSkoleni = reader.GetInt32(0),
                    Skoleni = reader.GetString(1),
                    DatumOd = reader.GetDateTime(2),
                    DatumDo = reader.GetDateTime(3),
                    Misto = reader.GetString(4),
                    IdVojak = reader.GetInt32(5),
                    Jmeno = reader.GetString(6),
                    Prijmeni = reader.GetString(7)
                };
                SkoleniUcastnici.Add(skoleni);
                FilteredSkoleniUcastnici.Add(skoleni);
            }
        }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    // PropertyChanged event
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}