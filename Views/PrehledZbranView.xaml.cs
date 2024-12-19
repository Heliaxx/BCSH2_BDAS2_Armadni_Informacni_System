using System;
using System.Windows.Controls;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
{
    /// <summary>
    /// Interakční logika pro PrehledZbranView.xaml
    /// </summary>
    public partial class PrehledZbranView : Page
    {
        public PrehledZbranView()
        {
            InitializeComponent();
            DataContext = new ViewModels.PrehledZbranViewModel();
        }
    }
}
