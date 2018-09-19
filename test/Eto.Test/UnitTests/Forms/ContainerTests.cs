using System;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;
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
					Assert.IsNotNull(panel);

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
					Assert.IsNotNull(panel);

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
				Assert.IsTrue(container.Enabled, "#1.1");
				Assert.IsTrue(enabledChild.Enabled, "#1.2");
				Assert.IsFalse(disabledChild.Enabled, "#1.3");
				Assert.AreEqual(neutralEnabled, neutralChild.Enabled, "#1.4");

				// setting container to disabled
				container.Enabled = false;

				Assert.IsFalse(container.Enabled, "#2.1");
				Assert.IsFalse(enabledChild.Enabled, "#2.2");
				Assert.IsFalse(disabledChild.Enabled, "#2.3");
				Assert.IsFalse(neutralChild.Enabled, "#2.4");

				// set child to enabled when parent is disabled, should still stay disabled
				enabledChild.Enabled = true;

				Assert.IsFalse(container.Enabled, "#3.1");
				Assert.IsFalse(enabledChild.Enabled, "#3.2");
				Assert.IsFalse(disabledChild.Enabled, "#3.3");
				Assert.IsFalse(neutralChild.Enabled, "#3.4");

				// set container back to enabled
				container.Enabled = true;

				Assert.IsTrue(container.Enabled, "#4.1");
				Assert.IsTrue(enabledChild.Enabled, "#4.2");
				Assert.IsFalse(disabledChild.Enabled, "#4.3");
				Assert.AreEqual(neutralEnabled, neutralChild.Enabled, "#4.4");
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
				Assert.IsTrue(container.Enabled, "#1.1");
				Assert.IsTrue(enabledChild.Enabled, "#1.2");
				Assert.IsFalse(disabledChild.Enabled, "#1.3");
				Assert.AreEqual(neutralEnabled, neutralChild.Enabled, "#1.4");

				addControls();

				// default values after added to the container
				Assert.IsTrue(container.Enabled, "#2.1");
				Assert.IsTrue(enabledChild.Enabled, "#2.2");
				Assert.IsFalse(disabledChild.Enabled, "#2.3");
				Assert.AreEqual(neutralEnabled, neutralChild.Enabled, "#2.4");

				removeControls();

				// setting container to disabled
				container.Enabled = false;

				// default values after removed from the container (and container set to disabled)
				Assert.IsTrue(enabledChild.Enabled, "#3.1");
				Assert.IsFalse(disabledChild.Enabled, "#3.2");
				Assert.AreEqual(neutralEnabled, neutralChild.Enabled, "#3.3");

				addControls();
				// values after adding back to the container
				Assert.IsFalse(container.Enabled, "#4.1");
				Assert.IsFalse(enabledChild.Enabled, "#4.2");
				Assert.IsFalse(disabledChild.Enabled, "#4.3");
				Assert.IsFalse(neutralChild.Enabled, "#4.4");

				// set child to enabled when parent is disabled, should still stay disabled
				enabledChild.Enabled = true;

				Assert.IsFalse(container.Enabled, "#5.1");
				Assert.IsFalse(enabledChild.Enabled, "#5.2");
				Assert.IsFalse(disabledChild.Enabled, "#5.3");
				Assert.IsFalse(neutralChild.Enabled, "#5.4");

				removeControls();
				// default values after removed from the container (again)
				Assert.IsTrue(enabledChild.Enabled, "#6.1");
				Assert.IsFalse(disabledChild.Enabled, "#6.2");
				Assert.AreEqual(neutralEnabled, neutralChild.Enabled, "#6.3");

				// set container back to enabled
				container.Enabled = true;

				addControls();

				Assert.IsTrue(container.Enabled, "#7.1");
				Assert.IsTrue(enabledChild.Enabled, "#7.2");
				Assert.IsFalse(disabledChild.Enabled, "#7.3");
				Assert.AreEqual(neutralEnabled, neutralChild.Enabled, "#7.4");
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
				Assert.IsTrue(control.Enabled, "#1.1");
				Assert.AreEqual(expectedCount, enabledChanged, "#1.2");

				// test setting to false without container, should trigger event
				control.Enabled = false;
				Assert.IsFalse(control.Enabled, "#1.3");
				Assert.AreEqual(++expectedCount, enabledChanged, "#1.4");

				// set back to true, should trigger event
				control.Enabled = true;
				Assert.IsTrue(control.Enabled, "#1.5");
				Assert.AreEqual(++expectedCount, enabledChanged, "#1.6");

				panel.Content = new TableLayout
				{
					Rows = { control }
				};

				panel.AttachNative();

				// set panel to enabled (which it should already be at), so no change event
				panel.Enabled = true;
				Assert.IsTrue(control.Enabled, "#2.1");
				Assert.AreEqual(expectedCount, enabledChanged, "#2.2"); // shouldn't have changed

				// change panel to disabled, which should now trigger the event
				panel.Enabled = false;
				Assert.IsFalse(control.Enabled, "#3.1");
				Assert.AreEqual(++expectedCount, enabledChanged, "#3.2");

				// set control to enabled, which should still stay false and not trigger the event
				control.Enabled = true;
				Assert.IsFalse(control.Enabled, "#4.1");
				Assert.AreEqual(expectedCount, enabledChanged, "#4.2");

				// set to same value again, should not fire changed event
				panel.Enabled = false;
				Assert.IsFalse(control.Enabled, "#5.1");
				Assert.AreEqual(expectedCount, enabledChanged, "#5.2");

				// remove from parent, should trigger changed event
				panel.Content = null;
				Assert.IsTrue(control.Enabled, "#6.1");
				Assert.AreEqual(++expectedCount, enabledChanged, "#6.2");

			});
		}

	}
}
