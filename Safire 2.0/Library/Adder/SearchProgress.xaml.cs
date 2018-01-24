using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Safire.Animation;
using Safire.Library.ViewModels;

namespace Safire.Library.Adder
{
	/// <summary>
	/// Interaction logic for SearchProgress.xaml
	/// </summary>
	public partial class SearchProgress : UserControl
	{
		public SearchProgress()
		{
			InitializeComponent();
			LibraryAdder.ListenToChanges += LibraryAdder_ListenToChanges;
			dpt.Tick += Callback;
			dpt.Interval=new TimeSpan(0,0,0,0,500);

		}
		DispatcherTimer dpt = new DispatcherTimer( );

		private void Callback(object sender, EventArgs eventArgs)
		{
			if (stopTick + 1000 < Environment.TickCount)
			{
				ring.FadeIn();
				tb1.FadeIn();
				btnCancel.FadeIn();
			}
			else
			{
				tbTrack.Text = tkCount + " tracks added";
			}

			if (trk != null && trk.Path == msGuid.ToString())
			{
				ring.FadeOut();
				tb1.FadeOut();
				btnCancel.FadeOut();
				tbTrack.Text = tkCount + " tracks added";
				dpt.Stop();
			}
			if (trk != null) tbTrack.Text = trk.Title + "\n" + tkCount + " tracks added";
		}

		private Guid msGuid;
		private TrackViewModel trk;
		void LibraryAdder_ListenToChanges(TrackViewModel tk, Guid mGuid)
		{
			trk = tk;
			msGuid = mGuid;
				tkCount++;
				dpt.Start();
			 
		}

		private long tkCount = 0;
	 

		private long stopTick;
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			stopTick = Environment.TickCount;
			LibraryAdder.GlobalSearchCancel = true;
			ring.FadeOut();
			tb1.FadeOut();
			btnCancel.FadeOut();
			tbTrack.Text = tkCount + " tracks added";
		}
	}
}
