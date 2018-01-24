using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using Safire.Animation;
using Safire.Core;

namespace Safire.SettingsPages
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        public SettingsWindow()
        {
            SupportSkinner.SetSkin(this);
            InitializeComponent();


            TreeViewItem item = new TreeViewItem();
            item.Header = "Appearance";
            item.ItemsSource = new string[] { "Themes", "Background" ,"Advanced" };

 
			TreeViewItem item1 = new TreeViewItem();
			item1.Header = "Player";
			item1.ItemsSource = new string[] { "Library", "Plugins" , "Play Screen", "Updates" };

             TreeViewItem item2 = new TreeViewItem();
            item2.Header = "Audio";
            item2.ItemsSource = new string[] { "General", "Crossfade", "Volume", };

             TreeViewItem item3 = new TreeViewItem();
            item3.Header = "Services";
            item3.ItemsSource = new string[] { "Scrobbler", "Media Content", "Top Charts"};

              
            tVw.Items.Add(item);
			tVw.Items.Add(item1);
            tVw.Items.Add(item2);
            tVw.Items.Add(item3);

            SupportSkinner.SetSkin(this);
            SupportSkinner.OnSkinChanged += SupportSkinner_OnSkinChanged;
			 
        }

        void SupportSkinner_OnSkinChanged()
        {
            SupportSkinner.SetSkin(this);
                          
        }

        private void tVw_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                
                Type type = Assembly.GetExecutingAssembly().GetType("Safire.SettingsPages." + tVw.SelectedValue.ToString().Replace(' ','_'));
                if (type != null)
                {
                    //class with the given name exists


                    var uri = new Uri(tVw.SelectedValue.ToString().Replace(' ', '_') + ".xaml", UriKind.RelativeOrAbsolute);
                    frame.Source = uri;	       
	                frame.Opacity = 0;
                    frame.FadeIn();

                }

                
            }
            catch (IOException ie)
            {
            }
        }


	    private void SettingsWindow_OnClosing(object sender, CancelEventArgs e)
	    {
		    e.Cancel = true;
			Hide();
	    }
    }
}