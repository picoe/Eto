using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class SplitterHandler : GtkControl<Gtk.Paned, Splitter>, ISplitter
	{
		Gtk.Alignment container;
		Control panel1;
		Control panel2;
		SplitterOrientation orientation;
		SplitterFixedPanel fixedPanel;
		int? position;

		public override Gtk.Widget ContainerControl
		{
			get { return container; }
		}

		public SplitterHandler ()
		{
			container = new Gtk.Alignment (0, 0, 1, 1);
			CreateControl ();
		}

		public int Position {
			get { return Control.Position; }
			set { position = Control.Position = value; }
		}
		
		public SplitterFixedPanel FixedPanel {
			get { return fixedPanel; }
			set {
				if (fixedPanel != value) {
					fixedPanel = value;
					CreateControl ();
				}
			}
		}

		public SplitterOrientation Orientation {
			get	{ return (Control is Gtk.HPaned) ? SplitterOrientation.Horizontal : SplitterOrientation.Vertical; }
			set {
				if (orientation != value) {
					orientation = value;
					CreateControl ();
				}
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
			if (container.Child != null)
				container.Remove (container.Child);
			container.Child = Control;
			if (old != null) {
				Control.Pack1 (old.Child1 ?? EmptyContainer (), fixedPanel != SplitterFixedPanel.Panel1, true);
				Control.Pack2 (old.Child2 ?? EmptyContainer (), fixedPanel != SplitterFixedPanel.Panel2, true);
				old.Destroy ();
			}
			else {
				Control.Pack1 (EmptyContainer (), fixedPanel != SplitterFixedPanel.Panel1, true);
				Control.Pack2 (EmptyContainer (), fixedPanel != SplitterFixedPanel.Panel2, true);
			}
		}
		
		Gtk.Widget EmptyContainer()
		{
			var bin = new Gtk.VBox();
			return bin;
		}

		public Control Panel1 {
			get { return panel1; }
			set {
				panel1 = value;
				var setposition = position != null && (Control.Child1 == null || Control.Child2 == null);
				if (Control.Child1 != null)
					Control.Remove (Control.Child1);
				var widget = panel1 != null ? panel1.GetContainerWidget () : EmptyContainer ();
				Control.Pack1 (widget, fixedPanel != SplitterFixedPanel.Panel1, true);
				if (setposition) Control.Position = position.Value;
				widget.ShowAll ();
			}
		}

		public Control Panel2 {
			get { return panel2; }
			set {
				panel2 = value;
				var setposition = position != null && (Control.Child1 == null || Control.Child2 == null);
				if (Control.Child2 != null)
					Control.Remove (Control.Child2);
				var widget = panel2 != null ? panel2.GetContainerWidget () : EmptyContainer ();
				Control.Pack2 (widget, fixedPanel != SplitterFixedPanel.Panel2, true);
				if (setposition) Control.Position = position.Value;
				widget.ShowAll ();
			}
		}
	}
}
