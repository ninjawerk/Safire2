using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Safire.Animation
{
   public static class Extensions
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
      
				targetControl.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
     
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
		/// Fade out the opacity of a control from current to 0 in 500ms no ease
		/// </summary>
		/// <param name="targetControl"></param>
		public static void ImmediateFadeOut(this UIElement targetControl)
		{
			try
			{
				fadeOutList.Add(targetControl);
				var fadeInAnimation = new DoubleAnimation(targetControl.Opacity, 0, new Duration(TimeSpan.FromSeconds(0)));
				Storyboard.SetTarget(fadeInAnimation, targetControl);
				Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(UIElement.OpacityProperty));
				var sb = new Storyboard();
				sb.Completed += FadeOutSB2Completed;
				sb.Children.Add(fadeInAnimation);
				sb.Begin();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
		/// <summary>
		/// Fade out the opacity of a control from current to 0 in 500ms no ease, to collapse state
		/// </summary>
		/// <param name="targetControl"></param>
		public static void FadeOutC(this UIElement targetControl)
		{
			try
			{
				fadeOutList.Add(targetControl);
				var fadeInAnimation = new DoubleAnimation(targetControl.Opacity, 0, new Duration(TimeSpan.FromSeconds(.5)));
				Storyboard.SetTarget(fadeInAnimation, targetControl);
				Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(UIElement.OpacityProperty));
				var sb = new Storyboard();
				sb.Completed += FadeOutSB2Completed;
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
		/// [AUTOMATIC EVENT]
		/// Set the visibility to hidden of an element after the fade out is done
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		static void FadeOutSB2Completed(object sender, EventArgs e)
		{
			try
			{
				if (fadeOutList.Count > fadeOutList.Count - 1)
				{
					fadeOutList[fadeOutList.Count - 1].Visibility = Visibility.Collapsed;
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
		/// <summary>
		/// Common UIElement Move animation to change the position of an element in an Async Animation
		/// </summary>
		/// <param name="element"></param>
		/// <param name="size"></param>
		public static void MoveTo(this UIElement target, double newX, double newY)
		{

			TranslateTransform trans = new TranslateTransform();
			target.RenderTransform = trans;

			DoubleAnimation anim1 = new DoubleAnimation();
			anim1.To = newX;
			anim1.Duration = TimeSpan.FromSeconds(10);

			DoubleAnimation anim2 = new DoubleAnimation();
			anim2.To = newY;
			anim2.Duration = TimeSpan.FromSeconds(10);

			trans.BeginAnimation(TranslateTransform.XProperty, anim1);
			trans.BeginAnimation(TranslateTransform.YProperty, anim2);
		}
		/// <summary>
		/// Color animation for WPF controls
		/// </summary>
		/// <param name="element"></param>
		/// <param name="color"></param>
		public static void ColorAnimation(this Shape element, Color color, Color colorMid)
		{

			//Height animation
			GradientStop sc1 = null; GradientStop sc2 = null;
			if (element != null)
			{
				var solidColorBrush = element.Fill as GradientBrush;
				if (solidColorBrush != null)
				{
					sc1 = solidColorBrush.GradientStops[0];
					sc2 = solidColorBrush.GradientStops[1];
				}



				ColorAnimation ColorAnim3 = new ColorAnimation();
				ColorAnim3.To = color;
				ColorAnim3.Duration = TimeSpan.FromSeconds(2);

				sc1.BeginAnimation(GradientStop.ColorProperty, ColorAnim3);

				ColorAnimation ColorAnim1 = new ColorAnimation();
				ColorAnim1.To = colorMid;
				ColorAnim1.Duration = TimeSpan.FromSeconds(2);

				sc2.BeginAnimation(GradientStop.ColorProperty, ColorAnim1);
			}
		}

    }
}
