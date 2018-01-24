using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using Kornea.Audio;
using Safire.Library.Adder;
using Safire.Library.ViewModels;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Tags;

namespace Safire.Core
{
    internal class CoreMain
    {
		public static LibraryAdder LibraryAdder = new LibraryAdder();
        private static TrackViewModel track;
        public static TrackViewModel CurrentTrack
        {
            get
            {
				if (!Player.Instance.NetStreamingConfigsLoaded)
                if (track != null )
                {
                    if (track.Path != Player.Instance.Path)
                    {
                        track = TaglibCore.BuildTrack(Player.Instance.Wave.Path);
                    }
                }
                else
                {
                    if (Player.Instance.Wave != null)
                        track = TaglibCore.BuildTrack(Player.Instance.Wave.Path);
                }
                return track;
            }
        }
		public static TrackViewModel CurrentNetStreamTrack
		{
			get
			{
				if (track != null)
				{
					if (track.Path != Player.Instance.Wave.Path)
					{
						track = BuildNetTags();
					}
				}
				else
				{
					if (Player.Instance.Wave != null)
						track = BuildNetTags();
				}
				return track;
			}
		}
		private static  SYNCPROC mySync;
		private static TAG_INFO _tagInfo;
	    static TrackViewModel BuildNetTags()
	    {

		    var t = new TrackViewModel();

			// get the meta tags (manually - will not work for WMA streams here)
			string[] icy = Bass.BASS_ChannelGetTagsICY(Player.Instance.Wave.Handle);
			if (icy == null)
			{
				// try http...
				icy = Bass.BASS_ChannelGetTagsHTTP(Player.Instance.Wave.Handle);
			}
			if (icy != null)
			{
				t.Title = icy.Where(c => c.Contains("icy-name:")).First().Replace("icy-name:", "");
				t.Bitrate = Convert.ToUInt32(icy.Where(c => c.Contains("icy-br:")).First().Replace("icy-br:", ""));
			}

			_tagInfo = new TAG_INFO(Player.Instance.Wave.Path);
		    t.Path = Player.Instance.Wave.Path;
			if (BassTags.BASS_TAG_GetFromURL(Player.Instance.Wave.Handle, _tagInfo))
		    {
			    // and display what we get
			    t.Album = _tagInfo.album;
			    t.Artist = _tagInfo.artist;
				if (!String.IsNullOrEmpty(_tagInfo.title)) t.Title = _tagInfo.title;

			    t.Genre = _tagInfo.genre;

			    if (t.Bitrate == 0) t.Bitrate = (uint) _tagInfo.bitrate;
		    }
		    mySync = new SYNCPROC(MetaSync);
			Bass.BASS_ChannelSetSync(Player.Instance.Wave.Handle, BASSSync.BASS_SYNC_META, 0, mySync, IntPtr.Zero);
		    return t;
	    }
		private static void MetaSync(int handle, int channel, int data, IntPtr user)
		{
			// BASS_SYNC_META is triggered on meta changes of SHOUTcast streams
			if (_tagInfo.UpdateFromMETA(Bass.BASS_ChannelGetTags(channel, BASSTag.BASS_TAG_META), false, true))
			{
				if (track != null)
				{
					track.Album = _tagInfo.album;
					track.Artist = _tagInfo.artist;
					track.Title = _tagInfo.title;
					track.Genre = _tagInfo.genre;
			 
					track.Bitrate = (uint)_tagInfo.bitrate;
				}
			}
		}
        public static string MyPath()
        {
            String exePath = Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
            string dir = Path.GetDirectoryName(exePath);
            return dir;
        }
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }
                  

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
        public static childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        public static childItem FindMouseOverChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && (child as UIElement).IsMouseOver)
                {
                    return (childItem)child;
                }
                else
                {
                    childItem childOfChild = FindMouseOverChild<childItem>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }
    }
}