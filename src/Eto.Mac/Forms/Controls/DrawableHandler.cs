using Eto.Mac.Drawing;

namespace Eto.Mac.Forms.Controls
{
	public class DrawableHandler : MacPanel<DrawableHandler.EtoDrawableView, Drawable, Drawable.ICallback>, Drawable.IHandler, IMacTextInputHandler
	{
		Brush backgroundBrush;
		Color backgroundColor;
		string markedText = string.Empty;
		NSRange selectedRange;
		bool isComposing;

		public bool SupportsCreateGraphics => false;

		public override NSView ContainerControl => Control;

		public class EtoDrawableView : MacPanelView
		{
			DrawableHandler Drawable => Handler as DrawableHandler;
			
			public EtoDrawableView()
			{
			}

			public EtoDrawableView(NativeHandle handle) : base(handle)
			{
			}

			public override void DrawRect(CGRect dirtyRect)
			{
				var drawable = Drawable;
				if (drawable == null)
					return;

				ApplicationHandler.QueueResizing = true;
				drawable.DrawRegion(dirtyRect);
				ApplicationHandler.QueueResizing = false;
			}

			// public override bool IsFlipped => true;  // uncomment to test flipped views with GraphicsHandler.

			public bool CanFocus { get; set; }

			public override bool AcceptsFirstResponder() => CanFocus && Drawable?.Enabled == true;

			public override NSView HitTest(CGPoint aPoint)
			{
				var view = base.HitTest(aPoint);
				if (view == ContentView)
				{
					// forward all events to this view, not the content view (which covers the drawable)
					// the properly enables AcceptsFirstMouse above, since the ContentView returns false
					return this;
				}
				return view;
			}
		}

		public Graphics CreateGraphics()
		{
			throw new NotSupportedException();
		}

		public override Color BackgroundColor
		{
			get { return backgroundColor; }
			set
			{
				if (backgroundColor != value)
				{
					backgroundColor = value;
					backgroundBrush = backgroundColor.A > 0 ? new SolidBrush(backgroundColor) : null;
					Invalidate(false);
				}
			}
		}

		public void Create()
		{
			Enabled = true;
			Control = new EtoDrawableView { Handler = this };
		}

		public void Create(bool largeCanvas)
		{
			Create();
		}

		public bool CanFocus
		{
			get { return Control.CanFocus; }
			set { Control.CanFocus = value; }
		}

		public override void Invalidate(bool invalidateChildren)
		{
			if (NeedsQueue)
			{
				AsyncQueue.Add(() => Invalidate(invalidateChildren));
				return;
			}
			base.Invalidate(invalidateChildren);
		}

		public override void Invalidate(Rectangle rect, bool invalidateChildren)
		{
			if (NeedsQueue)
			{
				AsyncQueue.Add(() => Invalidate(rect, invalidateChildren));
				return;
			}
			base.Invalidate(rect, invalidateChildren);
		}

		void DrawRegion(CGRect dirtyRect)
		{
			var context = NSGraphicsContext.CurrentContext;
			if (context == null)
				return;
				
			var bounds = Control.Bounds;

			// restrict dirty rect to the bounds of the drawable
			// macOS can give us dirty rects outside this range
			var dirty = dirtyRect.ToEto();
			dirty.Restrict(bounds.ToEto());

			var handler = new GraphicsHandler(Control, context, (float)bounds.Height, dirty.ToNS());

			// dirty rect should be flipped when passed to Drawabe.Paint event
			if (!Control.IsFlipped)
				dirty.Y = (float)(bounds.Height - dirty.Y - dirty.Height);
			
			using (var graphics = new Graphics(handler))
			{
				if (backgroundBrush != null)
					graphics.FillRectangle(backgroundBrush, dirty);

				var widget = Widget;
				if (widget != null)
					Callback.OnPaint(widget, new PaintEventArgs(graphics, dirty));
			}
		}

		public void Update(Rectangle rect)
		{
			Control.DisplayRect(rect.ToNS());
		}

		public void CancelTextComposition()
		{
			Control.InputContext?.DiscardMarkedText();
			EndComposition(raiseEvent: true);
		}

		public void CommitTextComposition()
		{
			if (!isComposing && string.IsNullOrEmpty(markedText))
				return;

			var text = markedText;
			EndComposition(raiseEvent: true);
			Control.InputContext?.DiscardMarkedText();

			if (string.IsNullOrEmpty(text))
				return;

			var args = new TextInputEventArgs(text);
			Callback.OnTextInput(Widget, args);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Drawable.TextCompositionEvent:
				case Drawable.TextInsertionBoundsRequestedEvent:
					if (EnsureTextInputImplemented())
						HandleEvent(Eto.Forms.Control.KeyDownEvent);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected override bool OnAcceptsFirstMouse(NSEvent theEvent)
		{
			if (CanFocus)
				return true;
			return base.OnAcceptsFirstMouse(theEvent);
		}

		public override void OnKeyDown(KeyEventArgs e)
		{
			if (isComposing)
			{
				// if we're composing, we need to handle the key events to properly update the composition text
				// however, we don't want to trigger any other key events (e.g. shortcuts) while composing, so we just return here
				return;
			}
			base.OnKeyDown(e);
		}

		bool IMacTextInputHandler.HasMarkedText => isComposing;

		NSRange IMacTextInputHandler.MarkedRange => isComposing ? new NSRange(0, markedText.Length) : new NSRange(NSRange.NotFound, 0);

		NSRange IMacTextInputHandler.SelectedRange => selectedRange;

		void IMacTextInputHandler.FinishComposition() => EndComposition(raiseEvent: true);

		void IMacTextInputHandler.SetMarkedText(string text, NSRange selectedRange, NSRange replacementRange)
		{
			markedText = text ?? string.Empty;
			this.selectedRange = selectedRange;

			isComposing = true;
			var args = new TextCompositionEventArgs(markedText, true);
			Callback.OnTextComposition(Widget, args);
			Invalidate(false);
		}

		void IMacTextInputHandler.UnmarkText()
		{
			EndComposition(raiseEvent: true);
		}

		CGRect IMacTextInputHandler.FirstRectForCharacterRange(NSRange range)
		{
			var view = Control;
			if (view == null)
				return CGRect.Empty;

			var args = new TextInsertionBoundsEventArgs();
			Callback.OnTextInsertionBoundsRequested(Widget, args);
			var localRect = args.Bounds ?? new RectangleF(0, 0, 1, (float)Math.Max(1, view.Bounds.Height));
			if (localRect.Width <= 0)
				localRect.Width = 1;
			if (localRect.Height <= 0)
				localRect.Height = 1;

			var rect = localRect.ToNS();
			if (!view.IsFlipped)
				rect.Y = view.Bounds.Height - rect.Y - rect.Height;

			rect = view.ConvertRectToView(rect, null);
			return view.Window?.ConvertRectToScreen(rect) ?? CGRect.Empty;
		}

		void EndComposition(bool raiseEvent)
		{
			if (!isComposing && string.IsNullOrEmpty(markedText))
				return;

			markedText = string.Empty;
			selectedRange = new NSRange(0, 0);
			isComposing = false;

			if (raiseEvent)
			{
				var args = new TextCompositionEventArgs(string.Empty, false);
				Callback.OnTextComposition(Widget, args);
			}

			Invalidate(false);
		}
	}
}
