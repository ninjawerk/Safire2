using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Windows;
using MahApps.Metro;
using MahApps.Metro.Controls;
using Safire.Animation;
using Safire.Controls.Window;
using Safire.Core;
using Safire.Library.Adder;
using Safire.Library.TableModels;
using Safire.Properties;
using Safire.SQLite;

namespace Safire.Wizards
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class Welcome : MetroWindow
	{
		private readonly string devid = "";
		private int wizIndex = 1;

		public Welcome()
		{
			SupportSkinner.SetSkin(this);
			InitializeComponent();
			ManagementObjectCollection mbsList = null;
			var mbs = new ManagementObjectSearcher("Select * From Win32_processor");
			mbsList = mbs.Get();

			foreach (ManagementObject mo in mbsList)
			{
				devid = mo["ProcessorID"].ToString();
			}

			//betaKey.Text = new string(betaKey.Text.Reverse().ToArray());
			nextControls.FadeIn();
			//devID.Text = devid;
			SupportSkinner.SetSkin(this);
			SupportSkinner.OnSkinChanged += SupportSkinner_OnSkinChanged;
		}

		private void SupportSkinner_OnSkinChanged()
		{
			SupportSkinner.SetSkin(this);
		}

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
				if (basecolors.Children[i] == sender)
				{
					var currentAccent = ThemeManager.Accents.ElementAt(Settings.Default.AccentIndex);
					var currentTheme = ThemeManager.AppThemes.ElementAt(i);
					var mw = Application.Current.MainWindow as MainWindow;
					if (mw != null) ThemeManager.ChangeAppStyle(mw, currentAccent, currentTheme);
					SupportSkinner.TriggerSkinChanges(Settings.Default.BaseIndex, i);
					break;
				}
			}
		}

		private void nextClick(object sender, RoutedEventArgs e)
		{
			wizIndex++;
			Properties.Settings.Default.AuTe = true;
			switch (wizIndex)
			{
				case 1:
					w1.FadeIn();
					break;
				case 2:

					w1.FadeOut();
					w3.FadeIn();
					//w2.FadeIn();
					break;
			 
				case 3:
					w3.FadeOut();
					w4.FadeIn();
					break;
				case 4:
				   
			 					 			 
					
				
					using (var db = new SQLiteConnection(Tables.DBPath))
					{
						if (db.Table<Library.TableModels.Fx>().Count() ==0)
						{
							var fx = SerialData.BinaryDeSerializeObject<List<Library.TableModels.Fx>>(
								System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\resources\\fxSuite.bin");
							foreach (var v in fx)
							{
								try
								{
									db.Insert(v);

								}
								catch (Exception)
								{

								}
							}
						}
					}
						  						if (!Settings.Default.AuTe) Application.Current.Shutdown();
						Close();

					break;
			}
			txtIndex.Text = wizIndex.ToString() + "/3";
		}

		private string GetSerial()
		{
			int i = 0;
			int p = 0;
			string t = "";
			foreach (char v in devid)
			{
				i++;
				if (i % 2 == 0)
					if (Char.IsDigit(v))
					{
						int c = v;
						c -= 48;
						c = c / 4;
						c += p;
						c %= 9;
						c += 48;
						t += Convert.ToChar(c);
					}
					else
					{
						int c = v;
						c -= 65;
						c += p;
						c %= 25;
						c += 65;
						t += Convert.ToChar(c);
					}

				p += v;
			}
			foreach (char v in devid)
			{
				i++;

				if (i % 2 == 1)
					if (Char.IsDigit(v))
					{
						int c = v;
						c -= 48;
						c = c / 4;
						c += p;
						c %= 9;
						c += 48;
						t += Convert.ToChar(c);
					}
					else
					{
						int c = v;
						c -= 65;
						c += p;
						c %= 25;
						c += 65;
						t += Convert.ToChar(c);
					}
				p -= v;
			}
			return t;
		}
		LibraryAdder lba = new LibraryAdder();
		private void LibraryBrowse(object sender, RoutedEventArgs e)
		{
			CoreMain.LibraryAdder.FromFolder();
		}
	}
}