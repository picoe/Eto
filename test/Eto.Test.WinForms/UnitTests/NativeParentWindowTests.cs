using System;
using Eto.Test.UnitTests;
using NUnit.Framework;
using Eto.Forms;
using System.Threading;
using sw = System.Windows;
using swf = System.Windows.Forms;
using Eto.WinForms.Forms;

namespace Eto.Test.WinForms.UnitTests
{
	[TestFixture]
	public class NativeParentWindowTests : TestBase
	{
		[Test]
		public void ControlInWinFormsWindowShouldReturnParentWindow()
		{
			Invoke(() =>
			{
				var nativeWindow = new swf.Form();
				var panel = new Panel();
				var winformsPanel = panel.ToNative(true);

				nativeWindow.Controls.Add(winformsPanel);

				// this ensures the hwnd is created for this form and for the element host
				var windowhandle = nativeWindow.Handle;
				var wpfhosthandle = winformsPanel.Handle;

				// get the parent window
				var parentWindow = panel.ParentWindow;

				Assert.IsNotNull(parentWindow, "#1");
				Assert.AreEqual(windowhandle, parentWindow.NativeHandle, "#2");
			});
		}

		[Test, ManualTest]
		public void FormShouldAllowOwningToWinForms()
		{
			bool passed = false;
			var ev = new ManualResetEvent(false);
			Invoke(() =>
			{
				var nativeWindow = new swf.Form();
				nativeWindow.MinimumSize = new System.Drawing.Size(200, 200);
				nativeWindow.FormClosed += (sender, e) => ev.Set();

				var showDialog = new Button { Text = "Show Owned Dialog" };
				showDialog.Click += (sender, e) =>
				{
					var parentWindow = showDialog.ParentWindow;
					if (parentWindow == null)
					{
						passed = false;
						nativeWindow.Close();
						return;
					}

					var form = new Form();

					var closeButton = new Button { Text = "Close" };
					closeButton.Click += (sender2, e2) => form.Close();

					form.Content = new StackLayout
					{
						Padding = 10,
						Spacing = 10,
						Items = {
							"This should show as a child to the parent, and should not be able to go behind it",
							closeButton
						}
					};

					form.Owner = parentWindow;
					form.Show();
				};

				var passButton = new Button { Text = "Pass" };
				passButton.Click += (sender, e) =>
				{
					passed = true;
					nativeWindow.Close();
					ev.Set();
				};
				var failButton = new Button { Text = "Fail" };
				failButton.Click += (sender, e) =>
				{
					passed = false;
					nativeWindow.Close();
					ev.Set();
				};

				var panel = new StackLayout
				{
					Items = {
						new Panel { Padding = 20, Content = showDialog },
						TableLayout.Horizontal(failButton, passButton)
					}
				};

				var winformsPanel = panel.ToNative(true);
				winformsPanel.Dock = swf.DockStyle.Fill;
				nativeWindow.Controls.Add(winformsPanel);

				nativeWindow.Show();
			});
			ev.WaitOne();
			Assert.IsTrue(passed);
		}
	}
}
