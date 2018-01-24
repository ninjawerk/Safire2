using System;
using Bridge;
using Safire.Animation;
using Safire.Library.ViewModels;

namespace Safire.Library.Files
{
	class AdderPlugin : LibraryComponent
	{
		public AdderPlugin()
		{
			Caption = " Adding songs";
			Type = ComponentType.Progressive;
			ToolTip = "View the search progress.";
			
			Category=LibraryComponentCategory.All;
		}

		public override void ChildrenToggled(string child)
		{
		 
		}

		public override void Initialize()
		{
			ComponentUI = new SearchProgress();
			LibraryAdder.ListenToChanges += LibraryAdder_ListenToChanges;
		}
		LibraryComponentState myState = LibraryComponentState.Inactive;
		private void LibraryAdder_ListenToChanges(TrackViewModel tk, Guid mguid)
		{
			if (myState != LibraryComponentState.Active)
			{
				RaiseStateChangedEvent(this, LibraryComponentState.Active);
				myState = LibraryComponentState.Active;
				App.Current.Dispatcher.BeginInvoke(new Action(() =>
				{
					var mw = App.Current.MainWindow as MainWindow;
					if (mw != null)
						mw.hLibrary.grdDD.FadeOut();
				}));

			}

			if (myState != LibraryComponentState.Inactive && tk.Path == mguid.ToString())
			{
				RaiseStateChangedEvent(this, LibraryComponentState.Inactive);
				myState = LibraryComponentState.Inactive;
			}
		}
	}
}
