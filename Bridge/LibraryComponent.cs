using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Bridge
{
	public enum LibraryComponentCategory
	{
		All,
		Artist,
		Album,
		Songs,
		Genre
	}

	public enum ComponentType
	{
		Static,
		Progressive
	}

	public enum LibraryComponentState
	{
		Active,
		Inactive,
		Hidden,
		Static
	}

	[InheritedExport(typeof (LibraryComponent))]
	public abstract class LibraryComponent : IPlugin,IDisposable
	{
		public delegate void StateChanged(LibraryComponent sender, LibraryComponentState state);

		public event StateChanged StateChangedEvent;
		public bool TaskTracker = false;
		private ComponentType _type = ComponentType.Static;
		private bool _initialized=false;
		public Page SettingsPage { get; set; }
		public UIElement ComponentUI { get; set; }

		public List<String> Children = new List<string>();
		//Icon
		public String Icon { get; set; }
		public String Caption { get; set; }
		public String ToolTip { get; set; }
		public LibraryComponentCategory Category { get; set; }

		public ComponentType Type
		{
			get { return _type; }
			set { _type = value; }
		}

		public void RaiseStateChangedEvent(LibraryComponent sender, LibraryComponentState state)
		{
			if (StateChangedEvent != null) StateChangedEvent(sender, state);
		}

		public bool Initialized
		{
			get { return _initialized; }
			set { _initialized = value; }
		}

		public abstract void ChildrenToggled(string child);
		public abstract void Initialize();
		public void Dispose()
		{
 		}
	}
}