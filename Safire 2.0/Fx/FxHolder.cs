using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Kornea.Audio;
using Kornea.Audio.DSP;
using Safire.Properties;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;

namespace Safire.Fx
{
	internal class FxHolder
	{
		// DSPs with higher priority are called before those with lower.
		//DSP chain priorities 
		public static int prtTrebleBooster = 1000;
		public static int prtBassBooster = 1001;
		public static int prtStereoEnhancer = 998;
		public static int prtBASS_BFX_PEAKEQ = 999;
		public static int prtBooster = 1000;
		 

		public static BEQA Booster;
		public static BassBoost BassBooster;
		public static StereoEnhancer StereoEnhancer; 	 
		public static EQ eql;
		

		public static List<float> EqValues = new List<float>();

		public static readonly float[] OctavesSpacing = new[]
            {
                31.5f, 63f, 125f, 250f, 500f, 1000f, 2000f, 4000f, 8000f, 16000f
            };

		private static int _fxEQ;
		public static bool MadeEq = false;

		public static void Initialize()
		{
			Player.Instance.PropertyChanged += Instance_PropertyChanged;

	 
			//load the eq values from settings
			try
			{
				EqValues = Settings.Default.EQValues.Split(',').Select(float.Parse).ToList();
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
			}
			if (EqValues.Count == 0)
			{
				for (int i = 0; i < 10; i++)
				{
					EqValues.Add(0);
				}
			}
		}

		private static void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ActiveStreamHandle" && !Player.Instance.NetStreamingConfigsLoaded)
			{

				if (Player.Instance.Wave != null)
				{
					//Bass booster
					BassBooster = new BassBoost(Player.Instance.Wave.Handle, prtBassBooster);
					BassBooster.SetBypass(!Settings.Default.afx | !Settings.Default.BassBoost);
					BassBooster.CutOff = Settings.Default.BassRatio;
					BassBooster.Bandwidth = 0.320f;
					BassBooster.d_vol = 1;
					BassBooster.p_vol = 1 - (Settings.Default.BassRatio / 80.0f);
					BassBooster.HighCutoff = Settings.Default.BassRatio;
					BassBooster.LowCutoff1 = Settings.Default.BassRatio;
					BassBooster.LowCutoff2 = Settings.Default.BassRatio;
					BassBooster.Start();

					//Stereo Enhancer

					StereoEnhancer = new StereoEnhancer(Player.Instance.Wave.Handle, prtStereoEnhancer);
					StereoEnhancer.Width = (Settings.Default.StereoWidth / 50);
					StereoEnhancer.SetBypass(!Settings.Default.afx | !Settings.Default.StereoWiden);
					StereoEnhancer.Start();

					//Equalizer 
					//Already made a EQ DSP for this Handle
					MadeEq = false;
					//Create the EQ

					SetBFX_EQ(Player.Instance.Handle);
					 
					Booster = new BEQA(Player.Instance.Wave.Handle, prtBooster);
					Booster.SetBypass(!Settings.Default.afx | !Settings.Default.boosters);
					Booster.Start();
					Booster.Eq.lg = Settings.Default.lg;
					Booster.Eq.mg = Settings.Default.mg;
					Booster.Eq.hg = Settings.Default.hg;
				   

					////Treble booster
					//TrebleBooster = new TrebleBooster(Player.Instance.Wave.Handle, prtTrebleBooster);
					//TrebleBooster.SetBypass(!Settings.Default.BassBoost);
					//TrebleBooster.CutOff = Settings.Default.BassRatio * 30;
					//TrebleBooster.Bandwidth = Settings.Default.BassSelectivity / 1000f;
					//TrebleBooster.d_vol = 1;
					//TrebleBooster.p_vol = 1 - (Settings.Default.BassRatio / 80.0f);
					//TrebleBooster.Start();					 
					ByPassAll(!Settings.Default.afx);

					 
				}
			}
			else if (e.PropertyName == "Play")
			{
			}
		}

		public static void ByPassAll(bool bypass)
		{
			if (Booster != null ) Booster.SetBypass(bypass |  !Settings.Default.boosters);
			if (BassBooster != null  ) BassBooster.SetBypass(bypass | !Settings.Default.BassBoost);
			if (StereoEnhancer != null) StereoEnhancer.SetBypass(bypass | !Settings.Default.StereoWiden);
			if (eql != null ) eql.SetBypass(bypass | !Settings.Default.Equalizer);
		}
		#region Equalizer
		public static void UpdateFX(int band, float gain)
		{
			if (Player.Instance.Wave != null) eql.Update(band, gain );
		}

		public static void SetBFX_EQ(int channel)
		{
			//if the EQ is already made then exit
			if (MadeEq)
			{
				if (eql != null)
				{
					eql.SetBypass(!Settings.Default.afx | !Settings.Default.Equalizer);
					for (int i = 0; i < FxHolder.EqValues.Count; i++)
					{
						UpdateFX(i, FxHolder.EqValues[i]);
					}
				}
				return;
			}
			if (Player.Instance.Wave != null) {eql = new EQ(Player.Instance.Wave.Handle, -1, OctavesSpacing);
			eql.SetBypass(!Settings.Default.afx | !Settings.Default.Equalizer);
			for (int i = 0; i < FxHolder.EqValues.Count; i++)
			{
				UpdateFX(i, FxHolder.EqValues[i]);
			}
			}
			//Now that the EQ is made
			MadeEq = true;
		}
		#endregion
	}
}