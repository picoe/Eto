using System;
using Eto.Forms;
using NUnit.Framework;
using System.Collections.Generic;
using Eto.Drawing;
using System.Threading;
using System.Runtime.ExceptionServices;
using System.Reflection;
using System.Linq;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class ControlTests : TestBase
	{
		[TestCaseSource(nameof(GetControlTypes))]
		public void DefaultValuesShouldBeCorrect(IControlTypeInfo<Control> controlType)
		{
			TestProperties(f => controlType.CreateControl(),
						   c => c.Enabled,
						   c => c.ToolTip,
						   c => c.TabIndex
			);
		}

		[TestCaseSource(nameof(GetControlTypes))]
		public void ControlShouldFireShownEvent(IControlTypeInfo<Control> controlType)
		{
			int shownCount = 0;
			int visualControlShownCount = 0;
			int expectedVisualShown = 0;
			Form(form =>
			{
				var ctl = controlType.CreateControl();

				// themed controls have visual controls!
				foreach (var visualControl in ctl.VisualControls)
				{
					expectedVisualShown++;
					visualControl.Shown += (sender, e) =>
					{
						visualControlShownCount++;
					};
				}
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
			Assert.AreEqual(expectedVisualShown, visualControlShownCount, "Visual controls didn't get Shown event triggered");
		}

		[TestCaseSource(nameof(GetControlTypes))]
		public void ControlShouldFireShownEventWhenAddedDynamically(IControlTypeInfo<Control> controlType)
		{
			Exception exception = null;
			int shownCount = 0;
			Form(form =>
			{
				form.Shown += (sender, e) => Application.Instance.AsyncInvoke(() =>
				{
					try
					{
						var ctl = controlType.CreateControl();
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
		public void ControlShouldFireShownEventWhenVisibleChanged(IControlTypeInfo<Control> controlType)
		{
			int shownCount = 0;
			int? initialShownCount = null;
			Form(form =>
			{
				var ctl = controlType.CreateControl();
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
							Items =
							{
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

		public class ControlGCTest
		{
			public string Description { get; set; }
			public Type ControlType { get; set; }
			public Action<object> Test { get; set; }
			public override string ToString()
			{
				if (!string.IsNullOrEmpty(Description))
					return $"{ControlType}: {Description}";
				return ControlType.ToString();
			}

		}

		public static ControlGCTest GCTest<T>(Action<T> action) => new ControlGCTest { ControlType = typeof(T), Test = c => action((T)c) };

		public static ControlGCTest GCTest<T>(string description, Action<T> action)
		{
			var test = GCTest<T>(action);
			test.Description = description;
			return test;
		}

		public static IEnumerable<ControlGCTest> GetControlGCItems()
		{
			// simply create all control types and ensure they can be GC'd without hooking up anything.
			foreach (var type in GetAllControlTypes())
			{
				if (Platform.Instance.IsWpf || Platform.Instance.IsMac)
				{
					// wpf and macos has (known) problems GC'ing a Window right away, so let's not test it.
					if (typeof(Window).GetTypeInfo().IsAssignableFrom(type.Type))
						continue;
				}

				if (Platform.Instance.IsWpf || Platform.Instance.IsWinForms)
				{ 
					// SWF.WebBrowser can't be GC'd for some reason either.  Not an Eto problem.
					if (typeof(WebView).GetTypeInfo().IsAssignableFrom(type.Type))
						continue;
				}

				yield return new ControlGCTest { ControlType = type.Type };
			}

			// extra tests for things that have known to cause a control not to be GC'd

			yield return GCTest("With Step", (Stepper c) =>
			{
				c.Step += (sender, e) => { /* do something */ };
			});

			yield return GCTest("With ValueChanged", (NumericStepper c) =>
			{
				c.ValueChanged += (sender, e) => { /* do something */ };
			});

			yield return GCTest("With Step", (TextStepper c) =>
			{
				c.Step += (sender, e) => { /* do something */ };
			});

			yield return GCTest("With Panels", (Splitter c) =>
			{
				c.Panel1 = new Panel();
				c.Panel2 = new Panel();
			});
		}

		[TestCaseSource(nameof(GetControlGCItems))]
		public void ControlsShouldCollectWhenNotReferenced(ControlGCTest test)
		{
			WeakReference reference = null;
			Invoke(() =>
			{
				var obj = Activator.CreateInstance(test.ControlType);
				test.Test?.Invoke(obj);
				reference = new WeakReference(obj);
				obj = null;
			});
			Thread.Sleep(100);
			GC.Collect();
			GC.WaitForPendingFinalizers();
			Assert.IsNotNull(reference);
			Assert.IsNull(reference.Target);
			Assert.IsFalse(reference.IsAlive);
		}

		[TestCaseSource(nameof(GetControlTypes))]
		public void ControlsShouldReturnAFont(IControlTypeInfo<Control> info)
		{
			Invoke(() =>
			{
				var control = info.CreateControl();
				if (control is CommonControl commonControl)
				{
					Assert.IsNotNull(commonControl.Font);
				}
				else if (control is GroupBox groupBox)
				{
					Assert.IsNotNull(groupBox.Font);
				}
				else
				{
					Assert.Pass("Control does not have a font property");
				}
			});
		}

		[TestCaseSource(nameof(GetControlTypes)), ManualTest]
		public void ControlsShouldNotHaveIntrinsicPadding(IControlTypeInfo<Control> info)
		{
			ManualForm("Controls should be touching horizontally and vertically,\nwithout being clipped.", form =>
			{
				return new TableLayout
				{
					Rows =
					{
						new TableRow(new TableCell(info.CreatePopulatedControl(), true), new TableCell(info.CreatePopulatedControl(), true)),
						new TableRow(new Panel { Content = info.CreatePopulatedControl() }, info.CreatePopulatedControl()),
						new TableRow(info.CreatePopulatedControl(), new Drawable { Content = info.CreatePopulatedControl() }),
						null
					}
				};
			});
		}

		[Test, ManualTest]
		public void PointToScreenShouldWorkOnSecondaryScreen()
		{
			bool wasClicked = false;
			Form childForm = null;
			try
			{
				ManualForm("The Form with the button should be above the text box exactly.\nClick the button to pass the test, close the window to fail.", form =>
				{
					var screens = Screen.Screens.ToArray();
					Assert.GreaterOrEqual(screens.Length, 2, "You must have a secondary monitor for this test");
					form.Location = Point.Round(screens[1].Bounds.Location) + new Size(50, 50);
					form.ClientSize = new Size(200, 200);

					var textBox = new TextBox { Text = "You shouldn't see this" };

					form.Shown += (sender, e) =>
					{
						childForm = new Form
						{
							WindowStyle = WindowStyle.None,
							ShowInTaskbar = false,
							Maximizable = false,
							Resizable = false,
							BackgroundColor = Colors.Red,
							Topmost = true,
							Location = Point.Round(textBox.PointToScreen(PointF.Empty)),
							Size = textBox.Size
						};
						var b = new Button { Text = "Click Me!" };
						b.Click += (sender2, e2) =>
						{
							wasClicked = true;
							childForm.Close();
							childForm = null;
							form.Close();
						};

						childForm.Content = new TableLayout { Rows = { b } };
						childForm.Show();
					};

					var layout = new DynamicLayout();
					layout.AddCentered(textBox);

					return layout;
				}, allowPass: false, allowFail: false);
			}
			finally
			{
				if (childForm != null)
					Application.Instance.Invoke(() => childForm.Close());
			}
			Assert.IsTrue(wasClicked, "The test completed without clicking the button");
		}

	}
}