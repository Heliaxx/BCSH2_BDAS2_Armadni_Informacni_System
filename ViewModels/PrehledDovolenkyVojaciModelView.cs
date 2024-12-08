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
    private ObservableCollection<Vojak> _vojaci;
    private ObservableCollection<DuvodDovolenky> _duvodyDovolenky;
    private string _searchText;

    public DuvodDovolenky SelectedDovolenka
    {
        get => _selectedDovolenka;
        set
        {
            if (_selectedDovolenka != value)
            {
                _selectedDovolenka = value;
                OnPropertyChanged(nameof(_selectedDovolenka));

                // Když se změní vybraný záznam v DataGrid, nastavíme vybraný důvod
                SelectedDovolenkaDuvod = DuvodyDovolenky
                    .FirstOrDefault(d => d.Duvod == _selectedDovolenka?.Duvod && d.DatumOd == _selectedDovolenka?.DatumOd);
            }
        }
    }

    public ObservableCollection<DuvodDovolenky> DuvodyDovolenky
    {
        get
        {
            // Zajistíme, že kolekce je inicializována, pokud není
            if (_duvodyDovolenky == null)
            {
                _duvodyDovolenky = new ObservableCollection<DuvodDovolenky>();
            }
            return _duvodyDovolenky;
        }
        set
        {
            _duvodyDovolenky = value;
        }
    }

    private DuvodDovolenky _selectedDuvodDovolenky;
    private DuvodDovolenky _selectedDovolenka;

    // Vybraný důvod dovolenky pro ComboBox
    public DuvodDovolenky SelectedDuvodDovolenky
    {
        get => _selectedDuvodDovolenky;
        set
        {
            if (_selectedDuvodDovolenky != value)
            {
                _selectedDuvodDovolenky = value;
                OnPropertyChanged(nameof(SelectedDuvodDovolenky)); // Opravené volání OnPropertyChanged

                // Když se změní vybraný důvod, nastavíme příslušné hodnoty ve vybraném záznamu
                if (_selectedDovolenka != null)
                {
                    _selectedDovolenka.Duvod = _selectedDuvodDovolenky?.Duvod;
                    _selectedDovolenka.DatumOd = _selectedDuvodDovolenky?.DatumOd ?? DateTime.MinValue;
                }
            }
        }
    }

    private Vojak _selectedVojak;
    private DuvodDovolenky _selectedDovolenkaDuvod;

    // Změna na public pro binding
    public ObservableCollection<PrehledDovolenkyVojaci> DovolenkyVojaci { get; set; } = new ObservableCollection<PrehledDovolenkyVojaci>();
    public ObservableCollection<PrehledDovolenkyVojaci> FilteredDovolenkyVojaci { get; set; } = new ObservableCollection<PrehledDovolenkyVojaci>();

    public ObservableCollection<Vojak> Vojaci
    {
        get { return _vojaci; }
        set
        {
            _vojaci = value;
            OnPropertyChanged(nameof(Vojaci));
        }
    }

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

    public Vojak SelectedVojak
    {
        get => _selectedVojak;
        set
        {
            _selectedVojak = value;
            OnPropertyChanged(nameof(SelectedVojak));
        }
    }

    public DuvodDovolenky SelectedDovolenkaDuvod
    {
        get => _selectedDovolenkaDuvod;
        set
        {
            _selectedDovolenkaDuvod = value;
            OnPropertyChanged(nameof(SelectedDovolenkaDuvod));
        }
    }

    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }

    // Konstruktor
    public PrehledDovolenkyVojaciViewModel()
    {
        _database = new Database();
        LoadDovolenkyVojaci();
        LoadVojaci();
        LoadDuvodyDovolenky();
        SaveCommand = new RelayCommand(SaveDovolenka);
        DeleteCommand = new RelayCommand(DeleteDovolenka);
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

    // Načítání vojáků
    private void LoadVojaci()
    {
        // Zde přidejte kód pro načítání vojáků z databáze, pokud je to potřeba
    }

    // Načítání důvodů dovolenek
    private void LoadDuvodyDovolenky()
    {
        DuvodyDovolenky.Clear(); // Vyprázdníme kolekci

        using (var connection = _database.GetOpenConnection())
        {
            // Upravený SQL dotaz, který nyní vrací Duvod, DatumOd a DatumDo
            var command = new OracleCommand("SELECT DISTINCT DUVOD, DATUM_OD, DATUM_DO FROM PREHLED_DOVOLENKY_VOJACI", connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var dovolenkaDuvod = new DuvodDovolenky
                {
                    Duvod = reader.GetString(0),
                    DatumOd = reader.GetDateTime(1),
                    DatumDo = reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2) // Kontrola, jestli je DATUM_DO null
                };

                DuvodyDovolenky.Add(dovolenkaDuvod);
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

    // Uložení vybrané dovolenky
    public void SaveDovolenka()
    {
        
    }

    // Smazání vybrané dovolenky
    public void DeleteDovolenka()
    {
        
    }

    // PropertyChanged event
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
