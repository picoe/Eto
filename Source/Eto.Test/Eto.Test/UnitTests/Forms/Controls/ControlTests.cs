using System;
using Eto.Forms;
using NUnit.Framework;
using System.Collections.Generic;
using Eto.Drawing;
using System.Threading;
using System.Runtime.ExceptionServices;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class ControlTests : TestBase
	{
		[TestCaseSource(nameof(GetControlTypes))]
		public void DefaultValuesShouldBeCorrect(Type controlType)
		{
			TestProperties(f => (Control)Activator.CreateInstance(controlType),
						   c => c.Enabled,
						   c => c.ToolTip,
						   c => c.TabIndex
			);
		}

		[TestCaseSource(nameof(GetControlTypes))]
		public void ControlShouldFireShownEvent(Type controlType)
		{
			int shownCount = 0;
			Form(form =>
			{
				var ctl = (Control)Activator.CreateInstance(controlType);
				ctl.Shown += (sender, e) =>
				{
					shownCount++;
					Application.Instance.AsyncInvoke(() =>
					{
						if (form.Loaded)
							form.Close();
					});
				};
				form.Content = TableLayout.AutoSized(ctl);
				Assert.AreEqual(0, shownCount);
			});
			Assert.AreEqual(1, shownCount);
		}

		[TestCaseSource(nameof(GetControlTypes))]
		public void ControlShouldFireShownEventWhenAddedDynamically(Type controlType)
		{
			Exception exception = null;
			int shownCount = 0;
			Form(form =>
			{
				form.Shown += (sender, e) => Application.Instance.AsyncInvoke(() =>
				{
					try
					{
						var ctl = (Control)Activator.CreateInstance(controlType);
						ctl.Shown += (sender2, e2) =>
						{
							shownCount++;
							Application.Instance.AsyncInvoke(() =>
							{
								if (form.Loaded)
									form.Close();
							});
						};
						form.Content = TableLayout.AutoSized(ctl);
					}
					catch (Exception ex)
					{
						exception = ex;
					}
				});
			});
			if (exception != null)
				ExceptionDispatchInfo.Capture(exception).Throw();

			Assert.AreEqual(1, shownCount);
		}

		[TestCaseSource(nameof(GetControlTypes))]
		public void ControlShouldFireShownEventWhenVisibleChanged(Type controlType)
		{
			int shownCount = 0;
			int? initialShownCount = null;
			Form(form =>
			{
				var ctl = (Control)Activator.CreateInstance(controlType);
				ctl.Shown += (sender2, e2) =>
				{
					shownCount++;
					Application.Instance.AsyncInvoke(() =>
					{
						if (form.Loaded)
							form.Close();
					});
				};
				ctl.Visible = false;
				Assert.AreEqual(0, shownCount);
				form.Content = TableLayout.AutoSized(ctl);
				Assert.AreEqual(0, shownCount);
				form.Shown += (sender, e) => Application.Instance.AsyncInvoke(() =>
				{
					initialShownCount = shownCount;
					ctl.Visible = true;
				});
			});

			Assert.AreEqual(0, initialShownCount, "#1"); // should not be initially called
			Assert.AreEqual(1, shownCount, "#2");
		}

		[ManualTest, Test]
		public void ControlsShouldHaveSaneDefaultWidths()
		{
			ManualForm(
				"Check to make sure the text/entry boxes have the correct widths and do not grow when entering text",
				form =>
				{
					var longText = "Some very long text that should not make the control grow larger than its default size";
					return new Scrollable
					{
						Content = new StackLayout
						{
							Items = {
								new Button { Text = "Button" },
								new Calendar(),
								new CheckBox { Text = "CheckBox" },
								new ColorPicker(),
								new ComboBox { Text = longText, Items = { "Item 1", "Item 2", "Item 3" } },
								new DateTimePicker(),
								new Drawable { Size = new Size(100, 20), BackgroundColor = Colors.Blue }, // not actually visible without a size
								new DropDown { Items = { "Item 1", "Item 2", "Item 3" } },
								new Expander { Header = "Hello", Content = new Label { Text = "Some content" } },
								new FilePicker { FilePath = "/some/path/that/is/long/which/should/not/make/it/too/big" },
								new GroupBox { Content = "Some content", Text = "Some text" },
								new LinkButton {  Text = "LinkButton"},
								new NumericStepper(),
								new PasswordBox(),
								new ProgressBar { Value = 50 },
								new RadioButton { Text = "RadioButton" },
								new RichTextArea { Text = longText },
								new SearchBox { Text = longText },
								new Slider { Value = 50 },
								new Spinner(),
								new Stepper(),
								new TabControl { Pages = { new TabPage { Text = "TabPage", Content = "Tab content" } } },
								new TextArea { Text = longText },
								new TextBox { Text = longText },
								new TextStepper { Text = longText },
								//new WebView()
							}
						}
					};
				});
		}
	}
}
