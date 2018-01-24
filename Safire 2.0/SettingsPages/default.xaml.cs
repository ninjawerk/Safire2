using Safire.Core;

namespace Safire.SettingsPages
{
    /// <summary>
    /// Interaction logic for AppearanceBackground.xaml
    /// </summary>
    public partial class Default : SkinnedPage
    {
		public Default()
        {
            InitializeComponent();      
            SupportSkinner.SetSkin(this);
            InvalidateVisual();
             
 
        }

 

  
         
    }
}
