using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;
using System;
using System.Windows.Controls;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
{
    /// <summary>
    /// Interakční logika pro PrehledSkoleniView.xaml
    /// </summary>
    public partial class PrehledSkoleniView : Page
    {
        public PrehledSkoleniView()
        {
            InitializeComponent();
            DataContext = new PrehledSkoleniViewModel();
        }
    }
}
