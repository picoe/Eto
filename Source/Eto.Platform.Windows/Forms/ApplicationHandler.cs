using System;
using System.Runtime.InteropServices;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Taskbar;
using Eto.Drawing;
using System.Threading;

namespace Eto.Platform.Windows
{
	public class ApplicationHandler : WidgetHandler<object, Application>, IApplication
	{
		string badgeLabel;
		bool attached;
		Thread mainThread;

		public static bool EnableScrollingUnderMouse = true;
		public static bool BubbleMouseEvents = true;

		public ApplicationHandler()
		{
			mainThread = Thread.CurrentThread;
		}

		public void RunIteration()
		{
			swf.Application.DoEvents();
		}

		public void Restart()
		{
			swf.Application.Restart();
		}

		public string BadgeLabel
		{
			get { return badgeLabel; }
			set
			{
				badgeLabel = value;
#if !__MonoCS__
				if ((bool)TaskbarManager.IsPlatformSupported)
				{
					if (!string.IsNullOrEmpty(badgeLabel))
					{
						var bmp = new sd.Bitmap(16, 16, sd.Imaging.PixelFormat.Format32bppArgb);
						using (var graphics = sd.Graphics.FromImage(bmp))
						{
							DrawBadgeLabel(bmp, graphics);
						}
						var icon = sd.Icon.FromHandle(bmp.GetHicon());

						TaskbarManager.Instance.SetOverlayIcon(icon, badgeLabel);
					}
					else
						TaskbarManager.Instance.SetOverlayIcon(null, null);
				}
#endif
			}
		}

		protected virtual void DrawBadgeLabel(sd.Bitmap bmp, sd.Graphics graphics)
		{
			var font = new sd.Font(sd.FontFamily.GenericSansSerif, 9, sd.FontStyle.Bold, sd.GraphicsUnit.Pixel);

			var size = graphics.MeasureString(badgeLabel, font, bmp.Size, sd.StringFormat.GenericTypographic);
			graphics.SmoothingMode = sd.Drawing2D.SmoothingMode.AntiAlias;
			graphics.FillEllipse(sd.Brushes.Red, new sd.Rectangle(0, 0, 16, 16));
			graphics.DrawEllipse(new sd.Pen(sd.Brushes.White, 2), new sd.Rectangle(0, 0, 15, 15));
			var pt = new sd.PointF((bmp.Width - size.Width - 0.5F) / 2, (bmp.Height - size.Height - 1) / 2);
			graphics.DrawString(badgeLabel, font, sd.Brushes.White, pt, sd.StringFormat.GenericTypographic);
		}

		public void Run(string[] args)
		{
			if (!attached)
			{
				swf.Application.EnableVisualStyles();
				if (!EtoEnvironment.Platform.IsMono)
					swf.Application.DoEvents();

				SetOptions();

				Widget.OnInitialized(EventArgs.Empty);

				if (Widget.MainForm != null && Widget.MainForm.Loaded)
					swf.Application.Run((swf.Form)Widget.MainForm.ControlObject);
				else
					swf.Application.Run();
			}
			else
			{
				Widget.OnInitialized(EventArgs.Empty);
			}
		}

		void SetOptions()
		{
			if (EnableScrollingUnderMouse)
				swf.Application.AddMessageFilter(new ScrollMessageFilter());

			if (BubbleMouseEvents)
			{
				var bubble = new BubbleEventFilter();
				bubble.AddBubbleMouseEvent((c, e) => c.OnMouseWheel(e), null, (int)Win32.WM.MOUSEWHEEL);
				bubble.AddBubbleMouseEvent((c, e) => c.OnMouseMove(e), null, (int)Win32.WM.MOUSEMOVE);
				bubble.AddBubbleMouseEvents((c, e) => c.OnMouseDown(e), true, (int)Win32.WM.LBUTTONDOWN, (int)Win32.WM.RBUTTONDOWN, (int)Win32.WM.MBUTTONDOWN);
				bubble.AddBubbleMouseEvents((c, e) => c.OnMouseDoubleClick(e), null, (int)Win32.WM.LBUTTONDBLCLK, (int)Win32.WM.RBUTTONDBLCLK, (int)Win32.WM.MBUTTONDBLCLK);
				bubble.AddBubbleMouseEvent((c, e) => c.OnMouseUp(e), false, (int)Win32.WM.LBUTTONUP, b => MouseButtons.Primary);
				bubble.AddBubbleMouseEvent((c, e) => c.OnMouseUp(e), false, (int)Win32.WM.RBUTTONUP, b => MouseButtons.Alternate);
				bubble.AddBubbleMouseEvent((c, e) => c.OnMouseUp(e), false, (int)Win32.WM.MBUTTONUP, b => MouseButtons.Middle);
				swf.Application.AddMessageFilter(bubble);
			}
		}

		public void Attach(object context)
		{
			attached = true;
			SetOptions();
		}

		public void OnMainFormChanged()
		{
		}

		public void Quit()
		{
			swf.Application.Exit();
		}

		public void Open(string url)
		{
			var info = new ProcessStartInfo(url);
			Process.Start(info);
		}

		public override void AttachEvent(string handler)
		{
			switch (handler)
			{
				case Application.TerminatingEvent:
					// handled by WindowHandler
					break;
				default:
					base.AttachEvent(handler);
					break;
			}
		}

		public void GetSystemActions(GenerateActionArgs args, bool addStandardItems)
		{

		}

		public void Invoke(Action action)
		{
			if (Widget.MainForm != null)
			{
				var window = Widget.MainForm.GetContainerControl();
				if (window == null) window = swf.Form.ActiveForm;

				if (window != null && window.InvokeRequired)
				{
					window.Invoke(action);
					return;
				}
			}
			if (Thread.CurrentThread == mainThread)
				action();
			else
				SynchronizationContext.Current.Post(state => action(), null);
		}

		public void AsyncInvoke(Action action)
		{
			if (Widget.MainForm != null)
			{
				var window = Widget.MainForm.GetContainerControl();
				if (window == null) window = swf.Form.ActiveForm;

				if (window != null && window.InvokeRequired)
				{
					window.BeginInvoke(action);
					return;
				}
			}
			SynchronizationContext.Current.Post(state => action(), null);
		}

		public Key CommonModifier
		{
			get
			{
				return Key.Control;
			}
		}

		public Key AlternateModifier
		{
			get
			{
				return Key.Alt;
			}
		}
	}
}
