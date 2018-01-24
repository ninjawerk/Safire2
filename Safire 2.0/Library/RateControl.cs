using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Safire.Controls.Interactive;
using Safire.Library.Queries;
using Safire.Library.TableModels;
using Safire.SQLite;

namespace Safire.Library
{
	class RateControl : TextBlock
	{
		public RateControl()
		{
			Text = "";
			MouseLeftButtonUp += RateControl_MouseLeftButtonUp;
		}
	 
		void RateControl_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Rate = (Rate <3) ? 5 : 0;
			using (var db = new SQLiteConnection(Tables.DBPath))
			{
				db.TimeExecution = true;
				if (RateType == "")
				{
					var query =
						db.Table<Track>()
							.FirstOrDefault(
								c => c.Path == Path);
					if (query != null)
					{
						query.Rate = Rate;
						try
						{
							TrackQuery.SaveTrack(query);
						}
						catch (Exception exception)
						{
							Console.WriteLine(exception);
						}
					}
				}
				else if (RateType == "Artist")
				{
					var query =
						db.Table<Artist>()
							.FirstOrDefault(
								c => c.Name == Path);
					if (query != null)
					{
						query.Rate = Rate;
						try
						{
							db.Update(query);
						}
						catch (Exception exception)
						{
							Console.WriteLine(exception);
						}
					}
				}
			}
		}
 
		public static readonly DependencyProperty CaptionProperty =
		   DependencyProperty.Register("Rate", typeof(double), typeof(RateControl),
									   new PropertyMetadata(0.0, OnCaptionPropertyChanged));
		[Category("Common Properties")]
		public double Rate
		{
			get { return (double) GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}

		private static void OnCaptionPropertyChanged(DependencyObject dependencyObject,
													 DependencyPropertyChangedEventArgs e)
		{
			var myUserControl = dependencyObject as RateControl;
			//myUserControl.OnPropertyChanged("Rate");
			myUserControl.OnCaptionPropertyChanged(e);
		}

		private void OnCaptionPropertyChanged(DependencyPropertyChangedEventArgs e)
		{

			if (Rate < 3 && Text != "")
			{
				Text = "";
				Opacity = 0.45;
			}
			else if (Text != "" && Rate >= 3)
			{
				Text = "";
				Opacity = 1;
			}

			

		}
		public static readonly DependencyProperty PathProperty =
		   DependencyProperty.Register("Path", typeof(string), typeof(RateControl),
									   new PropertyMetadata(string.Empty, OnPathPropertyChanged));
		[Category("Common Properties")]
		public string Path
		{
			get { return (string)GetValue(PathProperty); }
			set { SetValue(PathProperty, value); }
		}

		private static void OnPathPropertyChanged(DependencyObject dependencyObject,
													 DependencyPropertyChangedEventArgs e)
		{
			var myUserControl = dependencyObject as RateControl;
			//myUserControl.OnPropertyChanged("Rate");
		 
		}

		public string RateType { get; set; }
		 
	}
}
