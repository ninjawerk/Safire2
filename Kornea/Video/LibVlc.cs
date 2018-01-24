using System;
using System.Runtime.InteropServices;

namespace Kornea.Video
{
    // http://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc.html

    static class LibVlc
    {
        #region core
        [DllImport("libvlc")]
        public static extern IntPtr libvlc_new(int argc, [MarshalAs(UnmanagedType.LPArray,
          ArraySubType = UnmanagedType.LPStr)] string[] argv);

        [DllImport("libvlc")]
        public static extern void libvlc_release(IntPtr instance);
        #endregion

        #region media
        [DllImport("libvlc")]
        public static extern IntPtr libvlc_media_new_location(IntPtr p_instance,
          [MarshalAs(UnmanagedType.LPStr)] string psz_mrl);

        [DllImport("libvlc")]
        public static extern void libvlc_media_release(IntPtr p_meta_desc);
        #endregion

        #region media player
        [DllImport("libvlc")]
        public static extern IntPtr libvlc_media_player_new_from_media(IntPtr media);

        [DllImport("libvlc")]
        public static extern void libvlc_media_player_release(IntPtr player);

        [DllImport("libvlc")]
        public static extern void libvlc_media_player_set_hwnd(IntPtr player, IntPtr drawable);

        [DllImport("libvlc")]
        public static extern IntPtr libvlc_media_player_get_media(IntPtr player);

        [DllImport("libvlc")]
        public static extern void libvlc_media_player_set_media(IntPtr player, IntPtr media);

        [DllImport("libvlc")]
        public static extern int libvlc_media_player_play(IntPtr player);

        [DllImport("libvlc")]
        public static extern void libvlc_media_player_pause(IntPtr player);

        [DllImport("libvlc")]
        public static extern void libvlc_media_player_stop(IntPtr player);

        [DllImport("libvlc")]
        public static extern int libvlc_audio_get_volume(IntPtr player);

        [DllImport("libvlc")]
        public static extern int libvlc_audio_set_volume(IntPtr player, int val);

        [DllImport("libvlc")]
        public static extern float libvlc_media_player_get_position(IntPtr player);

        [DllImport("libvlc")]
        public static extern void libvlc_media_player_set_position(IntPtr player, float f_pos);


        #endregion

        #region exception
        [DllImport("libvlc")]
        public static extern void libvlc_clearerr();

        [DllImport("libvlc")]
        public static extern IntPtr libvlc_errmsg();
        #endregion
    }
}
