using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bridge;
	 
namespace youtube_pl
{
	/// <summary>
	/// Kornea bridge; plugin definition
	/// REMOVING CODES FROM THIS CLASS WILL MAKE YOUR PLUGIN UNSTABLE 
	/// </summary>
	class Plugin : LibraryComponent
	{
		public Plugin()
		{
			Caption = " Name";
			Type = ComponentType.Static;
			ToolTip = "Plugin description";
			Icon = "";
			Category = LibraryComponentCategory.All;
		}

		public override void ChildrenToggled(string child)
		{
				
		}

		public override void Initialize()
		{
			ComponentUI = new lcUI();
			Initialized = true;
		}
	}
	
}
