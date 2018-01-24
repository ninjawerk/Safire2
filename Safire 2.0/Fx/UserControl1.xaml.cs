using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Kornea.Audio;
using Kornea.Audio.AudioCore;
using Safire.Animation;
using Safire.Controls;
using Safire.Core;
using Safire.Fx.FxModels;
using Safire.Library.TableModels;
using Safire.Properties;
using Safire.SQLite;

namespace Safire.Fx
{
	/// <summary>
	///     Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class UserControl1 : UserControl
	{
		public bool ComponentInitialized = false;
		public UserControl1()
		{
			FxHolder.Initialize();
			InitializeComponent();
			if (togFx.IsChecked == true)
			{
				ovr.FadeOut();
				fxpan.FadeIn();
				fxbutt.FadeIn();
			}
		}

		public void Initialize()
		{
			if (ComponentInitialized) return;

			if (AccessPermission.LymAudioLoaded)
			{
				Player.Instance.PropertyChanged += Instance_PropertyChanged;
			}
			int i = 0;
			foreach (Slider e in EqStack.Children)
			{
				e.Tag = i;
				e.ValueChanged += e_ValueChanged;
				if (FxHolder.EqValues.Count > i) e.Value = FxHolder.EqValues[i++];
			}
			ComponentInitialized = true;

			foreach (LiveTile l in Grid.Children)
			{
				l.RefreshToggle();
			}

			RefreshCombos();

		}
		private void e_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!Settings.Default.Equalizer) return;
			var send = sender as Slider;
			int eq = send.Tag is int ? (int)send.Tag : 0;
			FxHolder.UpdateFX(eq, (float)send.Value);

