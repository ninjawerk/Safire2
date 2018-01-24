using Bridge;

namespace Safire.Library
{
	class MyLibraryPlugin : LibraryComponent
	{
		public override void ChildrenToggled(string child)
		{
			 
		}

		public override void Initialize()
		{

		}
		public MyLibraryPlugin()
		{
			Caption = " My Library";
			Type = ComponentType.Static;
			ToolTip = "Browse my library.";
			Icon = "";
			Category = LibraryComponentCategory.All;
			Children.Add(" Songs");
			Children.Add(" Artists");
			Children.Add(" Albums");

			Children.Add(" Genres");

		}
	}
}
