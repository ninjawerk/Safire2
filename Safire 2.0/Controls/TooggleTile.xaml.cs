using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Safire.Controls
{
	/// <summary>
	/// Interaction logic for TooggleTile.xaml
	/// </summary>
	public partial class ToggleTile : UserControl
	{
		public ToggleTile()
		{
			InitializeComponent();
			PreviewMouseUp += ToggleTile_PreviewMouseUp;
		}

		private bool toggled = false;
		void ToggleTile_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			toggled = !toggled;
			TogRectangle.Opacity = (toggled) ? 1 : .25;
		}

		public Grid Content
		{
			get { return  contentGrid  ; }
		}
	}
}
