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
		double relative = double.NaN;
		int suppressSplitterMoved;

		private int _prefer(int width1, int width2)
		{
			if (position.HasValue)
				width1 = position.Value;
			else
			{
				if (!double.IsNaN(relative))
				{
					if (fixedPanel == SplitterFixedPanel.Panel1)
						width1 = (int)Math.Round(relative);
					else if (fixedPanel == SplitterFixedPanel.Panel2)
						width2 = (int)Math.Round(relative);
					else if (relative <= 0.0)
						width1 = 0;
					else if (relative >= 1.0)
						width2 = 0;
					else
					{
						// both get at least the preferred size
						return (int)Math.Round(Math.Max(
							width1 / relative, width2 / (1 - relative)));
					}
				}
			}
			return width1 + width2 + SplitterWidth;
		}

		class EtoHPaned : Gtk.HPaned
		{
			public SplitterHandler Handler { get; set; }

			#if GTK2
			protected override void OnSizeRequested(ref Gtk.Requisition requisition)
			{
				var size1 = Child1.SizeRequest();
				var size2 = Child2.SizeRequest();
				requisition.Height = Math.Max(size1.Height, size2.Height);
				requisition.Width = Handler._prefer(size1.Width, size2.Width);
			}
			#else
			protected override void OnGetPreferredHeightForWidth(int width, out int minimum_height, out int natural_height)
			{
				base.OnGetPreferredHeightForWidth(width, out minimum_height, out natural_height);
				minimum_height = 0;
			}

			protected override void OnGetPreferredWidth(out int minimum_width, out int natural_width)
			{
				int min1, width1, min2, width2, sw = Handler.SplitterWidth;
				Child1.GetPreferredWidth(out min1, out width1);
				Child2.GetPreferredWidth(out min2, out width2);
				minimum_width = Handler._prefer(min1, min2);
				natural_width = Handler._prefer(width1, width2);
			}
			protected override void OnGetPreferredWidthForHeight(int height, out int minimum_width, out int natural_width)
			{
				int min1, width1, min2, width2, sw = Handler.SplitterWidth;
				Child1.GetPreferredWidthForHeight(height, out min1, out width1);
				Child2.GetPreferredWidthForHeight(height, out min2, out width2);
				minimum_width = Handler._prefer(min1, min2);
				natural_width = Handler._prefer(width1, width2);
			}
			#endif

			protected override void OnSizeAllocated(Gdk.Rectangle allocation)
			{
				var it = Handler;
				if (it == null || double.IsNaN(it.relative))
				{
					base.OnSizeAllocated(allocation);
					return;
				}
				it.suppressSplitterMoved++;
				base.OnSizeAllocated(allocation);
				it.SetRelative(it.relative);
				it.suppressSplitterMoved--;
			}
		}

		class EtoVPaned : Gtk.VPaned
		{
			public SplitterHandler Handler { get; set; }

			#if GTK2
			protected override void OnSizeRequested(ref Gtk.Requisition requisition)
			{
				var size1 = Child1.SizeRequest();
				var size2 = Child2.SizeRequest();
				requisition.Width = Math.Max(size1.Width, size2.Width);
				requisition.Height = Handler._prefer(size1.Height, size2.Height);
			}
			#else
			protected override void OnGetPreferredWidthForHeight(int height, out int minimum_width, out int natural_width)
			{
				base.OnGetPreferredWidthForHeight(height, out minimum_width, out natural_width);
				minimum_width = 0;
			}

			protected override void OnGetPreferredHeight(out int minimum_height, out int natural_height)
			{
				int min1, height1, min2, height2, sw = Handler.SplitterWidth;
				Child1.GetPreferredHeight(out min1, out height1);
				Child2.GetPreferredHeight(out min2, out height2);
				minimum_height = Handler._prefer(min1, min2);
				natural_height = Handler._prefer(height1, height2);
			}
			protected override void OnGetPreferredHeightForWidth(int width, out int minimum_height, out int natural_height)
			{
				int min1, height1, min2, height2, sw = Handler.SplitterWidth;
				Child1.GetPreferredHeightForWidth(width, out min1, out height1);
				Child2.GetPreferredHeightForWidth(width, out min2, out height2);
				minimum_height = Handler._prefer(min1, min2);
				natural_height = Handler._prefer(height1, height2);
			}
			#endif

			protected override void OnSizeAllocated(Gdk.Rectangle allocation)
			{
				var it = Handler;
				if (it == null || double.IsNaN(it.relative))
				{
					base.OnSizeAllocated(allocation);
					return;
				}
				it.suppressSplitterMoved++;
				base.OnSizeAllocated(allocation);
				it.SetRelative(it.relative);
				it.suppressSplitterMoved--;
			}
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
			set
			{
				if (value != position)
				{
					position = value;
					relative = double.NaN;
					if (Control.IsRealized)
						SetPosition(value);
				}
			}
		}

		public int SplitterWidth
		{
			get { return 5; /* = default; Control.StyleGetProperty("handle-size") as int ? or gutter_size? */; }
			set { /* not implemented */ }
		}

		int GetAvailableSize()
		{
			return GetAvailableSize(!Control.IsRealized);
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

		void UpdateRelative()
		{
			var pos = Position;
			if (fixedPanel == SplitterFixedPanel.Panel1)
				relative = pos;
			else
			{
				var sz = GetAvailableSize();
				if (fixedPanel == SplitterFixedPanel.Panel2)
					relative = sz <= 0 ? 0 : sz - pos;
				else
					relative = sz <= 0 ? 0.5 : pos / (double)sz;
			}
		}

		public double RelativePosition
		{
			get
			{
				if (double.IsNaN(relative))
					UpdateRelative();
				return relative;
			}
			set
			{
				if (relative == value)
					return;
				relative = value;
				position = null;
				if (Control.IsRealized)
					SetRelative(value);
				Callback.OnPositionChanged(Widget, EventArgs.Empty);
			}
		}

		void SetPosition(int newPosition)
		{
			position = null;
			var size = GetAvailableSize();
			relative = fixedPanel == SplitterFixedPanel.Panel1 ? Math.Max(0, newPosition)
				: fixedPanel == SplitterFixedPanel.Panel2 ? Math.Max(0, size - newPosition)
				: size <= 0 ? 0.5 : Math.Max(0.0, Math.Min(1.0, newPosition / (double)size));
			Control.Position = newPosition;
		}
		void SetRelative(double newRelative)
		{
			position = null;
			relative = newRelative;
			var size = GetAvailableSize();
			if (size <= 0)
				return;
			switch (fixedPanel)
			{
				case SplitterFixedPanel.Panel1:
					Control.Position = Math.Max(0, Math.Min(size, (int)Math.Round(relative)));
					break;
				case SplitterFixedPanel.Panel2:
					Control.Position = Math.Max(0, Math.Min(size, size - (int)Math.Round(relative)));
					break;
				case SplitterFixedPanel.None:
					Control.Position = Math.Max(0, Math.Min(size, (int)Math.Round(size * relative)));
					break;
			}
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
				Control = new EtoHPaned() { Handler = this };
			else
				Control = new EtoVPaned() { Handler = this };
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
			Control.Realized += (sender, e) =>
			{
				SetInitialPosition();
				HookEvents();
			};
		}

		void HookEvents()
		{
			Control.AddNotification("position", (o, args) =>
			{
				if (Widget.ParentWindow == null || !Widget.Loaded || suppressSplitterMoved > 0)
					return;
				// keep track of the desired position (for removing/re-adding/resizing the control)
				UpdateRelative();
				Callback.OnPositionChanged(Widget, EventArgs.Empty);
			});
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Splitter.PositionChangedEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			suppressSplitterMoved++;
			if (Control.IsRealized)
				SetInitialPosition();
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			suppressSplitterMoved--;
		}

		void SetInitialPosition()
		{
			suppressSplitterMoved++;
			try
			{
				if (position != null)
				{
					var pos = position.Value;
					if (fixedPanel != SplitterFixedPanel.Panel1)
					{
						var size = GetAvailableSize(false);
						var want = GetAvailableSize(true);
						if (size != want)
						{
							if (FixedPanel == SplitterFixedPanel.Panel2)
								pos += size - want;
							else
							{
								SetRelative(pos / (double)want);
								return;
							}
						}

					}
					SetPosition(pos);
				}
				else if (!double.IsNaN(relative))
				{
					SetRelative(relative);
				}
				else if (fixedPanel == SplitterFixedPanel.Panel1)
				{
					var size1 = Control.Child1.GetPreferredSize();
					SetRelative(Orientation == SplitterOrientation.Horizontal ? size1.Width : size1.Height);
				}
				else if (fixedPanel == SplitterFixedPanel.Panel2)
				{
					var size2 = Control.Child2.GetPreferredSize();
					SetRelative(Orientation == SplitterOrientation.Horizontal ? size2.Width : size2.Height);
				}
				else
				{
					var size1 = Control.Child1.GetPreferredSize();
					var size2 = Control.Child2.GetPreferredSize();
					SetRelative(Orientation == SplitterOrientation.Horizontal
						? size1.Width / (double)(size1.Width + size2.Width)
						: size1.Height / (double)(size1.Height + size2.Height));
				}
			}
			finally
			{
				suppressSplitterMoved--;
			}

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
