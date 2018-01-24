using System;

namespace Kornea.Video
{
    public class VlcMediaPlayer : IDisposable
    {
        public IntPtr Handle;
        private IntPtr drawable;
        private bool playing, paused;

        public VlcMediaPlayer()
        {
            string[] args = new string[] {
                "-I", "dummy", "--ignore-config",
                @"--plugin-path=\plugins"
                //,"--vout-filter=deinterlace", "--deinterlace-mode=blend"
            };
            Core.Players++;
            Core.Instance = new VlcInstance(args);
        }

        public VlcMediaPlayer(VlcMedia media)
        {
            string[] args = new string[] {
                "-I", "dummy", "--ignore-config",
                @"--plugin-path=\plugins"
                //,"--vout-filter=deinterlace", "--deinterlace-mode=blend"
            };
            Core.Players++;
            Core.Instance = new VlcInstance(args);
            Handle = LibVlc.libvlc_media_player_new_from_media(media.Handle);
            if (Handle == IntPtr.Zero) throw new VlcException();
        }

        public void Dispose()
        {
            Stop();
            LibVlc.libvlc_media_player_release(Handle);
            if (--Core.Players == 0)
            {
                Core.Instance.Dispose();
            }

        }

        IntPtr Drawable
        {
            get
            {
                return drawable;
            }
            set
            {
                LibVlc.libvlc_media_player_set_hwnd(Handle, value);
                drawable = value;
            }
        }

        public VlcMedia Media
        {
            get
            {
                IntPtr media = LibVlc.libvlc_media_player_get_media(Handle);
                if (media == IntPtr.Zero) return null;
                return new VlcMedia(media);
            }
            set
            {
                LibVlc.libvlc_media_player_set_media(Handle, value.Handle);
            }
        }

        public bool IsPlaying { get { return playing && !paused; } }

        public bool IsPaused { get { return playing && paused; } }

        public bool IsStopped { get { return !playing; } }

        public void Play()
        {
            int ret = LibVlc.libvlc_media_player_play(Handle);
            if (ret == -1)
                throw new VlcException();

            playing = true;
            paused = false;
        }

        public void Pause()
        {
            LibVlc.libvlc_media_player_pause(Handle);

            if (playing)
                paused ^= true;
        }

        public void Stop()
        {
            LibVlc.libvlc_media_player_stop(Handle);

            playing = false;
            paused = false;
        }

        public void Open(VlcMedia media, IntPtr hwnd)
        {
            Handle = LibVlc.libvlc_media_player_new_from_media(media.Handle);
            if (Handle == IntPtr.Zero) throw new VlcException();
            Drawable = hwnd;

        }
        public void Open(string path, IntPtr hwnd)
        {

            using (VlcMedia media = new VlcMedia(Core.Instance, path))
            {
                Handle = LibVlc.libvlc_media_player_new_from_media(media.Handle);
                if (Handle == IntPtr.Zero) throw new VlcException();
            }
            Drawable = hwnd;
        }

        private int _volume = 0;
        public int Volume
        {
            get
            {
                return LibVlc.libvlc_audio_get_volume(Handle);
            }
            set
            {
                LibVlc.libvlc_audio_set_volume(Handle, value);
            }
        }
        public float Position
        {
            get
            {
                return LibVlc.libvlc_media_player_get_position(Handle);
            }
            set
            {
                LibVlc.libvlc_media_player_set_position(Handle, value);
            }
        }
    }
}