using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;
using System;
using System.Windows.Controls;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
{
    /// <summary>
    /// Interakční logika pro PrehledSouboryView.xaml
    /// </summary>
    public partial class PrehledSouboryView : Page
    {
        public PrehledSouboryView()
        {
            InitializeComponent();
            DataContext = new PrehledSouboryViewModel();
        }
    }
}
