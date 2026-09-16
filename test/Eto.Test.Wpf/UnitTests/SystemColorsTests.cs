using Eto.Test.UnitTests;
using Eto.Wpf.Drawing;
using Eto.Wpf.Forms;
using NUnit.Framework;
using swk = System.Windows.Markup;
using swm = System.Windows.Media;
using swt = System.Windows.Threading;

namespace Eto.Test.Wpf.UnitTests
{
	/// <summary>
	/// The theme brushes take their color from a DynamicResource so they cannot be frozen, which leaves
	/// them owned by one thread - reading <see cref="SystemColors"/> elsewhere used to throw (RH-98688).
	/// </summary>
	[TestFixture]
	public class SystemColorsTests : TestBase
	{
		/// <summary>
		/// Touching <see cref="System.IO.Packaging.PackUriHelper"/> first because .NET Framework does not
		/// register the pack scheme on its own, and the Uri would not parse.
		/// </summary>
		static Uri PackUri(string path)
		{
			_ = System.IO.Packaging.PackUriHelper.UriSchemePack;
			return new Uri("pack://application:,,,/Eto.Test.Wpf;component/" + path);
		}

		static Uri MochaPalette => PackUri("themes/custom/MochaPalette.xaml");

		/// <summary>
		/// Shaped like the palette themes: the DynamicResource color is what leaves the brush unfreezable,
		/// and so owned by the thread that realizes it. Parsed rather than loaded from a pack Uri, which
		/// the net48 test host cannot resolve.
		/// </summary>
		const string TestResourceXaml = @"
			<ResourceDictionary xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
			                    xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
				<Color x:Key='Eto.Test.SystemColors.Color'>#FF214365</Color>
				<SolidColorBrush x:Key='WindowBackground' Color='{DynamicResource Eto.Test.SystemColors.Color}' />
			</ResourceDictionary>";

		static readonly Color TestColor = Color.FromArgb(0x21, 0x43, 0x65);

		// not Eto.Wpf's ToEto, whose PageOrientation overload needs a ReachFramework reference on net48
		static Color ToEto(swm.Color color) => Color.FromArgb(color.R, color.G, color.B, color.A);

		static Dictionary<string, Color> ReadAll() => new Dictionary<string, Color>
		{
			["ControlBackground"] = SystemColors.ControlBackground,
			["Control"] = SystemColors.Control,
			["ControlText"] = SystemColors.ControlText,
			["HighlightText"] = SystemColors.HighlightText,
			["Highlight"] = SystemColors.Highlight,
			["WindowBackground"] = SystemColors.WindowBackground,
			["DisabledText"] = SystemColors.DisabledText,
			["SelectionText"] = SystemColors.SelectionText,
			["Selection"] = SystemColors.Selection,
			["LinkText"] = SystemColors.LinkText
		};

		/// <summary>Runs <paramref name="read"/> on a thread pool thread, rethrowing as-is.</summary>
		static T FromBackgroundThread<T>(Func<T> read)
		{
			var task = Task.Run(read);
			if (!task.Wait(DefaultTimeout))
				Assert.Fail("Timed out reading from a background thread");
			return task.GetAwaiter().GetResult();
		}

		/// <summary>Merges <see cref="TestResourceXaml"/> for the duration of a test.</summary>
		class ResourceScope : IDisposable
		{
			readonly sw.ResourceDictionary _dictionary = (sw.ResourceDictionary)swk.XamlReader.Parse(TestResourceXaml);

			public swm.SolidColorBrush Brush => (swm.SolidColorBrush)_dictionary["WindowBackground"];

			public void Merge()
			{
				sw.Application.Current.Resources.MergedDictionaries.Add(_dictionary);
			}

			public void Dispose()
			{
				sw.Application.Current.Resources.MergedDictionaries.Remove(_dictionary);
				SystemColorsHandler.RefreshResourceColors();
			}
		}

