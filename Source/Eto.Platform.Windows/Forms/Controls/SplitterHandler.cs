using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class SplitterHandler : WindowsControl<System.Windows.Forms.SplitContainer, Splitter>, ISplitter
	{
		Control panel1;
		Control panel2;
		int? position;
		
		public SplitterHandler ()
		{
			Control = new SWF.SplitContainer ();
			Control.FixedPanel = SWF.FixedPanel.Panel1;
			Control.Panel1MinSize = 0;
			Control.Panel2MinSize = 0;
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler)
			{
				case Eto.Forms.Splitter.SplitterMovedEvent:
					// Hook SplitterMoving, not SplitterMoved,
					// because the latter fires even when the
					// splitter distance is changed programmatically.
					Control.SplitterMoving += (s, e) => {
						Widget.OnSplitterMoved(e);
					};
					break;
				default:
					base.AttachEvent(handler);
					break;
			}
		}
		public int Position {
			get { return Control.SplitterDistance; }
			set { position = value; Control.SplitterDistance = value; }
		}

		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);
			if (position != null) {
				Control.SplitterDistance = position.Value;
			}
			Control.Panel1Collapsed = panel1 == null || !(panel1.GetWindowsControl()).InternalVisible;
			Control.Panel2Collapsed = panel2 == null || !(panel2.GetWindowsControl()).InternalVisible;
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
					Control.FixedPanel = SWF.FixedPanel.None;
					break;
				case SplitterFixedPanel.Panel1:
					Control.FixedPanel = SWF.FixedPanel.Panel1;
					break;
				case SplitterFixedPanel.Panel2:
					Control.FixedPanel = SWF.FixedPanel.Panel2;
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
					return SplitterOrientation.Vertical;
				case SWF.Orientation.Vertical:
					return SplitterOrientation.Horizontal;
				}
			}
			set {
				switch (value) {
				default:
				case SplitterOrientation.Horizontal:
					Control.Orientation = SWF.Orientation.Vertical;
					break;
				case SplitterOrientation.Vertical:
					Control.Orientation = SWF.Orientation.Horizontal;
					break;
				}
			}
		}

		public Control Panel1 {
			get { return panel1; }
			set {
				if (panel1 != value) {
					Control.SuspendLayout ();
					if (panel1 != null) {
						SWF.Control c = panel1.GetSwfControl();
						c.VisibleChanged -= c1_VisibleChanged;
					}
					panel1 = value;
					Control.Panel1.Controls.Clear ();
					if (panel1 != null) {
						SWF.Control c = panel1.GetSwfControl();
						c.Dock = SWF.DockStyle.Fill;
						c.VisibleChanged += c1_VisibleChanged;
						Control.Panel1.Controls.Add (panel1.GetSwfControl());
					}
					if (Widget.Loaded)
						Control.Panel1Collapsed = panel1 == null || !(panel1.GetWindowsControl()).InternalVisible;
					Control.ResumeLayout ();
				}
			}
		}

		public Control Panel2 {
			get { return panel2; }
			set {
				if (panel2 != value) {
					Control.SuspendLayout ();
					if (panel2 != null) {
						SWF.Control c = panel2.GetSwfControl();
						c.VisibleChanged -= c2_VisibleChanged;
					}
					panel2 = value;
					Control.Panel2.Controls.Clear ();
					if (panel2 != null) {
						SWF.Control c = panel2.GetSwfControl();
						c.Dock = SWF.DockStyle.Fill;
						c.VisibleChanged += c2_VisibleChanged;
						Control.Panel2.Controls.Add (panel2.GetSwfControl());
					}
					if (Widget.Loaded)
						Control.Panel2Collapsed = panel2 == null || !(panel2.GetWindowsControl()).InternalVisible;
					Control.ResumeLayout ();
				}
			}
		}

		void c1_VisibleChanged (object sender, EventArgs e)
		{
			
			if (panel1 != null && (panel1.GetWindowsControl()).InternalVisible)
				Control.Panel1Collapsed = false;
			else
				Control.Panel1Collapsed = true;
		}

		void c2_VisibleChanged (object sender, EventArgs e)
		{
			if (panel2 != null && (panel2.GetWindowsControl()).InternalVisible)
				Control.Panel2Collapsed = false;
			else
				Control.Panel2Collapsed = true;
		}
	}
}
