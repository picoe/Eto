using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class SplitterHandler : GtkControl<Gtk.Paned, Splitter>, ISplitter
	{
		Control panel1;
		Control panel2;

		public SplitterHandler()
		{
			Control = new Gtk.HPaned();
		}

		public int Position
		{
			get
			{
				return Control.Position;
			}
			set
			{
				Control.Position = value;
			}
		}

		public SplitterOrientation Orientation
		{
			get	{ return (Control is Gtk.HPaned) ? SplitterOrientation.Horizontal : SplitterOrientation.Vertical; }
			set
			{
				Gtk.Paned old = Control;
				switch (value)
				{
					default:
					case SplitterOrientation.Horizontal:
						Control = new Gtk.HPaned();
						break;
					case SplitterOrientation.Vertical:
						Control = new Gtk.VPaned();
						break;
				}
				if (old != null)
				{
					Control.Parent = old.Parent;
					Control.Add1(old.Child1);
					Control.Add2(old.Child2);
					old.Destroy();
				}
			}
		}

		public Control Panel1
		{
			get { return panel1; }
			set
			{
				panel1 = value;
				if (Control.Child1 != null) Control.Remove(Control.Child1);
				if (panel1 != null)
				{
					Control.Add1((Gtk.Widget)panel1.ControlObject);
				}
			}
		}

		public Control Panel2
		{
			get { return panel2; }
			set
			{
				panel2 = value;
				if (Control.Child2 != null) Control.Remove(Control.Child2);
				if (panel2 != null)
				{
					Control.Add2((Gtk.Widget)panel2.ControlObject);
				}
			}
		}
	}
}
