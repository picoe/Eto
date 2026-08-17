using NUnit.Framework;
using System.Reflection;

namespace Eto.Test.UnitTests.Forms;

[TestFixture]
public class ContextMenuTests : TestBase
{
	[Test, InvokeOnUI]
	public void IncludeSystemItemsShouldDefaultToAll()
	{
		var menu = new ContextMenu();
		Assert.That(menu.IncludeSystemItems, Is.EqualTo(ContextMenuSystemItems.All));
	}

	[TestCase(ContextMenuSystemItems.None)]
	[TestCase(ContextMenuSystemItems.All)]
	[InvokeOnUI]
	public void IncludeSystemItemsShouldRoundTrip(ContextMenuSystemItems items)
	{
		var menu = new ContextMenu();
		menu.IncludeSystemItems = items;
		Assert.That(menu.IncludeSystemItems, Is.EqualTo(items));
	}

	/// <summary>
	/// <see cref="ContextMenuSystemItems.All"/> has every bit set so that it keeps meaning "everything" as more
	/// specific values are added - including for code already compiled against an earlier version, since enum
	/// members are inlined as constants.  Redefining it as an OR of the named values would break that.
	/// </summary>
	[Test]
	public void AllShouldIncludeValuesAddedInTheFuture()
	{
		var notYetDefined = (ContextMenuSystemItems)(1 << 20);
		Assert.That(ContextMenuSystemItems.All.HasFlag(notYetDefined), Is.True);
		Assert.That(ContextMenuSystemItems.All, Is.Not.EqualTo(ContextMenuSystemItems.None));
	}

	/// <summary>
	/// The value must be pushed to the native menu as soon as it is set, not when the menu is shown, otherwise it
	/// would never be applied to a menu assigned to a control via <see cref="Control.ContextMenu"/> since those are
	/// shown by the OS without going through <see cref="ContextMenu.Show(Control, PointF?)"/>.
	/// </summary>
	[Test, InvokeOnUI]
	public void IncludeSystemItemsShouldApplyToNativeMenuWithoutShowing()
	{
		if (!Platform.Instance.IsMac)
			Assert.Ignore("Only macOS adds its own items to a context menu");

		var menu = new ContextMenu(new ButtonMenuItem { Text = "Item 1" });
		Assert.That(GetNativeAllowsSystemItems(menu), Is.True, "#1 - System items should be allowed by default");

		menu.IncludeSystemItems = ContextMenuSystemItems.None;
		Assert.That(GetNativeAllowsSystemItems(menu), Is.False, "#2 - Should be applied to the native menu immediately");

		menu.IncludeSystemItems = ContextMenuSystemItems.All;
		Assert.That(GetNativeAllowsSystemItems(menu), Is.True, "#3 - Should be applied to the native menu immediately");
	}

	[Test, InvokeOnUI]
	public void IncludeSystemItemsShouldApplyWhenAssignedToControl()
	{
		var menu = new ContextMenu(new ButtonMenuItem { Text = "Item 1" }) { IncludeSystemItems = ContextMenuSystemItems.None };
		var textBox = new TextBox { ContextMenu = menu };

		Assert.That(textBox.ContextMenu, Is.SameAs(menu), "#1 - Menu should be returned as-is when assigned");
		Assert.That(textBox.ContextMenu.IncludeSystemItems, Is.EqualTo(ContextMenuSystemItems.None), "#2");

		if (Platform.Instance.IsMac)
			Assert.That(GetNativeAllowsSystemItems(menu), Is.False, "#3 - Native menu assigned to the control should not allow system items");
	}

	/// <summary>
	/// Reads NSMenu.AllowsContextMenuPlugIns without referencing the Mac backend from this shared project.
	/// </summary>
	static bool GetNativeAllowsSystemItems(ContextMenu menu)
	{
		var control = menu.ControlObject;
		var property = control?.GetType().GetProperty("AllowsContextMenuPlugIns", BindingFlags.Public | BindingFlags.Instance);
		Assert.That(property, Is.Not.Null, "Could not find NSMenu.AllowsContextMenuPlugIns on the native menu");
		return (bool)property.GetValue(control);
	}

	[Test, ManualTest]
	public void SystemItemsShouldOnlyBeIncludedWhenSpecified()
	{
		ManualForm(
			"Right click each text box.\n"
			+ "The top one should show ONLY 'Custom Item 1/2'.\n"
			+ "The bottom one should also show system items (AutoFill, Services, ...) on macOS.",
			form =>
			{
				var without = new TextBox
				{
					Text = "IncludeSystemItems = None",
					ContextMenu = CreateMenu(ContextMenuSystemItems.None)
				};
				var with = new TextBox
				{
					Text = "IncludeSystemItems = All",
					ContextMenu = CreateMenu(ContextMenuSystemItems.All)
				};

				return new StackLayout
				{
					Spacing = 10,
					HorizontalContentAlignment = HorizontalAlignment.Stretch,
					Items = { without, with }
				};
			});
	}

	[Test, ManualTest]
	public void SystemItemsShouldNotBeIncludedWhenShownManually()
	{
		ManualForm(
			"Right click the text box.  It should show ONLY 'Custom Item 1/2'.",
			form =>
			{
				var menu = CreateMenu(ContextMenuSystemItems.None);
				var textBox = new TextBox { Text = "Shown via ContextMenu.Show()" };
				textBox.MouseDown += (sender, e) =>
				{
					if (e.Buttons == MouseButtons.Alternate)
					{
						menu.Show(textBox);
						e.Handled = true;
					}
				};
				return textBox;
			});
	}

	/// <summary>
	/// A Drawable only accepts text input when its TextInput event is hooked up, so only that one should get the
	/// system's text input items - even though on macOS both share the same native class.
	/// </summary>
	[Test, ManualTest]
	public void SystemItemsShouldNotBeIncludedForControlsThatDontTakeTextInput()
	{
		ManualForm(
			"Right click each box.\n"
			+ "The top one handles TextInput, so it may show system items (AutoFill, ...) on macOS.\n"
			+ "The bottom one does NOT, so it should show ONLY 'Custom Item 1/2'.",
			form =>
			{
				var withTextInput = CreateDrawable("Handles TextInput", Colors.SteelBlue);
				withTextInput.TextInput += (sender, e) => Log.Write(sender, $"TextInput: {e.Text}");
				var withoutTextInput = CreateDrawable("No TextInput", Colors.DarkSlateGray);

				return new StackLayout
				{
					Spacing = 10,
					HorizontalContentAlignment = HorizontalAlignment.Stretch,
					Items = { withTextInput, withoutTextInput }
				};
			});
	}

	static Drawable CreateDrawable(string text, Color color)
	{
		var drawable = new Drawable
		{
			Size = new Size(250, 60),
			CanFocus = true,
			ContextMenu = CreateMenu(ContextMenuSystemItems.All)
		};
		drawable.Paint += (sender, e) =>
		{
			e.Graphics.FillRectangle(color, e.ClipRectangle);
			e.Graphics.DrawText(SystemFonts.Default(), Colors.White, 4, 4, text);
		};
		return drawable;
	}

	static ContextMenu CreateMenu(ContextMenuSystemItems includeSystemItems)
	{
		var menu = new ContextMenu(
			new ButtonMenuItem { Text = "Custom Item 1" },
			new ButtonMenuItem { Text = "Custom Item 2" });
		menu.IncludeSystemItems = includeSystemItems;
		return menu;
	}
}
