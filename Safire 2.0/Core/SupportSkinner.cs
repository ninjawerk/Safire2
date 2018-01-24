using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro;
using Safire.Properties;
using Safire.SettingsPages;

namespace Safire.Core
{
	static class SupportSkinner
	{


		public delegate void SkinChanged();

		public static event SkinChanged OnSkinChanged;

		public static void TriggerSkinChanges(int accind, int t)
		{
			Settings.Default.AccentIndex = accind;
			Settings.Default.BaseIndex = t; 
			var currentAccent = ThemeManager.Accents.ElementAt(Settings.Default.AccentIndex);
			var currentTheme = ThemeManager.AppThemes.ElementAt(Settings.Default.BaseIndex);
			 ThemeManager.ChangeAppStyle(App.Current, currentAccent, currentTheme);
			if (OnSkinChanged != null)
			{
				SolidColorBrush s = new SolidColorBrush();
				OnSkinChanged();
			}

		}

		public static void SetSkin(Window window)
		{
			var currentAccent = ThemeManager.Accents.ElementAt(Settings.Default.AccentIndex);
			var currentTheme = ThemeManager.AppThemes.ElementAt(Settings.Default.BaseIndex);
			if (window != null) ThemeManager.ChangeAppStyle(window, currentAccent, currentTheme);
			//window.Resources["TextB"] = new SolidColorBrush((Color)window.Resources["BlackColor"]);


		}
		public static void SetSkin(Page page)
		{
			var currentAccent = ThemeManager.Accents.ElementAt(Settings.Default.AccentIndex);
			var currentTheme = ThemeManager.AppThemes.ElementAt(Settings.Default.BaseIndex);
			if (page != null) ThemeManager.ChangeAppStyle(App.Current, currentAccent, currentTheme);
			//page.Resources["TextB"] = new SolidColorBrush((Color)page.Resources["BlackColor"]);
		}

	}
}
