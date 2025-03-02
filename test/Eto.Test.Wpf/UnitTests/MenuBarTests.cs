using Eto.Test.UnitTests;
using Eto.Wpf.Forms.Menu;
using NUnit.Framework;
using swi = System.Windows.Input;

namespace Eto.Test.Wpf.UnitTests
{
	[TestFixture]
	public class MenuBarTests : TestBase
	{
		[Test]
		public void MenuBarShouldSetInputBindingsForChildren()
		{
			Invoke(() =>
			{
				var form = new Form();
				form.Menu = new MenuBar();

				var file = new SubMenuItem { Text = "File" };

				var command = new Command { MenuText = "Click Me!" };
				command.Shortcut = Keys.Control | Keys.N;
				command.Executed += (sender, e) => MessageBox.Show("Woo!");
				file.Items.Add(command);

				// add the item (with child items) to the menu that is already set to the form
				form.Menu.Items.Add(file);

				// check to make sure the input binding for the command made it
				var host = form.Handler as IInputBindingHost;
				Assert.That(host.InputBindings.Count, Is.EqualTo(1));
				Assert.That(host.InputBindings[0], Is.InstanceOf<swi.InputBinding>());
				Assert.That(host.InputBindings[0].Gesture, Is.InstanceOf<EtoKeyGesture>());
				var kb = (EtoKeyGesture)host.InputBindings[0].Gesture;
				Assert.That(kb.Key, Is.EqualTo(swi.Key.N));
				Assert.That(kb.Modifiers, Is.EqualTo(swi.ModifierKeys.Control));
			});
		}
	}
}