			FxHolder.EqValues[eq] = (float)send.Value;
		}

		private void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
		}

		private void togBassBoost_Checked(object sender, RoutedEventArgs e)
		{
			if (FxHolder.BassBooster != null) FxHolder.BassBooster.SetBypass(!Settings.Default.afx | false);
		}

		private void togBassBoost_Unchecked(object sender, RoutedEventArgs e)
		{
			if (FxHolder.BassBooster != null) FxHolder.BassBooster.SetBypass(true);
		}

		private void sldBassBoost_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (FxHolder.BassBooster != null)
			{
				FxHolder.BassBooster.CutOff = Settings.Default.BassRatio;

				FxHolder.BassBooster.Bandwidth = 0.300f;
				FxHolder.BassBooster.d_vol = 0.4f;
				FxHolder.BassBooster.p_vol = 1.5f - (Settings.Default.BassRatio / 60.0f);
				FxHolder.BassBooster.HighCutoff = Settings.Default.BassRatio/1.5f;
				FxHolder.BassBooster.LowCutoff1 = Settings.Default.BassRatio / 1.5f;
				FxHolder.BassBooster.LowCutoff2 = Settings.Default.BassRatio * 1;
			}
		}


		private void togStereo_Unchecked(object sender, RoutedEventArgs e)
		{
			if (FxHolder.StereoEnhancer != null) FxHolder.StereoEnhancer.SetBypass(true);
		}

		private void togStereo_Checked(object sender, RoutedEventArgs e)
		{
			if (FxHolder.StereoEnhancer != null) FxHolder.StereoEnhancer.SetBypass(!Settings.Default.afx | false);
		}

		private void sldStereoWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (FxHolder.StereoEnhancer != null) FxHolder.StereoEnhancer.Width = (float)(sldStereoWidth.Value / 50);
		}

		private void sldBassRange_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (FxHolder.BassBooster != null)
				FxHolder.BassBooster.Bandwidth = Settings.Default.BassSelectivity / 1000f;
		}

		private void sldBassDepth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			//if (FxHolder.BassBooster != null) FxHolder.BassBooster.Cycles = (int)sldBassDepth.Value;
		}

		private void togEqualizer_Unchecked(object sender, RoutedEventArgs e)
		{
			if (FxHolder.eql != null) FxHolder.eql.SetBypass(true);
			int i = 0;
			foreach (float visualYSnappingGuideline in FxHolder.EqValues)
			{
				FxHolder.UpdateFX(i++, 0);
			}
			Settings.Default.Equalizer = false;
		}

		private void togEqualizer_Checked(object sender, RoutedEventArgs e)
		{
			if (FxHolder.eql != null) FxHolder.eql.SetBypass(!Settings.Default.afx | false);
			Settings.Default.Equalizer = true;
			FxHolder.SetBFX_EQ(Player.Instance.Handle);

		}

		private void sldbooster_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (FxHolder.Booster != null)
			{
				FxHolder.Booster.Eq.lg = Settings.Default.lg * 2;
				FxHolder.Booster.Eq.mg = Settings.Default.mg;
				FxHolder.Booster.Eq.hg = Settings.Default.hg;
			}
		}

		private void togBooster_Unchecked(object sender, RoutedEventArgs e)
		{
			if (FxHolder.Booster != null) FxHolder.Booster.SetBypass(true);
		}

		private void togBooster_Checked(object sender, RoutedEventArgs e)
		{
			if (FxHolder.Booster != null) FxHolder.Booster.SetBypass(!Settings.Default.afx | false);

		}

		private void writeBooster(object sender, RoutedEventArgs e)
		{
			var f = new Safire.Library.TableModels.Fx();
			f.Author = "Deshan Alahakoon";
			f.Type = "bstr";
			f.ID = Guid.NewGuid().ToString();
			var b = new Booster();
			b.lg = Settings.Default.lg;
			b.mg = Settings.Default.mg;
			b.hg = Settings.Default.hg;
			f.Data = Util.Serialize<Booster>(b);
			f.Name = cmbBooster.Text;
			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				db.Insert(f);
			}
			RefreshCombos();

		}

		void RefreshCombos()
		{
			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				cmbBooster.ItemsSource = db.Table<Library.TableModels.Fx>().Where(c => c.Type == "bstr").OrderBy(c=>c.Name);

				cmbEq.ItemsSource = db.Table<Library.TableModels.Fx>().Where(c => c.Type == "eq").OrderBy(c => c.Name);

				cmbSuite.ItemsSource = db.Table<Library.TableModels.Fx>().Where(c => c.Type == "suite").OrderBy(c => c.Name);
			}

		}

		private void cmbBooster_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cmbBooster.SelectedValue != null)
			{
				var b = cmbBooster.SelectedValue as Library.TableModels.Fx;
				var f = Util.Deserialize<Booster>(b.Data);
				Settings.Default.lg = f.lg;
				Settings.Default.mg = f.mg;
				Settings.Default.hg = f.hg;
			}
		}

		private void cmbEq_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cmbEq.SelectedValue != null)
			{
				var b = cmbEq.SelectedValue as Library.TableModels.Fx;
				var f = Util.Deserialize<List<float>>(b.Data);
				int i = 0;
				foreach (Slider es in EqStack.Children)
				{

					if (FxHolder.EqValues.Count > i) es.Value = f[i++];
				}
				FxHolder.EqValues = f;
			}
		}

		private void writeEq(object sender, RoutedEventArgs e)
		{

			var f = new Safire.Library.TableModels.Fx();
			f.Author = "Deshan Alahakoon";
			f.Type = "eq";
			f.ID = Guid.NewGuid().ToString();
			var b = new List<float>();
			foreach (Slider es in EqStack.Children)
			{

				b.Add((float)es.Value);
			}
			f.Data = Util.Serialize<List<float>>(b);
			f.Name = cmbEq.Text;
			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				db.Insert(f);
			}
			RefreshCombos();
		}

		private void EqReset(object sender, RoutedEventArgs e)
		{
			int i = 0;
			foreach (Slider es in EqStack.Children)
			{
				FxHolder.EqValues[i++] = 0.0f;
				es.Value = 0;
			}
			//	using (var db = new SQLiteConnection(Tables.DBPath))
			//	{
			//var f=		db.Table<Library.TableModels.Fx>().ToList();
			//SerialData.AbsBinarySerializeObject<List<Library.TableModels.Fx>>(@"E:\TurtleTheCat-leyon-6d17864ccb72\Safire 2.0\bin\Debug\fxSuite.bin", f);
			//	}

		}

		private void togFx_Checked(object sender, RoutedEventArgs e)
		{
			ovr.FadeOut();
			fxpan.FadeIn();
			fxbutt.FadeIn();
			FxHolder.ByPassAll(false);
			(App.Current.MainWindow as MainWindow).ShowFxToggle();
		}

		private void togFx_Unchecked(object sender, RoutedEventArgs e)
		{
			ovr.FadeIn();
			fxpan.FadeOut();
			fxbutt.FadeOut();
			FxHolder.ByPassAll(true);
			(App.Current.MainWindow as MainWindow).ShowFxToggle();
		}

		private void suiteWrite(object sender, RoutedEventArgs e)
		{
			var suite = new Suite();
			var f = new Safire.Library.TableModels.Fx();
			f.Author = "Deshan Alahakoon";
			f.Type = "suite";
			f.ID = Guid.NewGuid().ToString();
			//eq
			var eq = new List<float>();
			foreach (Slider es in EqStack.Children)
			{

				eq.Add((float)es.Value);
			}
			suite.EqValuesList = eq;
			//booster
			var bs = new Booster();
			bs.lg = Settings.Default.lg;
			bs.mg = Settings.Default.mg;
			bs.hg = Settings.Default.hg;
			suite.Booster = bs;
			//stereo
			suite.StereoEn = Settings.Default.StereoWidth;
			//bassen
			suite.BassEn = Settings.Default.BassRatio;

			suite.BassEnStatus = Settings.Default.BassBoost;
			suite.EqStatus = Settings.Default.Equalizer;
			suite.StereoEnStatus = Settings.Default.StereoWiden;
			suite.BoosterStatus = Settings.Default.boosters;

			f.Data = Util.Serialize<Suite>(suite);
			f.Name = cmbSuite.Text;
			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				db.Insert(f);
			}
			RefreshCombos();
		}

		private void cmbSuite_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cmbSuite.SelectedValue != null)
			{
				var b = cmbSuite.SelectedValue as Library.TableModels.Fx;
				var f = Util.Deserialize<Suite>(b.Data);
				int i = 0;


				if (f.EqStatus)
				{
					FxHolder.EqValues = f.EqValuesList;
					foreach (Slider es in EqStack.Children)
					{
						if (FxHolder.EqValues.Count > i) es.Value = f.EqValuesList[i++];
					}
				}

				if (f.BassEnStatus)
					Settings.Default.BassRatio = (int)f.BassEn;

				if (f.StereoEnStatus)
					Settings.Default.StereoWidth = (int)f.StereoEn;

				if (f.BoosterStatus)
				{
					Settings.Default.lg = f.Booster.lg;
					Settings.Default.mg = f.Booster.mg;
					Settings.Default.hg = f.Booster.hg;
				}

				Settings.Default.BassBoost = f.BassEnStatus;
				Settings.Default.Equalizer = f.EqStatus;
				Settings.Default.StereoWiden = f.StereoEnStatus;
				Settings.Default.boosters = f.BoosterStatus;
			}
		}
	}
}