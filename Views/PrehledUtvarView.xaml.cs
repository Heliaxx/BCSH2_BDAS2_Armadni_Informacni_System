using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;
using System;
using System.Windows.Controls;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
{
    /// <summary>
    /// Interakční logika pro PrehledUtvarView.xaml
    /// </summary>
    public partial class PrehledUtvarView : Page
    {
        public PrehledUtvarView()
        {
            InitializeComponent();
            DataContext = new PrehledUtvarViewModel();
        }
    }
}
