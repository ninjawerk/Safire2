using System.Linq;
using System.Windows;
using System.Windows.Media;
using MahApps.Metro;

namespace Upd
{
    static class SupportSkinner
    {


		public delegate void SkinChanged();

		public static event SkinChanged OnSkinChanged;

		public static void TriggerSkinChanges(int accind, int t)
		{
 
			var currentAccent = ThemeManager.Accents.ElementAt(6);
			var currentTheme = ThemeManager.AppThemes.ElementAt(0);
			ThemeManager.ChangeAppStyle(App.Current, currentAccent, currentTheme);
			if (OnSkinChanged != null)
			{
				SolidColorBrush s = new SolidColorBrush();
				OnSkinChanged();
			}

		}

		public static void SetSkin(Window window)
		{
			var currentAccent = ThemeManager.Accents.ElementAt(6);
			var currentTheme = ThemeManager.AppThemes.ElementAt(0);
			if (window != null) ThemeManager.ChangeAppStyle(window, currentAccent, currentTheme);
			//window.Resources["TextB"] = new SolidColorBrush((Color)window.Resources["BlackColor"]);


		}
	 

    }
}
