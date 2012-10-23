using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Windows
{
	public class DockLayoutHandler : WindowsLayout<SWF.Control, DockLayout>, IDockLayout
	{
		Control child;
		Padding padding;
		Control childToAdd;

		
		public DockLayoutHandler ()
		{
			padding = DockLayout.DefaultPadding;
		}

		public override SWF.Control Control {
			get {
				return Widget.Container != null ? (SWF.Control)Widget.Container.ContainerObject : null;
			}
			protected set {
				base.Control = value;
			}
		}

		public override Size DesiredSize
		{
			get {
				if (child != null)
				{
					var handler = child.Handler as IWindowsControl;
					if (handler != null)
						return handler.DesiredSize;
				}
				var container = Widget.Container.GetContainerControl();
				return container != null ? Generator.Convert (container.PreferredSize) : Size.Empty;
			}
		}

		public override void SetScale (bool xscale, bool yscale)
		{
			base.SetScale (xscale, yscale);
			child.SetScale (xscale, yscale);
		}


		public Padding Padding {
			get {
				return padding;
			}
			set {
				padding = value;
				SWF.Control parent = Control;
				if (parent != null) {
					parent.Padding = Generator.Convert (padding);
				}
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
				SWF.Control parent = Control;
				parent.SuspendLayout ();
	
				SWF.Control childControl;

				if (value != null) {
					childControl = value.GetContainerControl();
					childControl.Dock = SWF.DockStyle.Fill;
					value.SetScale (XScale, YScale);
					parent.Padding = Generator.Convert (padding);
					parent.Controls.Add (childControl);
				}
	
				if (this.child != null) {
					child.SetScale (false, false);
					childControl = this.child.GetContainerControl ();
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