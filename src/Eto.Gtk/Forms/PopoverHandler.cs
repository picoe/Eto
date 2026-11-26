#if GTK3
namespace Eto.GtkSharp.Forms
{
	public class PopoverHandler : WidgetHandler<Gtk.Popover, Popover, Popover.ICallback>, Popover.IHandler
	{
		Control _content;
		PopoverBehavior _behavior;
		Gtk.Window _toplevel;
		bool _isOpen;
		bool _shownPending;

		protected override Gtk.Popover CreateControl() => new Gtk.Popover((Gtk.Widget)null);

		protected override void Initialize()
		{
			base.Initialize();
			Behavior = PopoverBehavior.Transient;
			Control.SizeAllocated += Connector.HandleSizeAllocated;
			Control.Closed += Connector.HandleClosed;
		}

		public Control Content
		{
			get => _content;
			set
			{
				if (ReferenceEquals(_content, value))
					return;
				var old = _content;
				_content = value;
				if (Control.Visible)
				{
					old?.DetachNative();
					SetChild();
				}
			}
		}

		public PopoverBehavior Behavior
		{
			get => _behavior;
			set
			{
				_behavior = value;
				// a modal popover closes when clicking anywhere else in the application
				Control.Modal = value != PopoverBehavior.ApplicationDefined;
			}
		}

		// GTK keeps the popover visible while it animates closed
		public bool IsOpen => _isOpen;

		public void Show(Control relativeTo, RectangleF? rectangle, PopoverPosition position)
		{
			var widget = relativeTo.GetContainerWidget();
			if (widget == null)
				throw new ArgumentException("The control has no native widget to show the popover from", nameof(relativeTo));

			SetChild();
			Control.RelativeTo = widget;
			Control.PointingTo = Rectangle.Round(rectangle ?? new RectangleF(relativeTo.Size)).ToGdk();
			Control.Position = position switch
			{
				PopoverPosition.Top => Gtk.PositionType.Top,
				PopoverPosition.Left => Gtk.PositionType.Left,
				PopoverPosition.Right => Gtk.PositionType.Right,
				_ => Gtk.PositionType.Bottom
			};
			SetToplevel(widget.Toplevel as Gtk.Window);
			if (!_isOpen)
			{
				// GTK only raises closed once per mapping, so finish closing before reopening
				if (Control.Visible)
					Control.Hide();
				_isOpen = true;
				_shownPending = true;
			}
			Control.Popup();
		}

		public void Close()
		{
			Control.Popdown();
			SetClosed();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Popover.ShownEvent:
				case Popover.ClosedEvent:
					// always raised
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		void SetChild()
		{
			var child = _content?.ToNative(true);
			if (ReferenceEquals(Control.Child, child))
				return;
			if (Control.Child != null)
				Control.Remove(Control.Child);
			if (child != null)
				Control.Add(child);
		}

		void SetToplevel(Gtk.Window toplevel)
		{
			if (ReferenceEquals(_toplevel, toplevel))
				return;
			if (_toplevel != null)
			{
				_toplevel.FocusOutEvent -= Connector.HandleToplevelFocusOut;
				_toplevel.Hidden -= Connector.HandleToplevelHidden;
			}
			_toplevel = toplevel;
			if (_toplevel != null)
			{
				_toplevel.FocusOutEvent += Connector.HandleToplevelFocusOut;
				_toplevel.Hidden += Connector.HandleToplevelHidden;
			}
		}

		void SetClosed()
		{
			if (!_isOpen)
				return;
			_isOpen = false;
			_shownPending = false;
			SetToplevel(null);
			// unload the content so it can be attached again the next time it's shown
			_content?.DetachNative();
			Callback.OnClosed(Widget, EventArgs.Empty);
		}

		protected new PopoverConnector Connector => (PopoverConnector)base.Connector;

		protected override WeakConnector CreateConnector() => new PopoverConnector();

		protected class PopoverConnector : WeakConnector
		{
			public new PopoverHandler Handler => (PopoverHandler)base.Handler;

			public void HandleSizeAllocated(object o, Gtk.SizeAllocatedArgs args)
			{
				// GTK shows the popover before it is sized and placed, so wait for that before raising Shown
				var h = Handler;
				if (h == null || !h._shownPending || args.Allocation.Width <= 1)
					return;
				h._shownPending = false;
				Application.Instance.AsyncInvoke(() =>
				{
					if (h._isOpen)
						h.Callback.OnShown(h.Widget, EventArgs.Empty);
				});
			}

			public void HandleClosed(object sender, EventArgs e) => Handler?.SetClosed();

			public void HandleToplevelHidden(object sender, EventArgs e)
			{
				// GTK doesn't raise closed when the window goes away
				var h = Handler;
				if (h == null)
					return;
				h.Control.Popdown();
				h.SetClosed();
			}

			public void HandleToplevelFocusOut(object o, Gtk.FocusOutEventArgs args)
			{
				// modal popovers only close for clicks inside the application, so close for other applications too
				var h = Handler;
				if (h?.Behavior == PopoverBehavior.Transient && h.Control.Visible)
					h.Control.Popdown();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				SetToplevel(null);
			base.Dispose(disposing);
		}
	}
}
#endif
