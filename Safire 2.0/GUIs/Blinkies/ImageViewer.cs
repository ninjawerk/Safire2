 using System.Windows;
using System.Windows.Media;
 using Kornea.Blink;
 using Size = System.Windows.Size;

namespace Safire.GUIs.Blinkies
{
    class ImageViewer : Renderer
    {
        private ImageSource img = null;

        public ImageViewer(BlinkGrid blinkGrid,ImageSource ims) : base(blinkGrid)
        {
            img = ims;
            blinkGrid.InvalidateVisual();
        }

        public override void Draw(DrawingContext dc)
        {
           
            double height = 0;
            double width = 0;
            if (img.Width <= img.Height)
            {
                if (img.Height > base.BlinkG.ActualHeight) height = BlinkG.ActualHeight;
                else height = img.Height;

                width = height*img.Width/img.Height;

            }
            else
            {
                if (img.Width > base.BlinkG.ActualWidth) width = BlinkG.ActualWidth;
                else width = img.Width;

                height = width * img.Height / img.Width;
            }

            var loc = new System.Windows.Point((base.BlinkG.ActualWidth - width)/2, (base.BlinkG.ActualHeight - height)/2);
            dc.DrawImage(img, new Rect(loc, new Size(width, height)));
 
        }
    }
}
