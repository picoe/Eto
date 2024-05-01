using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eto.Test.UnitTests;
using NUnit.Framework;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Wpf.Forms;

namespace Eto.Test.Wpf.UnitTests
{
	[TestFixture]
	public class ScreenToClientTests : TestBase
	{
		
		class NonActivatingForm : swf.Form
		{
			protected override bool ShowWithoutActivation => true;
		}

		[Test, ManualTest]
		public void ScreenLocationsShouldBeCorrect()
		{
			var mre = new ManualResetEvent(false);
			Invoke(() =>
			{
				var wfwindows = new List<swf.Form>();
				var windows = new List<Window>();
				
				void CreateWindow(Rectangle rect)
				{
					var window = new Form
					{
						ShowActivated = false,
						ShowInTaskbar = false,
						Minimizable = false,
						Maximizable = false,
						Bounds = rect,
						Content = TableLayout.AutoSized("Click to dismiss", centered: true)
					};
					window.MouseDown += (sender, e) =>
					{
						foreach (var w in windows)
						{
							w.Close();
						}
						foreach (var w in wfwindows)
						{
							w.Close();
						}
						mre.Set();
					};
					window.Show();
					windows.Add(window);

					bool perMonitor = false;

					var sdrect = WpfHelpers.ToNativeScreen(rect, window.Screen, perMonitor: perMonitor).ToSD();

					var wfwindow = new NonActivatingForm
					{
						Bounds = sdrect,
						ShowInTaskbar = false,
						MinimizeBox = false,
						MaximizeBox = false,
						StartPosition = swf.FormStartPosition.Manual,
						BackColor = sd.Color.Blue
					};
					wfwindow.Controls.Add(new swf.Label { Text = "Move me", Dock = swf.DockStyle.Fill, ForeColor = sd.Color.White });
					wfwindow.LocationChanged += (sender, e) =>
					{
						var loc = wfwindow.RectangleToScreen(wfwindow.ClientRectangle);
						window.Bounds = (Rectangle)WpfHelpers.ToEtoScreen(loc.ToEto(), window.Screen, perMonitor: perMonitor);
					};
					wfwindow.SizeChanged += (sender, e) =>
					{
						var loc = wfwindow.RectangleToScreen(wfwindow.ClientRectangle);
						window.Bounds = (Rectangle)WpfHelpers.ToEtoScreen(loc.ToEto(), window.Screen, perMonitor: perMonitor);
					};
					wfwindow.Show();
					wfwindows.Add(wfwindow);
					
				}
				
				foreach (var screen in Screen.Screens)
				{
					var bounds = (Rectangle)screen.WorkingArea;
					var size = new Size(200, 200);
					CreateWindow(new Rectangle(bounds.TopLeft, size));
					CreateWindow(new Rectangle(bounds.TopRight - new Size(size.Width, 0), size));
					CreateWindow(new Rectangle(bounds.BottomLeft - new Size(0, size.Height), size));
					CreateWindow(new Rectangle(bounds.BottomRight - size, size));
				}
			});
			mre.WaitOne();
		}

	}
}