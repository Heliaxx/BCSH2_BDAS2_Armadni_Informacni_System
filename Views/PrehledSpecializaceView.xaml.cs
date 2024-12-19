using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;
using System;
using System.Windows.Controls;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
{
    /// <summary>
    /// Interakční logika pro PrehledSpecializaceView.xaml
    /// </summary>
    public partial class PrehledSpecializaceView : Page
    {
        public PrehledSpecializaceView()
        {
            InitializeComponent();
            DataContext = new PrehledSpecializaceViewModel();
        }
    }
}
