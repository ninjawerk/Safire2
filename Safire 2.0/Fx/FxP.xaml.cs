using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.AddOn.WaDsp;
using Un4seen.Bass.Misc;

namespace Safire.Fx
{
    /// <summary>
    ///     Interaction logic for FxP.xaml
    /// </summary>
    public partial class FxP : UserControl
    {
        #region FX Handles

        private readonly BASS_BFX_APF _apFfx = new BASS_BFX_APF();
        private readonly BASS_BFX_AUTOWAH _autoWahfx = new BASS_BFX_AUTOWAH();
        private readonly BASS_DX8_CHORUS _chorusfx = new BASS_DX8_CHORUS();
        private readonly BASS_DX8_COMPRESSOR _compressorfx = new BASS_DX8_COMPRESSOR();
        private readonly BASS_DX8_DISTORTION _distortionfx = new BASS_DX8_DISTORTION();


        private readonly BASS_DX8_ECHO _echofx = new BASS_DX8_ECHO();


        private readonly BASS_DX8_FLANGER _flangerfx = new BASS_DX8_FLANGER();
        private readonly BASS_DX8_GARGLE _garglefx = new BASS_DX8_GARGLE();
        private readonly BASS_DX8_I3DL2REVERB _i3Dl2Reverbfx = new BASS_DX8_I3DL2REVERB();
        private readonly BASS_BFX_PHASER _phaserfx = new BASS_BFX_PHASER();
        private readonly BASS_DX8_REVERB _reverbfx = new BASS_DX8_REVERB();
        private int _fxApfHandle;
        private int _fxAutoWahHandle;
        private int _fxChorusHandle;


        private int _fxCompressorHandle;


        private int _fxDistortionHandle;
        private int _fxEchoHandle;
        private int _fxFlangerHandle;
        private int _fxGargleHandle;
        private int _fxI3Dl2ReverbHandle;


        private int _fxPhaserHandle;
        private int _fxReverbHandle;

        private DSP_PeakLevelMeter _plm;

        #endregion

        public FxP()
        {
            
            InitializeComponent();
            EQ.GlobalUiEq = true;    
            EQ.Init("Eq");
            EQ.BoostFactor = 1;
        }

        #region Effect Reactor

        public void UpdateChorus()
        {
            BassEngine engine = BassEngine.Instance;
            if (chorusITM.switchx)
            {
                if (chorusITM.fxState == false) Bass.BASS_FXSetParameters(_fxChorusHandle, _chorusfx);
                if (chorusITM.fxState == false)
                    _fxChorusHandle = Bass.BASS_ChannelSetFX(engine.ActiveStreamHandle, BASSFXType.BASS_FX_DX8_CHORUS, 0);

                switch (chorusITM.SelUTx)
                {
                    case "Default Preset":
                        _chorusfx.Preset_Default();
                        break;

                    case "Preset: A":
                        _chorusfx.Preset_A();
                        break;

                    case "Preset: B":
                        _chorusfx.Preset_B();
                        break;
                }
            }
            else
            {
                Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxChorusHandle);
            }
        }


