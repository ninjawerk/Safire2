using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Bridge;

namespace Safire.Core.Plugins
{
	public class PluginCore
	{
		public delegate void plugsLoaded();
		public event plugsLoaded PluginsLoaded;

		protected virtual void OnPluginsLoaded()
		{
			plugsLoaded handler = PluginsLoaded;
			if (handler != null) handler();
		}

	 

		private static PluginCore myins = null;

		public static PluginCore Instance()
		{
			myins = myins ?? new PluginCore();
			return myins;
		}

		public void Compose()
		{
			Thread tr = new Thread(() =>
			{
				dirCatalog = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory);
				AssemblyCatalog assemblyCat = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());
				AggregateCatalog catalog = new AggregateCatalog(assemblyCat, dirCatalog);
				CompositionContainer container = new CompositionContainer(catalog);
				try
				{
					container.ComposeParts(this);
				}
				catch (Exception)
				{
					
				 
				}
				
				OnPluginsLoaded();
			});
			tr.Start();
		}
		//Plugin Access 
		//Module Enumerables
		DirectoryCatalog dirCatalog;


		#region Playlist Modules
		/// <summary>
		/// Playlist Modules
		/// Filter by Extension (no dots)
		/// </summary>
		[ImportMany(typeof(Playlist), AllowRecomposition = true)]
		public IEnumerable<Playlist> PlaylistModules;

		public Playlist GetPlaylist(string ext)
		{
			if (PlaylistModules != null)
				foreach (var playlistModule in PlaylistModules)
				{
					if (playlistModule.FileExtension == ext.Replace(".", ""))
					{

						return (Playlist)Activator.CreateInstance(playlistModule.GetType());

					}
				}
			return null;
		}

		#endregion

		#region Library Components
		/// <summary>
		/// Library Component
		/// </summary>
		[ImportMany(typeof(LibraryComponent), AllowRecomposition = true)]
		public IEnumerable<LibraryComponent> LibraryComponents;


		#endregion
	}
}
