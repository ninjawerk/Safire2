using System.Windows.Media;

namespace Kornea.Blink
{
    public abstract class Renderer
    {
        public BlinkGrid BlinkG = null;

        public Renderer(BlinkGrid blinkGrid)
        {
            BlinkG = blinkGrid;
        }

   


        public abstract void Draw(DrawingContext dc);

    }
}
