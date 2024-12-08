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

public class PrehledSpecializaceVojaciViewModel : INotifyPropertyChanged
{
    private readonly Database _database;
    private string _searchText;


    // Změna na public pro binding
    public ObservableCollection<PrehledSpecializaceVojak> SpecializaceVojaci { get; set; } = new ObservableCollection<PrehledSpecializaceVojak>();
    public ObservableCollection<PrehledSpecializaceVojak> FilteredSpecializaceVojaci { get; set; } = new ObservableCollection<PrehledSpecializaceVojak>();

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
    public PrehledSpecializaceVojaciViewModel()
    {
        _database = new Database();
        LoadSpecializaceVojaci();
    }

    private void ApplyFilter()
    {
        FilteredSpecializaceVojaci.Clear();

        foreach (var specializaceVojak in SpecializaceVojaci)
        {
            if (string.IsNullOrWhiteSpace(SearchText) ||
                specializaceVojak.Nazev.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                specializaceVojak.Popis.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                specializaceVojak.Jmeno.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                specializaceVojak.Prijmeni.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                FilteredSpecializaceVojaci.Add(specializaceVojak);
            }
        }
    }

    // Načítání dovolenek vojáků z databáze
    private void LoadSpecializaceVojaci()
    {
        SpecializaceVojaci.Clear();
        FilteredSpecializaceVojaci.Clear();
        try
        {
            using (var connection = _database.GetOpenConnection())
        {
            var command = new OracleCommand("SELECT * FROM PREHLED_SPECIALIZACE_VOJAK", connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var specializace = new PrehledSpecializaceVojak
                {
                    IdSpecializace = reader.GetInt32(0),
                    Nazev = reader.GetString(1),
                    StupenNarocnosti = reader.GetInt32(2),
                    Popis = reader.GetString(3),
                    IdVojak = reader.GetInt32(4),
                    Jmeno = reader.GetString(5),
                    Prijmeni = reader.GetString(6)
                };
                SpecializaceVojaci.Add(specializace);
                FilteredSpecializaceVojaci.Add(specializace);
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
