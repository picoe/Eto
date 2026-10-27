using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;
using Eto.Test.UnitTests;
using Eto.Wpf.Forms;
using NUnit.Framework;

namespace Eto.Test.Wpf.UnitTests
{
	/// <summary>
	/// Checks that menu icons land on whole device pixels at fractional display scales.
	/// </summary>
	/// <remarks>
	/// A menu template built from fixed DIP values only lines up with the device grid when every
	/// vertical offset down to the icon is a multiple of 4: 125% and 175% scale by 5/4 and 7/4, so
	/// an offset that is not a multiple of 4 puts the icon on a half pixel and WPF blends or clips
	/// away its top row. A 22 DIP row pitch, or a popup inset of 3 or 5 DIP, does exactly that.
	///
	/// Windows turn UseLayoutRounding on (WpfWindow.Initialize) and popups inherit it, so this
	/// turns it back off for the menu under test. That is deliberate, and it is what makes one
	/// measurement answer for every scale: with rounding on, the offsets come back as whole device
	/// pixels for the DPI the test happens to run at no matter how the template is built, which
	/// proves nothing - and rounding is not the fix anyway, since it snaps the offset but hands the
	/// icon an arrange slot a device row short and the top row of the bitmap is dropped.
	/// </remarks>
	[TestFixture]
	public class MenuIconDpiTests : TestBase
	{
		static readonly double[] DisplayScales = { 1.0, 1.25, 1.5, 1.75, 2.0 };

		const int IconSize = 16;
		const int ItemCount = 4;

		/// <param name="ownedByEto">
		/// False for the themes that leave menus to WPF's own templates, which do not follow the rule:
		/// None measures 6/28/50/72 DIP in both the menu and its submenus (a 3 DIP popup inset and a
		/// 22 DIP row pitch), so every other row is half a pixel out at 125% and 175%; Fluent's menus
		/// are on-grid but its submenus sit at 22/54/86/118, so every row is out. Neither is Eto's to
		/// style - the submenu inset in particular is baked into WPF's MenuItem template with no
		/// property reaching it - so those are reported as warnings rather than failing the run, and
		/// only the themes Eto styles itself are held to the rule.
		/// </param>
		public static IEnumerable<TestCaseData> ThemeCases()
		{
			yield return new TestCaseData((Func<Theme>)(() => ThemesHandler.GetNone()), false) { TestName = "None" };
#if NET9_0_OR_GREATER
			yield return new TestCaseData((Func<Theme>)(() => Themes.Light), false) { TestName = "Fluent" };
#endif
			yield return new TestCaseData((Func<Theme>)(() => new PaletteTheme(
				"Mocha",
				ThemeStyle.Dark,
				new Uri("pack://application:,,,/Eto.Test.Wpf;component/themes/custom/MochaPalette.xaml"))), true)
			{ TestName = "Palette" };
		}

		[TestCaseSource(nameof(ThemeCases))]
		public void MenuIconsShouldLandOnWholeDevicePixels(Func<Theme> createTheme, bool ownedByEto)
		{
			WithMenu(createTheme, surfaces =>
			{
				Assert.That(surfaces, Is.Not.Empty, "no menu item icons were realized");
				Assert.That(surfaces.Keys, Does.Contain(Submenu), "the submenu did not open");

				var failures = new StringBuilder();
				foreach (var surface in surfaces)
				{
					var icons = surface.Value;
					for (int i = 0; i < icons.Count; i++)
					{
						var (top, height) = icons[i];
						foreach (var scale in DisplayScales)
						{
							if (!IsWholePixel(top * scale))
								failures.AppendLine($"  {surface.Key} item {i} top {top} DIP -> {top * scale} px at {scale * 100}%");
							if (!IsWholePixel(height * scale))
								failures.AppendLine($"  {surface.Key} item {i} height {height} DIP -> {height * scale} px at {scale * 100}%");
						}
					}
				}

				if (failures.Length == 0)
					return;

				var message = $"menu icons do not land on whole device pixels:{Environment.NewLine}{failures}";
				if (ownedByEto)
					Assert.Fail(message);
				else
					Assert.Warn("menus here come from WPF's own templates - " + message);
			});
		}

		const string TopLevel = "menu";
		const string Submenu = "submenu";

