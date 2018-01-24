using Safire.Core;

namespace Safire.SettingsPages
{
    /// <summary>
    /// Interaction logic for AppearanceBackground.xaml
    /// </summary>
	public partial class Library : SkinnedPage
    {
        public Library()
        {
            InitializeComponent();      
            SupportSkinner.SetSkin(this);
            InvalidateVisual();
      
 
        }

         

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
 
        }
         
    }
}
