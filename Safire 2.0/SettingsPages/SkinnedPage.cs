using System;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using Safire.Core;

namespace Safire.SettingsPages
{
    public abstract class SkinnedPage : Page
    {


        protected override void OnInitialized(EventArgs e)
        {
            SupportSkinner.SetSkin(this);
            base.OnInitialized(e);
            SupportSkinner.OnSkinChanged += SupportSkinner_OnSkinChanged;
        }

        void SupportSkinner_OnSkinChanged()
        {
            SupportSkinner.SetSkin(this);
        }

	    public MetroWindow OwnerWindow
	    {
		    get { return App.SettingsWindow; }
	    }
    }
}
