using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Kornea.Audio;
using Kornea.Audio.Reactor;
using Kornea.Windows;
using MahApps.Metro;
using Safire.Core;
using Safire.Library.Core;
using Safire.Library.Cycling;
using Safire.Library.TableModels;
using Safire.Properties;
using Safire.SettingsPages;
using Safire.Wizards;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace Safire
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
	public partial class App : Application, ISingleInstanceApp
    {

	    public static SettingsWindow SettingsWindow = null;
        protected override void OnStartup(StartupEventArgs e)
        {
           
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline),
new FrameworkPropertyMetadata { DefaultValue = Settings.Default.DFPS });
            Tables.Initialize();

			 
        }




		private const string Unique = "SafireLeyonII";
        [STAThread]
        public static void Main2()
		{
			if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
			{
				var application = new App();
				application.InitializeComponent();
				 
				var currentAccent = ThemeManager.Accents.ElementAt(Settings.Default.AccentIndex);
				var currentTheme = ThemeManager.AppThemes.ElementAt(Settings.Default.BaseIndex);
				ThemeManager.ChangeAppStyle(application, currentAccent, currentTheme);
				application.Run();
				// Allow single instance code to perform cleanup operations
				SingleInstance<App>.Cleanup();
			}
		}
		#region ISingleInstanceApp Members
		public bool SignalExternalCommandLineArgs(IList<string> args)
		{
			if (args.Count > 1)
			{
				var cm1 = args[1];
				if (!string.IsNullOrEmpty(cm1))
				{
					var aw = new AudioWave(cm1, OutputMode.DirectSound, Player.Instance.Volume);
					aw.ReactorUsageLocked = true;
					aw.Play();
					aw.ReactorUsageLocked = false;
					var fader = new PanFade(aw, Player.Instance.Wave, 10, 2000, true, Player.Instance.Volume);
					fader.StartAndKill();

					Player.Instance.NewMedia(ref aw);
				}
			}
			return true;
		}
		#endregion


        protected override void OnActivated(EventArgs e)
        {


        }
		 
    }
}
