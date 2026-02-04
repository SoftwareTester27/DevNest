using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Win32;
using System;
using System.IO;
using System.Diagnostics;

namespace FluentUITest
{
	public sealed partial class MainWindow : Window
	{
		// 👑 global access (used by SettingsPage)
		public static MainWindow Instance { get; private set; }

		// page order = slide direction logic
		private readonly Type[] _pageOrder =
		{
			typeof(MainPage),
			typeof(BrowseMaterials),
			typeof(BrowseImages),
			typeof(DownloadContent),
			typeof(SettingsPage)
		};

		private int _currentPageIndex = 0;

		public MainWindow()
		{
			InitializeComponent();
			Instance = this;

			ExtendsContentIntoTitleBar = true;
			SetTitleBar(CustomDragRegion);

			RootGrid.RequestedTheme = ElementTheme.Dark;

			SetDesktopWallpaperBackground();

			// 🚀 FORCE HOME PAGE ON STARTUP
			_currentPageIndex = 0;
			contentFrame.Navigate(typeof(MainPage));
		}

		// ================= WALLPAPER =================
		private void SetDesktopWallpaperBackground()
		{
			try
			{
				string path = Registry.GetValue(
					@"HKEY_CURRENT_USER\Control Panel\Desktop",
					"WallPaper",
					null
				) as string;

				if (!string.IsNullOrEmpty(path) && File.Exists(path))
				{
					WallpaperRect.Fill = new ImageBrush
					{
						ImageSource = new BitmapImage(new Uri(path)),
						Stretch = Stretch.UniformToFill
					};
				}
			}
			catch
			{
				Debug.WriteLine("Failed to set wallpaper.");
			}
		}

		// ================= NAVIGATION =================
		private void NvSample_SelectionChanged(
			NavigationView sender,
			NavigationViewSelectionChangedEventArgs args)
		{
			Type target = null;

			if (args.IsSettingsSelected)
			{
				target = typeof(SettingsPage);
			}
			else if (args.SelectedItem is NavigationViewItem item)
			{
				target = item.Tag switch
				{
					"MainPage" => typeof(MainPage),
					"BrowseMaterials" => typeof(BrowseMaterials),
					"BrowseImages" => typeof(BrowseImages),
					"DownloadContent" => typeof(DownloadContent),
					_ => null
				};
			}

			if (target != null)
				SafeNavigate(target);
		}

		private void SafeNavigate(Type page)
		{
			try
			{
				int newIndex = Array.IndexOf(_pageOrder, page);
				if (newIndex == -1 || newIndex == _currentPageIndex)
					return;

				var transition = new SlideNavigationTransitionInfo
				{
					Effect = newIndex > _currentPageIndex
						? SlideNavigationTransitionEffect.FromRight
						: SlideNavigationTransitionEffect.FromLeft
				};

				_currentPageIndex = newIndex;
				contentFrame.Navigate(page, null, transition);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Navigation failed: {ex}");

				_ = new ContentDialog
				{
					Title = "Navigation Error",
					Content = $"Failed to open page:\n{ex.Message}",
					CloseButtonText = "OK"
				}.ShowAsync();
			}
		}
	}
}