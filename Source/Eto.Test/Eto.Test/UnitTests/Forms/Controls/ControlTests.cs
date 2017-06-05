using System;
using Eto.Forms;
using NUnit.Framework;
using System.Collections.Generic;
using Eto.Drawing;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class ControlTests : TestBase
	{
		public static IEnumerable<Func<Control>> Controls()
		{
			yield return () => new TextBox();
			yield return () => new Button();
			yield return () => new Drawable();
			yield return () => new Label();
		}

		[TestCaseSource("Controls")]
		public void DefaultValuesShouldBeCorrect(Func<Control> control)
		{
			TestProperties(f => control(),
						   c => c.Enabled,
						   c => c.ToolTip,
						   c => c.TabIndex
			);

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
