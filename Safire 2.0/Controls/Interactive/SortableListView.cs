using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Safire.Core;
using Safire.Library.Adder;
using Safire.Library.ViewModels;

namespace Safire.Controls.Interactive
{
	public class SortableListView : ListView
	{
		public bool BottomLocked = false;
		public ContextMenu HeaderMenu;
		public bool UIScroll = false;
		private ListSortDirection _lastDirection = ListSortDirection.Ascending;
		private GridViewColumnHeader _lastHeaderClicked;
		private ScrollViewer sv;

		#region Raise Drag Started

		public delegate void DragStarted();

		public event DragStarted DraggingStarted;

		public void RaiseDragStarted()
		{

			if (DraggingStarted != null)
			{
				DraggingStarted();
			}
		}

		#endregion

		#region Raise Drag Finished

		public delegate void DragFinished(DragDropEffects e);

		public event DragFinished DragFinish;

		public void RaiseDragFinished(DragDropEffects e)
		{

			if (DragFinish != null)
			{
				DragFinish(e);
			}
		}

		#endregion
		//DragDrop Fields
		private Point startPoint;
		public SortableListView()
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				AddHandler(
					ButtonBase.ClickEvent,
					new RoutedEventHandler(GridViewColumnHeaderClickedHandler));
				AddHandler(
					MouseRightButtonUpEvent,
					new RoutedEventHandler(GridViewColumnHeaderClickedHandler));

				Loaded += SortableListView_Loaded;

				//Drag Support Events
				PreviewMouseLeftButtonDown += SortableListView_PreviewMouseLeftButtonDown;
				PreviewMouseMove += SortableListView_PreviewMouseMove;