		/// <summary>
		/// Opens a context menu with <see cref="ItemCount"/> icon items and a submenu of the same
		/// under the given theme, and hands the callback each popup's icon offsets and heights, in
		/// DIPs relative to that popup's own root. Submenus are measured too because their chrome
		/// comes from the MenuItem template rather than the ContextMenu style, so it can be off-grid
		/// even when the top level menu is fine.
		/// </summary>
		static void WithMenu(Func<Theme> createTheme, Action<IDictionary<string, IList<(double Top, double Height)>>> test)
		{
			ContextMenu menu = null;
			SubMenuItem submenu = null;
			Panel panel = null;
			Theme previousTheme = null;

			Shown(form =>
			{
				previousTheme = Application.Instance.Theme;
				Application.Instance.Theme = createTheme();

				menu = new ContextMenu();
				for (int i = 0; i < ItemCount; i++)
					menu.Items.Add(new ButtonMenuItem { Text = "Item " + i, Image = CreateIcon() });

				submenu = new SubMenuItem { Text = "More" };
				for (int i = 0; i < ItemCount; i++)
					submenu.Items.Add(new ButtonMenuItem { Text = "Sub " + i, Image = CreateIcon() });
				menu.Items.Add(submenu);

				form.Content = panel = new Panel { Size = new Size(200, 100) };
			},
			() =>
			{
				var control = (swc.ContextMenu)menu.ControlObject;
				var submenuControl = (swc.MenuItem)submenu.ControlObject;
				try
				{
					// see the remarks on this fixture
					control.UseLayoutRounding = false;
					menu.Show(panel, PointF.Empty);
					control.UpdateLayout();

					submenuControl.IsSubmenuOpen = true;
					control.UpdateLayout();

					var surfaces = new Dictionary<string, IList<(double, double)>>();
					surfaces[TopLevel] = GetIconBounds(control);

					var popup = Descendants(control).OfType<System.Windows.Controls.Primitives.Popup>().FirstOrDefault(p => p.IsOpen);
					if (popup?.Child is sw.FrameworkElement child)
					{
						child.UpdateLayout();
						surfaces[Submenu] = GetIconBounds(child);
					}

					test(surfaces);
				}
				finally
				{
					submenuControl.IsSubmenuOpen = false;
					control.IsOpen = false;
					Application.Instance.Theme = previousTheme;
				}
			});
		}

		static Image CreateIcon()
		{
			var bitmap = new Bitmap(IconSize, IconSize, PixelFormat.Format32bppRgba);
			using (var graphics = new Graphics(bitmap))
				graphics.FillRectangle(Colors.Red, 0, 0, IconSize, IconSize);
			return bitmap;
		}

		/// <summary>
		/// The Image visuals are the ones Eto puts in MenuItem.Icon, so they are found the same way
		/// whatever the theme calls its template parts.
		/// </summary>
		static IList<(double Top, double Height)> GetIconBounds(sw.FrameworkElement surface)
		{
			var bounds = new List<(double, double)>();
			foreach (var image in Descendants(surface).OfType<swc.Image>())
				bounds.Add((LayoutTop(image, surface), image.ActualHeight));
			return bounds;
		}

		/// <summary>
		/// Sums layout offsets up to <paramref name="surface"/>. TransformToAncestor would be simpler
		/// but includes render transforms, and a menu caught mid open animation then reports its
		/// animated position rather than where it lays out.
		/// </summary>
		static double LayoutTop(System.Windows.Media.Visual visual, sw.DependencyObject surface)
		{
			double top = 0;
			sw.DependencyObject current = visual;
			while (current != null && current != surface)
			{
				if (current is System.Windows.Media.Visual v)
					top += System.Windows.Media.VisualTreeHelper.GetOffset(v).Y;
				current = System.Windows.Media.VisualTreeHelper.GetParent(current);
			}
			return top;
		}

		static IEnumerable<sw.FrameworkElement> Descendants(sw.DependencyObject parent)
		{
			int count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < count; i++)
			{
				var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
				if (child is sw.FrameworkElement fe)
					yield return fe;
				foreach (var descendant in Descendants(child))
					yield return descendant;
			}
		}

		static bool IsWholePixel(double devicePixels) =>
			Math.Abs(devicePixels - Math.Round(devicePixels)) < 1e-6;
	}
}
