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
		Size? minimumSize;

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
		
		public Size? MinimumSize {
			get { return minimumSize; }
			set {
				minimumSize = value;
				this.Control.MinimumSize = (value ?? Size.Empty).ToSD ();
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

		
		public override void SuspendLayout ()
		{
			base.SuspendLayout ();
			if (Widget.Layout != null)
			{
				var layout = Widget.Layout.Handler as IWindowsLayout;
				if (layout != null)
				{
					var control = layout.LayoutObject as SWF.Control;
					if (control != null)
					{
						control.SuspendLayout ();
						if (!EnableRedrawDuringSuspend && control.IsHandleCreated)
							Win32.SendMessage (control.Handle, Win32.WM.SETREDRAW, IntPtr.Zero, IntPtr.Zero);
					}
				}
				
			}
		}
		
		public override void ResumeLayout ()
		{
			base.ResumeLayout ();
			if (Widget.Layout != null)
			{
				var layout = Widget.Layout.Handler as IWindowsLayout;
				if (layout != null)
				{
					var control = layout.LayoutObject as SWF.Control;
					if (control != null)
					{
						if (!EnableRedrawDuringSuspend && control.IsHandleCreated)
							Win32.SendMessage (control.Handle, Win32.WM.SETREDRAW, new IntPtr(1), IntPtr.Zero);
						control.ResumeLayout ();
					}
				}
				
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
