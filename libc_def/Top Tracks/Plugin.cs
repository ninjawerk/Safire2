using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Bridge;
using Kornea.Audio;
using Kornea.Audio.Reactor;

namespace libc_def.Top_Tracks
{
	class Plugin : LibraryComponent
	{
		public Plugin()
		{
			Caption = " Top Charts";
			Type = ComponentType.Static;
			ToolTip = "View charts for today.";
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
	public class ImgConverter : IValueConverter
	{
		public string val = "";
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				val = (string) value;
				var tString = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
			  @"\Safire\Cache\" + System.IO.Path.GetFileName( (string)value) ;
				
				if (File.Exists(tString))
					return new BitmapImage(new Uri(tString) );
					   
				BitmapImage bi = new BitmapImage();
				
				bi.DownloadCompleted += (sender, args) =>
				{
					 

					var encoder = new JpegBitmapEncoder();
					encoder.Frames.Add(BitmapFrame.Create(sender as BitmapImage));

					FileStream fs = new FileStream(tString, FileMode.Create);

					encoder.Save(fs);
					fs.Close();
				};
				bi.BeginInit();
				if (value.ToString() != "")
				{
					bi.UriSource = new Uri(value.ToString());


					bi.EndInit();
				
					
					return bi;
				}
				return null;
			}
			else
			{
				return null;
			}
		}

	 

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
