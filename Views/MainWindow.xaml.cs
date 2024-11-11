using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;
using System.Windows;

namespace BCSH2_BDAS2_Armadni_Informacni_System
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void VojakViewButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new VojakView();
            //LoginWindow loginWindow = new LoginWindow();
            //loginWindow.Show();
            //this.Close();
        }

        private void Page2Button_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Page2();
            //VojakView VojakView = new VojakView();
            //VojakView.Show();
            //this.Close();
        }
    }
}
