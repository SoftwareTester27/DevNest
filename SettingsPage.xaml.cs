using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FluentUITest
{
	public sealed partial class SettingsPage : Page
	{
		public SettingsPage()
		{
			InitializeComponent();

			// 🔒 Force dark mode for this page too (extra safety)
			RequestedTheme = ElementTheme.Dark;
		}

		// ================= PLACEHOLDER SETTINGS HANDLERS =================
		// Keep this page for app info, toggles, links, etc.
		// DO NOT put theme logic here anymore.

		private void OnResetAppClicked(object sender, RoutedEventArgs e)
		{
			// Example button handler
			// Add real logic later if needed
		}

		private void OnAboutClicked(object sender, RoutedEventArgs e)
		{
			_ = new ContentDialog
			{
				Title = "About",
				Content = "FluentUITest\nDark mode only 🖤",
				CloseButtonText = "OK",
				XamlRoot = this.XamlRoot
			}.ShowAsync();
		}
	}
}
