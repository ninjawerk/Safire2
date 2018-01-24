using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kornea.Blink
{
    class Snap
    {
        public static RenderTargetBitmap GetSnap(Control control)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)control.ActualWidth, (int)control.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(control);
            return rtb;
        }

    }
}
