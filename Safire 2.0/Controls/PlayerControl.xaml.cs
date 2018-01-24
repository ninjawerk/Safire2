using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Kornea.Audio;
using Kornea.Audio.AudioCore;
using Safire.Library.Cycling;


namespace Safire.Controls
{
    /// <summary>
    ///     Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class PlayerControl : UserControl
    {
        public PlayerControl()
        {
            InitializeComponent();
            if (AccessPermission.LymAudioLoaded) Player.Instance.PropertyChanged += Instance_PropertyChanged;
        }

        private void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "StreamStatus":
                    if (Player.Instance.Wave.StreamStatus == StreamStatus.CanPause)
                    {
						if (!Player.Instance.NetStreamingConfigsLoaded)
						{
							btnPlayPause.Content = "";
							btnPlayPause.FontSize = 30;
						}
						else
						{
							btnPlayPause.Content = "";
							btnPlayPause.FontSize = 30;
						}
                    }
                    else
                    {

                        //Show play
                        btnPlayPause.Content = "";
                        btnPlayPause.FontSize = 35;
                    }
                    break;
				case "ActiveStreamHandle":
		            btnNext.IsEnabled = true;
		            btnPrev.IsEnabled = true;
					btnStop.IsEnabled = true;
		            break;
				case "NetStreamHandle":
						 btnNext.IsEnabled = false;
						 btnPrev.IsEnabled = false;
						 btnStop.IsEnabled = false;
		            break;
            }
        }

        private void MainToggle(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Player.Instance.Wave != null && Player.Instance.Wave.StreamStatus == StreamStatus.CanPause && !Player.Instance.NetStreamingConfigsLoaded) // Already Playing, Show Play
            {
                Player.Instance.Pause();   
            }
            else if (Player.Instance.Wave != null && Player.Instance.Wave.StreamStatus != StreamStatus.CanPause) // Already Paused, Show Paused
            {
                Player.Instance.Play();              
            }
			else if (Player.Instance.Wave != null && Player.Instance.Wave.StreamStatus == StreamStatus.CanPause &&  Player.Instance.NetStreamingConfigsLoaded) // Already Playing, Show Play
			{
				Player.Instance.Stop();
			}
            if (Player.Instance.Wave != null && Player.Instance.Wave.StreamStatus == StreamStatus.CanPause)
            {
                
                //Show Pause
				if (!Player.Instance.NetStreamingConfigsLoaded) { 
                btnPlayPause.Content = "";
                btnPlayPause.FontSize = 30;
				}
				else
				{
					btnPlayPause.Content = "";
					btnPlayPause.FontSize = 30;
				}
            }
            else
            {
               
                //Show play
                btnPlayPause.Content = "";
                btnPlayPause.FontSize = 35;
            }
        }
 

        private void StopUp(object sender, MouseButtonEventArgs e)
        {
            Player.Instance.Stop();
        }
 


        private void MuteUp(object sender, MouseButtonEventArgs e)
        {
            if (Player.Instance.Wave != null) Player.Instance.Wave.Mute();
        }


   
        private void btnVol_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            Cycler.NextSong();
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            Cycler.PrevSong();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            Player.Instance.Stop();
        }

        private void btnNext3_Click(object sender, RoutedEventArgs e)
        {
            Player.Instance.Mute();
			if (Config.Muted)
	        {
		        btnMute.Content = "";
	        }
	        else btnMute.Content = "";
			     
        }

      

        
    }
}