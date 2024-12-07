using System.Windows;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
{
    public partial class PrehledNadrizenychWindow : Window
    {
        public PrehledNadrizenychWindow()
        {
            InitializeComponent();
            DataContext = new ViewModels.PrehledNadrizenychViewModel();
        }
    }
}
