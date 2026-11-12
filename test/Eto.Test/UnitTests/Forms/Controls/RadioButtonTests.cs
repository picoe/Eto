using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Internal;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class RadioButtonTests : TestBase
	{
		[ManualTest]
		[TestCase(true)]
		[TestCase(false)]
		public void RadioButtonGroupsShouldOperateTogether(bool useSeparateContainers)
		{
			bool? success = null;
			ManualForm("Clicking on RadioButton 2 should uncheck RadioButton 1", form =>
			{
				var rb1 = new RadioButton { Text = "RadioButton 1", Checked = true };
				rb1.CheckedChanged += (sender, e) =>
				{
					Log.Write(rb1, $"{rb1.Text}: {rb1.Checked}");
				};

				var rb2 = new RadioButton(rb1) { Text = "RadioButton 2" };
				rb2.CheckedChanged += (sender, e) =>
				{
					Log.Write(rb2, $"{rb2.Text}: {rb2.Checked}");
					if (success == null)
						success = rb2.Checked == true && rb1.Checked == false;
				};

				if (useSeparateContainers)
				{
					return new TableLayout
					{
						Rows =
						{
							new TableLayout { Rows = { rb1 } },
							new TableLayout { Rows = { rb2 } }
						}
					};
				}
				else
				{
					return new TableLayout { Rows = { rb1, rb2 } };
				}
			});

			Assert.That(success, Is.True, "Checked values are incorrect");
		}

		[Test, ManualTest]
		public void RadioButtonsInDifferentContainersShouldNotAffectOtherGroups()
		{
			ManualForm("Clicking on RadioButton 1.* should not affect RadioButton 2.* and vise versa", form =>
			{
				var rb1_1 = new RadioButton { Text = "RadioButton 1.1", Checked = true };
				rb1_1.CheckedChanged += (sender, e) => Log.Write(rb1_1, $"{rb1_1.Text}: {rb1_1.Checked}");

				var rb1_2 = new RadioButton(rb1_1) { Text = "RadioButton 1.2" };
				rb1_2.CheckedChanged += (sender, e) => Log.Write(rb1_2, $"{rb1_2.Text}: {rb1_2.Checked}");

				var rb2_1 = new RadioButton { Text = "RadioButton 2.1", Checked = true };
				rb2_1.CheckedChanged += (sender, e) => Log.Write(rb2_1, $"{rb2_1.Text}: {rb2_1.Checked}");

				var rb2_2 = new RadioButton(rb2_1) { Text = "RadioButton 2.2" };
				rb2_2.CheckedChanged += (sender, e) => Log.Write(rb2_2, $"{rb2_2.Text}: {rb2_2.Checked}");

				return new TableLayout
				{
					Rows = {
						new TableLayout { Rows = { new TableRow(rb1_1, rb2_1) } },
						new TableLayout { Rows = { new TableRow(rb1_2, rb2_2) } }
					}
				};
			});

		}

		[TestCase(true)]
		[TestCase(false)]
		public void RadioButtonGroupsShouldWorkProgrammatically(bool useSeparateContainers)
		{
			RadioButton rb1 = null, rb2 = null, rb3 = null;
			int rb1changed = 0, rb2changed = 0, rb3changed = 0;
			Shown(form =>
			{
				rb1 = new RadioButton();
				rb1.CheckedChanged += (sender, e) => rb1changed++;

				rb2 = new RadioButton(rb1);
				rb2.CheckedChanged += (sender, e) => rb2changed++;

				rb3 = new RadioButton(rb1);
				rb3.CheckedChanged += (sender, e) => rb3changed++;

				if (useSeparateContainers)
				{
					form.Content = new TableLayout
					{
						Rows =
						{
							new TextBox(), // on winforms, when a radio button gets focus it also gets checked
							new TableLayout { Rows = { rb1 } },
							new TableLayout { Rows = { rb2 } },
							new TableLayout { Rows = { rb3 } }
						}
					};
				}
				else
				{
					form.Content = new TableLayout
					{
						Rows =
						{
							new TextBox(), // on winforms, when a radio button gets focus it also gets checked
							rb1, rb2, rb3
						}
					};
				}
			}, () =>
			{
				// none checked is valid
				Assert.That(rb1.Checked, Is.False, "#1.1");
				Assert.That(rb2.Checked, Is.False, "#1.2");
				Assert.That(rb3.Checked, Is.False, "#1.3");
				Assert.That(rb1changed, Is.EqualTo(0), "#1.4");
				Assert.That(rb2changed, Is.EqualTo(0), "#1.5");
				Assert.That(rb3changed, Is.EqualTo(0), "#1.6");

				rb2.Checked = true;

				Assert.That(rb1.Checked, Is.False, "#2.1");
				Assert.That(rb2.Checked, Is.True, "#2.2");
				Assert.That(rb3.Checked, Is.False, "#2.3");
				Assert.That(rb1changed, Is.EqualTo(0), "#2.4");
				Assert.That(rb2changed, Is.EqualTo(1), "#2.5");
				Assert.That(rb3changed, Is.EqualTo(0), "#2.6");

				rb3.Checked = true;

				Assert.That(rb1.Checked, Is.False, "#3.1");
				Assert.That(rb2.Checked, Is.False, "#3.2");
				Assert.That(rb3.Checked, Is.True, "#3.3");
				Assert.That(rb1changed, Is.EqualTo(0), "#3.4");
				Assert.That(rb2changed, Is.EqualTo(2), "#3.5");
				Assert.That(rb3changed, Is.EqualTo(1), "#3.6");
			});
		}

		static void AssertGroups(RadioButton a1, RadioButton a2, RadioButton b1, RadioButton b2)
		{
			a1.Checked = true;
			b1.Checked = true;
			a2.Checked = true;
			Assert.That(a1.Checked, Is.False, "#1.1 - a1 should be unchecked by a2");
			Assert.That(a2.Checked, Is.True, "#1.2");
			Assert.That(b1.Checked, Is.True, "#1.3 - other group should be unaffected");

			b2.Checked = true;
			Assert.That(b1.Checked, Is.False, "#2.1 - b1 should be unchecked by b2");
			Assert.That(a2.Checked, Is.True, "#2.2 - other group should be unaffected");
		}

		[Test]
		public void ControllerShouldKeepOnlyOneChecked()
		{
			RadioButton rb1 = null, rb2 = null;
			int rb1changed = 0, rb2changed = 0;
			Shown(form =>
			{
				rb1 = new RadioButton { Checked = true };
				rb2 = new RadioButton { Checked = true };
				rb1.CheckedChanged += (sender, e) => rb1changed++;
				rb2.CheckedChanged += (sender, e) => rb2changed++;
				return new TableLayout(new TextBox(), rb1, rb2);
			}, _ =>
			{
				rb2.Controller = rb1;
				Assert.That(rb2.Checked, Is.True, "#1 - the joining button should keep its check");
				Assert.That(rb1.Checked, Is.False, "#2");
				Assert.That(rb2changed, Is.EqualTo(0), "#3 - rb2 never changed");
				Assert.That(rb1changed, Is.EqualTo(1), "#4");
			});
		}

		[Test]
		public void ControllerShouldLinkRadioButtons()
		{
			Invoke(() =>
			{
				var a1 = new RadioButton();
				var b1 = new RadioButton();
				var a2 = new RadioButton { Controller = a1 };
				var b2 = new RadioButton { Controller = b1 };
				Assert.That(a2.Controller, Is.SameAs(a1), "#1");
				AssertGroups(a1, a2, b1, b2);

				a2.Controller = null;
				a1.Checked = true;
				Assert.That(a2.Checked, Is.True, "#2 - unlinked button should keep its check");
			});
		}

		[Test]
		public void ControllerFromConstructorShouldBeReturned()
		{
			Invoke(() =>
			{
				var a1 = new RadioButton();
				Assert.That(new RadioButton(a1).Controller, Is.SameAs(a1));
				Assert.That(a1.Controller, Is.Null);
			});
		}

		// expects buttons in the order a1, b1, a2, b2, with the first of each group named a1 and b1
		static void AssertControllerGroups(Container content)
		{
			var buttons = content.Children.OfType<RadioButton>().ToList();
			Assert.That(buttons.Count, Is.EqualTo(4), "should have loaded four buttons");
			AssertGroups(buttons[0], buttons[2], buttons[1], buttons[3]);
		}

		[TestCase(false)]
		[TestCase(true)]
		public void ControllerShouldWorkFromXeto(bool forward)
		{
			Shown(form =>
			{
				// forward references the last button from the others
				var xeto = forward
					? @"<RadioButton x:Name='a1' Controller='{x:Reference a2}'/>
						<RadioButton x:Name='b1' Controller='{x:Reference b2}'/>
						<RadioButton x:Name='a2'/>
						<RadioButton x:Name='b2'/>"
					: @"<RadioButton x:Name='a1'/>
						<RadioButton x:Name='b1'/>
						<RadioButton Controller='{x:Reference a1}'/>
						<RadioButton Controller='{x:Reference b1}'/>";
				xeto = $"<StackLayout xmlns='{Eto.Serialization.Xaml.XamlReader.EtoFormsNamespace}' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>{xeto}</StackLayout>";
				return Eto.Serialization.Xaml.XamlReader.Load<StackLayout>(new StringReader(xeto), null);
			}, AssertControllerGroups);
		}

		static string ControllerJson(bool forward, string name, string reference)
		{
			string Button(string n, string r) => $"{{ $type: 'RadioButton'{(n != null ? $", {name}: '{n}'" : "")}{(r != null ? $", Controller: {string.Format(reference, r)}" : "")} }}";
			var items = forward
				? new[] { Button("a1", "a2"), Button("b1", "b2"), Button("a2", null), Button("b2", null) }
				: new[] { Button("a1", null), Button("b1", null), Button(null, "a1"), Button(null, "b1") };
			return $"{{ $type: 'StackLayout', Items: [ {string.Join(", ", items)} ] }}";
		}

		[TestCase(false, "'{{x:Reference {0}}}'")]
		[TestCase(true, "'{{x:Reference {0}}}'")]
		[TestCase(false, "{{ $ref: '{0}' }}")]
		[TestCase(true, "{{ $ref: '{0}' }}")]
		public void ControllerShouldWorkFromJeto(bool forward, string reference)
		{
			Shown(form => Eto.Serialization.Xaml.JetoReader.Load<StackLayout>(new StringReader(ControllerJson(forward, "$name", reference)), null), AssertControllerGroups);
		}

		[TestCase(false, "$name", "'{{x:Reference {0}}}'")]
		[TestCase(true, "$name", "'{{x:Reference {0}}}'")]
		[TestCase(false, "$name", "{{ $ref: '{0}' }}")]
		[TestCase(true, "$name", "{{ $ref: '{0}' }}")]
		[TestCase(false, "$id", "{{ $ref: '{0}' }}")]
		[TestCase(true, "$id", "{{ $ref: '{0}' }}")]
		[TestCase(false, "ID", "{{ $ref: '{0}' }}")]
		public void ControllerShouldWorkFromJson(bool forward, string name, string reference)
		{
			Shown(form => Eto.Serialization.Json.JsonReader.Load<StackLayout>(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(ControllerJson(forward, name, reference)))), AssertControllerGroups);
		}
	}
}
