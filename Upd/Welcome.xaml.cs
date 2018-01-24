using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.Zip;
using MahApps.Metro.Controls;
using Upd;


namespace Safire.Wizards
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class Welcome : MetroWindow
	{
		private readonly string devid = "";
		private int wizIndex = 1;
		private Definition myDefinition = null;
		public Welcome()
		{
			SupportSkinner.SetSkin(this);
			InitializeComponent();
			Definition q = new Definition();
			q.Author = "Deshan Alahakoon";
			q.CodeName = "Leyon";
			q.PatchNotes = "First Update Check";
			q.ReleasedDateTime = DateTime.Now;
			q.Version = 1.0;
			q.URL = "http://www.lyracloud.com/Leyon/update.zip";

			var h = Serialize(q);
			Definition cdef = BinaryDeSerializeObject<Definition>(dir + @"\myDef.bin");
			SupportSkinner.SetSkin(this);
			SupportSkinner.OnSkinChanged += SupportSkinner_OnSkinChanged;
			Thread tr = new Thread(() =>
			{
				if (!CheckForInternetConnection())
				{
					Dispatcher.BeginInvoke(new Action(() =>
					{
						homeProg.FadeOut();
						homeSub.Text = "Cannot connect right now";
						largeIcon.Text = "";
						nextCaption.Text = "Finish";
						nextControls.FadeIn();
						wizIndex = 3;
						txtIndex.FadeOut();
					}));
				}
				string xml;
				using (var w = new WebClient())
				{
					xml = w.DownloadString("http://www.lyracloud.com/Leyon/def.xml");
				}
				try
				{
					myDefinition = Deserialize<Definition>(xml);
					if ((cdef != null && cdef.Version < (myDefinition.Version)) || cdef == null)
					{
						if (myDefinition != null)
						{
							Dispatcher.BeginInvoke(new Action(() =>
							{
								w1.FadeOut();
								w2.FadeIn();
								txtNotes.Text = myDefinition.PatchNotes;
								nextCaption.Text = "I agree";
								nextControls.FadeIn();
								txtIndex.Text = ++wizIndex + "/4";
								vers.Text = "version " + myDefinition.Version;
							}));
						}
					}
					else
					{
						Dispatcher.BeginInvoke(new Action(() =>
						{
							homeProg.FadeOut();
							homeSub.Text = "Everything is up to date!";
							largeIcon.Text = "";
							nextCaption.Text = "Finish";
							nextControls.FadeIn();
							w3.FadeOut();
							w1.FadeIn();
							wizIndex = 3;
							txtIndex.FadeOut();
						}));
					}
				}
				catch
				{
					Dispatcher.BeginInvoke(new Action(() =>
				{
					homeProg.FadeOut();
					homeSub.Text = "Cannot find updates";
					largeIcon.Text = "";
					nextCaption.Text = "Finish";
					nextControls.FadeIn();
					wizIndex = 3;
					txtIndex.FadeOut();
				}));
				}

			});
			tr.Start();

		}
		#region utils
		public static bool CheckForInternetConnection()
		{
			try
			{
				using (var client = new WebClient())
				using (var stream = client.OpenRead("http://www.google.com"))
				{
					return true;
				}
			}
			catch
			{
				return false;
			}
		}
		public static string Serialize<T>(T value)
		{

			if (value == null)
			{
				return null;
			}

			XmlSerializer serializer = new XmlSerializer(typeof(T));

			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
			settings.Indent = false;
			settings.OmitXmlDeclaration = false;

			using (StringWriter textWriter = new StringWriter())
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
				{
					serializer.Serialize(xmlWriter, value);
				}
				return textWriter.ToString();
			}
		}
		public static T Deserialize<T>(string xml)
		{
			if (string.IsNullOrEmpty(xml))
			{
				return default(T);
			}
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			XmlReaderSettings settings = new XmlReaderSettings();
			// No settings need modifying here
			using (StringReader textReader = new StringReader(xml))
			{
				using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
				{
					return (T)serializer.Deserialize(xmlReader);
				}
			}
		}
		public static T BinaryDeSerializeObject<T>(string filename)
		{
			try
			{
				if (!File.Exists(filename)) return default(T);
				var stream = new FileStream(filename, FileMode.Open);
				var serializer = new XmlSerializer(typeof(T));
				var xs = (T)serializer.Deserialize(stream);
				return xs;
			}
			catch
			{
				return default(T);
			}
			;
		}
		#endregion

		private void SupportSkinner_OnSkinChanged()
		{
			SupportSkinner.SetSkin(this);
		}


		private void nextClick(object sender, RoutedEventArgs e)
		{
			wizIndex++;
			switch (wizIndex)
			{
				case 3:
					w2.FadeOut();
					w3.FadeIn();
					//Update here
					prog.IsIndeterminate = false;
					sts.Text = "Downloading";
				
					Update();
					nextControls.FadeOut();
					break;
				case 4:
					w3.FadeOut();
					Close();
					break;
			}
			txtIndex.Text = wizIndex.ToString() + "/4";
		}
	 
		string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
		void Update()
		{
		

			Thread tr = new Thread(() =>
			{
				string xml;
				using (var w = new WebClient())
				{
					w.DownloadProgressChanged += (sender, args) => Dispatcher.BeginInvoke(new Action(() =>
					{
						prog.Maximum = args.TotalBytesToReceive;
						prog.Value = args.BytesReceived;
					}));
					string tString = dir + @"\data\";
					if (!Directory.Exists(tString)) Directory.CreateDirectory(tString);

					w.DownloadFileAsync(new Uri(myDefinition.URL), dir + @"\data\upda.bin");
					w.DownloadFileCompleted += (sender, args) =>
					{
						try
						{
							Dispatcher.BeginInvoke(new Action(() =>
							{
								prog.IsIndeterminate = true;
								sts.Text = "Installing";
							}));
							using (ZipInputStream s = new ZipInputStream(File.OpenRead(dir + @"\data\upda.bin")))
							{

								ZipEntry theEntry;
								while ((theEntry = s.GetNextEntry()) != null)
								{

									try
									{
										string directoryName = Path.GetDirectoryName(theEntry.Name);
										string fileName = Path.GetFileName(theEntry.Name);

										// create directory
										if (directoryName.Length > 0)
										{
											Directory.CreateDirectory(dir + @"\" + directoryName);
										}

										if (fileName != String.Empty)
										{
											using (FileStream streamWriter = File.Create(dir + @"\" + theEntry.Name))
											{

												int size = 2048;
												byte[] data = new byte[2048];
												while (true)
												{
													size = s.Read(data, 0, data.Length);
													if (size > 0)
													{
														streamWriter.Write(data, 0, size);
													}
													else
													{
														break;
													}
												}
											}

										}
									}
									catch
									{
									}
								}
							}

							//ArchiveManager.UnArchive(, dir + @"\");
							Dispatcher.BeginInvoke(new Action(() =>
							{
								homeProg.FadeOut();
								homeSub.Text = "Updated sucessfully.";
								largeIcon.Text = "";
								nextCaption.Text = "Finish";
								nextControls.FadeIn();
								w3.FadeOut();
								w1.FadeIn();
							}));
							Stream stream = File.Open(dir + @"\myDef.bin", FileMode.Create);
							var xmlSerializer = new XmlSerializer(typeof(Definition));
							xmlSerializer.Serialize(stream, myDefinition);
							stream.Close();
							 
						}
						catch
						{
						}

					};
				}
				
			});
			tr.Start();
		}
	}
}