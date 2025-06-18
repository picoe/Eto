using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class ContainerTests : TestBase
	{
		[TestCaseSource(nameof(GetPanelTypes)), ManualTest]
		public void PanelPaddingShouldWork(IContainerTypeInfo<Panel> type)
		{
			ManualForm(
				"There should be 40px padding around the blue rectangle",
				form =>
				{
					var panel = type.CreateControl();
					Assert.That(panel, Is.Not.Null);

					panel.Padding = 40;
					panel.Content = new Panel
					{
						BackgroundColor = Colors.Blue,
						Size = new Size(200, 200)
					};
					return type.CreateContainer(panel);
				});
		}

		[TestCaseSource(nameof(GetPanelTypes)), ManualTest]
		public void PanelPaddingBottomRightShouldWork(IContainerTypeInfo<Panel> type)
		{
			ManualForm(
				"There should be 40px padding at the bottom and right of the blue rectangle",
				form =>
				{
					var panel = type.CreateControl();
					Assert.That(panel, Is.Not.Null);

					panel.Padding = new Padding(0, 0, 40, 40);
					panel.Content = new Panel
					{
						BackgroundColor = Colors.Blue,
						Size = new Size(200, 200)
					};
					return type.CreateContainer(panel);
				});
		}

		public static IEnumerable<object[]> GetContainersAndControls()
		{
			foreach (var container in GetContainerTypes())
			{
				foreach (var control in GetControlTypes())
				{
					yield return new object[] { container, control };
				}
			}
		}

		[TestCaseSource(nameof(GetContainersAndControls))]
		public void EnabledShouldAffectChildControls(IContainerTypeInfo<Container> containerType, IControlTypeInfo<Control> controlType)
		{
			Invoke(() =>
			{
				var enabledChild = controlType.CreateControl();
				var disabledChild = controlType.CreateControl();
				var neutralChild = controlType.CreateControl();
				var childPanel = new TableLayout
				{
					Rows = { enabledChild, disabledChild, neutralChild }
				};

				var container = containerType.CreateControl(childPanel);

				// load the control
				container.AttachNative();

				var neutralEnabled = neutralChild.Enabled;
				// disabled child should always be disabled
				disabledChild.Enabled = false;
				enabledChild.Enabled = true;

				// default values
				Assert.That(container.Enabled, Is.True, "#1.1");
				Assert.That(enabledChild.Enabled, Is.True, "#1.2");
				Assert.That(disabledChild.Enabled, Is.False, "#1.3");
				Assert.That(neutralChild.Enabled, Is.EqualTo(neutralEnabled), "#1.4");

				// setting container to disabled
				container.Enabled = false;

				Assert.That(container.Enabled, Is.False, "#2.1");
				Assert.That(enabledChild.Enabled, Is.False, "#2.2");
				Assert.That(disabledChild.Enabled, Is.False, "#2.3");
				Assert.That(neutralChild.Enabled, Is.False, "#2.4");

				// set child to enabled when parent is disabled, should still stay disabled
				enabledChild.Enabled = true;

				Assert.That(container.Enabled, Is.False, "#3.1");
				Assert.That(enabledChild.Enabled, Is.False, "#3.2");
				Assert.That(disabledChild.Enabled, Is.False, "#3.3");
				Assert.That(neutralChild.Enabled, Is.False, "#3.4");

				// set container back to enabled
				container.Enabled = true;

				Assert.That(container.Enabled, Is.True, "#4.1");
				Assert.That(enabledChild.Enabled, Is.True, "#4.2");
				Assert.That(disabledChild.Enabled, Is.False, "#4.3");
				Assert.That(neutralChild.Enabled, Is.EqualTo(neutralEnabled), "#4.4");
			});
		}

		[TestCaseSource(nameof(GetContainersAndControls))]
		public void EnabledShouldAffectChildControlsWhenDynamicallyAdded(IContainerTypeInfo<Container> containerType, IControlTypeInfo<Control> controlType)
		{
			Invoke(() =>
			{
				var enabledChild = controlType.CreateControl();
				var disabledChild = controlType.CreateControl();
				var neutralChild = controlType.CreateControl();
				var childPanel = new Panel();

				void addControls() => childPanel.Content = new TableLayout
				{
					Rows = { enabledChild, disabledChild, neutralChild }
				};

				void removeControls() => childPanel.Content = null;

				var container = containerType.CreateControl(childPanel);

				// load the control (for virtual containers like Stack/Dynamic layouts)
				container.AttachNative();

				var neutralEnabled = neutralChild.Enabled;
				// disabled child should always be disabled
				disabledChild.Enabled = false;
				enabledChild.Enabled = true;

				// default values
				Assert.That(container.Enabled, Is.True, "#1.1");
				Assert.That(enabledChild.Enabled, Is.True, "#1.2");
				Assert.That(disabledChild.Enabled, Is.False, "#1.3");
				Assert.That(neutralChild.Enabled, Is.EqualTo(neutralEnabled), "#1.4");

				addControls();

				// default values after added to the container
				Assert.That(container.Enabled, Is.True, "#2.1");
				Assert.That(enabledChild.Enabled, Is.True, "#2.2");
				Assert.That(disabledChild.Enabled, Is.False, "#2.3");
				Assert.That(neutralChild.Enabled, Is.EqualTo(neutralEnabled), "#2.4");

				removeControls();

				// setting container to disabled
				container.Enabled = false;

				// default values after removed from the container (and container set to disabled)
				Assert.That(enabledChild.Enabled, Is.True, "#3.1");
				Assert.That(disabledChild.Enabled, Is.False, "#3.2");
				Assert.That(neutralChild.Enabled, Is.EqualTo(neutralEnabled), "#3.3");

				addControls();
				// values after adding back to the container
				Assert.That(container.Enabled, Is.False, "#4.1");
				Assert.That(enabledChild.Enabled, Is.False, "#4.2");
				Assert.That(disabledChild.Enabled, Is.False, "#4.3");
				Assert.That(neutralChild.Enabled, Is.False, "#4.4");

				// set child to enabled when parent is disabled, should still stay disabled
				enabledChild.Enabled = true;

				Assert.That(container.Enabled, Is.False, "#5.1");
				Assert.That(enabledChild.Enabled, Is.False, "#5.2");
				Assert.That(disabledChild.Enabled, Is.False, "#5.3");
				Assert.That(neutralChild.Enabled, Is.False, "#5.4");

				removeControls();
				// default values after removed from the container (again)
				Assert.That(enabledChild.Enabled, Is.True, "#6.1");
				Assert.That(disabledChild.Enabled, Is.False, "#6.2");
				Assert.That(neutralChild.Enabled, Is.EqualTo(neutralEnabled), "#6.3");

				// set container back to enabled
				container.Enabled = true;

				addControls();

				Assert.That(container.Enabled, Is.True, "#7.1");
				Assert.That(enabledChild.Enabled, Is.True, "#7.2");
				Assert.That(disabledChild.Enabled, Is.False, "#7.3");
				Assert.That(neutralChild.Enabled, Is.EqualTo(neutralEnabled), "#7.4");
			});
		}

		[TestCaseSource(nameof(GetControlTypes))]
		public void EnabledShouldTriggerChangedEventsOnChildren(IControlTypeInfo<Control> controlInfo)
		{
			Invoke(() =>
			{
				int enabledChanged = 0;
				var panel = new Panel();
				var control = controlInfo.CreateControl();
				control.EnabledChanged += (sender, e) => enabledChanged++;
				// if it's already enabled, it shouldn't fire a changed event when we set it.
				var expectedCount = control.Enabled ? 0 : 1;
				control.Enabled = true;
				Assert.That(control.Enabled, Is.True, "#1.1");
				Assert.That(enabledChanged, Is.EqualTo(expectedCount), "#1.2");

				// test setting to false without container, should trigger event
				control.Enabled = false;
				Assert.That(control.Enabled, Is.False, "#1.3");
				Assert.That(enabledChanged, Is.EqualTo(++expectedCount), "#1.4");

				// set back to true, should trigger event
				control.Enabled = true;
				Assert.That(control.Enabled, Is.True, "#1.5");
				Assert.That(enabledChanged, Is.EqualTo(++expectedCount), "#1.6");

				panel.Content = new TableLayout
				{
					Rows = { control }
				};

				panel.AttachNative();

				// set panel to enabled (which it should already be at), so no change event
				panel.Enabled = true;
				Assert.That(control.Enabled, Is.True, "#2.1");
				Assert.That(enabledChanged, Is.EqualTo(expectedCount), "#2.2"); // shouldn't have changed

				// change panel to disabled, which should now trigger the event
				panel.Enabled = false;
				Assert.That(control.Enabled, Is.False, "#3.1");
				Assert.That(enabledChanged, Is.EqualTo(++expectedCount), "#3.2");

				// set control to enabled, which should still stay false and not trigger the event
				control.Enabled = true;
				Assert.That(control.Enabled, Is.False, "#4.1");
				Assert.That(enabledChanged, Is.EqualTo(expectedCount), "#4.2");

				// set to same value again, should not fire changed event
				panel.Enabled = false;
				Assert.That(control.Enabled, Is.False, "#5.1");
				Assert.That(enabledChanged, Is.EqualTo(expectedCount), "#5.2");

				// remove from parent, should trigger changed event
				panel.Content = null;
				Assert.That(control.Enabled, Is.True, "#6.1");
				Assert.That(enabledChanged, Is.EqualTo(++expectedCount), "#6.2");

			});
		}

		enum TestEnum
		{
			Entry1,
			Entry2,
			Entry3
		}

		[TestCase(true)]
		[TestCase(false)]
		[ManualTest]
		public void EnabledShouldBeToggleable(bool initiallyEnabled)
		{
			int incorrectTableLayoutEnabledState = 0;
			int changedCount = 0;
			bool initialStateMatchesInitiallyEnabled = false;
			ManualForm("You should be able to toggle the radio buttons between enabled and disabled.\nChange the check box twice and verify the result.",
				form =>
				{
					var label = new Label { TextAlignment = TextAlignment.Left };
					var radio = new EnumRadioButtonList<TestEnum> { Orientation = Orientation.Vertical };
					var check = new CheckBox { Text = "Enable radio buttons", Checked = initiallyEnabled };
					radio.EnabledChanged += (sender, e) => label.Text = $"EnabledChanged->radio.Enabled({radio.Enabled})";
					radio.Enabled = initiallyEnabled;
					Assert.That(radio.Enabled, Is.EqualTo(initiallyEnabled), "#1.1");
					check.CheckedChanged += (sender, e) =>
					{
						var isChecked = check.Checked == true;
						radio.Enabled = isChecked;
						changedCount++;
						// check visual child enabled state
						var tableLayout = radio.VisualChildren.OfType<TableLayout>().FirstOrDefault();
						if (tableLayout?.Enabled != isChecked)
							incorrectTableLayoutEnabledState++;
					};

					radio.LoadComplete += (sender, e) =>
					{
						var theButton = radio.VisualChildren.OfType<RadioButton>().FirstOrDefault();
						if (theButton?.Enabled == initiallyEnabled)
							initialStateMatchesInitiallyEnabled = true;
					};

					form.ClientSize = new Size(450, -1);
					form.Resizable = true;
					return new TableLayout { Rows = { check, radio, label } };
				});

			Assert.That(incorrectTableLayoutEnabledState, Is.EqualTo(0), "#2.1 - internal TableLayout did not have the correct enabled state");
			Assert.That(changedCount, Is.GreaterThanOrEqualTo(2), "#2.2 - The check box was not toggled at least twice");
			Assert.That(initialStateMatchesInitiallyEnabled, Is.True, "#2.3 - initial state of radio button did not match");
		}


		[Test]
		public void ChildrenShouldAllowRemingAndAddingToAnotherContainer() => ShownAsync(form =>
		{
			var content = new Panel { Size = new Size(200, 200) };
			form.Content = content;
			return content;			
		}, async panel =>
		{
			var container = new Panel();
			var control = new TextBox(); // { BackgroundColor = Colors.Blue, Size = new Size(200, 200) };
			container.Content = control;
			panel.Content = container;

			await Task.Delay(1000);

			Assert.That(container.Content, Is.EqualTo(control), "#1.1 - Content should be set correctly");
			Assert.That(control.Parent, Is.EqualTo(container), "#1.2 - Child's parent should be the container");

			var container2 = new TableLayout();
			container2.Rows.Add(new TableRow(new TableCell(control)));
			panel.Content = container2;

			await Task.Delay(1000);

			Assert.That(container.Content, Is.Null, "#2.1 - Content should be removed");
			Assert.That(container2.Rows[0].Cells[0].Control, Is.EqualTo(control), "#2.2 - Content on the second container should be set correctly");
			Assert.That(control.Parent, Is.EqualTo(container2), "#2.3 - Child's parent should be the second container");

			var container3 = new GroupBox();
			container3.Content = control;
			panel.Content = container3;

			await Task.Delay(1000);

			Assert.That(container2.Rows[0].Cells[0].Control, Is.Null, "#3.1 - Content on the second container should be set correctly");
			Assert.That(container3.Content, Is.EqualTo(control), "#3.2 - Content should be set correctly");
			Assert.That(control.Parent, Is.EqualTo(container3), "#3.3 - Child's parent should be the second container");

			container3.Content = null;

			await Task.Delay(1000);
			
			Assert.That(control.Parent, Is.Null, "#4.2 - Control should not have a parent");
		}, timeout: 10000);

	}
}
