using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Windows
{

	public abstract class WindowsContainer<TControl, TWidget> : WindowsControl<TControl, TWidget>, IContainer
		where TControl : swf.Control
		where TWidget : Container
	{
		Size minimumSize;

		protected WindowsContainer()
		{
			EnableRedrawDuringSuspend = false;
		}

		public override Size? DefaultSize
		{
			get
			{
				var min = ContainerControl.MinimumSize;
				ContainerControl.MinimumSize = sd.Size.Empty;
				var size = ContainerControl.GetPreferredSize(Size.MaxValue.ToSD()).ToEto();
				ContainerControl.MinimumSize = min;
				return size;
			}
		}

		public bool EnableRedrawDuringSuspend { get; set; }

		public override Size GetPreferredSize(Size availableSize)
		{
			var size = base.GetPreferredSize(availableSize);
			return Size.Max(minimumSize, size);
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
