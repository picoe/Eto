using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using System.Diagnostics;
using System.Threading;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.Collections.Generic;

namespace Eto.Platform.Windows
{
	public class ApplicationHandler : WidgetHandler<object, Application>, IApplication
	{
		string badgeLabel;
		bool attached;
		readonly Thread mainThread;
		SynchronizationContext context;
		public static bool EnableScrollingUnderMouse = true;
		public static bool BubbleMouseEvents = true;
		public static bool BubbleKeyEvents = true;

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
				if (TaskbarManager.IsPlatformSupported)
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
				// creates sync context
				using (var ctl = new swf.Control())
				{
				}
				context = SynchronizationContext.Current;

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

		static void SetOptions()
		{
			if (EnableScrollingUnderMouse)
				swf.Application.AddMessageFilter(new ScrollMessageFilter());

			if (BubbleMouseEvents)
			{
				var bubble = new BubbleEventFilter();
				bubble.AddBubbleMouseEvent((c, e) => c.OnMouseWheel(e), null, Win32.WM.MOUSEWHEEL);
				bubble.AddBubbleMouseEvent((c, e) => c.OnMouseMove(e), null, Win32.WM.MOUSEMOVE);
				bubble.AddBubbleMouseEvents((c, e) => c.OnMouseDown(e), true, Win32.WM.LBUTTONDOWN, Win32.WM.RBUTTONDOWN, Win32.WM.MBUTTONDOWN);
				bubble.AddBubbleMouseEvents((c, e) => {
					c.OnMouseDoubleClick(e);
					if (!e.Handled)
						c.OnMouseDown(e);
				}, null, Win32.WM.LBUTTONDBLCLK, Win32.WM.RBUTTONDBLCLK, Win32.WM.MBUTTONDBLCLK);
				bubble.AddBubbleMouseEvent((c, e) => c.OnMouseUp(e), false, Win32.WM.LBUTTONUP, b => MouseButtons.Primary);
				bubble.AddBubbleMouseEvent((c, e) => c.OnMouseUp(e), false, Win32.WM.RBUTTONUP, b => MouseButtons.Alternate);
				bubble.AddBubbleMouseEvent((c, e) => c.OnMouseUp(e), false, Win32.WM.MBUTTONUP, b => MouseButtons.Middle);
				swf.Application.AddMessageFilter(bubble);
			}
			if (BubbleKeyEvents)
			{
				var bubble = new BubbleEventFilter();
				bubble.AddBubbleKeyEvent((c, e) => c.OnKeyDown(e), Win32.WM.KEYDOWN, KeyEventType.KeyDown);
				bubble.AddBubbleKeyEvent((c, e) => c.OnKeyDown(e), Win32.WM.SYSKEYDOWN, KeyEventType.KeyDown);
				bubble.AddBubbleKeyCharEvent((c, e) => c.OnKeyDown(e), Win32.WM.CHAR, KeyEventType.KeyDown);
				bubble.AddBubbleKeyCharEvent((c, e) => c.OnKeyDown(e), Win32.WM.SYSCHAR, KeyEventType.KeyDown);
				bubble.AddBubbleKeyEvent((c, e) => c.OnKeyUp(e), Win32.WM.KEYUP, KeyEventType.KeyUp);
				bubble.AddBubbleKeyEvent((c, e) => c.OnKeyUp(e), Win32.WM.SYSKEYUP, KeyEventType.KeyUp);
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

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Application.TerminatingEvent:
					// handled by WindowHandler
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public void GetSystemActions(List<BaseAction> actions, ISubMenuWidget menu, ToolBar toolBar, bool addStandardItems)
		{
		}

		public void Invoke(Action action)
		{
			if (Widget.MainForm != null)
			{
				var window = Widget.MainForm.GetContainerControl() ?? swf.Form.ActiveForm;

				if (window != null && window.InvokeRequired)
				{
					window.Invoke(action);
					return;
				}
			}
			if (Thread.CurrentThread == mainThread)
				action();
			else if (context != null)
				context.Post(state => action(), null);
		}

		public void AsyncInvoke(Action action)
		{
			if (Widget.MainForm != null)
			{
				var window = Widget.MainForm.GetContainerControl() ?? swf.Form.ActiveForm;

				if (window != null && window.InvokeRequired)
				{
					window.BeginInvoke(action);
					return;
				}
			}
			if (context != null)
				context.Post(state => action(), null);
		}

		public Keys CommonModifier
		{
			get
			{
				return Keys.Control;
			}
		}

		public Keys AlternateModifier
		{
			get
			{
				return Keys.Alt;
			}
		}
	}
}
