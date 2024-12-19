using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;
using System;
using System.Windows.Controls;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
{
    /// <summary>
    /// Interakční logika pro PrehledSkoleniUcastniciView.xaml
    /// </summary>
    public partial class PrehledSkoleniUcastniciView : Page
    {
        public PrehledSkoleniUcastniciView()
        {
            InitializeComponent();
            DataContext = new PrehledSkoleniUcastniciViewModel();
        }
    }
}
