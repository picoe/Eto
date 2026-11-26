namespace Eto.Mac.Forms
{
	public class PopoverHandler : WidgetHandler<NSPopover, Popover, Popover.ICallback>, Popover.IHandler
	{
		Control _content;
		EtoPopoverDelegate _delegate;

		class EtoPopoverDelegate : NSPopoverDelegate
		{
			readonly WeakReference _handler;

			public EtoPopoverDelegate(PopoverHandler handler) => _handler = new WeakReference(handler);

			PopoverHandler Handler => _handler.Target as PopoverHandler;

			public override void DidShow(NSNotification notification)
			{
				var h = Handler;
				h?.Callback.OnShown(h.Widget, EventArgs.Empty);
			}

			public override void DidClose(NSNotification notification) => Handler?.OnDidClose();
		}

		protected override NSPopover CreateControl() => new NSPopover
		{
			Behavior = NSPopoverBehavior.Transient,
			Animates = true
		};

		protected override void Initialize()
		{
			base.Initialize();
			Control.Delegate = _delegate = new EtoPopoverDelegate(this);
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
				if (Control.Shown)
				{
					old?.DetachNative();
					SetContentView();
				}
			}
		}

		public PopoverBehavior Behavior
		{
			get => Control.Behavior switch
			{
				NSPopoverBehavior.Semitransient => PopoverBehavior.Semitransient,
				NSPopoverBehavior.ApplicationDefined => PopoverBehavior.ApplicationDefined,
				_ => PopoverBehavior.Transient
			};
			set => Control.Behavior = value switch
			{
				PopoverBehavior.Semitransient => NSPopoverBehavior.Semitransient,
				PopoverBehavior.ApplicationDefined => NSPopoverBehavior.ApplicationDefined,
				_ => NSPopoverBehavior.Transient
			};
		}

		public bool IsOpen => Control.Shown;

		public void Show(Control relativeTo, RectangleF? rectangle, PopoverPosition position)
		{
			var view = relativeTo.GetContainerView();
			if (view == null)
				throw new ArgumentException("The control has no native view to show the popover from", nameof(relativeTo));

			if (!Control.Shown)
				SetContentView();

			var rect = (rectangle ?? new RectangleF(relativeTo.Size)).ToNS();
			if (!view.IsFlipped)
				rect.Y = view.Bounds.Height - rect.Bottom;

			// edges are in the positioning view's coordinates, so which Y edge is on top depends on whether it is flipped
			var edge = position switch
			{
				PopoverPosition.Top => view.IsFlipped ? NSRectEdge.MinYEdge : NSRectEdge.MaxYEdge,
				PopoverPosition.Left => NSRectEdge.MinXEdge,
				PopoverPosition.Right => NSRectEdge.MaxXEdge,
				_ => view.IsFlipped ? NSRectEdge.MaxYEdge : NSRectEdge.MinYEdge
			};
			Control.Show(rect, view, edge);
		}

		public void Close() => Control.Close();

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

		void SetContentView()
		{
			var size = _content?.GetPreferredSize() ?? SizeF.Empty;
			var view = _content?.ToNative(true) ?? new NSView();
			view.Frame = new CGRect(CGPoint.Empty, size.ToNS());
			Control.ContentViewController = new NSViewController { View = view };
			Control.ContentSize = size.ToNS();
		}

		void OnDidClose()
		{
			// unload the content so it can be attached again the next time it's shown
			_content?.DetachNative();
			Callback.OnClosed(Widget, EventArgs.Empty);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && Control != null)
			{
				if (Control.Shown)
					Control.Close();
				Control.Delegate = null;
				_delegate = null;
			}
			base.Dispose(disposing);
		}
	}
}
