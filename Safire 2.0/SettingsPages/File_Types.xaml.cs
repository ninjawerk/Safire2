using System.Windows.Controls;
using Kornea.Windows.FileAssociation;
using Safire.Core;
using Safire.Library.Core;

namespace Safire.SettingsPages
{
	/// <summary>
	/// Interaction logic for AppearanceBackground.xaml
	/// </summary>
	public partial class File_Types : SkinnedPage
	{
		public File_Types()
		{
			InitializeComponent();
			SupportSkinner.SetSkin(this);
			InvalidateVisual();

			foreach (var v in pan.Children)
			{
				if (v.GetType() == typeof(CheckBox))
				{
					var c = v as CheckBox;
					 var f = new AF_FileAssociator(c.Content.ToString());

					//var t = FileAssociation.GetExecFileAssociatedToExtension(c.Content.ToString());
					c.Checked += c_Checked;
					c.Unchecked += c_Unchecked;
					if (Main.MyPath() + "\\Safire 2.0.exe" == f.Executable.Path)
					{
						c.IsChecked = true;
					}
					else
					{
						c.IsChecked = false;
					}
				}
			}
		}

		void c_Unchecked(object sender, System.Windows.RoutedEventArgs e)
		{
			var checkBox = sender as CheckBox;
			if (checkBox != null)
			{
				AF_FileAssociator fa = new AF_FileAssociator(checkBox.Content.ToString());
 
		fa.Delete();
			}
		}

		void c_Checked(object sender, System.Windows.RoutedEventArgs e)
		{
			var checkBox = sender as CheckBox;
			if (checkBox != null)
			{
				AF_FileAssociator fa = new AF_FileAssociator(checkBox.Content.ToString());
				fa.Create("SafireII",
			"Media file",
			new ProgramIcon(Main.MyPath() + @"\resources\ico.ico"),
			new ExecApplication(Main.MyPath() + "\\Safire 2.0.exe"),
			new OpenWithList(new string[] { Main.MyPath() + "\\Safire 2.0.exe" }));
			}
		}
	}
}
