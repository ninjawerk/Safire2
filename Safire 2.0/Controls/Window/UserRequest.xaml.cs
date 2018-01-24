using System.Windows;
using MahApps.Metro.Controls;
using Safire.Core;

namespace Safire.Controls.Window
{
	public enum ConfirmType
	{
		Affirmative,
		Negative,
		Auxillary
	}
	public enum ConfirmResult
	{
		Nulled,
		Affirmative,
		Negative,
		Auxiliary
	}
	/// <summary>
	/// Interaction logic for UserRequest.xaml
	/// </summary>
	public partial class UserRequest : MetroWindow
	{
		ConfirmResult MyResult = new ConfirmResult();


		public static ConfirmResult ShowDialogBox(string Prompt, string affirmative, string negative, string auxiliary, string Title = "Confirm")
		{
			UserRequest ur = new UserRequest();
			ur.Title = Title;
			ur.body.Text = Prompt;

			ur.btnAff.Content = affirmative;
			ur.btnNeg.Content = negative;
			ur.btnAux.Content = auxiliary;

			ur.ShowDialog();
			return ur.MyResult;

		}
		public static ConfirmResult ShowDialogBox(string Prompt, string affirmative, string negative , string Title = "Confirm")
		{
			UserRequest ur = new UserRequest();
			ur.Title = Title;
			ur.body.Text = Prompt;

			ur.btnAff.Content = affirmative;
			ur.btnNeg.Content = negative;
			ur.btnAux.Visibility=Visibility.Collapsed;

			ur.ShowDialog();
			return ur.MyResult;

		}
		public static ConfirmResult ShowDialogBox(string Prompt, string affirmative , string Title = "Confirm")
		{
			UserRequest ur = new UserRequest();
			ur.Title = Title;
			ur.body.Text = Prompt;

			ur.btnAff.Content = affirmative;
			ur.btnNeg.Visibility = Visibility.Collapsed;
			ur.btnAux.Visibility = Visibility.Collapsed;

			ur.ShowDialog();
			return ur.MyResult;
		}
		public UserRequest()
		{
			this.InitializeComponent();
			SupportSkinner.SetSkin(this);
			SupportSkinner.OnSkinChanged += SupportSkinner_OnSkinChanged;
			// Insert code required on object creation below this point.
		}

		void SupportSkinner_OnSkinChanged()
		{
			SupportSkinner.SetSkin(this);
		}
  
		private void btnYes_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			MyResult = ConfirmResult.Affirmative;
			Close();
		}

		private void btnNo_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			MyResult = ConfirmResult.Negative;
			Close();
		}

		private void btnAux_Click(object sender, RoutedEventArgs e)
		{
			MyResult = ConfirmResult.Auxiliary;
			Close();
		}
	}
}