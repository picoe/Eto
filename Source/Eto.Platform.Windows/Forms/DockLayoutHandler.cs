using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Windows
{
	public class DockLayoutHandler : WindowsLayout<object, DockLayout>, IDockLayout
	{
		Control child;
		Padding padding;
		Control childToAdd;
		
		public DockLayoutHandler ()
		{
			padding = DockLayout.DefaultPadding;
		}
		
		public override object Control {
			get {
				return Widget.Container.ContainerObject;
			}
			protected set {
				base.Control = value;
			}
		}
		
		public Padding Padding {
			get {
				return padding;
			}
			set {
				padding = value;
				SWF.Control parent = (SWF.Control)Widget.Container.ContainerObject;
				if (parent != null) {
					parent.Padding = Generator.Convert (padding);
				}
				/*if (child != null) {
					SWF.Control c = (SWF.Control)child.ControlObject;
					c.Margin = Generator.Convert (padding);
				}*/
			}
		}
		
		public Control Content {
			get { return (Widget.Container == null) ? childToAdd : this.child; }
			set {
				if (Widget.Container == null) {
					childToAdd = value;
					return;
				}
				if (child == value)
					return;
				SWF.Control parent = (SWF.Control)Widget.Container.ContainerObject;
				parent.SuspendLayout ();
	
				SWF.Control childControl;

				if (value != null) {
					childControl = (SWF.Control)value.ControlObject;
					childControl.Dock = SWF.DockStyle.Fill;
					//childControl.Margin = Generator.Convert (padding);
					parent.Padding = Generator.Convert (padding);
					if (!childControl.AutoSize)
						parent.MinimumSize = childControl.Size;
					parent.Controls.Add (childControl);
				}
	
				if (this.child != null) {
					childControl = (SWF.Control)this.child.ControlObject;
					parent.Controls.Remove (childControl);
				}
	
				this.child = value;
				parent.ResumeLayout ();
			}
		}

		public override void AttachedToContainer ()
		{
			base.AttachedToContainer ();
			if (childToAdd != null)
				this.Content = childToAdd;
		}

	}
}