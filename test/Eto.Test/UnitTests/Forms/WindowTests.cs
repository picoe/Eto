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


		[TestCase(true, true, true, false)]
		[TestCase(true, true, false, true)]
		[TestCase(true, false, true, false)]
		[TestCase(true, false, false, true)]
		[TestCase(false, true, true, false)]
		[TestCase(false, true, false, true)]
		[TestCase(false, false, true, false)]
		[TestCase(false, false, false, true)]
		public void FormShouldSizeWithLabelCorrectly(bool useForm, bool useSize, bool setWidth, bool setHeight)
		{
			bool wasClosed = false;
			var mre = new ManualResetEvent(false);
			Application.Instance.Invoke(() =>
			{
				var label = new Label();
				label.TextColor = Colors.White;
				label.Text = Sections.Controls.RichTextAreaSection.LoremText;

				Window window = useForm ? (Window)new Form { ShowActivated = false } : new Dialog();
				window.BackgroundColor = Colors.Blue;
				// window.ShowInTaskbar = false;
				// window.WindowStyle = WindowStyle.None;
				// window.Resizable = true;
				// window.Topmost = true;
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

				window.MouseDown += (sender, e) =>
				{
					window.Close();
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
			mre.WaitOne(10000);
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
					var label = new Label { Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus dictum ultricies augue, non mollis ligula sagittis ac." };
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

