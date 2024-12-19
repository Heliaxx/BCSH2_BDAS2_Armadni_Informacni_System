using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;
using System;
using System.Windows.Controls;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
{
    /// <summary>
    /// Interakční logika pro PrehledRoleView.xaml
    /// </summary>
    public partial class PrehledRoleView : Page
    {
        public PrehledRoleView()
        {
            InitializeComponent();
            DataContext = new PrehledRoleViewModel();
        }
    }
}
