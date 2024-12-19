using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;
using System;
using System.Windows.Controls;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
{
    /// <summary>
    /// Interakční logika pro PrehledSpecializaceVojaciView.xaml
    /// </summary>
    public partial class PrehledSpecializaceVojaciView : Page
    {
        public PrehledSpecializaceVojaciView()
        {
            InitializeComponent();
            DataContext = new PrehledSpecializaceVojaciViewModel();
        }
    }
}
