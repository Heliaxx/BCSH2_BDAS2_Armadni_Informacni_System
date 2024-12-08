using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
{
    /// <summary>
    /// Interakční logika pro PrehledDovolenkyVojaciView.xaml
    /// </summary>
    public partial class PrehledDovolenkyVojaciView : Page
    {
        public PrehledDovolenkyVojaciView()
        {
            InitializeComponent();
            DataContext = new PrehledDovolenkyVojaciViewModel();
        }
    }
}
