using System.Windows;
using System.Windows.Controls;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;
using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
{
    public partial class PrehledJednotkyView : Page
    {
        private readonly PrehledJednotkyViewModel _viewModel;

        public PrehledJednotkyView()
        {
            InitializeComponent();
            _viewModel = new PrehledJednotkyViewModel();
            DataContext = _viewModel;
        }


        // Obslužná metoda pro výběr jednotky v DataGridu
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Získáme vybranou jednotku
            var selectedJednotka = (PrehledJednotky)((DataGrid)sender).SelectedItem;
            _viewModel.SelectedJednotka = selectedJednotka;
        }


        // Obslužná metoda pro tlačítko "Přidat"
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.SelectedItem = null;
        }

        // Obslužná metoda pro tlačítko "Uložit"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveJednotka();
        }

        // Obslužná metoda pro tlačítko "Smazat"
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DeleteJednotka();
        }
    }
}
