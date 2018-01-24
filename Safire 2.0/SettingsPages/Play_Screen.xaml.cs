using System;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using Safire.Core;
using Safire.GUIs;
using Safire.Library.Imaging;
using Settings = Safire.Properties.Settings;

namespace Safire.SettingsPages
{
    /// <summary>
    /// Interaction logic for AppearanceBackground.xaml
    /// </summary>
    public partial class Play_Screen : SkinnedPage
    {
        public Play_Screen()
        {
            InitializeComponent();
          
            SupportSkinner.SetSkin(this);
            InvalidateVisual();
        }
       
    }
}
