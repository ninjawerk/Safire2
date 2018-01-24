using System;
using System.Collections.Generic;
using System.IO;
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
using Kornea.Audio;
using Kornea.Audio.Reactor;
using Safire.Library.ViewModels;

namespace Safire
{
	/// <summary>
	/// Interaction logic for DuoList.xaml
	/// </summary>
	public partial class DuoList : UserControl
	{
		public DuoList()
		{
			this.InitializeComponent();
		}

        private void lst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void duotracks_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            duotracks.Tag = duotracks.SelectedIndex;
            Safire.Library.Cycling.Items.trackSource = duotracks;
            var tracky = duotracks.SelectedItem as TrackViewModel;
            if (tracky != null && File.Exists(tracky.Path))
            {
                var aw = new AudioWave(tracky.Path, OutputMode.DirectSound, Player.Instance.Volume);
                aw.ReactorUsageLocked = true;
                aw.Play();
                aw.ReactorUsageLocked = false;
                var fader = new PanFade(aw, Player.Instance.Wave, 10, 2000, true, Player.Instance.Volume);
                fader.StartAndKill();

                Player.Instance.NewMedia(ref aw);
                //Player.Instance.Play();
            }
        }
	}
}