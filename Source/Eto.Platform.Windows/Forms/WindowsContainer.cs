using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Windows
{
	
	public abstract class WindowsContainer<T, W> : WindowsControl<T, W>, IContainer
		where T: System.Windows.Forms.Control
		where W: Container
	{
		Size minimumSize;

		public WindowsContainer()
		{
			EnableRedrawDuringSuspend = false;
		}

		protected IWindowsLayout WindowsLayout
		{
			get { return Widget.Layout != null && Widget.Layout.InnerLayout != null ? Widget.Layout.InnerLayout.Handler as IWindowsLayout : null; }
		}

		public bool EnableRedrawDuringSuspend { get; set; }

		protected bool SkipLayoutScale { get; set; }

		public override Size DesiredSize
		{
			get
			{
				var size = Size.Empty;
				var layout = WindowsLayout;

				if (layout != null)
					size = Size.Max (layout.DesiredSize, size);

				var desired = base.DesiredSize;
				if (desired.Width >= 0)
					size.Width = desired.Width;
				if (desired.Height >= 0)
					size.Height = desired.Height;
				if (this.MinimumSize != null)
					size = Size.Max (this.MinimumSize.Value, size);
				return size;
			}
		}

		public override void SetScale (bool xscale, bool yscale)
		{
			if (!SkipLayoutScale)
			{
				var layout = WindowsLayout;

				if (layout != null)
					layout.SetScale (xscale, yscale);
			}
			base.SetScale (xscale, yscale);
		}
		
		public Size MinimumSize {
			get { return minimumSize; }
			set {
				minimumSize = value;
				this.Control.MinimumSize = value.ToSD ();
			}
		}


		public virtual SWF.Control ContentContainer
		{
			get { return (SWF.Control)this.Control; }
		}

		public object ContainerObject
		{
			get { return this.ContentContainer; }
		}

		public override Size ClientSize
		{
			get	{ return new Size(ContentContainer.ClientSize.Width, ContentContainer.ClientSize.Height); }
			set { base.ClientSize = value; }
		}

		bool restoreRedraw;
		
		public override void SuspendLayout ()
		{
			base.SuspendLayout ();
			if (!EnableRedrawDuringSuspend && Control.IsHandleCreated)
			{
				restoreRedraw = (int)Win32.SendMessage(Control.Handle, Win32.WM.SETREDRAW, IntPtr.Zero, IntPtr.Zero) == 0;
			}
		}
		
		public override void ResumeLayout ()
		{
			base.ResumeLayout();
			if (restoreRedraw)
			{
				Win32.SendMessage(Control.Handle, Win32.WM.SETREDRAW, new IntPtr(1), IntPtr.Zero);
				Control.Refresh();
				restoreRedraw = false;
			}
		}

		public override void SetLayout (Layout layout)
		{
			base.SetLayout (layout);

			SWF.Control control = ((IWindowsLayout)layout.Handler).LayoutObject as SWF.Control;
			if (control != null)
			{
				control.Dock = SWF.DockStyle.Fill;
				((SWF.Control)ContainerObject).Controls.Add(control);
			}
		}
	}
}
