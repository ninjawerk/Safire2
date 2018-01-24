using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Safire.Animation
{
    class Colors
    {
        /// <summary>
        /// The Player Controls Background rectangle animation aero to metro and metro to aero
        /// </summary>
        /// <returns></returns>
        public static RadialGradientBrush ControlBarBackground(bool aero)
        {
            RadialGradientBrush ln = new RadialGradientBrush();

            ln.Center = new Point(0.5, 1);
            ln.RadiusX = 0.8;
            ln.RadiusY = 1;
            ln.GradientOrigin = new Point(0.5, 0.6);
            var TC = ((SolidColorBrush)App.Current.FindResource((aero) ? "PrimaryBrushCB" : "HighlightDarkColor")).Color;

            //Top Color
            GradientStop C1 = new GradientStop();
            C1.Color = new Color() { R = TC.R, G = TC.G, B = TC.B, A = (byte) (255*((!aero)?.75:1)) };
            ln.GradientStops.Add(C1);

            //Middle Color
            GradientStop C2 = new GradientStop();
             C2.Color = new Color() { R = TC.R, G = TC.G, B = TC.B, A = (byte)(255 * ((!aero) ? .65 : 1)) };
            C2.Offset = 0.378;
            ln.GradientStops.Add(C2);

            //Bottom Color
            GradientStop C3 = new GradientStop();
             C3.Color = new Color() { R = TC.R, G = TC.G, B = TC.B, A = (byte)(255 * ((!aero) ? 0 : 1)) };
            C3.Offset = 1;
            ln.GradientStops.Add(C3);
            TC = ((SolidColorBrush)App.Current.FindResource((!aero) ? "PrimaryBrushCB" : "HighlightDarkColor")).Color;

            ColorAnimation ColorAnim3 = new ColorAnimation();
            ColorAnim3.To = new Color() { R = TC.R, G = TC.G, B = TC.B, A = (byte)(255 * ((aero) ? 0 : 1)) };
            ColorAnim3.Duration = TimeSpan.FromSeconds(.25);
            ColorAnim3.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseIn  }; 
            C3.BeginAnimation(GradientStop.ColorProperty, ColorAnim3);

            ColorAnimation ColorAnim2 = new ColorAnimation();
            ColorAnim2.To = new Color() { R = TC.R, G = TC.G, B = TC.B, A = (byte)(255 * ((aero) ? .65 : 1)) };
            ColorAnim2.Duration = TimeSpan.FromSeconds(.25);
            ColorAnim2.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }; 
            C2.BeginAnimation(GradientStop.ColorProperty, ColorAnim2);

            ColorAnimation ColorAnim1 = new ColorAnimation();
            ColorAnim1.To =new Color() { R = TC.R, G = TC.G, B = TC.B, A = (byte) (255*((aero)?.75:1)) };
            ColorAnim1.Duration = TimeSpan.FromSeconds(.25);
            ColorAnim1.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }; 
            C1.BeginAnimation(GradientStop.ColorProperty, ColorAnim1);

            return ln;
        }

        public static LinearGradientBrush PartitionBackground(bool aero)
        {
            LinearGradientBrush ln = new LinearGradientBrush();
            ln.Transform = new RotateTransform(90);
       
            var TC = ((SolidColorBrush)App.Current.FindResource((aero) ? "PrimaryBrush" : "AeroBrush")).Color;

            //Top Color
            GradientStop C1 = new GradientStop();
            C1.Color = new Color() { R = TC.R, G = TC.G, B = TC.B, A = (byte)(255 * ((!aero) ? 0 : 1)) };
            ln.GradientStops.Add(C1);

            //Bottom Color
            GradientStop C3 = new GradientStop();
            C3.Color = new Color() { R = TC.R, G = TC.G, B = TC.B, A = (byte)(255 * ((!aero) ? 0 : 1)) };
            C3.Offset = 1;
            ln.GradientStops.Add(C3);

            TC = ((SolidColorBrush)App.Current.FindResource((!aero) ? "PrimaryBrush" : "AeroBrush")).Color;

            ColorAnimation ColorAnim3 = new ColorAnimation();
            ColorAnim3.To = new Color() { R = TC.R, G = TC.G, B = TC.B, A = (byte)(255 * ((aero) ? 0 : 1)) };
            ColorAnim3.Duration = TimeSpan.FromSeconds(.25);
            ColorAnim3.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseIn };
            C3.BeginAnimation(GradientStop.ColorProperty, ColorAnim3);
 

            ColorAnimation ColorAnim1 = new ColorAnimation();
            ColorAnim1.To = new Color() { R = TC.R, G = TC.G, B = TC.B, A = (byte)(255 * ((aero) ? 0 : 1)) };
            ColorAnim1.Duration = TimeSpan.FromSeconds(.25);
            ColorAnim1.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut };
            C1.BeginAnimation(GradientStop.ColorProperty, ColorAnim1);

            return ln;
        }

        public static Color CalculateAverageColor(BitmapSource source)
        {
            if (source.Format.BitsPerPixel != 32)
                throw new ApplicationException("expected 32bit image");

            Color cl;
            System.Windows.Size sz = new System.Windows.Size(source.PixelWidth, source.PixelHeight);

            //read bitmap 
            int pixelsSz = (int)sz.Width * (int)sz.Height * (source.Format.BitsPerPixel / 8);
            int stride = ((int)sz.Width * source.Format.BitsPerPixel + 7) / 8;

            byte[] pixels = new byte[pixelsSz];
            source.CopyPixels(pixels, stride, 0);

            //find the average color for the image
            const int alphaThershold = 10;
            UInt64 r, g, b, a; r = g = b = a = 0;
            UInt64 pixelCount = 0;

            for (int y = 0; y < sz.Height; y++)
            {
                for (int x = 0; x < sz.Width; x++)
                {
                    int index = (int)((y * sz.Width) + x) * 4;
                    if (pixels[index + 3] <= alphaThershold) //ignore transparent
                        continue;

                    pixelCount++;
                    a += pixels[index + 3];
                    r += pixels[index + 2];
                    g += pixels[index + 1];
                    b += pixels[index];
                }
            }

            //average color result
            cl = Color.FromArgb((byte) (a / pixelCount/20),
								(byte)(r / pixelCount / 20),
								(byte)(g / pixelCount / 20),
								(byte)(b / pixelCount / 20));

            return cl;
        }
    }
}
