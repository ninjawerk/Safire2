using System;
using System.Management;
using System.Windows.Media;
using Microsoft.Win32;
using Safire.Core;

namespace Safire.SettingsPages
{
    /// <summary>
    /// Interaction logic for AppearanceBackground.xaml
    /// </summary>
    public partial class Advanced : SkinnedPage
    {
        public Advanced()
        {
            InitializeComponent();
            ManagementObjectSearcher searcher
= new ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration");
            if (lfps != null) lfps.Text = "(" + cfps.Value.ToString() + ") FPS";

            string graphicsCard = string.Empty;
            foreach (ManagementObject mo in searcher.Get())
            {
                foreach (PropertyData property in mo.Properties)
                {
                    if (property.Name == "Description")
                    {
                        graphicsCard = property.Value.ToString();
                    }
                }
            }
            details.Text = "Graphics Card : " + graphicsCard
            + "\n" + "DirectX Version : " + GetDirectxMajorVersion() + "\n";
            details.Text += "Max. Texture Size: " + RenderCapability.MaxHardwareTextureSize.Width + "x" + RenderCapability.MaxHardwareTextureSize.Height + "px.\n";
 
            string rc = "Rendering Capability: " + ((RenderCapability.Tier == 0) ? "Okay" : (RenderCapability.Tier == 1) ? "Good" : "Excellent");
            details.Text += rc + "\n";
            SupportSkinner.SetSkin(this);
            InvalidateVisual();
        }
        private int GetDirectxMajorVersion()
        {
            int directxMajorVersion = 0;

            var OSVersion = Environment.OSVersion;

            // if Windows Vista or later
            if (OSVersion.Version.Major >= 6)
            {
                // if Windows 7 or later
                if (OSVersion.Version.Major > 6 || OSVersion.Version.Minor >= 1)
                {
                    directxMajorVersion = 11;
                }
                // if Windows Vista
                else
                {
                    directxMajorVersion = 10;
                }
            }
            // if Windows XP or earlier.
            else
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DirectX"))
                {
                    string versionStr = key.GetValue("Version") as string;
                    if (!string.IsNullOrEmpty(versionStr))
                    {
                        var versionComponents = versionStr.Split('.');
                        if (versionComponents.Length > 1)
                        {
                            int directXLevel;
                            if (int.TryParse(versionComponents[1], out directXLevel))
                            {
                                directxMajorVersion = directXLevel;
                            }
                        }
                    }
                }
            }

            return directxMajorVersion;
        }


        private void cfpsValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (lfps != null) lfps.Text = "(" + cfps.Value.ToString() + ") FPS";
        }
    }
}
