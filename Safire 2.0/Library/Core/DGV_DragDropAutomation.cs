using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using Safire.Library.ViewModels;
using Xceed.Wpf.DataGrid;

namespace Safire.Library.Core
{
	class DGV_DragDropAutomation
	{
		private DataGrid myObj = null;
		private Point startPoint;
		private bool DragsViable;

		public void Register(DataGrid ic)
		{
			if (ic != null)
			{
				myObj = ic;
				myObj.PreviewMouseMove += PreviewMouseMove;
				myObj.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDown;
				myObj.SelectionMode=DataGridSelectionMode.Extended;
			}
		}

		private void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			System.Windows.Point pt = e.GetPosition((UIElement)sender);
			// Initiate the hit test by setting up a hit test result callback method.
			VisualTreeHelper.HitTest(myObj, null, new HitTestResultCallback(myCallback), new PointHitTestParameters(pt));
			// Store the mouse position
			startPoint = e.GetPosition(null);
 		}

		private void PreviewMouseMove(object sender, MouseEventArgs e)
		{
			// Get the current mouse position
			Point mousePos = e.GetPosition(null);
			Vector diff = startPoint - mousePos;

			if (DragsViable && e.LeftButton == MouseButtonState.Pressed &&
				(Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
				Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
			{
				// Get the dragged ListViewItem
				bool open = false;
				var mw = App.Current.MainWindow as MainWindow;
				var flyout = mw.Flyouts.Items[0] as Flyout;
				if (flyout != null)
					open = flyout.IsOpen;
				mw.ShowPlaylist();
				// Find the data behind the ListViewItem
				if (myObj.SelectedItem != null)
				{
					List<TrackViewModel> tracks;
					tracks = new List<TrackViewModel>();
					foreach (var v in myObj.SelectedItems)
						tracks.Add(v as TrackViewModel);
					// Initialize the drag & drop operation
					DataObject dragData = new DataObject("trackList", tracks);
					DragDrop.DoDragDrop(myObj, dragData, DragDropEffects.Move);
				}
				if (!open) mw.HidePlaylist();
			}
 		}

 
		 
		// If a child visual object is hit, toggle its opacity to visually indicate a hit. 
		public HitTestResultBehavior myCallback(HitTestResult result)
		{
			DragsViable = false;
			if (result.VisualHit.GetType() == typeof(TextBlock))
			{
				DragsViable = true;

			}
			// Stop the hit test enumeration of objects in the visual tree. 
			return HitTestResultBehavior.Stop;
		}
	}
	class XDGV_DragDropAutomation
	{
		private DataGridControl myObj = null;
		private Point startPoint;
		private bool DragsViable;

		public void Register(DataGridControl ic)
		{
			if (ic != null)
			{
				myObj = ic;
				myObj.PreviewMouseMove += PreviewMouseMove;
				myObj.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDown;
				myObj.SelectionMode =SelectionMode.Extended;
			}
		}

		private void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			System.Windows.Point pt = e.GetPosition((UIElement)sender);
			// Initiate the hit test by setting up a hit test result callback method.
			VisualTreeHelper.HitTest(myObj, null, new HitTestResultCallback(myCallback), new PointHitTestParameters(pt));
			// Store the mouse position
			startPoint = e.GetPosition(null);
		}

		private void PreviewMouseMove(object sender, MouseEventArgs e)
		{
			// Get the current mouse position
			Point mousePos = e.GetPosition(null);
			Vector diff = startPoint - mousePos;

			if (DragsViable && e.LeftButton == MouseButtonState.Pressed &&
				(Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
				Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
			{
				// Get the dragged ListViewItem
				bool open = false;
				var mw = App.Current.MainWindow as MainWindow;
				var flyout = mw.Flyouts.Items[0] as Flyout;
				if (flyout != null)
					open = flyout.IsOpen;
				mw.ShowPlaylist();
				// Find the data behind the ListViewItem
				if (myObj.SelectedItem != null)
				{
					List<TrackViewModel> tracks;
					tracks = new List<TrackViewModel>();
					foreach (var v in myObj.SelectedItems)
						tracks.Add(v as TrackViewModel);
					// Initialize the drag & drop operation
					try
					{
						DataObject dragData = new DataObject("trackList", tracks);
						DragDrop.DoDragDrop(myObj, dragData, DragDropEffects.Move);
					}
					catch (Exception exception)
					{
						Console.WriteLine(exception);
					}
				}
				if(!open)mw.HidePlaylist();
			}
		}



		// If a child visual object is hit, toggle its opacity to visually indicate a hit. 
		public HitTestResultBehavior myCallback(HitTestResult result)
		{
			DragsViable = false;
			if (result.VisualHit.GetType() == typeof(TextBlock))
			{
				DragsViable = true;

			}
			// Stop the hit test enumeration of objects in the visual tree. 
			return HitTestResultBehavior.Stop;
		}
	}
}
