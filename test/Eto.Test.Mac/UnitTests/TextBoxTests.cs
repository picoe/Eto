using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Mac.UnitTests
{
	[TestFixture]
	public class TextBoxTests : TestBase
	{
		/// <summary>
		/// It is the field editor that supplies a text field's menu, not the NSTextField, so the field editor must
		/// fall back to the standard editing menu when the TextBox has no ContextMenu of its own - otherwise right
		/// clicking it shows nothing at all.
		/// </summary>
		[Test]
		public void RightClickShouldShowDefaultMenuWithoutContextMenu()
		{
			TextBox textBox = null;
			ContextMenu contextMenu = null;
			Shown(form =>
			{
				textBox = new TextBox { Text = "Some Text" };
				contextMenu = new ContextMenu(new ButtonMenuItem { Text = "Custom Item 1" });
				form.Content = textBox;
			},
			() =>
			{
				textBox.Focus();

				var defaultItems = GetFieldEditorMenuItemCount(textBox);
				Assert.That(defaultItems, Is.GreaterThan(0), "#1 - Should fall back to the standard editing menu");

				textBox.ContextMenu = contextMenu;
				Assert.That(GetFieldEditorMenuItemCount(textBox), Is.EqualTo(1), "#2 - ContextMenu should replace the default menu");

				textBox.ContextMenu = null;
				Assert.That(GetFieldEditorMenuItemCount(textBox), Is.EqualTo(defaultItems), "#3 - Should go back to the standard editing menu");
			});
		}

		static readonly IntPtr s_menuForEvent = Selector.GetHandle("menuForEvent:");

		/// <summary>
		/// Asks the native field editor which menu it would show for a right click.  This has to go through
		/// objc_msgSend since the managed NSView members dispatch to the superclass for managed subclasses, which
		/// would bypass the very override being tested here.
		/// </summary>
		static int GetFieldEditorMenuItemCount(TextBox textBox)
		{
			var editor = ((NSControl)textBox.ControlObject).CurrentEditor;
			Assert.That(editor, Is.Not.Null, "TextBox should have a field editor while focused");

			// the location doesn't matter, menuForEvent: just needs a real event
			var theEvent = NSEvent.MouseEvent(NSEventType.RightMouseDown, CGPoint.Empty, 0, 0, 0, null, 0, 1, 1);

			var menu = Eto.Mac.Messaging.IntPtr_objc_msgSend_IntPtr(editor.Handle, s_menuForEvent, theEvent.Handle);
			return menu == IntPtr.Zero ? 0 : (int)Runtime.GetNSObject<NSMenu>(menu).Count;
		}
	}
}
