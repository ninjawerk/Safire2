using System;

namespace Kornea.Video
{
  public  class VlcMedia : IDisposable
    {
        internal IntPtr Handle;
        
        public VlcMedia(VlcInstance instance, string url)
        {
            Handle = LibVlc.libvlc_media_new_location(instance.Handle, url);
            if (Handle == IntPtr.Zero) throw new VlcException();
        }

        internal VlcMedia(IntPtr handle)
        {
            this.Handle = handle;
        }

        public void Dispose()
        {
            LibVlc.libvlc_media_release(Handle);
        }
    }
}