using System;
using NUnit.Framework;
using Eto.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Specialized;
using Eto.Drawing;
using System.Threading;

namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class WindowTests : TestBase
	{
		[TestCase(true)]
		[TestCase(false)]
		[ManualTest]
		public void WindowShouldAutoSize(bool useForm)
		{
			void DoTest(Window window)
			{
				window.AutoSize = true;
				window.MinimumSize = Size.Empty;

				var bottomPanel = new StackLayout();
				var rightPanel = new StackLayout { Orientation = Orientation.Horizontal };

				var autoSize = new CheckBox { Text = "AutoSize", Checked = window.AutoSize };
				autoSize.CheckedChanged += (sender, e) => {
					window.AutoSize = autoSize.Checked == true;
				};

				var addBottomButton = new Button { Text = "Add bottom control" };
				addBottomButton.Click += (sender, e) => {
					bottomPanel.Items.Add(new Panel { Size = new Size(20, 20) });
					autoSize.Checked = window.AutoSize;
				};

				var addRightButton = new Button { Text = "Add right control" };
				addRightButton.Click += (sender, e) => {
					rightPanel.Items.Add(new Panel { Size = new Size(20, 20) });
					autoSize.Checked = window.AutoSize;
				};

				var resetButton = new Button { Text = "Reset" };
				resetButton.Click += (sender, e) =>
				{
					window.SuspendLayout();
					bottomPanel.Items.Clear();
					rightPanel.Items.Clear();
					window.ResumeLayout();
					autoSize.Checked = window.AutoSize;
				};

				window.SizeChanged += (sender, e) => autoSize.Checked = window.AutoSize;

				var layout = new DynamicLayout();
				layout.BeginHorizontal();
				layout.BeginCentered();
				layout.Add(addRightButton);
				layout.Add(addBottomButton);
				layout.Add(resetButton);
				layout.Add(autoSize);
				layout.EndCentered();
				layout.Add(rightPanel);
				layout.EndHorizontal();
				layout.Add(bottomPanel);

				window.Content = layout;
			}
			if (useForm) Form(DoTest, -1); else Dialog(DoTest, -1);

		}


		[ManualTest]
		[TestCase(true, true, true, false)]
		[TestCase(true, true, false, true)]
		[TestCase(true, false, true, false)]
		[TestCase(true, false, false, true)]
		[TestCase(false, true, true, false)]
		[TestCase(false, true, false, true)]
		[TestCase(false, false, true, false)]
		[TestCase(false, false, false, true)]
		public void WindowShouldHaveCorrectInitialSizeWithWrappedLabel(bool useForm, bool useSize, bool setWidth, bool setHeight)
		{
			bool wasClosed = false;
			var mre = new ManualResetEvent(false);
			Application.Instance.Invoke(() =>
			{
				const string infoText = "Click to change text.\n";
				var label = new Label();
				label.TextColor = Colors.White;
				label.Text = infoText + Utility.LoremText;

				Window window = useForm ? (Window)new Form { ShowActivated = false } : new Dialog();
				window.AutoSize = true;
				window.BackgroundColor = Colors.Blue;
				window.Resizable = false;
				window.Content = label;

				if (useSize)
				{
					if (setWidth && setHeight)
						window.Size = new Size(150, 150);
					else if (setWidth)
						window.Size = new Size(150, -1);
					else if (setHeight)
						window.Size = new Size(-1, 150);
				}
				else
				{
					if (setWidth && setHeight)
						window.Width = window.Height = 150;
					else if (setWidth)
						window.Width = 150;
					else if (setHeight)
						window.Height = 150;
				}

				label.MouseDown += (sender, e) => {
					label.Text = infoText + Utility.GenerateLoremText(new Random().Next(200));
				};

				window.Closed += (sender, e) =>
				{
					mre.Set();
					wasClosed = true;
				};

				window.Owner = Application.Instance.MainForm;
				if (window is Form f)
					f.Show();
				else if (window is Dialog d)
					d.ShowModal();
			});
			mre.WaitOne(-1);
			Assert.IsTrue(wasClosed, "#1 Form was not closed.  You need to click on it to confirm it is sized correctly");
		}

		[Test]
		public void WindowShouldReportInitialSize()
		{
			Form(form =>
			{
				Size? size = null;
				form.Content = new Panel { Size = new Size(300, 300) };
				form.SizeChanged += (sender, e) => size = form.Size;

				form.Shown += (sender, e) =>
				{
					Assert.IsNotNull(size, "#1");
					Assert.IsTrue(size.Value.Width >= 300, "#2");
					Assert.IsTrue(size.Value.Height >= 300, "#3");
					form.Close();
				};
			});
		}

		[Test]
		public void DefaultFormValuesShouldBeCorrect()
		{
			TestProperties(f => f,
				f => f.CanFocus,
				f => f.ShowActivated,
				f => f.Enabled
			);
		}

		public class SubSubForm : SubForm
		{
			protected override void OnClosed(EventArgs e)
			{
				base.OnClosed(e);
			}
		}

		public class SubForm : Form
		{
			protected override void OnClosed(EventArgs e)
			{
				base.OnClosed(e);
			}
		}

		[Test]
		public void ClosedEventShouldFireOnceWithMultipleSubclasses()
		{
			int closed = 0;
			Form<SubSubForm>(form =>
			{
				form.Content = new Panel { Size = new Size(300, 300) };
				form.Closed += (sender, e) => closed++;
				form.Shown += (sender, e) => form.Close();
			});
			Assert.AreEqual(1, closed, "Closed event should only fire once");
		}

		[TestCase(true)]
		[TestCase(false)]
		[ManualTest]
		public void InitialLocationOfFormShouldBeCorrect(bool withOwner)
		{
			ManualForm("This form should be located at the top left of the screen", form =>
			{
				if (withOwner)
					form.Owner = Application.Instance.MainForm;
				form.Location = new Point(0, 0);

				return new Panel { Size = new Size(200, 200) };
			});
		}
		[TestCase(true)]
		[TestCase(false)]
		[ManualTest]
		public void InitialLocationOfDialogShouldBeCorrect(bool withOwner)
		{
			ManualDialog("This dialog should be located at the top left of the screen", form =>
			{
				if (withOwner)
					form.Owner = Application.Instance.MainForm;
				form.Location = new Point(0, 0);

				return new Panel { Size = new Size(200, 200) };
			});
		}

		[TestCase(-1)]
		[TestCase(100)] // fails on Mac and Wpf currently, due to the bottom label being wider than this size...
		[TestCase(250)]
		[ManualTest]
		public void SizeOfFormShouldWorkWithLabels(int width)
		{
			ManualForm("Form should not have large space at\nthe bottom or between labels", form =>
			{
				Label CreateLabel()
				{
					var label = new Label { Text = Utility.GenerateLoremText(20) };
					if (width > 0)
						label.Width = width;
					return label;
				}

				var layout = new TableLayout();
				layout.Rows.Add(CreateLabel());
				layout.Rows.Add(CreateLabel());
				layout.Rows.Add(CreateLabel());
				layout.Rows.Add(CreateLabel());

				return layout;
			});
		}
	}
}

