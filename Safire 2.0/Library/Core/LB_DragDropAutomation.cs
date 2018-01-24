using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Safire.Library.ViewModels;
using Xceed.Wpf.DataGrid;
using DataObject = System.Windows.DataObject;
using DragDropEffects = System.Windows.DragDropEffects;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Safire.Controls
{
	class LB_DragDropAutomation
	{
		public delegate  List<TrackViewModel> ProcessList(ListBox lst);

		private ProcessList myProcessor;
		private ListBox myObj = null;
		private Point startPoint;
		private bool DragsViable;

		public void Register(ListBox ic, ProcessList processorCallback)
		{
			if (ic != null )
			{
				myProcessor = processorCallback ;
				myObj = ic;
				myObj.PreviewMouseMove += PreviewMouseMove;
				myObj.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDown;
				myObj.AllowDrop = true;
				 
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
				var mw = App.Current.MainWindow as MainWindow;
				mw.ShowPlaylist();
			 
				if (myObj.SelectedItem != null)
				{
					List<TrackViewModel> tracks  ;

					if (myProcessor == null)
					{
						tracks = new List<TrackViewModel>();
						foreach (var v in myObj.SelectedItems)
							tracks.Add(v as TrackViewModel);			
					}
					else
					{
						tracks = myProcessor(myObj);
					}

					// Initialize the drag & drop operation
					DataObject dragData = new DataObject("trackList", tracks);
					DragDrop.DoDragDrop(myObj, dragData, DragDropEffects.Move);
				}
			mw.HidePlaylist();
			}
 		}
 
		// If a child visual object is hit, toggle its opacity to visually indicate a hit. 
		public HitTestResultBehavior myCallback(HitTestResult result)
		{
			DragsViable = false;
			if (result.VisualHit.GetType() == typeof(TextBlock) ||
				result.VisualHit.GetType() == typeof(Image) ||
				result.VisualHit.GetType() == typeof(Border))
			{
				DragsViable = true;

			}
			// Stop the hit test enumeration of objects in the visual tree. 
			return HitTestResultBehavior.Stop;
		}
	}
	class XDG_DragDropAutomation
	{
		public delegate List<TrackViewModel> ProcessList(DataGridControl lst);

		private ProcessList myProcessor;
		private DataGridControl myObj = null;
		private Point startPoint;
		private bool DragsViable;

		public void Register(DataGridControl ic, ProcessList processorCallback)
		{
			if (ic != null)
			{
				myProcessor = processorCallback;
				myObj = ic;
				myObj.PreviewMouseMove += PreviewMouseMove;
				myObj.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDown;
				myObj.AllowDrop = true;

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
				var mw = App.Current.MainWindow as MainWindow;
				mw.ShowPlaylist();

				if (myObj.SelectedItem != null)
				{
					List<TrackViewModel> tracks;

					if (myProcessor == null)
					{
						tracks = new List<TrackViewModel>();
						foreach (var v in myObj.SelectedItems)
							tracks.Add(v as TrackViewModel);
					}
					else
					{
						tracks = myProcessor(myObj);
					}

					// Initialize the drag & drop operation
					DataObject dragData = new DataObject("trackList", tracks);
					DragDrop.DoDragDrop(myObj, dragData, DragDropEffects.Move);
				}
				mw.HidePlaylist();
			}
		}

		// If a child visual object is hit, toggle its opacity to visually indicate a hit. 
		public HitTestResultBehavior myCallback(HitTestResult result)
		{
			DragsViable = false;
			if (result.VisualHit.GetType() == typeof(TextBlock) ||
				result.VisualHit.GetType() == typeof(Image) ||
				result.VisualHit.GetType() == typeof(Border))
			{
				DragsViable = true;

			}
			// Stop the hit test enumeration of objects in the visual tree. 
			return HitTestResultBehavior.Stop;
		}
	}
}
