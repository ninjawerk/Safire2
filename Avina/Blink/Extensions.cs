using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Kornea.Blink
{
    static class Extensions
    {
        public static List<UIElement> fadeOutList = new List<UIElement>();

        /// <summary>
        /// Common UIElement Size animation to change the size of an element in an Async Animation
        /// </summary>
        /// <param name="element"></param>
        /// <param name="size"></param>
        public static void SizeAnimation(this UIElement element, Size size)
        {
            //Height animation
            DoubleAnimation AnimHeight = new DoubleAnimation();
            AnimHeight.To = size.Height;
            AnimHeight.Duration = TimeSpan.FromSeconds(.25);
            AnimHeight.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseIn };
            element.BeginAnimation(FrameworkElement.HeightProperty, AnimHeight);

            //Width animation
            if (element.RenderSize.Width != size.Width)
            {
                DoubleAnimation AnimWidth = new DoubleAnimation();
                AnimWidth.To = size.Width;
                AnimWidth.Duration = TimeSpan.FromSeconds(.25);
                AnimWidth.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseIn };
                element.BeginAnimation(FrameworkElement.WidthProperty, AnimWidth);
            }
        }

        /// <summary>
        /// Color Animation to transit the color in an async animation
        /// </summary>
        /// <param name="element"></param>
        /// <param name="color"></param>
        public static void ColorAnimation(this Panel element, Color color)
        {
            //Height animation
            SolidColorBrush sc;
            var solidColorBrush = element.Background as SolidColorBrush;
            if (solidColorBrush != null)
            {
                sc = new SolidColorBrush(solidColorBrush.Color);
            }
            else
            {
                sc = new SolidColorBrush();
            }
            element.Background = sc;
            ColorAnimation ColorAnim3 = new ColorAnimation();
            ColorAnim3.To = color;
            ColorAnim3.Duration = TimeSpan.FromSeconds(.25);
            ColorAnim3.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseIn };
            sc.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnim3);

        }

        /// <summary>
        /// Fade in the control opacity from current to full in 500ms no ease
        /// </summary>
        /// <param name="targetControl"></param>
        public static void FadeIn(this UIElement targetControl)
        {
            try
            {
                if (fadeOutList.Contains(targetControl)) fadeOutList.Remove(targetControl);

                targetControl.Visibility = Visibility.Visible;
                var fadeInAnimation = new DoubleAnimation(targetControl.Opacity, 1, new Duration(TimeSpan.FromSeconds(.5)));
                Storyboard.SetTarget(fadeInAnimation, targetControl);
                Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(UIElement.OpacityProperty));
                var sb = new Storyboard();
                sb.Children.Add(fadeInAnimation);
                sb.Begin();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Fade out the opacity of a control from current to 0 in 500ms no ease
        /// </summary>
        /// <param name="targetControl"></param>
        public static void FadeOut(this UIElement targetControl)
        {
            try
            {
                fadeOutList.Add(targetControl);
                var fadeInAnimation = new DoubleAnimation(targetControl.Opacity, 0, new Duration(TimeSpan.FromSeconds(.5)));
                Storyboard.SetTarget(fadeInAnimation, targetControl);
                Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(UIElement.OpacityProperty));
                var sb = new Storyboard();
                sb.Completed += FadeOutSBCompleted;
                sb.Children.Add(fadeInAnimation);
                sb.Begin();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// [AUTOMATIC EVENT]
        /// Set the visibility to hidden of an element after the fade out is done
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void FadeOutSBCompleted(object sender, EventArgs e)
        {
            try
            {
                if (fadeOutList.Count > fadeOutList.Count - 1)
                {
                    fadeOutList[fadeOutList.Count - 1].Visibility = Visibility.Hidden;
                    fadeOutList.RemoveAt(fadeOutList.Count - 1);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        /// Color animation for WPF controls
        /// </summary>
        /// <param name="element"></param>
        /// <param name="color"></param>
        public static void ColorAnimation(this Control element, Color color)
        {
            //Height animation
            SolidColorBrush sc;
            if (element != null)
            {
                var solidColorBrush = element.Background as SolidColorBrush;
                if (solidColorBrush != null)
                {
                    sc = new SolidColorBrush(solidColorBrush.Color);
                }
                else
                {
                    sc = new SolidColorBrush();
                }

                element.Background = sc;
                ColorAnimation ColorAnim3 = new ColorAnimation();
                ColorAnim3.To = color;
                ColorAnim3.Duration = TimeSpan.FromSeconds(.325);
                ColorAnim3.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseIn };
                sc.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnim3);
            }
        }

    }
}
