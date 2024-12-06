using System.Windows;
using System.Windows.Controls;
using BCSH2_BDAS2_Armadni_Informacni_System.Models;
using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
{
    public partial class PrehledJednotkyView : Page
    {
        public PrehledJednotkyView()
        {
            InitializeComponent();
            DataContext = new PrehledJednotkyViewModel();
        }
    }
}
