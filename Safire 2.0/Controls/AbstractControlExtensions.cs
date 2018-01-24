using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Safire.Controls
{
	static class AbstractControlExtensions
	{
		/// Method to get scrollbar in a visual object
		public  static ScrollViewer GetScrollbar( this DependencyObject dep)
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dep); i++)
			{
				var child = VisualTreeHelper.GetChild(dep, i);
				if (child != null && child is ScrollViewer)
					return child as ScrollViewer;
				else
				{
					ScrollViewer sub = GetScrollbar( child);
					if (sub != null)
						return sub;
				}
			}
			return null;
		}
	}
}
