using System;
using Eto.Test.UnitTests;
using NUnit.Framework;
using Eto.Forms;
using System.Threading;
using g = Gtk;


namespace Eto.Test.Gtk.UnitTests
{
	[TestFixture]
	public class NativeParentWindowTests : TestBase
	{
		[Test]
		public void ControlInNativeWindowShouldReturnParentWindow()
		{
			Invoke(() =>
			{
				var window = new g.Window(g.WindowType.Toplevel);

				var panel = new Panel();

				window.Child = panel.ToNative(true);

				var parentWindow = panel.ParentWindow;
				Assert.IsNotNull(parentWindow, "#1");
				Assert.AreSame(window, parentWindow.ControlObject, "#2");
			});
		}

		/*
		[Test, ManualTest]
		public void DialogShouldAllowAttachingToNativeWindow()
		{
			bool passed = false;
			var ev = new ManualResetEvent(false);
			Invoke(() =>
			{
				var nswindow = new NSWindow(new CGRect(100, 100, 300, 300), NSWindowStyle.Titled, NSBackingStore.Buffered, false);
				nswindow.ReleasedWhenClosed = false;

				var showDialog = new Button { Text = "Show Attached Dialog" };
				showDialog.Click += (sender, e) =>
				{
					var dlg = new Dialog();
					dlg.DisplayMode = DialogDisplayMode.Attached;

					var closeButton = new Button { Text = "Close" };
					closeButton.Click += (sender2, e2) => dlg.Close();

					dlg.Content = new StackLayout
					{
						Padding = 10,
						Spacing = 10,
						Items = {
							"This should show as attached",
							closeButton
						}
					};

					dlg.ShowModal(showDialog);
				};

				var passButton = new Button { Text = "Pass" };
				passButton.Click += (sender, e) =>
				{
					passed = true;
					nswindow.Close();
					ev.Set();
				};
				var failButton = new Button { Text = "Fail" };
				failButton.Click += (sender, e) =>
				{
					passed = false;
					nswindow.Close();
					ev.Set();
				};

				var panel = new StackLayout
				{
					Items = {
						new Panel { Padding = 20, Content = showDialog },
						TableLayout.Horizontal(failButton, passButton)
					}
				};

				nswindow.ContentView = panel.ToNative(true);

				var parentWindow = panel.ParentWindow;

				nswindow.MakeKeyAndOrderFront(nswindow);
			});
			ev.WaitOne();
			Assert.IsTrue(passed);
		}*/
	}
}
