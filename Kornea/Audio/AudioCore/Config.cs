using Kornea.Audio.Reactor;
using Un4seen.Bass;

namespace Kornea.Audio.AudioCore
{
    class Config
    {
        private static bool _reactorFade = true;

        public static void LoadConfigs()
        {
            BassNet.Registration("deshanalahakoon@gmail.com", "2X9231425152222");
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_DEV_DEFAULT  | BASSConfig.BASS_CONFIG_FLOATDSP  , true);
            
            //Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 0);
        }
        public static void LoadPlugins()
        {
            Un4seen.Bass.AddOn.Fx.BassFx.BASS_FX_GetVersion(1);
            Bass.BASS_PluginLoad("bass_fx.dll");
            Bass.BASS_PluginLoad("bass_aac.dll");
            Bass.BASS_PluginLoad("basswma.dll");
            Bass.BASS_PluginLoad("basswv.dll");
            Bass.BASS_PluginLoad("bassmidi.dll");
            Bass.BASS_PluginLoad("bassflac.dll");
        }

        /// <summary>
        /// Fade on Play and Stop
        /// </summary>
        public static bool ReactorFade
        {
            get { return _reactorFade; }
            set { _reactorFade = value; }
        }

        /// <summary>
        /// Default FadeMode for the Player
        /// </summary>
        public static FadeMode ReactorFadeMode { get; set; }

    }
}
