using System;
using Eto.Forms;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using Eto.Platform.GtkSharp.Drawing;
using Eto.Drawing;
using System.Collections.Generic;

namespace Eto.Platform.GtkSharp
{
	public class ApplicationHandler : WidgetHandler<object, Application>, IApplication
	{
		bool attached;
		Gtk.StatusIcon statusIcon;

		protected override void Initialize()
		{
			base.Initialize();
			Helper.Init();
		}

		public static int MainThreadID { get; set; }
		
		public void RunIteration ()
		{
			Gtk.Application.RunIteration ();
		}

		public void Restart ()
		{
			Gtk.Application.Quit ();

			// TODO: restart!
		}

		string badgeLabel;

		public string BadgeLabel
		{
			get { return badgeLabel; }
			set
			{
				badgeLabel = value;
				if (!string.IsNullOrEmpty (badgeLabel))
				{
					if (statusIcon == null) {
						statusIcon = new Gtk.StatusIcon ();
						statusIcon.Activate += delegate {
							var form = Application.Instance.MainForm;
							if (form != null)
								form.BringToFront ();
						};
					}
					var bmp = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, 32, 32);
					using (var graphics = new Graphics(new Bitmap (Widget.Generator, new BitmapHandler (bmp)))) {
						graphics.Clear ();
						DrawBadgeLabel (graphics, new Size (bmp.Width, bmp.Height), badgeLabel);
					}
					statusIcon.Pixbuf = bmp;
					statusIcon.Visible = true;
				}
				else if (statusIcon != null)
					statusIcon.Visible = false;
			}
		}

		protected virtual void DrawBadgeLabel (Graphics graphics, Size size, string badgeLabel)
		{
			var rect = new Rectangle (size);
			rect.Inflate (-2, -2);
			graphics.FillEllipse (Brushes.Red (Generator), rect);
			graphics.DrawEllipse (new Pen(Colors.White, 2, Generator), rect);
			var font = new Font(SystemFont.Bold, 10);
			var labelSize = graphics.MeasureString (font, badgeLabel);
			var labelPosition = ((PointF)(rect.Size - labelSize) / 2) + rect.Location;
			graphics.DrawText (font, Colors.White, labelPosition, badgeLabel);
			graphics.Flush ();
		}

		public void Invoke (Action action)
		{
			if (Thread.CurrentThread.ManagedThreadId == ApplicationHandler.MainThreadID)
				action ();
			else {
				var resetEvent = new ManualResetEvent (false);

				Gtk.Application.Invoke (delegate {
					try
					{
						action();
					}
					finally
					{
						resetEvent.Set();
					}
				});

				resetEvent.WaitOne ();
			}
		}

		public void AsyncInvoke (Action action)
		{
			Gtk.Application.Invoke (delegate {
				action ();
			});
		}

		public void Attach(object context)
		{
			attached = true;
		}

		public void OnMainFormChanged()
		{
		}

		public void Run (string[] args)
		{
			//if (!Platform.IsWindows) Gdk.Threads.Init(); // do this in windows, and it stalls!  ugh
			MainThreadID = Thread.CurrentThread.ManagedThreadId;
			
			Widget.OnInitialized (EventArgs.Empty);
			if (!attached)
			{
				Gtk.Application.Run();
				Gdk.Threads.Leave();
			}
		}

		public override void AttachEvent (string id)
		{
			switch (id) {
			case Application.TerminatingEvent:
				// called automatically
				break;
			default:
				base.AttachEvent (id);
				break;
			}
		}

		public void Quit ()
		{
			var args = new CancelEventArgs();
			var mainForm = Widget.MainForm != null ? Widget.MainForm.Handler as IGtkWindow : null;
			if (mainForm != null)
				args.Cancel = !mainForm.CloseWindow(Widget.OnTerminating);
			else
				Widget.OnTerminating(args);

			if (!args.Cancel)
				Gtk.Application.Quit();
		}
		
		public void Open (string url)
		{
			var info = new ProcessStartInfo (url);
			Process.Start (info);
		}

		public IEnumerable<Command> GetSystemCommands()
		{
			yield break;
		}

		public void CreateStandardMenu(MenuItemCollection menu, IEnumerable<Command> commands)
		{
		}

		public Keys CommonModifier {
			get {
				return Keys.Control;
			}
		}

		public Keys AlternateModifier {
			get {
				return Keys.Alt;
			}
		}
	}
}
