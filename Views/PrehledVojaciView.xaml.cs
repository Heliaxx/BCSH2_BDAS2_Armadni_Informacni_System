using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;
using System;
using System.Windows.Controls;


namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
{
    /// <summary>
    /// Interakční logika pro PrehledVojaciView.xaml
    /// </summary>
    public partial class PrehledVojaciView : Page
    {
        public PrehledVojaciView()
        {
            InitializeComponent();
            this.DataContext = new PrehledVojaciViewModel();
        }
    }
}
