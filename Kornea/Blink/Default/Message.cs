using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Kornea.Blink.Default
{
    public class Message : Renderer
    {
        public Brush TextColor = Brushes.White;
        private string message;

        public Message(BlinkGrid blinkGrid)
            : base(blinkGrid)
        {
        }

        public void DisplayMessage(string s)
        {
            message = s;
            base.BlinkG.Transit_In();
            base.BlinkG.InvalidateVisual();
        }

        public override void Draw(DrawingContext dc)
        {
            var tpf = new Typeface("Verdana");
            var ft = new FormattedText(message,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight, tpf, 36, TextColor);
            if (message != "")
                dc.DrawText(ft,
                    new Point((base.BlinkG.Width - ft.Width) / 2, (base.BlinkG.Height - ft.Height) / 2));
        }
    }
}