using System;
using System.IO;
using System.Net.Http;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.Storage;

namespace FluentUITest
{
	public sealed partial class BrowseMaterials : Page
	{
		public BrowseMaterials()
		{
			this.InitializeComponent();
		}

		// BUTTON CLICK HANDLER (all cards)
		private async void DownloadRepo_Click(object sender, RoutedEventArgs e)
		{
			string url = (sender as Button)?.Tag?.ToString()
						 ?? "https://github.com/SoftwareTester27/CLIculator/archive/refs/heads/main.zip";

			// 🔥 extract repo name safely (CLIculator)
			Uri uri = new Uri(url);
			string[] segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
			string repoName = segments.Length > 1 ? segments[1] : "Repository";

			string fileName = $"{repoName}.zip";

			try
			{
				StorageFolder documents = KnownFolders.DocumentsLibrary;
				StorageFile file = await documents.CreateFileAsync(
					fileName,
					CreationCollisionOption.ReplaceExisting
				);

				using HttpClient client = new HttpClient();
				byte[] data = await client.GetByteArrayAsync(url);

				await FileIO.WriteBytesAsync(file, data);

				// show info bar success
				ShowInfoBarWithCountdown(
					"Resource download successful 🎉",
					$"{fileName} saved to Documents",
					InfoBarSeverity.Success
				);
			}
			catch (Exception ex)
			{
				ShowInfoBarWithCountdown(
					"Resource download failed 💀",
					ex.Message,
					InfoBarSeverity.Error
				);
			}
		}

		// SHOW INFOBAR + 10s PROGRESS COUNTDOWN
		private void ShowInfoBarWithCountdown(string title, string message, InfoBarSeverity severity)
		{
			DownloadInfoBar.Title = title;
			InfoBarMessageText.Text = message;
			DownloadInfoBar.Severity = severity;
			DownloadInfoBar.IsOpen = true;

			// reset progress bar
			ProgressScale.ScaleX = 1;
			DownloadProgressBar.Visibility = Visibility.Visible;

			// slide in animation
			InfoBarTransform.Y = 120;
			var slideIn = new DoubleAnimation
			{
				From = 120,
				To = 0,
				Duration = TimeSpan.FromMilliseconds(350),
				EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
			};
			Storyboard.SetTarget(slideIn, InfoBarTransform);
			Storyboard.SetTargetProperty(slideIn, "Y");
			new Storyboard { Children = { slideIn } }.Begin();

			// progress bar countdown animation (10s)
			var progressAnim = new DoubleAnimation
			{
				From = 1,
				To = 0,
				Duration = TimeSpan.FromSeconds(10),
				EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
			};
			Storyboard.SetTarget(progressAnim, ProgressScale);
			Storyboard.SetTargetProperty(progressAnim, "ScaleX");

			var sb = new Storyboard();
			sb.Children.Add(progressAnim);

			sb.Completed += (_, __) => PlaySlideOut();
			sb.Begin();
		}

		// SLIDE INFOBAR OUT
		private void PlaySlideOut()
		{
			var slideOut = new DoubleAnimation
			{
				From = 0,
				To = 120,
				Duration = TimeSpan.FromMilliseconds(300),
				EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
			};
			Storyboard.SetTarget(slideOut, InfoBarTransform);
			Storyboard.SetTargetProperty(slideOut, "Y");

			var sb = new Storyboard();
			sb.Children.Add(slideOut);
			sb.Completed += (_, __) => DownloadInfoBar.IsOpen = false;
			sb.Begin();
		}
	}
}
