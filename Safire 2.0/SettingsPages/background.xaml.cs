using System;
using System.IO;
using System.Windows.Controls;
using Safire.Core;
using Safire.Library.Imaging;

namespace Safire.SettingsPages
{
    /// <summary>
    /// Interaction logic for AppearanceBackground.xaml
    /// </summary>
    public partial class Background : SkinnedPage
    {
        public Background()
        {
            InitializeComponent();
            img.Source=Bitmap.GetImage("dbak.ee");
            SupportSkinner.SetSkin(this);
            InvalidateVisual();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "Image Files |*.jpeg;*.png;*.jpg;*.gif";


            Nullable<bool> result = dlg.ShowDialog();


            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
               File.Delete("dbak.ee");
                File.Copy(filename,"dbak.ee");
                img.Source = Bitmap.GetImage("dbak.ee");
                var mw = App.Current.MainWindow as MainWindow;
                mw.BackgroundImage.Source = Bitmap.GetImage("dbak.ee");
            }
        }
    }
}
