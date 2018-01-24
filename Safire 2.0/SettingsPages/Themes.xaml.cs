using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro;
using Safire.Core;
using Safire.Properties;

namespace Safire.SettingsPages
{
    /// <summary>
    /// Interaction logic for Themes.xaml
    /// </summary>
    public partial class Themes : SkinnedPage
    {


        private void accentselected(object sender, RoutedEventArgs e)
        {

            for (int i = 0; i < accentColors.Children.Count; i++)
            {
                if (accentColors.Children[i] == sender)
                {
					var currentAccent = ThemeManager.Accents.ElementAt(i);
					var currentTheme = ThemeManager.AppThemes.ElementAt(Settings.Default.BaseIndex);
                   
                    var mw = Application.Current.MainWindow as MainWindow;
					if (mw != null) ThemeManager.ChangeAppStyle(mw, currentAccent, currentTheme);
					SupportSkinner.TriggerSkinChanges(i, Settings.Default.BaseIndex);
                    break;
                }
            }
        }

        private void baseselected(object sender, System.Windows.RoutedEventArgs e)
        {
            for (int i = 0; i < accentColors.Children.Count; i++)
            {
                if (baseColors.Children[i] == sender)
                {
					var currentAccent = ThemeManager.Accents.ElementAt(Settings.Default.AccentIndex);
					var currentTheme = ThemeManager.AppThemes.ElementAt(i); 
					var mw = Application.Current.MainWindow as MainWindow;
					if (mw != null) ThemeManager.ChangeAppStyle(mw, currentAccent, currentTheme);
					SupportSkinner.TriggerSkinChanges(Settings.Default.BaseIndex,i  );
                    break;
                }
            }
        }
    }
}