				//Drop Events
				Drop += SortableListView_Drop;
				DragEnter += SortableListView_DragEnter;
			}
		}

		void SortableListView_DragEnter(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent("myFormat") ||
	  sender == e.Source)
			{
				e.Effects = DragDropEffects.None;
			}
		}

		void SortableListView_Drop(object sender, DragEventArgs e)
		{
			ListView listView = sender as ListView;
			if (e.Data.GetDataPresent("trackList"))
			{
				Object tracklist = e.Data.GetData("trackList");

				var trackViewModels = tracklist as List<TrackViewModel>;
				if (trackViewModels != null)
					foreach (var v in trackViewModels)
					{
						if (listView != null) listView.Items.Add(v);
					}

			}
			Guid mySearchGuid = new Guid();
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				// Note that you can have more than one file.
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

				// Assuming you have one file that you care about, pass it off to whatever
				// handling code you have defined.
				Thread nt = new Thread(o =>
				{
					foreach (var file in files)
					{
						//check if its a dir or file
						if (Directory.Exists(file))
						{
							mySearchGuid = new LibraryAdder().FromFolder(file, (tk, guid) =>
							{
								Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
								{
									if (listView != null && tk != null  && 
										mySearchGuid!= Guid.Empty && 
										guid == mySearchGuid)

										listView.Items.Add(tk);
								}));
							});
						}
						else if (File.Exists(file))
						{
							var tk = LibraryAdder.AddFile(file);
							Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
							{
								if (listView != null && tk != null)
									listView.Items.Add(tk);
							}));
						}
						

					}
				});
				nt.Start();
			}
		}

		#region Drag Events

		private void SortableListView_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			// Get the current mouse position
			Point mousePos = e.GetPosition(null);
			Vector diff = startPoint - mousePos;

			if (e.LeftButton == MouseButtonState.Pressed &&
				(Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
				Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
			{

				// Get the dragged ListViewItem
				ListView listView = sender as ListView;
				ListViewItem listViewItem =
					FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);

				// Find the data behind the ListViewItem
				if (listViewItem != null)
				{
					Object contact = listView.ItemContainerGenerator.
											  ItemFromContainer(listViewItem);
					RaiseDragStarted();
					// Initialize the drag & drop operation
					DataObject dragData = new DataObject("myFormat", contact);
					RaiseDragFinished(DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move));
				}
			}
		}



		private void SortableListView_PreviewMouseLeftButtonDown(object sender,
																 MouseButtonEventArgs e)
		{
			// Store the mouse position
			startPoint = e.GetPosition(null);
		}

		// Helper to search up the VisualTree
		private static T FindAnchestor<T>(DependencyObject current)
			where T : DependencyObject
		{
			do
			{
				if (current is T)
				{
					return (T)current;
				}
				current = VisualTreeHelper.GetParent(current);
			} while (current != null);
			return null;
		}

		#endregion

		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
			{
				if (SelectedItems != null)
					while (SelectedItems.Count > 0)
					{
						Items.Remove(SelectedItems[0]);
					}
				//Items.Remove(SelectedItem);
			}
			base.OnKeyUp(e);
		}


		private void SortableListView_Loaded(object sender, RoutedEventArgs e)
		{
			sv = CoreMain.FindVisualChild<ScrollViewer>(this);
			if (sv != null) sv.ScrollChanged += sv_ScrollChanged;
		}

		public void ScrollToBottomIfLocked()
		{
			UIScroll = false;
			try
			{
				if (BottomLocked)
				{
					//sv.ScrollToBottom();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			UIScroll = true;
		}

		private void sv_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (UIScroll)
			{
				if (sv.ScrollableHeight == sv.VerticalOffset)
				{
					BottomLocked = true;
				}
				else
				{
					BottomLocked = false;
				}
			}
		}


		#region Sort Columns

		private void Sort(string sortBy, ListSortDirection direction)
		{
			ICollectionView dataView =
				CollectionViewSource.GetDefaultView(ItemsSource);

			if (dataView != null)
			{
				dataView.SortDescriptions.Clear();
				var sd = new SortDescription(sortBy, direction);
				dataView.SortDescriptions.Add(sd);
				dataView.Refresh();
			}
		}

		private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
		{
			var headerClicked = e.OriginalSource as GridViewColumnHeader;
			ListSortDirection direction;

			if (headerClicked != null &&
				headerClicked.Role != GridViewColumnHeaderRole.Padding)
			{
				if (headerClicked != _lastHeaderClicked)
				{
					direction = ListSortDirection.Ascending;
				}
				else
				{
					if (_lastDirection == ListSortDirection.Ascending)
					{
						direction = ListSortDirection.Descending;
					}
					else
					{
						direction = ListSortDirection.Ascending;
					}
				}

				// see if we have an attached SortPropertyName value
				string sortBy = GetSortPropertyName(headerClicked.Column);
				if (string.IsNullOrEmpty(sortBy))
				{
					// otherwise use the column header name
					sortBy = headerClicked.Column.Header as string;
				}
				Sort(sortBy, direction);

				_lastHeaderClicked = headerClicked;
				_lastDirection = direction;
			}
		}

		#endregion

		#region SortPropertyNameProperty

		public static readonly DependencyProperty SortPropertyNameProperty =
			DependencyProperty.RegisterAttached("SortPropertyName", typeof(string), typeof(SortableListView));

		public static string GetSortPropertyName(GridViewColumn obj)
		{
			return (string)obj.GetValue(SortPropertyNameProperty);
		}

		public static void SetSortPropertyName(GridViewColumn obj, string value)
		{
			obj.SetValue(SortPropertyNameProperty, value);
		}

		#endregion

		#region Context Menu Header

		public static readonly DependencyProperty ContextMenuHeaderProperty =
			DependencyProperty.Register("ContextMenuHeader", typeof(ContextMenu), typeof(SortableListView),
										new PropertyMetadata(null, OnCaptionPropertyChanged));

		[Category("Common Properties")]
		public ContextMenu Caption
		{
			get { return GetValue(ContextMenuHeaderProperty) as ContextMenu; }
			set { SetValue(ContextMenuHeaderProperty, value); }
		}

		private static void OnCaptionPropertyChanged(DependencyObject dependencyObject,
													 DependencyPropertyChangedEventArgs e)
		{
			var myUserControl = dependencyObject as SortableListView;
			//myUserControl.OnPropertyChanged("ContextMenuHeader");
			//myUserControl.OnCaptionPropertyChanged(e);
		}

		#endregion
	}
}