using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;
using System;
using System.Windows.Controls;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
{
    /// <summary>
    /// Interakční logika pro PrehledLogyView.xaml
    /// </summary>
    public partial class PrehledLogyView : Page
    {
        public PrehledLogyView()
        {
            InitializeComponent();
            DataContext = new PrehledLogyViewModel();
        }
    }
}
