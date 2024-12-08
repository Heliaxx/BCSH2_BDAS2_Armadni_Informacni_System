using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;
using System.Windows.Controls;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
{
    public partial class ProfilUzivatele : Page
    {
        public ProfilUzivatele()
        {
            InitializeComponent();
            DataContext = new ProfilUzivateleViewModel();
        }
    }
}
