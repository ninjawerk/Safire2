using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using Kornea.Audio;
using Lpfm.LastFmScrobbler;
using Lpfm.LastFmScrobbler.Api;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Safire.Controls.Window;
using Safire.Properties;

namespace Safire.Core
{
	class LastFm
	{
		private const string LpfmRegistryNameSpace = "HKEY_CURRENT_USER\\Software\\Safire";
		private const string ApiKey = "1d3afe8559c38d819987e7bf617222f5";
		private const string ApiSecret = "cc6d3e0fe66297196b75682690226cd4";
		private static QueuingScrobbler _scrobbler;
		public delegate void SessionChanged();

		public static event SessionChanged MySessionChanged;

		public static void RaiseMySessionChangedEvent()
		{
			// Your logic
			if (MySessionChanged != null)
			{
				MySessionChanged();
			}
		}
		static Scrobbler scrobbler = new Scrobbler(ApiKey, ApiSecret);
		/// <summary>
		/// Initialize scrobbler session for safire
		/// </summary>
		public static   void Initialize()
		{
			try
			{
			 
				if (Settings.Default.Scrobble && Core.Connectivity.CheckForInternetConnection())
				{
					string sessionKey = GetRegistrySetting(sessionKeyRegistryKeyName, null);

					if (string.IsNullOrEmpty(sessionKey))
					{
				 
						var mw = App.Current.MainWindow as MainWindow;
						mw.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;

						var mySettings = new MetroDialogSettings()
						{
							AffirmativeButtonText = "Done",
							NegativeButtonText = "Cancel",

							ColorScheme = MetroDialogColorScheme.Theme
						};
						ThreadPool.QueueUserWorkItem(state =>
						{
							// instantiate a new scrobbler
 							// get a url to authenticate this application
							string url = scrobbler.GetAuthorisationUri();
							// open the URL in the default browser
							Process.Start(url);
						});
						var result =  
							UserRequest.ShowDialogBox( 
								"To connect Safire to your last.fm account, follow the instructions given in the page that just opened up in your browser. Click \'Done\' only after you allow access.",
								 "Done","Cancel","Scrobbler");
						
						if (result == ConfirmResult.Affirmative)
						{
							ThreadPool.QueueUserWorkItem(_=>GetSessionKey());
						}
					}

				}
			}
			catch
			{

			}
			Player.Instance.PropertyChanged += Instance_PropertyChanged;
		}

		public static   void errormessage(bool success)
		{
			var mw = App.Current.MainWindow as MainWindow;
			mw.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;

			if (!success)
			 
					 UserRequest.ShowDialogBox(
						  "Could not connect with your account. :(","Ok", "Scrobbler"
						   );
			else
				UserRequest.ShowDialogBox(
			  "Connected to your last.fm account " + LastFm.GetUserName() + " :)", "Ok", "Scrobbler"
			   );
		}
		static void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ActiveStreamHandle")
			{
				ScrobbleNowPlaying();
			}
		}

		public static string GetUserName()
		{
			string user = GetRegistrySetting("ScrobblerUser", null);
			return user;
		}
		public static string GetRealName()
		{
			string name = GetRegistrySetting("ScrobblerRealName", null);
			return name;
		}
		const string sessionKeyRegistryKeyName = "ScrobblerSessionKey";
		private static   void GetSessionKey()
		{
			// try get the session key from the registry
			string sessionKey = GetRegistrySetting(sessionKeyRegistryKeyName, null);

			if (string.IsNullOrEmpty(sessionKey))
			{
				// instantiate a new scrobbler
				
				int tried = 0;
				bool error = false;
		 
					error = false;
					// Try get session key from Last.fm
					try
					{
						sessionKey = scrobbler.GetSession();
						var user = scrobbler.GetSessionUser();
						// successfully got a key. Save it to the registry for next time
						SetRegistrySetting(sessionKeyRegistryKeyName, sessionKey);
						SetRegistrySetting("ScrobblerUser", user);
						SetRegistrySetting("ScrobblerRealName", Services.User.GetRealName(user));
						RaiseMySessionChangedEvent();
					}
					catch (LastFmApiException exception)
					{
						error = true;
					}
					App.Current.Dispatcher.BeginInvoke(new Action(()=>errormessage(!error)));

				}
		 
			_scrobbler = new QueuingScrobbler(ApiKey, ApiSecret, sessionKey);

		}
		private delegate void ProcessScrobblesDelegate();

		private static void ProcessScrobbles()
		{
			// Processes the scrobbles and discards any responses. This could be improved with thread-safe
			//  logging and/or error handling

			var lst = _scrobbler.Process();
		}

		public static string GetRegistrySetting(string valueName, string defaultValue)
		{
			if (string.IsNullOrEmpty(valueName)) throw new ArgumentException("valueName cannot be empty or null", "valueName");
			valueName = valueName.Trim();

			object regValue = Registry.GetValue(LpfmRegistryNameSpace, valueName, defaultValue);

			if (regValue == null)
			{
				// Key does not exist
				return defaultValue;
			}
			else
			{
				return regValue.ToString();
			}
		}

		public static void SetRegistrySetting(string valueName, string value)
		{
			if (string.IsNullOrEmpty(valueName)) throw new ArgumentException("valueName cannot be empty or null", "valueName");
			valueName = valueName.Trim();

			Registry.SetValue(LpfmRegistryNameSpace, valueName, value);
		}
		public static Track CurrentTrack { get; set; }

		public static void ScrobbleTrack(Track tr)
		{
			if (!Settings.Default.Scrobble) return;
			Thread thr = new Thread(() =>
			{
				var doProcessScrobbles = new ProcessScrobblesDelegate(ProcessScrobbles);
				// Scrobble the track that just finished
				if (_scrobbler != null)
				{
					_scrobbler.Scrobble(tr);
					_scrobbler.Process();
				}
			});
			thr.Start();

		}
		public static void ScrobbleNowPlaying()
		{
			if (!Settings.Default.Scrobble) return;
			Thread thr = new Thread(() =>
			{
				var doProcessScrobbles = new ProcessScrobblesDelegate(ProcessScrobbles);
				CurrentTrack = new Track();
				CurrentTrack.WhenStartedPlaying = DateTime.Now;
				CurrentTrack.TrackName = CoreMain.CurrentTrack.Title;
				CurrentTrack.AlbumName = CoreMain.CurrentTrack.Album;
				CurrentTrack.ArtistName = CoreMain.CurrentTrack.Artist;
				CurrentTrack.TrackNumber = (int)CoreMain.CurrentTrack.TrackPosition;
				CurrentTrack.Duration = new TimeSpan(0, 0, 0, (int)CoreMain.CurrentTrack.Duration);
				// we are using the Queuing scrobbler here so that we don't block the form while the scrobble request is being sent
				// to the Last.fm web service. The request will be sent when the Process() method is invoked
				if (_scrobbler != null)
				{
					_scrobbler.NowPlaying(CurrentTrack);

					// Begin invoke with no callback fires and forgets the scrobbler process. Processing runs asynchronously while 
					//  the form thread continues
					_scrobbler.Process();
				}
			});
			thr.Start();
		}


	}
}
