using System;
using Eto.Test.UnitTests;
using NUnit.Framework;
using Eto.Forms;
using System.Threading;
using sw = System.Windows;
using swc = System.Windows.Controls;
using swf = System.Windows.Forms;


namespace Eto.Test.Wpf.UnitTests
{
	[TestFixture]
	public class NativeParentWindowTests : TestBase
	{
		[Test]
		public void ControlInWpfWindowShouldReturnParentWindow()
		{
			Invoke(() =>
			{
				var nativeWindow = new sw.Window();
				var panel = new Panel();

				nativeWindow.Content = panel.ToNative(true);

				var parentWindow = panel.ParentWindow;
				Assert.IsNotNull(parentWindow, "#1");
				Assert.AreSame(nativeWindow, parentWindow.ControlObject, "#2");
			});
		}

		[Test]
		public void ControlInWinFormsWindowShouldReturnParentWindow()
		{
			Invoke(() =>
			{
				var nativeWindow = new swf.Form();
				var panel = new Panel();
				var wpfPanel = panel.ToNative(true);
				var wpfhost = new swf.Integration.ElementHost();
				wpfhost.Child = wpfPanel;

				nativeWindow.Controls.Add(wpfhost);

				// this ensures the hwnd is created for this form and for the element host
				var windowhandle = nativeWindow.Handle;
				var wpfhosthandle = wpfhost.Handle;

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
				nativeWindow.MinimumSize = new System.Drawing.Size(300, 300);
				nativeWindow.FormClosed += (sender, e) => ev.Set();
				var wpfhost = new swf.Integration.ElementHost();
				wpfhost.Dock = swf.DockStyle.Fill;

				nativeWindow.Controls.Add(wpfhost);

				var showDialog = new Button { Text = "Show Owned Dialog" };
				showDialog.Click += (sender, e) =>
				{
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

					form.Owner = showDialog.ParentWindow;
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

				wpfhost.Child = panel.ToNative(true);

				var parentWindow = panel.ParentWindow;

				nativeWindow.Show();
			});
			ev.WaitOne();
			Assert.IsTrue(passed);
		}

		[Test, ManualTest]
		public void FormShouldAllowOwningToWpf()
		{
			bool passed = false;
			var ev = new ManualResetEvent(false);
			Invoke(() =>
			{
				var nativeWindow = new sw.Window();
				nativeWindow.SizeToContent = sw.SizeToContent.WidthAndHeight;
				nativeWindow.Closed += (sender, e) => ev.Set();

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

				var wpfPanel = panel.ToNative(true);

				nativeWindow.HorizontalContentAlignment = sw.HorizontalAlignment.Stretch;
				nativeWindow.VerticalContentAlignment = sw.VerticalAlignment.Stretch;
				nativeWindow.Content = wpfPanel;

				nativeWindow.Show();
			});
			ev.WaitOne();
			Assert.IsTrue(passed);
		}

	}
}
