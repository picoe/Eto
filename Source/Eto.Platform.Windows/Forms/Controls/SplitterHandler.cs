using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class SplitterHandler : WindowsControl<System.Windows.Forms.SplitContainer, Splitter>, ISplitter
	{
		Control panel1 = null;
		Control panel2 = null;

		
		public SplitterHandler ()
		{
			Control = new SWF.SplitContainer ();
			Control.FixedPanel = SWF.FixedPanel.Panel1;
			Control.Panel1MinSize = 0;
			Control.Panel2MinSize = 0;
		}
		
		#region ISplitter Members

		public int Position {
			get { return Control.SplitterDistance; }
			set { Control.SplitterDistance = value; }
		}
		
		public SplitterFixedPanel FixedPanel {
			get { 
				switch (Control.FixedPanel) {
				case SWF.FixedPanel.None:
					return SplitterFixedPanel.None;
				case SWF.FixedPanel.Panel1:
					return SplitterFixedPanel.Panel1;
				case SWF.FixedPanel.Panel2:
					return SplitterFixedPanel.Panel2;
				default:
					throw new NotSupportedException ();
				}
			}
			set {
				switch (value) {
				case SplitterFixedPanel.None:
					Control.FixedPanel = System.Windows.Forms.FixedPanel.None;
					break;
				case SplitterFixedPanel.Panel1:
					Control.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
					break;
				case SplitterFixedPanel.Panel2:
					Control.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
					break;
				default:
					throw new NotSupportedException ();
				}
			}
		}

		public SplitterOrientation Orientation {
			get {
				switch (Control.Orientation) {
				default:
				case SWF.Orientation.Horizontal:
					return SplitterOrientation.Horizontal;
				case SWF.Orientation.Vertical:
					return SplitterOrientation.Vertical;
				}
			}
			set {
				switch (value) {
				default:
				case SplitterOrientation.Horizontal:
					Control.Orientation = SWF.Orientation.Horizontal;
					break;
				case SplitterOrientation.Vertical:
					Control.Orientation = SWF.Orientation.Vertical;
					break;
				}
			}
		}

		public Control Panel1 {
			get { return panel1; }
			set {
				if (panel1 != value) {
					if (panel1 != null) {
						SWF.Control c = (SWF.Control)panel1.ControlObject;
						c.VisibleChanged -= c1_VisibleChanged;
					}
					panel1 = value;
					Control.Panel1.Controls.Clear ();
					if (panel1 != null) {
						SWF.Control c = (SWF.Control)panel1.ControlObject;
						c.Dock = SWF.DockStyle.Fill;
						c.VisibleChanged += c1_VisibleChanged;
						Control.Panel1.Controls.Add ((SWF.Control)panel1.ControlObject);
					}
					Control.Panel1Collapsed = panel1 == null || !panel1.Visible;
				}
			}
		}

		public Control Panel2 {
			get { return panel2; }
			set {
				if (panel2 != value) {
					if (panel2 != null) {
						SWF.Control c = (SWF.Control)panel2.ControlObject;
						c.VisibleChanged -= c2_VisibleChanged;
					}
					panel2 = value;
					Control.Panel2.Controls.Clear ();
					if (panel2 != null) {
						SWF.Control c = (SWF.Control)panel2.ControlObject;
						c.Dock = SWF.DockStyle.Fill;
						c.VisibleChanged += c2_VisibleChanged;
						Control.Panel2.Controls.Add ((SWF.Control)panel2.ControlObject);
					}
					Control.Panel2Collapsed = panel2 == null || !panel2.Visible;
				}
			}
		}

		void c1_VisibleChanged (object sender, EventArgs e)
		{
			
			if (panel1 != null && ((IWindowsControl)panel1.Handler).InternalVisible)
				Control.Panel1Collapsed = false;
			else
				Control.Panel1Collapsed = true;
		}

		void c2_VisibleChanged (object sender, EventArgs e)
		{
			if (panel2 != null && ((IWindowsControl)panel2.Handler).InternalVisible)
				Control.Panel2Collapsed = false;
			else
				Control.Panel2Collapsed = true;
		}

		#endregion
	}
}