		/// <summary>
		/// themes/SystemColorsTests.xaml loaded through a pack Uri, so WPF creates its values lazily. That
		/// is the only way a brush can come to be owned by a background thread - a resource dictionary
		/// refuses to take one that already is. Null where the pack Uri does not resolve, as on net48.
		/// </summary>
		static sw.ResourceDictionary LoadDeferredResources()
		{
			try
			{
				return new sw.ResourceDictionary { Source = PackUri("themes/SystemColorsTests.xaml") };
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>What WindowBackground falls back to once its theme brush has been skipped.</summary>
		static Color WindowBrushKeyColor()
		{
			var resource = sw.Application.Current.TryFindResource(sw.SystemColors.WindowBrushKey);
			return ToEto(resource is swm.SolidColorBrush brush ? brush.Color : sw.SystemColors.WindowColor);
		}

		/// <summary>
		/// The net48 test host never creates a WPF Application, so there are no application resources for
		/// the system colors to resolve and every one of them falls back to the OS color.
		/// </summary>
		[SetUp]
		public void RequireApplicationResources()
		{
			if (sw.Application.Current == null)
				Assert.Ignore("This test host has no WPF Application, so no resource can be resolved.");
		}

		/// <summary>The end-to-end RH-98688 case: every brush unfreezable, read from a pool thread.</summary>
		[Test]
		public void PaletteThemeColorsShouldBeReadableFromBackgroundThread()
		{
			Dictionary<string, Color> expected = null;
			Theme previousTheme = null;
			try
			{
				Invoke(() =>
				{
					previousTheme = Application.Instance.Theme;
					Application.Instance.Theme = new PaletteTheme("Test (Mocha Dark)", ThemeStyle.Dark, MochaPalette);
					expected = ReadAll();
				});

				// Eto.Palette.Background.Color from MochaPalette.xaml, not a color the OS would have given us
				Assert.That(expected["WindowBackground"], Is.EqualTo(Color.FromArgb(0x1E, 0x1E, 0x2E)), "The palette should have overridden the window background");

				Assert.That(FromBackgroundThread(ReadAll), Is.EqualTo(expected));
			}
			finally
			{
				Invoke(() => Application.Instance.Theme = previousTheme);
			}
		}

		[Test]
		public void UnfreezableBrushShouldBeReadableFromBackgroundThread()
		{
			Invoke(() =>
			{
				using (var scope = new ResourceScope())
				{
					scope.Merge();
					Assert.That(scope.Brush.CanFreeze, Is.False, "The brush should be unfreezable, like the palette theme brushes");

					// the UI thread realized the brush, so it is the only one allowed to read it
					Assert.That(SystemColors.WindowBackground, Is.EqualTo(TestColor));
					Assert.That(FromBackgroundThread(() => SystemColors.WindowBackground), Is.EqualTo(TestColor));
				}
			});
		}

		/// <summary>The direction the RH-98688 stack trace was taken from: a pool thread owns the brush.</summary>
		[Test]
		public void BrushOwnedByAnotherThreadShouldNotThrow()
		{
			Invoke(() =>
			{
				var deferred = LoadDeferredResources();
				if (deferred == null)
					Assert.Ignore("The pack Uri for the test resources does not resolve on this framework.");

				sw.Application.Current.Resources.MergedDictionaries.Add(deferred);
				try
				{
					// the pool thread looks the brush up first, so it owns it from here on
					FromBackgroundThread(() => deferred["WindowBackground"]);

					var expected = WindowBrushKeyColor();
					Color actual = default;
					Assert.DoesNotThrow(() => actual = SystemColors.WindowBackground);
					Assert.That(actual, Is.Not.EqualTo(TestColor), "The brush cannot be read from here, so its color should not have been used");
					Assert.That(actual, Is.EqualTo(expected));
				}
				finally
				{
					sw.Application.Current.Resources.MergedDictionaries.Remove(deferred);
					SystemColorsHandler.RefreshResourceColors();
				}
			});
		}

		/// <summary>
		/// Resolving from a background thread would make it the brush's owner, so the colors last read on
		/// the application's thread are used there instead.
		/// </summary>
		[Test]
		public void BackgroundThreadShouldNotResolveResourcesItself()
		{
			Invoke(() =>
			{
				using (var scope = new ResourceScope())
				{
					// read on the UI thread first, so there is a known color to fall back to
					var before = SystemColors.WindowBackground;
					Assert.That(before, Is.Not.EqualTo(TestColor));

					scope.Merge();

					Assert.That(FromBackgroundThread(() => SystemColors.WindowBackground), Is.EqualTo(before), "A background thread should reuse the color last read on the UI thread rather than resolve the resource itself");

					// which is what the refresh after a theme change is for
					SystemColorsHandler.RefreshResourceColors();

					Assert.That(FromBackgroundThread(() => SystemColors.WindowBackground), Is.EqualTo(TestColor));
				}
			});
		}

		/// <summary>
		/// SystemEvents.UserPreferenceChanged is raised on a thread of its own, so the refresh has to find
		/// its way to the application's thread rather than read the brushes from where it was called.
		/// </summary>
		[Test]
		public void RefreshFromAnotherThreadShouldStillRefresh()
		{
			Assert.That(Application.Instance.Invoke(() => Thread.CurrentThread.ManagedThreadId), Is.Not.EqualTo(Thread.CurrentThread.ManagedThreadId), "The test should not be running on the UI thread");

			ResourceScope scope = null;
			try
			{
				var before = Application.Instance.Invoke(() =>
				{
					scope = new ResourceScope();
					// read on the UI thread first, so there is a known color to fall back to
					var color = SystemColors.WindowBackground;
					scope.Merge();
					return color;
				});
				Assert.That(before, Is.Not.EqualTo(TestColor));
				Assert.That(SystemColors.WindowBackground, Is.EqualTo(before));

				SystemColorsHandler.RefreshResourceColors();
				// wait behind the posted refresh at its own priority; a plain Invoke is served before it
				sw.Application.Current.Dispatcher.Invoke(() => { }, swt.DispatcherPriority.Background);

				Assert.That(SystemColors.WindowBackground, Is.EqualTo(TestColor));
			}
			finally
			{
				Application.Instance.Invoke(() => scope?.Dispose());
			}
		}
	}
}