        public void UpdateReverb()
        {
            BassEngine engine = BassEngine.Instance;
            if (ReverbITM.switchx)
            {
                if (ReverbITM.fxState == false) Bass.BASS_FXSetParameters(_fxReverbHandle, _reverbfx);
                if (ReverbITM.fxState == false)
                    _fxReverbHandle = Bass.BASS_ChannelSetFX(engine.ActiveStreamHandle, BASSFXType.BASS_FX_DX8_REVERB, 0);
                _reverbfx.fReverbMix = 100f;
                switch (ReverbITM.SelUTx)
                {
                    case "Default Preset":
                        //Reverbfx.Preset_Default();
                        break;
                }
            }
            else
            {
                Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxReverbHandle);
            }
        }

        public void UpdateEcho()
        {
            BassEngine engine = BassEngine.Instance;
            if (EchoITM.switchx)
            {
                if (EchoITM.fxState == false) Bass.BASS_FXSetParameters(_fxEchoHandle, _echofx);
                if (EchoITM.fxState == false)
                    _fxEchoHandle = Bass.BASS_ChannelSetFX(engine.ActiveStreamHandle, BASSFXType.BASS_FX_DX8_ECHO, 0);
                switch (EchoITM.SelUTx)
                {
                    case "Default Preset":
                        _echofx.Preset_Default();
                        break;

                    case "Preset: Short":
                        _echofx.Preset_Long();
                        break;

                    case "Preset: Long":
                        _echofx.Preset_Small();
                        break;
                }
            }
            else
            {
                Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxEchoHandle);
            }
        }


        public void UpdateI3Dl2Reverb()
        {
            BassEngine engine = BassEngine.Instance;
            if (I3DL2ReverbITM.switchx)
            {
                if (I3DL2ReverbITM.fxState == false) Bass.BASS_FXSetParameters(_fxI3Dl2ReverbHandle, _i3Dl2Reverbfx);
                if (I3DL2ReverbITM.fxState == false)
                    _fxI3Dl2ReverbHandle = Bass.BASS_ChannelSetFX(engine.ActiveStreamHandle,
                                                                  BASSFXType.BASS_FX_DX8_I3DL2REVERB, 0);
                switch (I3DL2ReverbITM.SelUTx)
                {
                    case "Default Preset":
                        _i3Dl2Reverbfx.Preset_Default();
                        break;
                }
            }
            else
            {
                Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxI3Dl2ReverbHandle);
            }
        }


        public void UpdateGargle()
        {
            BassEngine engine = BassEngine.Instance;
            if (GargleITM.switchx)
            {
                if (GargleITM.fxState == false) Bass.BASS_FXSetParameters(_fxGargleHandle, _garglefx);
                if (GargleITM.fxState == false)
                    _fxGargleHandle = Bass.BASS_ChannelSetFX(engine.ActiveStreamHandle, BASSFXType.BASS_FX_DX8_GARGLE, 0);

                switch (GargleITM.SelUTx)
                {
                    case "Default Preset":
                        _garglefx.Preset_Default();
                        break;
                }
            }
            else
            {
                Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxGargleHandle);
            }
        }


        public void UpdateFlanger()
        {
            BassEngine engine = BassEngine.Instance;
            if (FlangerITM.switchx)
            {
                if (FlangerITM.fxState == false) Bass.BASS_FXSetParameters(_fxFlangerHandle, _flangerfx);
                if (FlangerITM.fxState == false)
                    _fxFlangerHandle = Bass.BASS_ChannelSetFX(engine.ActiveStreamHandle, BASSFXType.BASS_FX_DX8_FLANGER,
                                                              0);
                switch (FlangerITM.SelUTx)
                {
                    case "Default Preset":
                        _flangerfx.Preset_Default();
                        break;

                    case "Preset: A":
                        _flangerfx.Preset_A();
                        break;

                    case "Preset: B":
                        _flangerfx.Preset_B();
                        break;
                }
            }
            else
            {
                Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxFlangerHandle);
            }
        }


        public void UpdateCompressor()
        {
            BassEngine engine = BassEngine.Instance;
            if (CompressorITM.switchx)
            {
                if (CompressorITM.fxState == false) Bass.BASS_FXSetParameters(_fxCompressorHandle, _compressorfx);
                if (CompressorITM.fxState == false)
                    _fxCompressorHandle = Bass.BASS_ChannelSetFX(engine.ActiveStreamHandle,
                                                                 BASSFXType.BASS_FX_DX8_COMPRESSOR, 0);


                switch (FlangerITM.SelUTx)
                {
                    case "Default Preset":
                        _compressorfx.Preset_Default();
                        break;

                    case "Preset: Soft":
                        _compressorfx.Preset_Soft();
                        break;

                    case "Preset: Soft 2":
                        _compressorfx.Preset_Soft2();
                        break;

                    case "Preset: Medium":
                        _compressorfx.Preset_Medium();
                        break;

                    case "Preset: Hard":
                        _compressorfx.Preset_Hard();
                        break;

                    case "Preset: Hard 2":
                        _compressorfx.Preset_Hard2();
                        break;

                    case "Preset: Hard Commercial":
                        _compressorfx.Preset_HardCommercial();
                        break;
                }
            }
            else
            {
                Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxCompressorHandle);
            }
        }


        public void UpdateDistortion()
        {
            BassEngine engine = BassEngine.Instance;
            if (DistortionITM.switchx)
            {
                if (DistortionITM.fxState == false) Bass.BASS_FXSetParameters(_fxDistortionHandle, _distortionfx);
                if (DistortionITM.fxState == false)
                    _fxDistortionHandle = Bass.BASS_ChannelSetFX(engine.ActiveStreamHandle,
                                                                 BASSFXType.BASS_FX_DX8_DISTORTION, 0);
                _distortionfx.Preset_Default();

                switch (DistortionITM.SelUTx)
                {
                    case "Default Preset":
                        _distortionfx.Preset_Default();
                        break;
                }
            }
            else
            {
                Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxDistortionHandle);
            }
        }


        public void UpdateAutoWah()
        {
            BassEngine engine = BassEngine.Instance;
            if (AutoWahITM.switchx)
            {
                if (AutoWahITM.fxState == false) Bass.BASS_FXSetParameters(_fxAutoWahHandle, _autoWahfx);
                if (AutoWahITM.fxState == false)
                    _fxAutoWahHandle = Bass.BASS_ChannelSetFX(engine.ActiveStreamHandle, BASSFXType.BASS_FX_BFX_AUTOWAH,
                                                              0);
                _autoWahfx.Preset_SlowAutoWah();
                switch (AutoWahITM.SelUTx)
                {
                    case "Default Preset":
                        _autoWahfx.Preset_HiFastAutoWah();
                        break;
                }
            }
            else
            {
                Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxAutoWahHandle);
            }
        }


        public void UpdateApf()
        {
            BassEngine engine = BassEngine.Instance;
            if (APFITM.switchx)
            {
                if (APFITM.fxState == false) Bass.BASS_FXSetParameters(_fxApfHandle, _apFfx);
                if (APFITM.fxState == false)
                    _fxApfHandle = Bass.BASS_ChannelSetFX(engine.ActiveStreamHandle, BASSFXType.BASS_FX_BFX_APF, 0);
                _apFfx.Preset_RobotVoice();

                switch (APFITM.SelUTx)
                {
                    case "Default Preset":
                        _apFfx.Preset_RobotVoice();
                        break;
                }
            }
            else
            {
                Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxApfHandle);
            }
        }

        public void UpdatePhaser()
        {
            BassEngine engine = BassEngine.Instance;
            if (PhaserITM.switchx)
            {
                if (PhaserITM.fxState == false) Bass.BASS_FXSetParameters(_fxPhaserHandle, _phaserfx);
                if (PhaserITM.fxState == false)
                    _fxPhaserHandle = Bass.BASS_ChannelSetFX(engine.ActiveStreamHandle, BASSFXType.BASS_FX_BFX_PHASER, 0);
                _phaserfx.Preset_PhaseShift();
                switch (PhaserITM.SelUTx)
                {
                    case "Default Preset":
                        break;
                }
            }
            else
            {
                Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxPhaserHandle);
            }
        }

        #endregion

        #region Toggle Handles

        private void ChorusToggle(object sender, RoutedEventArgs e)
        {
            UpdateChorus();
        }

        private void ReverbToggled(object sender, RoutedEventArgs e)
        {
            UpdateReverb();
        }

        private void EchoToggled(object sender, RoutedEventArgs e)
        {
            UpdateEcho();
        }

        private void I3Dl2ReverbToggled(object sender, RoutedEventArgs e)
        {
            UpdateI3Dl2Reverb();
        }

        private void GargleToggled(object sender, RoutedEventArgs e)
        {
            UpdateGargle();
        }

        private void FlangerToggled(object sender, RoutedEventArgs e)
        {
            UpdateFlanger();
        }

        private void CompressorToggled(object sender, RoutedEventArgs e)
        {
            UpdateCompressor();
        }

        private void DistortionToggled(object sender, RoutedEventArgs e)
        {
            UpdateDistortion();
        }

        private void AutoWahToggled(object sender, RoutedEventArgs e)
        {
            UpdateAutoWah();
        }

        private void ApfToggled(object sender, RoutedEventArgs e)
        {
            UpdateApf();
        }

        private void PhaserToggled(object sender, RoutedEventArgs e)
        {
            UpdatePhaser();
        }

        #endregion

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            chorusITM.AddItem("Default Preset");
            chorusITM.AddItem("Preset: A");
            chorusITM.AddItem("Preset: B");

            ReverbITM.AddItem("Default Preset");

            EchoITM.AddItem("Default Preset");
            EchoITM.AddItem("Preset: Long");
            EchoITM.AddItem("Preset: Short");

            I3DL2ReverbITM.AddItem("Default Preset");

            GargleITM.AddItem("Default Preset");

            FlangerITM.AddItem("Default Preset");
            FlangerITM.AddItem("Preset: A");
            FlangerITM.AddItem("Preset: B");

            DistortionITM.AddItem("Default Preset");

            CompressorITM.AddItem("Default Preset");
            CompressorITM.AddItem("Preset: Soft");
            CompressorITM.AddItem("Preset: Soft 2");
            CompressorITM.AddItem("Default Medium");
            CompressorITM.AddItem("Preset: Hard");
            CompressorITM.AddItem("Preset: Hard 2");
            CompressorITM.AddItem("Preset: Hard Commercial");
        }

        public void BassInit()
        {
            if (Global.EngineOnline)
            {
                //eq.intializeEQ();
                foreach (FxItem i in Stack.Children)
                {
                    i.Direct2Def();
                }
                BassFx.BASS_FX_GetVersion();
                 
                BassEngine engine = BassEngine.Instance;
                engine.PropertyChanged += PropertyChanged;
            }
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.ToString(CultureInfo.InvariantCulture) == "CanPause")
            {
                KillallFxStates();
                RefreshAll();
            }
        }

        public void Initialize()
        {
            EQ.IntializeEq();
        }

        public void RefreshAll()
        {
            try
            {
                _plm = new DSP_PeakLevelMeter(BassEngine.Instance.ActiveStreamHandle, 1);
                _plm.CalcRMS = true;
                _plm.Notification += UpdatePeakMeterDisplay;
                UpdateChorus();
                UpdateReverb();
                UpdateEcho();
                UpdateI3Dl2Reverb();
                UpdateGargle();
                UpdateFlanger();
                UpdateCompressor();
                UpdateDistortion();
                UpdateAutoWah();
                UpdateApf();
                UpdatePhaser();
            }
            catch
            {
            }
            ;
        }

        private void UpdatePeakMeterDisplay(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke((Action) (() =>
                {
                    progressBarRecL.Value = _plm.LevelL;
                    progressBarRecR.Value = _plm.LevelR;
                    labelRMS.Text = String.Format("RMS: {0:#00.0} dBV | AVG: {1:#00.0} dBV | Peak: {2:#00.0} dBV",
                                                  _plm.RMS_dBV,
                                                  _plm.AVG_dBV,
                                                  Math.Max(_plm.PeakHoldLevelL_dBV, _plm.PeakHoldLevelR_dBV));
                    LPeakHold.Text = String.Format("Left peak hold level: {0:#00.0} dBV",
                                                   _plm.PeakHoldLevelL_dBV
                        );
                    RPeakHold.Text = String.Format("Right peak hold level: {0:#00.0} dBV",
                                                   _plm.PeakHoldLevelR_dBV
                        );
                }), DispatcherPriority.Render);
        }

        public void KillallFxStates()
        {
            BassEngine engine = BassEngine.Instance;
            chorusITM.fxState = false;
            Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxChorusHandle);

            ReverbITM.fxState = false;
            Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxReverbHandle);

            EchoITM.fxState = false;
            Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxEchoHandle);

            I3DL2ReverbITM.fxState = false;
            Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxI3Dl2ReverbHandle);

            GargleITM.fxState = false;
            Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxGargleHandle);

            FlangerITM.fxState = false;
            Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxFlangerHandle);

            CompressorITM.fxState = false;
            Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxCompressorHandle);

            DistortionITM.fxState = false;
            Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxDistortionHandle);

            AutoWahITM.fxState = false;
            Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxAutoWahHandle);

            APFITM.fxState = false;
            Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxApfHandle);

            PhaserITM.fxState = false;
            Bass.BASS_ChannelRemoveFX(engine.ActiveStreamHandle, _fxPhaserHandle);
        }

        private void ControlUnloaded(object sender, RoutedEventArgs e)
        {
        }

        public void Save()
        {
            enh.Save();
            EQ.Save();
        }

        private void enh_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void EQ_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}