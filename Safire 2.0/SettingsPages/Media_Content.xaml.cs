using Safire.Core;

namespace Safire.SettingsPages
{
    /// <summary>
    /// Interaction logic for AppearanceBackground.xaml
    /// </summary>
    public partial class Media_Content : SkinnedPage
    {
        public Media_Content()
        {
            InitializeComponent();      
            SupportSkinner.SetSkin(this);
            InvalidateVisual();
             
 
        }

 

  
         
    }
}
