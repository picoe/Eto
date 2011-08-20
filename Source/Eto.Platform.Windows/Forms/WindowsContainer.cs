using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Windows
{
	
	public abstract class WindowsContainer<T, W> : WindowsControl<T, W>, IContainer
		where T: SWF.Control
		where W: Container
	{


		public SWF.Control ContainerControl
		{
			get { return (SWF.Control)this.ContainerObject; }
		}

		public override Size ClientSize
		{
			get	{ return new Size(ContainerControl.ClientSize.Width, ContainerControl.ClientSize.Height); }
		}

		
		public virtual object ContainerObject
		{
			get	{ return ControlObject; }
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
