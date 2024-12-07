using System;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Win32;
using System.Windows.Input;
using Oracle.ManagedDataAccess.Client;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;
using BCSH2_BDAS2_Armadni_Informacni_System.Helpers;
using System.ComponentModel;
using BCSH2_BDAS2_Armadni_Informacni_System.Views;
using System.Windows;

namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
{
    internal class PrehledSouboryViewModel : INotifyPropertyChanged
    {
        private readonly Database _database;

        // ObservableCollection pro propojení s View
        public ObservableCollection<PrehledSoubory> Soubory { get; set; } = new ObservableCollection<PrehledSoubory>();

        private PrehledSoubory _selectedSoubor;

        public event PropertyChangedEventHandler PropertyChanged;

        public PrehledSoubory SelectedSoubor
        {
            get => _selectedSoubor;
            set
            {
                _selectedSoubor = value;
                OnPropertyChanged(nameof(SelectedSoubor));
            }
        }

        private bool _canAddOrDelete = false;
        public bool CanAddOrDelete
        {
            get => _canAddOrDelete;
            set
            {
                _canAddOrDelete = value;
                OnPropertyChanged(nameof(CanAddOrDelete));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Příkazy pro tlačítka
        public ICommand AddCommand { get; }
        public ICommand ShowCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand DownloadCommand { get; }

        public PrehledSouboryViewModel()
        {
            _database = new Database();

            // Inicializace příkazů
            AddCommand = new RelayCommand(AddSoubor);
            ShowCommand = new RelayCommand(ShowSoubor);
            DeleteCommand = new RelayCommand(DeleteSoubor);
            DownloadCommand = new RelayCommand(DownloadSoubor);
            SetUserRolePermissions();
            NačístSoubory();
        }

        private void SetUserRolePermissions()
        {
            string userRole = ProfilUzivateleManager.CurrentUser?.Role;
            _canAddOrDelete = !(userRole == "Vojáci");
        }

        // Načtení souborů z databáze
        public void NačístSoubory()
        {
            Soubory.Clear();
            using (var connection = _database.GetOpenConnection())
            {
                var command = new OracleCommand("SELECT * FROM PREHLED_SOUBORY", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Soubory.Add(new PrehledSoubory
                    {
                        IdSoubor = reader.GetInt32(0),
                        NazevSouboru = reader.GetString(1),
                        TypSouboru = reader.GetString(2),
                        PriponaSouboru = reader.GetString(3),
                        DatumNahrani = reader.GetDateTime(4)
                    });
                }
            }
        }

        private void AddSoubor()
        {
            // Otevření dialogu pro výběr souboru
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|Text Files (*.txt)|*.txt",
                Title = "Vyberte soubor"
            };

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                // Načtení souboru jako byte array
                byte[] fileData = File.ReadAllBytes(openFileDialog.FileName);

                // Získání potřebných informací o souboru
                string fileName = Path.GetFileName(openFileDialog.FileName);
                string fileExtension = Path.GetExtension(openFileDialog.FileName).TrimStart('.');
                string fileType = GetFileType(fileExtension); // Určení typu souboru na základě přípony
                DateTime uploadDate = DateTime.Now;

                // Uložení souboru do databáze
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("INSERT INTO SOUBORY (NAZEV_SOUBORU, TYP_SOUBORU, PRIPONA_SOUBORU, OBSAH_SOUBORU, DATUM_NAHRANI) VALUES (:nazev, :typ, :pripona, :obsah, :datum)", connection);
                    command.Parameters.Add(new OracleParameter(":nazev", fileName));
                    command.Parameters.Add(new OracleParameter(":typ", fileType));
                    command.Parameters.Add(new OracleParameter(":pripona", fileExtension));
                    command.Parameters.Add(new OracleParameter(":obsah", fileData));
                    command.Parameters.Add(new OracleParameter(":datum", uploadDate));

                    command.ExecuteNonQuery();
                }

                // Po přidání souboru, znovu načteme seznam souborů
                NačístSoubory();
            }
        }

        // Metoda pro určení typu souboru
        private string GetFileType(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case "jpg":
                case "png":
                    return "Obrázek";
                case "txt":
                    return "Textový soubor";
                default:
                    return "Neznámý";
            }
        }

        // Smazání souboru
        private void DeleteSoubor()
        {
            if (SelectedSoubor != null)
            {
                using (var connection = _database.GetOpenConnection())
                {
                    var command = new OracleCommand("DELETE FROM SOUBORY WHERE ID_SOUBOR = :id", connection);
                    command.Parameters.Add(new OracleParameter(":id", SelectedSoubor.IdSoubor));
                    command.ExecuteNonQuery();
                }

                NačístSoubory();
            }
        }

        // Zobrazení souboru
        private void ShowSoubor()
        {
            if (SelectedSoubor != null)
            {
                // Získání souboru z databáze podle ID
                byte[] fileContent = ZobrazitSoubor(SelectedSoubor.IdSoubor);

                if (fileContent != null)
                {
                    string fileType = SelectedSoubor.TypSouboru.ToLower();

                    // Otevření nového okna a předání dat
                    var souborWindow = new SouborWindow(fileContent, fileType);
                    souborWindow.Show();
                }
            }
        }

        private byte[] ZobrazitSoubor(int souborId)
        {
            byte[] fileContent = null;

            using (var connection = _database.GetOpenConnection())
            {
                // SQL dotaz pro získání souboru podle jeho ID
                var command = new OracleCommand("SELECT OBSAH_SOUBORU FROM SOUBORY WHERE ID_SOUBOR = :id", connection);
                command.Parameters.Add(new OracleParameter(":id", souborId));

                // Spuštění dotazu a načtení výsledků
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // Pokud existuje soubor, načteme jeho obsah
                    fileContent = reader.IsDBNull(0) ? null : (byte[])reader[0];
                }
            }

            return fileContent;
        }

        private void DownloadSoubor()
        {
            if (SelectedSoubor != null)
            {
                // Získání obsahu souboru z databáze
                byte[] fileContent = ZobrazitSoubor(SelectedSoubor.IdSoubor);

                if (fileContent != null)
                {
                    // Dialog pro výběr umístění a názvu souboru
                    var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                    {
                        FileName = SelectedSoubor.NazevSouboru,
                        DefaultExt = SelectedSoubor.PriponaSouboru,
                        Filter = $"{SelectedSoubor.TypSouboru} Files (*{SelectedSoubor.PriponaSouboru})|*{SelectedSoubor.PriponaSouboru}|All Files (*.*)|*.*"
                    };

                    bool? result = saveFileDialog.ShowDialog();

                    if (result == true)
                    {
                        // Uložení souboru na zvolené místo
                        File.WriteAllBytes(saveFileDialog.FileName, fileContent);

                        MessageBox.Show("Soubor byl úspěšně uložen!", "Stažení souboru", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Obsah souboru nelze načíst.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Nevybrali jste žádný soubor.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
