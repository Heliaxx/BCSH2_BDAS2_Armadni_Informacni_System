using System.Windows;

namespace BCSH2_BDAS2_Armadni_Informacni_System
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Page1Button_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Page1();
            //LoginWindow loginWindow = new LoginWindow();
            //loginWindow.Show();
            //this.Close();
        }

        private void Page2Button_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Page2();
            //Page1 page1 = new Page1();
            //page1.Show();
            //this.Close();
        }
    }
}
