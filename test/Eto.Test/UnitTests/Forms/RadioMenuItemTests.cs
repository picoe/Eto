using NUnit.Framework;
using Eto.Serialization.Json;
using Eto.Serialization.Xaml;
using System.Text;

namespace Eto.Test.UnitTests.Forms
{
	public class RadioMenuItemTests : TestBase
	{
		static void AssertLinked(RadioMenuItem first, RadioMenuItem second)
		{
			first.Checked = true;
			second.Checked = true;
			Assert.That(first.Checked, Is.False, $"{first.Text} should be unchecked by {second.Text}");
			Assert.That(second.Checked, Is.True, $"{second.Text} should be checked");
		}

		static void AssertNotLinked(RadioMenuItem first, RadioMenuItem second)
		{
			first.Checked = true;
			second.Checked = true;
			Assert.That(first.Checked, Is.True, $"{first.Text} should not be unchecked by {second.Text}");
			Assert.That(second.Checked, Is.True, $"{second.Text} should be checked");
		}

		// expects items in the order a1, b1, a2, b2
		static void AssertGroups(IList<RadioMenuItem> items)
		{
			Assert.That(items.Count, Is.EqualTo(4), "should have loaded four items");
			var a1 = items[0];
			var b1 = items[1];
			var a2 = items[2];
			var b2 = items[3];

			b1.Checked = true;
			AssertLinked(a1, a2);
			Assert.That(b1.Checked, Is.True, "other group should be unaffected");

			AssertLinked(b1, b2);
			Assert.That(a2.Checked, Is.True, "other group should be unaffected");
		}

		[Test, InvokeOnUI]
		public void CheckingItemFromCodeShouldUncheckController()
		{
			var a1 = new RadioMenuItem { Text = "a1" };
			var a2 = new RadioMenuItem(a1) { Text = "a2" };
			AssertLinked(a1, a2);
			AssertLinked(a2, a1);
		}

		[Test, InvokeOnUI]
		public void ControllerShouldLinkItems()
		{
			var a1 = new RadioMenuItem { Text = "a1" };
			var b1 = new RadioMenuItem { Text = "b1" };
			var a2 = new RadioMenuItem { Text = "a2", Controller = a1 };
			var b2 = new RadioMenuItem { Text = "b2", Controller = b1 };
			var menu = new ContextMenu { Items = { a1, b1, a2, b2 } };
			Assert.That(a2.Controller, Is.SameAs(a1));
			AssertGroups(new[] { a1, b1, a2, b2 });

			a2.Controller = null;
			AssertNotLinked(a1, a2);
		}

		[Test, InvokeOnUI]
		public void ControllerSetAfterAddingShouldLinkItems()
		{
			var a1 = new RadioMenuItem { Text = "a1" };
			var a2 = new RadioMenuItem { Text = "a2" };
			var menu = new ContextMenu { Items = { a1, a2 } };
			AssertNotLinked(a1, a2);

			a2.Controller = a1;
			AssertLinked(a1, a2);
		}

		[Test, InvokeOnUI]
		public void ControllerFromConstructorShouldBeReturned()
		{
			var a1 = new RadioMenuItem();
			Assert.That(new RadioMenuItem(a1).Controller, Is.SameAs(a1));
			Assert.That(a1.Controller, Is.Null);
		}

		[InvokeOnUI]
		[TestCase(false)]
		[TestCase(true)]
		public void ControllerShouldWorkFromXeto(bool forward)
		{
			// forward references the last item from the others
			var items = forward
				? @"<RadioMenuItem x:Name='a1' Text='a1' Controller='{x:Reference a2}'/>
					<RadioMenuItem x:Name='b1' Text='b1' Controller='{x:Reference b2}'/>
					<SeparatorMenuItem/>
					<RadioMenuItem x:Name='a2' Text='a2'/>
					<RadioMenuItem x:Name='b2' Text='b2'/>"
				: @"<RadioMenuItem x:Name='a1' Text='a1'/>
					<RadioMenuItem x:Name='b1' Text='b1'/>
					<SeparatorMenuItem/>
					<RadioMenuItem Text='a2' Controller='{x:Reference a1}'/>
					<RadioMenuItem Text='b2' Controller='{x:Reference b1}'/>";
			var xeto = $"<ContextMenu xmlns='{XamlReader.EtoFormsNamespace}' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'><SubMenuItem Text='Sub'>{items}</SubMenuItem></ContextMenu>";
			var menu = XamlReader.Load<ContextMenu>(new StringReader(xeto), null);
			var submenu = (SubMenuItem)menu.Items[0];
			AssertGroups(submenu.Items.OfType<RadioMenuItem>().ToList());
		}

		static string ControllerJson(bool forward, string reference)
		{
			string Item(string n, string r) => $"{{ $type: 'RadioMenuItem', Text: '{n}', $name: '{n}'{(r != null ? $", Controller: {string.Format(reference, r)}" : "")} }}";
			var items = forward
				? new[] { Item("a1", "a2"), Item("b1", "b2"), Item("a2", null), Item("b2", null) }
				: new[] { Item("a1", null), Item("b1", null), Item("a2", "a1"), Item("b2", "b1") };
			return $"{{ $type: 'ContextMenu', Items: [ {string.Join(", ", items)} ] }}";
		}

		[InvokeOnUI]
		[TestCase(false, "'{{x:Reference {0}}}'")]
		[TestCase(true, "'{{x:Reference {0}}}'")]
		[TestCase(false, "{{ $ref: '{0}' }}")]
		[TestCase(true, "{{ $ref: '{0}' }}")]
		public void ControllerShouldWorkFromJeto(bool forward, string reference)
		{
			var menu = JetoReader.Load<ContextMenu>(new StringReader(ControllerJson(forward, reference)), null);
			AssertGroups(menu.Items.OfType<RadioMenuItem>().ToList());
		}

		[InvokeOnUI]
		[TestCase(false, "'{{x:Reference {0}}}'")]
		[TestCase(true, "'{{x:Reference {0}}}'")]
		[TestCase(false, "{{ $ref: '{0}' }}")]
		[TestCase(true, "{{ $ref: '{0}' }}")]
		public void ControllerShouldWorkFromJson(bool forward, string reference)
		{
			var menu = JsonReader.Load<ContextMenu>(new MemoryStream(Encoding.UTF8.GetBytes(ControllerJson(forward, reference))));
			AssertGroups(menu.Items.OfType<RadioMenuItem>().ToList());
		}
	}
}
