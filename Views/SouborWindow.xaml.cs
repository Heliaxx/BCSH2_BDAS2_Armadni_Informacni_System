using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
{
    public partial class SouborWindow : Window
    {
        public SouborWindow(byte[] fileContent, string fileType)
        {
            InitializeComponent();

            if (fileType == "obrázek")
            {
                // Zobrazení obrázku
                using (var memoryStream = new MemoryStream(fileContent))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = memoryStream;
                    bitmap.EndInit();
                    ImageView.Source = bitmap;
                }

                // Zobrazení obrázku a skrytí textu
                ImageView.Visibility = Visibility.Visible;
                TextView.Visibility = Visibility.Collapsed;
            }
            else if (fileType == "textový soubor")
            {
                // Zobrazení textového obsahu
                string textContent = System.Text.Encoding.UTF8.GetString(fileContent);
                TextView.Text = textContent;

                // Zobrazení textu a skrytí obrázku
                TextView.Visibility = Visibility.Visible;
                ImageView.Visibility = Visibility.Collapsed;
            }
        }
    }
}

//namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
//{
//    public partial class SouborWindow : Window
//    {
//        public SouborWindow()
//        {
//            InitializeComponent();
//            DataContext = new PrehledSouboryViewModel();
//        }
//    }
//}