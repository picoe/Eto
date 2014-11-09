using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
	public class SplitterHandler : GtkContainer<Gtk.Paned, Splitter, Splitter.ICallback>, Splitter.IHandler
	{
		readonly Gtk.EventBox container;
		Control panel1;
		Control panel2;
		SplitterOrientation orientation;
		SplitterFixedPanel fixedPanel;
		int? position;

		class EtoHPaned : Gtk.HPaned
		{
			#if GTK2
			protected override void OnSizeRequested(ref Gtk.Requisition requisition)
			{
				base.OnSizeRequested(ref requisition);
				if (PositionSet && Child1 != null)
				{
					var childreq = Child1.Requisition;
					if (childreq.Width > 0)
						requisition.Width += Position - childreq.Width;
				}
			}
			#endif
		}

		class EtoVPaned : Gtk.VPaned
		{
			#if GTK2
			protected override void OnSizeRequested(ref Gtk.Requisition requisition)
			{
				base.OnSizeRequested(ref requisition);
				if (PositionSet && Child1 != null)
				{
					var childreq = Child1.Requisition;
					if (childreq.Height > 0)
						requisition.Height += Position - childreq.Height;
				}
			}
			#endif
		}

		public override Gtk.Widget ContainerControl
		{
			get { return container; }
		}

		public SplitterHandler()
		{
			container = new Gtk.EventBox();
			Create();
		}

		public int Position
		{
			get { return Control.Position; }
			set { position = Control.Position = value; }
		}

		public SplitterFixedPanel FixedPanel
		{
			get { return fixedPanel; }
			set
			{
				if (fixedPanel != value)
				{
					fixedPanel = value;
					Create();
				}
			}
		}

		public SplitterOrientation Orientation
		{
			get	{ return (Control is Gtk.HPaned) ? SplitterOrientation.Horizontal : SplitterOrientation.Vertical; }
			set
			{
				if (orientation != value)
				{
					orientation = value;
					Create();
				}
			}
		}

		void Create()
		{
			Gtk.Paned old = Control;
			if (orientation == SplitterOrientation.Horizontal)
				Control = new EtoHPaned();
			else
				Control = new EtoVPaned();
			if (container.Child != null)
				container.Remove(container.Child);
			container.Child = Control;
			if (old != null)
			{
				var child1 = old.Child1;
				var child2 = old.Child2;
				old.Remove(child2);
				old.Remove(child1);
				Control.Pack1(child1 ?? EmptyContainer(), fixedPanel != SplitterFixedPanel.Panel1, true);
				Control.Pack2(child2 ?? EmptyContainer(), fixedPanel != SplitterFixedPanel.Panel2, true);
				old.Destroy();
			}
			else
			{
				Control.Pack1(EmptyContainer(), fixedPanel != SplitterFixedPanel.Panel1, true);
				Control.Pack2(EmptyContainer(), fixedPanel != SplitterFixedPanel.Panel2, true);
			}
			if (position != null)
				Control.Position = position.Value;
		}

		static Gtk.Widget EmptyContainer()
		{
			var bin = new Gtk.VBox();
			return bin;
		}

		public Control Panel1
		{
			get { return panel1; }
			set
			{
				panel1 = value;
				var setposition = position != null && (Control.Child1 == null || Control.Child2 == null);
				if (Control.Child1 != null)
					Control.Remove(Control.Child1);
				var widget = panel1 != null ? panel1.GetContainerWidget() : EmptyContainer();
				Control.Pack1(widget, fixedPanel != SplitterFixedPanel.Panel1, true);
				if (setposition)
					Control.Position = position.Value;
				widget.ShowAll();
			}
		}

		public Control Panel2
		{
			get { return panel2; }
			set
			{
				panel2 = value;
				var setposition = position != null && (Control.Child1 == null || Control.Child2 == null);
				if (Control.Child2 != null)
					Control.Remove(Control.Child2);
				var widget = panel2 != null ? panel2.GetContainerWidget() : EmptyContainer();
				Control.Pack2(widget, fixedPanel != SplitterFixedPanel.Panel2, true);
				if (setposition)
					Control.Position = position.Value;
				widget.ShowAll();
			}
		}
	}
}
