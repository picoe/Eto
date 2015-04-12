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
		SplitterPositionMode mode;
		int? position;
		bool created;

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
			get
			{
				switch (mode)
				{
					default:
						return Control.Position;
					case SplitterPositionMode.Far:
						return Math.Max(0, (Orientation == SplitterOrientation.Horizontal ?
							Control.Allocation.Width : Control.Allocation.Height) - Control.Position - SplitterWidth);
					case SplitterPositionMode.Percent:
						var width = (Orientation == SplitterOrientation.Horizontal ?
							Control.Allocation.Width : Control.Allocation.Height) - SplitterWidth;
						return width <= 0 ? 0 : Control.Position * 100 / width;
				}
			}
			set
			{
				position = Control.Position = value;
			}
		}

		int GetAvailableSize()
		{
			return GetAvailableSize(!created);
		}
		int GetAvailableSize(bool desired)
		{
			if (desired)
			{
				var size = PreferredSize;
				var pick = Orientation == SplitterOrientation.Horizontal ?
					size.Width : size.Height;
				if (pick >= 0)
					return pick - SplitterWidth;
			}
			return (Orientation == SplitterOrientation.Horizontal ?
				Control.Allocation.Width : Control.Allocation.Height) - SplitterWidth;
		}

		void SetPosition(int pos, SplitterPositionMode mode)
		{
			int size = GetAvailableSize(false);
			if (mode == SplitterPositionMode.Far)
				pos = size - pos;
			else if (mode == SplitterPositionMode.Percent)
				pos = (1 + 2 * size * pos) / 200;
			Control.Position = Math.Max(0, Math.Min(size, pos));
		}

		public SplitterPositionMode PositionMode
		{
			get { return mode; }
			set { mode = value; }
		}

		public int SplitterWidth
		{
			get { return 5; /* just u guess :( */ }
			set { /* unfortunatelly I cannot see binding for gtk_set_gutter_size in Gtk# */ }
		}

		public SplitterFixedPanel FixedPanel
		{
			get { return fixedPanel; }
			set
			{
				if (fixedPanel != value)
				{
					fixedPanel = value;
					((Gtk.Paned.PanedChild)Control[Control.Child1]).Resize = value != SplitterFixedPanel.Panel1;
					((Gtk.Paned.PanedChild)Control[Control.Child2]).Resize = value != SplitterFixedPanel.Panel2;
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

			created = false;
			Control.SizeAllocated += Control_SizeAllocated;
		}

		void SetInitialPosition()
		{
			if (position != null)
			{
				var pos = position.Value;
				if (mode != SplitterPositionMode.Percent &&
					(FixedPanel == SplitterFixedPanel.None ||
					(FixedPanel == SplitterFixedPanel.Panel1) != (mode == SplitterPositionMode.Near)))
				{
					var size = GetAvailableSize(false);
					var want = GetAvailableSize(true);
					var diff = size - want;
					if (diff != 0)
					{
						if (FixedPanel == SplitterFixedPanel.None)
							pos = pos * size / want;
						else
							pos += mode == SplitterPositionMode.Near ? diff : -diff;
					}
				}
				SetPosition(pos, mode);
				return;
			}
			var horiz = Orientation == SplitterOrientation.Horizontal;
			switch (FixedPanel)
			{
				case SplitterFixedPanel.Panel1:
					var size1 = Control.Child1.SizeRequest();
					SetPosition(horiz ? size1.Width : size1.Height, SplitterPositionMode.Near);
					break;
				case SplitterFixedPanel.Panel2:
					var size2 = Control.Child2.SizeRequest();
					SetPosition(horiz ? size2.Width : size2.Height, SplitterPositionMode.Far);
					break;
				default:
					var sone = Control.Child1.SizeRequest();
					var stwo = Control.Child2.SizeRequest();
					var one = horiz ? sone.Width : sone.Height;
					var two = horiz ? stwo.Width : stwo.Height;
					SetPosition(one * GetAvailableSize(true) / (one + two), SplitterPositionMode.Near);
					break;
			}
		}


		void Control_SizeAllocated(object o, Gtk.SizeAllocatedArgs args)
		{
			Control.SizeAllocated -= Control_SizeAllocated;
			SetInitialPosition();
			created = true;
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
