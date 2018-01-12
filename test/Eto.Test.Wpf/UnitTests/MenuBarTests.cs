using Eto.Forms;
using Eto.Test.UnitTests;
using Eto.Wpf.Forms.Menu;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

				var file = new ButtonMenuItem { Text = "File" };

				var command = new Command { MenuText = "Click Me!" };
				command.Shortcut = Keys.Control | Keys.N;
				command.Executed += (sender, e) => MessageBox.Show("Woo!");
				file.Items.Add(command);

				// add the item (with child items) to the menu that is already set to the form
				form.Menu.Items.Add(file);

				// check to make sure the input binding for the command made it
				var host = form.Handler as IInputBindingHost;
				Assert.AreEqual(1, host.InputBindings.Count);
				Assert.IsInstanceOf<swi.KeyBinding>(host.InputBindings[0]);
				var kb = (swi.KeyBinding)host.InputBindings[0];
				Assert.AreEqual(swi.Key.N, kb.Key);
				Assert.AreEqual(swi.ModifierKeys.Control, kb.Modifiers);
			});
		}
	}
}
