using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Mac.UnitTests
{
	[TestFixture]
    public class DrawableTests : TestBase
    {
		[ManualTest, Test]
		public void MappingPlatformCommandShouldNotCrash() => ManualForm("Click the Edit > Cut menu item, it should not crash", form =>
		{
			form.Menu = new MenuBar();

			var drawable = new Drawable();
			drawable.Content = "I should have focus!";
			drawable.Size = new Size(100, 100);
			drawable.MapPlatformCommand("cut", new Command((sender, e) => MessageBox.Show("You clicked me! woo!")));
			drawable.MapPlatformCommand("copy", null);

			drawable.BackgroundColor = Colors.Green;
			drawable.CanFocus = true;
			drawable.Focus();

			return drawable;

		});

		/// <summary>
		/// The NSTextInputClient protocol has to be added to the native class of the control, not the instance, so
		/// hooking up TextInput on one Drawable used to make every Drawable in the app look like a text input to the
		/// system - which adds items such as AutoFill to their context menus.
		/// </summary>
		[Test]
		public void TextInputShouldOnlyApplyToTheControlThatHandlesIt()
		{
			Drawable withTextInput = null;
			Drawable withoutTextInput = null;
			Shown(form =>
			{
				withTextInput = new Drawable { Size = new Size(50, 50), CanFocus = true };
				withTextInput.TextInput += (sender, e) => { };
				withoutTextInput = new Drawable { Size = new Size(50, 50), CanFocus = true };

				form.Content = new StackLayout { Items = { withTextInput, withoutTextInput } };
			},
			() =>
			{
				// created after the protocol was already added to the class
				var createdAfter = new Drawable { Size = new Size(50, 50), CanFocus = true };

				Assert.That(IsTextInputClient(withTextInput), Is.True, "#1 - Drawable handling TextInput should be a text input client");
				Assert.That(IsTextInputClient(withoutTextInput), Is.False, "#2 - Drawable not handling TextInput should not be a text input client");
				Assert.That(IsTextInputClient(createdAfter), Is.False, "#3 - Drawable created afterwards should not be a text input client");
			});
		}

		static readonly IntPtr s_textInputClientProtocol = Eto.Mac.ObjCExtensions.GetProtocolHandle("NSTextInputClient");
		static readonly IntPtr s_conformsToProtocol = Selector.GetHandle("conformsToProtocol:");
		static readonly IntPtr s_inputContext = Selector.GetHandle("inputContext");

		/// <summary>
		/// Asks the native view whether the system considers it a text input client.  This has to go through
		/// objc_msgSend since the managed NSView members dispatch to the superclass for managed subclasses, which
		/// would bypass the very overrides being tested here.
		/// </summary>
		static bool IsTextInputClient(Control control)
		{
			var handle = ((NSView)control.ControlObject).Handle;
			var conforms = Eto.Mac.Messaging.bool_objc_msgSend_IntPtr(handle, s_conformsToProtocol, s_textInputClientProtocol);
			var inputContext = Eto.Mac.Messaging.IntPtr_objc_msgSend(handle, s_inputContext);
			Assert.That(conforms, Is.EqualTo(inputContext != IntPtr.Zero), "conformsToProtocol: and inputContext should agree");
			return conforms;
		}
	}
}