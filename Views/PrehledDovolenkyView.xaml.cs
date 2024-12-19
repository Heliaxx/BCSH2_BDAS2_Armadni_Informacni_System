using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;
using System;
using System.Windows.Controls;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
{
    /// <summary>
    /// Interakční logika pro PrehledDovolenkyView.xaml
    /// </summary>
    public partial class PrehledDovolenkyView : Page
    {
        public PrehledDovolenkyView()
        {
            InitializeComponent();
            DataContext = new PrehledDovolenkyViewModel();
        }
    }
}
