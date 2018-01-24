using System;

namespace Kornea.Video
{
   public class VlcInstance : IDisposable
    {
        internal IntPtr Handle;

        public VlcInstance(string[] args)
        {
            Handle = LibVlc.libvlc_new(args.Length, args);
            if (Handle == IntPtr.Zero) throw new VlcException();
        }

        public void Dispose()
        {
            LibVlc.libvlc_release(Handle);
        }
    }
}