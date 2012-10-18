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
		
		public Size? MinimumSize {
			get {
				if (this.Control.MinimumSize == SD.Size.Empty)
					return null;
				else
					return Generator.Convert(this.Control.MinimumSize);
			}
			set {
				if (value != null)
					this.Control.MinimumSize = Generator.Convert (value.Value);
				else
					this.Control.MinimumSize = System.Drawing.Size.Empty;
			}
		}


		public SWF.Control ContentContainer
		{
			get { return (SWF.Control)this.ContainerObject; }
		}

		public override Size ClientSize
		{
			get	{ return new Size(ContentContainer.ClientSize.Width, ContentContainer.ClientSize.Height); }
			set { base.ClientSize = value; }
		}

		
		public virtual object ContainerObject
		{
			get	{ return Control; }
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
