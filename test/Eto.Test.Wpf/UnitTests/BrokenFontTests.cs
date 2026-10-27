using System.IO;
using Eto.Test.UnitTests;
using Eto.Wpf.Drawing;
using Eto.Wpf.Forms;
using Eto.Wpf.Forms.Controls;
using NUnit.Framework;
using swm = System.Windows.Media;

namespace Eto.Test.Wpf.UnitTests
{
	/// <summary>
	/// A font that WPF is unable to load throws from inside its layout pass, where there is nowhere for
	/// the exception to be handled, so it takes down the application (RH-97119). These check that a
	/// <see cref="DropDown"/> showing each item in its own font survives one.
	/// </summary>
	[TestFixture]
	public class BrokenFontTests : TestBase
	{
		/// <summary>
		/// A font family WPF knows about but can no longer read the glyphs of, which is what the users
		/// hitting RH-97119 have installed.
		/// </summary>
		class BrokenFontScope : IDisposable
		{
			readonly string _directory;
			FileStream _lock;

			public swm.FontFamily WpfFamily { get; }

			public Drawing.FontFamily Family { get; }

			public BrokenFontScope()
			{
				var source = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
				if (!File.Exists(source))
					Assert.Ignore($"Cannot build the broken font, '{source}' is not installed.");

				_directory = Path.Combine(Path.GetTempPath(), "EtoBrokenFont_" + Guid.NewGuid().ToString("N"));
				Directory.CreateDirectory(_directory);
				var fileName = Path.Combine(_directory, "broken.ttf");
				File.Copy(source, fileName);

				// Let WPF discover the family while the file is still readable, so it ends up in its font
				// list like any installed font would be..
				var baseUri = new Uri(_directory + Path.DirectorySeparatorChar);
				var location = swm.Fonts.GetFontFamilies(baseUri).FirstOrDefault()?.Source;
				if (location == null)
					Assert.Ignore("WPF did not pick up the copied font, cannot build the broken font.");

				WpfFamily = new swm.FontFamily(baseUri, location);
				Family = new Drawing.FontFamily(new FontFamilyHandler(WpfFamily));

				// ..then take it away. WPF now knows of the font but can't read it, and throws while
				// formatting text with it - exactly what an unreadable installed font does.
				_lock = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
			}

			public Drawing.Font CreateFont(float size = 12) => new Drawing.Font(Family, size);

			public void Dispose()
			{
				_lock?.Dispose();
				_lock = null;
				try
				{
					Directory.Delete(_directory, true);
				}
				catch (IOException)
				{
					// WPF may still have the font file open, leave it to the temp folder cleanup
				}
			}
		}

		static sw.FrameworkElement LayOut(Control content)
		{
			// forces the template to be applied to the WPF controls, which is what lays out the content
			var native = content.ToNative(false);
			native.Measure(new sw.Size(double.PositiveInfinity, double.PositiveInfinity));
			native.Arrange(new sw.Rect(native.DesiredSize));
			return native;
		}

		/// <summary>
		/// Guards the repro itself - if WPF ever stops throwing here, none of the tests below prove anything.
		/// </summary>
		[Test, InvokeOnUI]
		public void BrokenFontShouldThrowFromWpf()
		{
			using (var broken = new BrokenFontScope())
			{
				var textBlock = new swc.TextBlock { Text = "Sample Text", FontFamily = broken.WpfFamily };
				Assert.Throws<UnauthorizedAccessException>(() => textBlock.Measure(new sw.Size(double.PositiveInfinity, double.PositiveInfinity)));
			}
		}

		/// <summary>
		/// Arrange needs guarding as well as measure: an element that fails to measure fails to arrange too,
		/// and either one is fatal. Without this it is easy to guard only the measure and still crash.
		/// </summary>
		[Test, InvokeOnUI]
		public void BrokenFontShouldThrowFromWpfArrange()
		{
			using (var broken = new BrokenFontScope())
			{
				var textBlock = new swc.TextBlock { Text = "Sample Text", FontFamily = broken.WpfFamily };
				var host = new swc.Grid();
				host.Children.Add(textBlock);
				try
				{
					host.Measure(new sw.Size(double.PositiveInfinity, double.PositiveInfinity));
				}
				catch (UnauthorizedAccessException)
				{
					// the measure failure this test isn't about
				}
				Assert.Throws<UnauthorizedAccessException>(() => host.Arrange(new sw.Rect(0, 0, 100, 20)));
			}
		}

		static DropDown CreateFontDropDown(BrokenFontScope broken, string brokenItem, params string[] items)
		{
			var dropDown = new DropDown();
			dropDown.DataStore = items;
			dropDown.FormatItem += (sender, e) =>
			{
				if (Equals(e.Item, brokenItem))
					e.Font = broken.CreateFont(e.Font?.Size ?? 12);
			};
			return dropDown;
		}

		[Test, InvokeOnUI]
		public void BrokenFontShouldNotCrashWhenMeasuringDropDown()
		{
			using (var broken = new BrokenFontScope())
			{
				var dropDown = CreateFontDropDown(broken, "Item 2", "Item 1", "Item 2", "Item 3");

				sw.FrameworkElement native = null;
				Assert.DoesNotThrow(() => native = LayOut(TableLayout.AutoSized(dropDown)));

				Assert.That(native.DesiredSize.Width, Is.GreaterThan(0), "#1 the drop down should still have been sized to the items that do work");
			}
		}

		/// <summary>
		/// The open drop down's items live in a popup, which WPF lays out in its own visual tree rooted at
		/// the popup - no Eto control is an ancestor there, so the item template has to guard itself.
		/// </summary>
		[Test, InvokeOnUI]
		public void BrokenFontShouldNotCrashWhenOpeningDropDown()
		{
			using (var broken = new BrokenFontScope())
			{
				// a width means the items aren't measured up front, so this is the first time the
				// broken font is used - the same as the user simply clicking the drop down.
				var dropDown = CreateFontDropDown(broken, "Item 2", "Item 1", "Item 2", "Item 3");
				dropDown.Width = 100;
				LayOut(TableLayout.AutoSized(dropDown));

				var comboBox = (EtoComboBox)dropDown.ControlObject;
				Assert.DoesNotThrow(() =>
				{
					comboBox.IsDropDownOpen = true;
					var popupChild = comboBox.Popup?.Child;
					popupChild?.Measure(new sw.Size(double.PositiveInfinity, double.PositiveInfinity));
					popupChild?.Arrange(new sw.Rect(popupChild.DesiredSize));
					comboBox.IsDropDownOpen = false;
				});
			}
		}
	}
}
