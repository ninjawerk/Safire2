using System;
using Kornea.Properties;
using Safire.Animation;
using Safire.Core;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using MahApps.Metro.Controls;

namespace Safire.Controls
{
	class LiveTile : Border, INotifyPropertyChanged, IDisposable
	{

		private ToggleSwitch tog;
		private Expander expander;
		Grid acc2Grid = new Grid();
		Grid acc3Grid = new Grid();

		public LiveTile()
		{
		}
		/// <summary>
		///     For tile highlighting
		///     ON/OFF Tile
		/// </summary>
		#region IsChecked
		public static readonly DependencyProperty IsCheckedProperty =
			DependencyProperty.Register("IsChecked", typeof(bool), typeof(LiveTile),
										new PropertyMetadata(false, OnIsCheckedPropertyChanged));

		[Category("Common Properties")]
		public bool IsChecked
		{
			get { return (bool)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}

		private static void OnIsCheckedPropertyChanged(DependencyObject dependencyObject,
												  DependencyPropertyChangedEventArgs e)
		{
			var myUserControl = dependencyObject as LiveTile;
			myUserControl.OnPropertyChanged("IsChecked");

		}
		#endregion

		/// <summary>
		///     Only CLR - No Dependencies
		///     Tracks if the tile doesn't need a ON/OFF state
		///     If true, its a always ON TILE, else ON/OFF depends
		///     if its True, internal toggleswitch wont have any effect
		/// </summary>
		#region IsActiveDefault

		[Category("Common Properties")]
		public bool IsActiveDefault { get; set; }

		#endregion

		/// <summary>
		///     Tile size animation on expander toggles
		/// </summary>
		#region Internal Expander Animation

		/// <summary>
		/// Tile Animation when expanded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void c_Expanded(object sender, RoutedEventArgs e)
		{
			if (acc2Grid.Background != null) expander.ColorAnimation(((SolidColorBrush)acc2Grid.Background).Color);

			double height = expander.Margin.Top + 5 + expander.ActualHeight;
			this.SizeAnimation(new Size(ActualWidth, height));
		}

		/// <summary>
		/// Tile animation on collapse
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void c_Collapsed(object sender, RoutedEventArgs e)
		{


			double height = expander.Margin.Top + 40;
			Dispatcher.DelayInvoke("VirtExpanderCollapse",
								   () => { this.SizeAnimation(new Size(this.ActualWidth, height)); }, new TimeSpan(500),
								   DispatcherPriority.Normal);
			expander.ColorAnimation(Color.FromArgb(0, 0, 0, 0));
		}

		#endregion

		/// <summary>
		///     After the UI is initialized
		/// </summary>
		#region Initialized Override


		/// <summary>
		///     Hook onto the toggleswitch check, uncheck events
		///     to automatically turn the tile ON and OFF
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInitialized(EventArgs e)
		{
			SupportSkinner.OnSkinChanged += SupportSkinner_OnSkinChanged;
			acc2Grid.SetResourceReference(Grid.BackgroundProperty, "AccentColorBrush2");
			acc3Grid.SetResourceReference(Grid.BackgroundProperty, "AccentColorBrush3");


			foreach (ToggleSwitch c in CoreMain.FindVisualChildren<ToggleSwitch>(this))
			{
				tog = c;
				c.Checked += c_Checked;
				c.Unchecked += c_Unchecked;
			}
			ChangeEnabled(Child, (bool) tog.IsChecked);
			foreach (Expander c in CoreMain.FindVisualChildren<Expander>(this))
			{
				expander = c;
				c.Collapsed += c_Collapsed;
				c.Expanded += c_Expanded;
			}
			base.OnInitialized(e);
			
		}

		void SupportSkinner_OnSkinChanged()
		{
			try
			{
				acc2Grid.SetResourceReference(Grid.BackgroundProperty, "AccentColorBrush2");
				acc3Grid.SetResourceReference(Grid.BackgroundProperty, "AccentColorBrush3");
				if (IsChecked) Background = acc3Grid.Background;

				if (expander != null && expander.IsExpanded) expander.ColorAnimation(((SolidColorBrush)acc2Grid.Background).Color);

				if (Background != null && IsChecked && expander != null)
				{
					var ColorBack = new SolidColorBrush(((SolidColorBrush)Background).Color);
					Background = ColorBack;
					var ColorAnim = new ColorAnimation();
					if (acc3Grid.Background != null) ColorAnim.To = ((SolidColorBrush)acc3Grid.Background).Color;
					ColorAnim.Duration = TimeSpan.FromSeconds(.25);
					ColorAnim.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseIn };
					ColorBack.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnim);
				}
			}
			catch (Exception)
			{ }
		}

		#endregion

		/// <summary>
		///     ToggleSwitch hooked events, for automatic Tile ON/OFF
		/// </summary>
		#region Internal ToggleSwitch Events

