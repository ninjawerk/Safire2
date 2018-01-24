using System;
using Safire.Core;

namespace Safire.SettingsPages
{
    /// <summary>
    /// Interaction logic for AppearanceBackground.xaml
    /// </summary>
    public partial class Scrobbler : SkinnedPage
    {
        public Scrobbler()
        {
            InitializeComponent();      
            SupportSkinner.SetSkin(this);
            InvalidateVisual();
            LastFm.MySessionChanged += LastFm_MySessionChanged;
            usr.Text = "Username: " + LastFm.GetUserName();
            rname.Text = "Real name: " + LastFm.GetRealName();
 
        }

        void LastFm_MySessionChanged()
        {
	        Dispatcher.Invoke(new Action(() =>
	        {
				usr.Text = "Username: " + LastFm.GetUserName();
				rname.Text = "Real name: " + LastFm.GetRealName();
	        }));
          
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LastFm.SetRegistrySetting("ScrobblerSessionKey", "");
            LastFm.Initialize();
        }
         
    }
}
