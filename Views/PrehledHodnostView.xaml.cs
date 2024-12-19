using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;
using System;
using System.Windows.Controls;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
{
    /// <summary>
    /// Interakční logika pro PrehledHodnostView.xaml
    /// </summary>
    public partial class PrehledHodnostView : Page
    {
        public PrehledHodnostView()
        {
            InitializeComponent();
            DataContext = new PrehledHodnostViewModel();
        }
    }
}