		/// <summary>
		///     when internal Toggleswitch is unchecked, then cut off the highlight
		///     automatically sets the ischecked value to false
		///     and the tile will be turned OFF
		///     **ONLY works if IsActiveDefault is turned off
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void c_Unchecked(object sender, RoutedEventArgs e)
		{
			if (DesignerProperties.GetIsInDesignMode(this)) return;
			if (!IsActiveDefault)
			{
				IsChecked = false;
				if (Background != null)
				{
					var ColorBack = new SolidColorBrush(color: ((SolidColorBrush)Background).Color);

					Background = ColorBack;
					var ColorAnim = new ColorAnimation();
					ColorAnim.To = Color.FromArgb(0, 0, 0, 0);
					ColorAnim.Duration = TimeSpan.FromSeconds(.25);
					ColorAnim.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseIn };
					ColorBack.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnim);
				}
				ChangeEnabled(Child, false);
			}
		}

		/// <summary>
		///     when internal Toggleswitch is checked, then highlight
		///     automatically sets the ischecked value to true
		///     and the tile will be turned ON
		///     **ONLY works if IsActiveDefault is turned off
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void c_Checked(object sender, RoutedEventArgs e)
        {
			if (DesignerProperties.GetIsInDesignMode(this)) return;
            if (!IsActiveDefault)
            {
                IsChecked = true;
                if (Background != null)
                {
                    var ColorBack = new SolidColorBrush(((SolidColorBrush)Background).Color);
                    Background = ColorBack;
                    var ColorAnim = new ColorAnimation();
                    if (acc3Grid.Background != null) ColorAnim.To = ((SolidColorBrush)acc3Grid.Background).Color;
                    ColorAnim.Duration = TimeSpan.FromSeconds(.25);
                    ColorAnim.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseIn };
                    ColorBack.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnim);
                }
	            ChangeEnabled(Child, true);
            }
        }

		/// <summary>
		/// When the toggle switch is automatically set, that means not user interacted
		/// Then it wont fire the check uncheck events
		/// this is a manual method to reflect the changes of the tile
		/// </summary>
		public void RefreshToggle()
		{
			if (DesignerProperties.GetIsInDesignMode(this)) return;
			if (!IsActiveDefault)
			{
				if (tog.IsChecked == true)
				{
					IsChecked = true;
					var ColorBack = new SolidColorBrush(((SolidColorBrush)Background).Color);
					Background = ColorBack;
					var ColorAnim = new ColorAnimation();
					if (acc3Grid.Background != null) ColorAnim.To = ((SolidColorBrush)acc3Grid.Background).Color;
					ColorAnim.Duration = TimeSpan.FromSeconds(.25);
					ColorAnim.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseIn };
					ColorBack.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnim);
				}
				else
				{
					IsChecked = false;
					var ColorBack = new SolidColorBrush(((SolidColorBrush)Background).Color);
					Background = ColorBack;
					var ColorAnim = new ColorAnimation();
					ColorAnim.To = Color.FromArgb(0, 0, 0, 0);
					ColorAnim.Duration = TimeSpan.FromSeconds(.25);
					ColorAnim.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseIn };
					ColorBack.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnim);

				}
			}
		}
		#endregion

		/// <summary>
		///     1 Automatically Change the tile when mouse enters/leaves
		///     2 Turns on the Tile on double click
		/// </summary>
		//#region MouseEnter/MouseLeave

		///// <summary>
		/////     Highlight the tile with the highlight color
		/////     irrespective of the isChecked value
		///// </summary>
		///// <param name="e"></param>
		//protected override void OnMouseEnter(MouseEventArgs e)
		//{
		//    var ColorBack = new SolidColorBrush(((SolidColorBrush) Background).Color);
		//    Background = ColorBack;
		//    var ColorAnim = new ColorAnimation();
		//    ColorAnim.To = ((SolidColorBrush) (Application.Current.FindResource("TileHover"))).Color;
		//    ColorAnim.Duration = TimeSpan.FromSeconds(.25);
		//    ColorAnim.EasingFunction = new CircleEase {EasingMode = EasingMode.EaseIn};
		//    ColorBack.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnim);
		//}

		///// <summary>
		/////     Change the background to normal, if the tile is not checked
		/////     if the tile is checked set the background to the highlight color
		///// </summary>
		///// <param name="e"></param>
		//protected override void OnMouseLeave(MouseEventArgs e)
		//{
		//    SolidColorBrush ColorBack = null;
		//    ColorBack = (SolidColorBrush) Background;
		//    Background = ColorBack;
		//    var ColorAnim = new ColorAnimation();
		//    ColorAnim.To =
		//        ((SolidColorBrush) (Application.Current.FindResource((IsChecked) ? "TileHighlight" : "TileBackground")))
		//            .Color;
		//    ColorAnim.Duration = TimeSpan.FromSeconds(.25);
		//    ColorAnim.EasingFunction = new CubicEase {EasingMode = EasingMode.EaseIn};
		//    ColorBack.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnim);
		//}

		/// <summary>
		/// Turns on the Tile on double click
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				if (tog != null) tog.IsChecked = !tog.IsChecked;
			}
		}
		//#endregion

		/// <summary>
		///     Methods for property change notifications
		/// </summary>
		
		#region NotiyProp

		public event PropertyChangedEventHandler PropertyChanged;
		[NotifyPropertyChangedInvocator]
		internal void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		public   void ChangeEnabled(DependencyObject depObj, bool en)
		{
			if ( DesignerProperties.GetIsInDesignMode(this)) return;
			 
			if (depObj == null) return;

			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
			{
				var child = VisualTreeHelper.GetChild(depObj, i);
				if (child is Slider)
				{
					(child as Slider).IsEnabled = en;
				}
				ChangeEnabled(child, en);
			}
		}
		public void Dispose()
		{

		}
	}
}