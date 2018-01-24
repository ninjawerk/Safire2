using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Safire.Controls.Window;
using Safire.Core;
using Safire.Core.Updater;
using Safire.GUIs;
using Safire.Library.Imaging;
using Settings = Safire.Properties.Settings;

namespace Safire.SettingsPages
{
    /// <summary>
    /// Interaction logic for AppearanceBackground.xaml
    /// </summary>
    public partial class Updates : SkinnedPage
    {
        public Updates()
        {
            InitializeComponent();
          
            SupportSkinner.SetSkin(this);
            InvalidateVisual();
			Definition cdef = Util.BinaryDeSerializeObject<Definition>(CoreMain.MyPath() + @"\myDef.bin");
	        if (cdef != null)
		        currentVersion.Text = "Current version details:\nVersion: \t" + cdef.Version + "\nReleased on:" +
		                              cdef.ReleasedDateTime.ToShortDateString();
        }

		private   void Button_Click(object sender, RoutedEventArgs e)
		{
			OwnerWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;

			var mySettings = new MetroDialogSettings()
			{
				AffirmativeButtonText = "Yes",
				NegativeButtonText = "Not now",
				FirstAuxiliaryButtonText = "Never",
				ColorScheme = MetroDialogColorScheme.Theme
			};

			var result = UserRequest.ShowDialogBox("New updates available! Do you want to update now?",
				"Yes", "Not now", "Never", "UPDATE");

			if (result != ConfirmResult.Auxiliary)

				switch (result)
				{
					case ConfirmResult.Negative:

						break;
					case ConfirmResult.Affirmative:
						const int ERROR_CANCELLED = 1223; //The operation was canceled by the user.

						ProcessStartInfo info = new ProcessStartInfo(CoreMain.MyPath() + @"\upd.exe");
						info.UseShellExecute = true;
						info.Verb = "runas";
						try
						{
							Process.Start(info);
							App.Current.Shutdown();
						}
						catch (Win32Exception ex)
						{
							if (ex.NativeErrorCode == ERROR_CANCELLED)
								OwnerWindow.ShowMessageAsync("Error", "Update was cancelled!");


						}
						break;
					case ConfirmResult.Auxiliary:
						Settings.Default.checkUpdates = false;
						break;

					default:
						throw new ArgumentOutOfRangeException();
				}
		}
       
    }
}
