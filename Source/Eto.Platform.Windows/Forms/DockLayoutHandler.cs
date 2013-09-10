using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Windows
{
	public class DockLayoutHandler : WindowsLayout<SWF.TableLayoutPanel, DockLayout>, IDockLayout
	{
		Control child;
		Padding padding;

		
		public DockLayoutHandler ()
		{
			padding = DockLayout.DefaultPadding;
			Control = new SWF.TableLayoutPanel {
				Dock = SWF.DockStyle.Fill,
				RowCount = 1,
				ColumnCount = 1,
				Size = SD.Size.Empty,
				AutoSizeMode = SWF.AutoSizeMode.GrowAndShrink,
				AutoSize = true
			};
			Control.ColumnStyles.Add (new SWF.ColumnStyle (SWF.SizeType.Percent, 1));
			Control.RowStyles.Add (new SWF.RowStyle (SWF.SizeType.Percent, 1));
			Padding = DockLayout.DefaultPadding;
		}

		public override object LayoutObject
		{
			get { return Control; }
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
				return container != null ? container.PreferredSize.ToEto () : Size.Empty;
			}
		}

		public override void SetScale (bool xscale, bool yscale)
		{
			base.SetScale (xscale, yscale);
			if (child != null)
				child.SetScale (xscale, yscale);
		}


		public Padding Padding {
			get { return Control.Padding.ToEto (); }
			set { Control.Padding = value.ToSWF (); }
		}
		
		public Control Content {
			get { return child; }
			set {
				// Always add value to Control
				// even if it is currently added.

				Control.SuspendLayout ();
	
				SWF.Control childControl;

				if (this.child != null)
				{
					child.SetScale(false, false);
					childControl = this.child.GetContainerControl();
					Control.Controls.Remove(childControl);
				}
	
				if (value != null) {
					childControl = value.GetContainerControl();
					childControl.Dock = SWF.DockStyle.Fill;
					value.SetScale (XScale, YScale);
					Control.Controls.Add (childControl, 0, 0);
				}
	
				this.child = value;
				Control.ResumeLayout ();
			}
		}
	}
}