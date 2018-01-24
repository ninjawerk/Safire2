using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using MahApps.Metro;

namespace Upd
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			var currentAccent = ThemeManager.Accents.ElementAt(6);
			var currentTheme = ThemeManager.AppThemes.ElementAt(1);
			ThemeManager.ChangeAppStyle(this, currentAccent, currentTheme);
		}
	}
}
