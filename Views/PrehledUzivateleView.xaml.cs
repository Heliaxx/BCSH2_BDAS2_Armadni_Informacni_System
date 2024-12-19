using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;
using System;
using System.Windows.Controls;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
{
    /// <summary>
    /// Interakční logika pro PrehledUzivateleView.xaml
    /// </summary>
    public partial class PrehledUzivateleView : Page
    {
        public PrehledUzivateleView()
        {
            InitializeComponent();
            DataContext = new PrehledUzivateleViewModel(this);
        }
    }
}
