using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Windows
{

	public abstract class WindowsContainer<T, W> : WindowsControl<T, W>, IContainer
		where T : swf.Control
		where W : Container
	{
		Size minimumSize;

		protected WindowsContainer()
		{
			EnableRedrawDuringSuspend = false;
		}

		public bool EnableRedrawDuringSuspend { get; set; }

		public override Size DesiredSize
		{
			get
			{
				var size = Size.Empty;
				var desired = base.DesiredSize;
				if (desired.Width >= 0)
					size.Width = desired.Width;
				if (desired.Height >= 0)
					size.Height = desired.Height;
				return Size.Max(minimumSize, size);
			}
		}

		public Size MinimumSize
		{
			get { return minimumSize; }
			set
			{
				minimumSize = value;
				SetMinimumSize();
			}
		}

		bool restoreRedraw;

		public override void SuspendLayout()
		{
			base.SuspendLayout();
			if (!EnableRedrawDuringSuspend && Control.IsHandleCreated && EtoEnvironment.Platform.IsWindows)
			{
				restoreRedraw = (int)Win32.SendMessage(Control.Handle, Win32.WM.SETREDRAW, IntPtr.Zero, IntPtr.Zero) == 0;
			}
		}

		public override void ResumeLayout()
		{
			base.ResumeLayout();
			if (restoreRedraw)
			{
				Win32.SendMessage(Control.Handle, Win32.WM.SETREDRAW, new IntPtr(1), IntPtr.Zero);
				Control.Refresh();
				restoreRedraw = false;
			}
		}
	}
}
