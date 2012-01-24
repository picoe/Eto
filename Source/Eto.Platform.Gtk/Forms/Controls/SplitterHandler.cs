using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class SplitterHandler : GtkControl<Gtk.Paned, Splitter>, ISplitter
	{
		Control panel1;
		Control panel2;
		SplitterOrientation orientation;
		SplitterFixedPanel fixedPanel;
		int? position;

		public SplitterHandler ()
		{
			Control = new Gtk.HPaned ();
		}

		public int Position {
			get { return Control.Position; }
			set { position = Control.Position = value; }
		}
		
		public SplitterFixedPanel FixedPanel {
			get { return fixedPanel; }
			set {
				fixedPanel = value;
				CreateControl ();
			}
		}

		public SplitterOrientation Orientation {
			get	{ return (Control is Gtk.HPaned) ? SplitterOrientation.Horizontal : SplitterOrientation.Vertical; }
			set {
				orientation = value;
				CreateControl ();
			}
		}
		
		void CreateControl ()
		{
			Gtk.Paned old = Control;
			switch (orientation) {
			default:
			case SplitterOrientation.Horizontal:
				Control = new Gtk.HPaned ();
				break;
			case SplitterOrientation.Vertical:
				Control = new Gtk.VPaned ();
				break;
			}
			if (old != null) {
				if (old.Parent != null) Control.Parent = old.Parent;
				if (old.Child1 != null) Control.Pack1 (old.Child1, fixedPanel != SplitterFixedPanel.Panel1, true);
				if (old.Child2 != null) Control.Pack2 (old.Child2, fixedPanel != SplitterFixedPanel.Panel2, true);
				old.Destroy ();
			}
		}

		public Control Panel1 {
			get { return panel1; }
			set {
				panel1 = value;
				var setposition = position != null && (Control.Child1 == null || Control.Child2 == null);
				if (Control.Child1 != null)
					Control.Remove (Control.Child1);
				if (panel1 != null) {
					var widget = (Gtk.Widget)panel1.ControlObject;
					Control.Pack1 (widget, fixedPanel != SplitterFixedPanel.Panel1, true);
					if (setposition) Control.Position = position.Value;
					widget.ShowAll ();
				}
			}
		}

		public Control Panel2 {
			get { return panel2; }
			set {
				panel2 = value;
				var setposition = position != null && (Control.Child1 == null || Control.Child2 == null);
				if (Control.Child2 != null)
					Control.Remove (Control.Child2);
				if (panel2 != null) {
					var widget = (Gtk.Widget)panel2.ControlObject;
					Control.Pack2 (widget, fixedPanel != SplitterFixedPanel.Panel2, true);
					if (setposition) Control.Position = position.Value;
					widget.ShowAll ();
				}
			}
		}
	}
}
