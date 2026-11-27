namespace Eto.Forms.ThemedControls;

using Eto.Drawing;

/// <summary>
/// Themed handler for the <see cref="Popover"/>, drawn to look like the macOS popover, for platforms without a native one.
/// </summary>
/// <remarks>
/// Shows the content in a borderless window, so the platform must support transparent windows for the arrow and shadow to look right.
/// On Gtk under Wayland the window can't take keyboard focus, so text boxes in the content can't be typed into.
/// </remarks>
/// <since>2.14</since>
public class ThemedPopoverHandler : WidgetHandler<Widget, Popover, Popover.ICallback>, Popover.IHandler
{
	Control _content;
	Form _window;
	PopoverFrame _frame;
	Control _relativeTo;
	Window _owner;
	RectangleF? _rectangle;
	PopoverPosition _position;
	Color? _backgroundColor;
	Color? _borderColor;

	/// <summary>
	/// Gets or sets the fill color of the popover, or uses <see cref="SystemColors.Control"/> when not set, or <see cref="SystemColors.WindowBackground"/> where that isn't solid.
	/// </summary>
	/// <since>2.14</since>
	public Color BackgroundColor
	{
		get => _backgroundColor ?? DefaultBackgroundColor;
		set => _backgroundColor = value;
	}

	// macOS's control color is a translucent overlay, which would let whatever is behind the popover show through
	static Color DefaultBackgroundColor
	{
		get
		{
			var color = SystemColors.Control;
			return color.A < 1 ? SystemColors.WindowBackground : color;
		}
	}

	/// <summary>
	/// Gets or sets the color of the outline, or uses a faint <see cref="SystemColors.ControlText"/> when not set.
	/// </summary>
	/// <since>2.14</since>
	public Color BorderColor
	{
		get => _borderColor ?? new Color(SystemColors.ControlText, 0.15f);
		set => _borderColor = value;
	}

	/// <summary>
	/// Gets or sets the radius of the corners.
	/// </summary>
	/// <since>2.14</since>
	public float CornerRadius { get; set; } = 10;

	/// <summary>
	/// Gets or sets the size of the arrow, where the width is along the edge and the height is how far it sticks out.
	/// </summary>
	/// <since>2.14</since>
	public SizeF ArrowSize { get; set; } = new SizeF(26, 12);

	/// <summary>
	/// Gets or sets how far the shadow extends around the popover, or 0 for no shadow.
	/// </summary>
	/// <since>2.14</since>
	public int ShadowSize { get; set; } = 14;

	/// <summary>
	/// Gets or sets the color of the shadow right at the edge of the popover; it fades out from there.
	/// </summary>
	/// <since>2.14</since>
	public Color ShadowColor { get; set; } = new Color(0, 0, 0, 0.2f);

	/// <summary>
	/// Gets or sets a value indicating that the outline is drawn smoothed.
	/// </summary>
	/// <remarks>
	/// Turn off when the window's transparency is a color key, where smoothed edges would leave a colored fringe.
	/// </remarks>
	/// <since>2.14</since>
	public bool AntiAlias { get; set; } = true;

	/// <inheritdoc/>
	public Control Content
	{
		get => _content;
		set
		{
			if (ReferenceEquals(_content, value))
				return;
			_content = value;
			if (_frame != null)
			{
				SetFrameContent(value);
				UpdateLocation();
			}
		}
	}

	/// <inheritdoc/>
	public PopoverBehavior Behavior { get; set; }

	/// <inheritdoc/>
	public bool IsOpen => _window != null;

	/// <inheritdoc/>
	public void Show(Control relativeTo, RectangleF? rectangle, PopoverPosition position)
	{
		if (!ReferenceEquals(_relativeTo, relativeTo))
		{
			if (_relativeTo != null)
				_relativeTo.UnLoad -= RelativeTo_UnLoad;
			_relativeTo = relativeTo;
			_relativeTo.UnLoad += RelativeTo_UnLoad;
		}
		_rectangle = rectangle;
		_position = position;

		SetOwner(relativeTo.ParentWindow);

		if (_window == null)
		{
			_frame = new PopoverFrame(this);
			_window = CreateWindow();
			// resolves the frame's data context through the popover, so hosting the content doesn't change it
			((BindableWidget)_window).Parent = Widget;
			_window.Content = _frame;
			SetFrameContent(_content);
			_window.Owner = _owner;
			_window.Shown += Window_Shown;
			_window.Closed += Window_Closed;
			_window.LostFocus += Window_LostFocus;
			_window.KeyDown += Window_KeyDown;
			UpdateLocation();
			_window.Show();
		}
		else
		{
			_window.Owner = _owner;
			UpdateLocation();
		}
	}

	/// <inheritdoc/>
	public void Close() => _window?.Close();

	/// <summary>
	/// Creates the borderless window that hosts the popover each time it is shown.
	/// </summary>
	/// <remarks>
	/// Override to adjust the window for the platform. Areas outside the popover are painted with the window's <see cref="Control.BackgroundColor"/>.
	/// </remarks>
	/// <returns>A new window that is not yet shown.</returns>
	/// <since>2.14</since>
	protected virtual Form CreateWindow() => new Form
	{
		WindowStyle = WindowStyle.None,
		Resizable = false,
		Minimizable = false,
		Maximizable = false,
		ShowInTaskbar = false,
		// the frame draws its own shadow, and a system one would trace around it
		ShowShadow = false,
		BackgroundColor = Colors.Transparent
	};

	void SetOwner(Window owner)
	{
		if (ReferenceEquals(_owner, owner))
			return;
		if (_owner != null)
		{
			_owner.GotFocus -= Owner_GotFocus;
			_owner.LocationChanged -= Owner_Moved;
			_owner.SizeChanged -= Owner_Moved;
		}
		_owner = owner;
		if (_owner != null)
		{
			_owner.GotFocus += Owner_GotFocus;
			_owner.LocationChanged += Owner_Moved;
			_owner.SizeChanged += Owner_Moved;
		}
	}

	void UpdateLocation()
	{
		if (_window == null || _relativeTo?.Loaded != true)
			return;

		var anchor = _relativeTo.RectangleToScreen(_rectangle ?? new RectangleF(_relativeTo.Size));
		var inset = PopoverFrame.BorderWidth;
		var bodySize = (_content?.GetPreferredSize() ?? SizeF.Empty) + inset * 2;
		var arrow = ArrowSize;
		var screen = Screen.FromRectangle(anchor).WorkingArea;
		var position = ChooseSide(anchor, bodySize, screen);
		var vertical = position == PopoverPosition.Top || position == PopoverPosition.Bottom;

		// keep the body on screen along the edge, then aim the arrow at the anchor from wherever it ends up
		const float screenMargin = 4;
		PointF body;
		float arrowOffset;
		if (vertical)
		{
			var x = Clamp(anchor.MiddleX - bodySize.Width / 2, screen.Left + screenMargin, screen.Right - screenMargin - bodySize.Width);
			var y = position == PopoverPosition.Bottom ? anchor.Bottom + arrow.Height : anchor.Top - arrow.Height - bodySize.Height;
			body = new PointF(x, y);
			arrowOffset = anchor.MiddleX - x;
		}
		else
		{
			var y = Clamp(anchor.MiddleY - bodySize.Height / 2, screen.Top + screenMargin, screen.Bottom - screenMargin - bodySize.Height);
			var x = position == PopoverPosition.Right ? anchor.Right + arrow.Height : anchor.Left - arrow.Height - bodySize.Width;
			body = new PointF(x, y);
			arrowOffset = anchor.MiddleY - y;
		}
		var edgeLength = vertical ? bodySize.Width : bodySize.Height;
		var arrowMin = CornerRadius + arrow.Width / 2;
		arrowOffset = edgeLength < arrowMin * 2 ? edgeLength / 2 : Clamp(arrowOffset, arrowMin, edgeLength - arrowMin);

		var margin = GetMargin(position);
		_frame.Position = position;
		_frame.ArrowOffset = arrowOffset;
		_frame.Padding = new Padding(margin.Left + inset, margin.Top + inset, margin.Right + inset, margin.Bottom + inset);
		// with a color key, controls that take their parent's color would show the key as holes,
		// so give them the fill color and paint the key around the outline instead
		var outside = _window.BackgroundColor;
		_frame.OutsideColor = outside;
		_frame.BackgroundColor = outside.A >= 1 ? BackgroundColor : outside;
		_frame.Invalidate();

		_window.ClientSize = Size.Ceiling(new SizeF(bodySize.Width + margin.Horizontal, bodySize.Height + margin.Vertical));
		_window.Location = Point.Round(new PointF(body.X - margin.Left, body.Y - margin.Top));
	}

	PopoverPosition ChooseSide(RectangleF anchor, SizeF bodySize, RectangleF screen)
	{
		var needed = (IsVertical(_position) ? bodySize.Height : bodySize.Width) + ArrowSize.Height;
		float Space(PopoverPosition p) => p switch
		{
			PopoverPosition.Top => anchor.Top - screen.Top,
			PopoverPosition.Bottom => screen.Bottom - anchor.Bottom,
			PopoverPosition.Left => anchor.Left - screen.Left,
			_ => screen.Right - anchor.Right
		};
		var opposite = _position switch
		{
			PopoverPosition.Top => PopoverPosition.Bottom,
			PopoverPosition.Bottom => PopoverPosition.Top,
			PopoverPosition.Left => PopoverPosition.Right,
			_ => PopoverPosition.Left
		};
		return Space(_position) < needed && Space(opposite) > Space(_position) ? opposite : _position;
	}

	static bool IsVertical(PopoverPosition position) => position == PopoverPosition.Top || position == PopoverPosition.Bottom;

	// space around the body for the shadow, plus the arrow on the side facing the anchor
	Padding GetMargin(PopoverPosition position)
	{
		var shadow = Math.Max(0, ShadowSize);
		var arrow = (int)Math.Ceiling(ArrowSize.Height);
		return new Padding(
			shadow + (position == PopoverPosition.Right ? arrow : 0),
			shadow + (position == PopoverPosition.Bottom ? arrow : 0),
			shadow + (position == PopoverPosition.Left ? arrow : 0),
			shadow + (position == PopoverPosition.Top ? arrow : 0));
	}

	static float Clamp(float value, float min, float max) => Math.Max(min, Math.Min(max, value));

	/// <inheritdoc/>
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

	void Window_Shown(object sender, EventArgs e) => Callback.OnShown(Widget, EventArgs.Empty);

	void SetFrameContent(Control content)
	{
		_frame.Content = content;
		// the frame only hosts it visually, the popover stays its logical parent
		if (content != null)
			((BindableWidget)content).Parent = Widget;
	}

	void Window_Closed(object sender, EventArgs e)
	{
		var window = _window;
		if (window == null)
			return;
		_window = null;
		SetOwner(null);
		if (_relativeTo != null)
		{
			_relativeTo.UnLoad -= RelativeTo_UnLoad;
			_relativeTo = null;
		}
		// release the content so it can be shown again, and isn't disposed with the window
		_frame.Content = null;
		_frame = null;
		Callback.OnClosed(Widget, EventArgs.Empty);
		Application.Instance.AsyncInvoke(window.Dispose);
	}

	void Window_LostFocus(object sender, EventArgs e)
	{
		// Wpf raises this asynchronously, so it can arrive from the previous window after the popover is shown again
		if (Behavior == PopoverBehavior.Transient && ReferenceEquals(sender, _window))
			Close();
	}

	void Window_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyData == Keys.Escape)
		{
			e.Handled = true;
			Close();
		}
	}

	void Owner_GotFocus(object sender, EventArgs e)
	{
		// the owner only gets focus back from the popover when the user clicks on it
		if (Behavior == PopoverBehavior.Semitransient)
			Close();
	}

	void Owner_Moved(object sender, EventArgs e) => UpdateLocation();

	void RelativeTo_UnLoad(object sender, EventArgs e) => Close();

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if (disposing)
			Close();
		base.Dispose(disposing);
	}

	class PopoverFrame : Drawable
	{
		public const int BorderWidth = 1;

		readonly ThemedPopoverHandler _handler;

		public PopoverPosition Position { get; set; }

		// distance from the start of the edge with the arrow (left or top of the body) to the arrow's tip
		public float ArrowOffset { get; set; }

		// the window's background, painted around the outline when it's a solid color key
		public Color OutsideColor { get; set; }

		public PopoverFrame(ThemedPopoverHandler handler)
		{
			_handler = handler;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var g = e.Graphics;
			g.AntiAlias = _handler.AntiAlias;
			if (OutsideColor.A >= 1)
				g.FillRectangle(OutsideColor, 0, 0, Width, Height);

			var margin = _handler.GetMargin(Position);
			var body = new RectangleF(margin.Left, margin.Top, Width - margin.Horizontal, Height - margin.Vertical);
			using var path = CreatePath(body);

			var shadowSize = Math.Max(0, _handler.ShadowSize);
			if (shadowSize > 0 && _handler.ShadowColor.A > 0)
			{
				// overlapping strokes of decreasing width; each layer's alpha is picked so the stack
				// fades quadratically, which reads softer than a straight linear ramp
				var edgeAlpha = _handler.ShadowColor.A;
				float Coverage(int d) => edgeAlpha * (float)Math.Pow(1.0 - (double)d / shadowSize, 2);
				g.SaveTransform();
				g.TranslateTransform(0, shadowSize / 6f);
				for (int i = shadowSize; i > 0; i--)
				{
					var layerAlpha = 1f - (1f - Coverage(i - 1)) / (1f - Coverage(i));
					using var pen = new Pen(new Color(_handler.ShadowColor, layerAlpha), i * 2) { LineJoin = PenLineJoin.Round };
					g.DrawPath(pen, path);
				}
				g.RestoreTransform();
			}

			g.FillPath(_handler.BackgroundColor, path);
			g.DrawPath(_handler.BorderColor, path);

			base.OnPaint(e);
		}

		// outline of the body traced clockwise from the top left, with the arrow on the edge facing the anchor
		GraphicsPath CreatePath(RectangleF body)
		{
			var path = new GraphicsPath();
			var r = Math.Max(0, Math.Min(_handler.CornerRadius, Math.Min(body.Width, body.Height) / 2));
			var d = r * 2;
			var arrow = _handler.ArrowSize;
			var half = arrow.Width / 2;
			float left = body.Left, top = body.Top, right = body.Right, bottom = body.Bottom;

			path.MoveTo(left + r, top);
			if (Position == PopoverPosition.Bottom)
				AddArrow(path, new PointF(left + ArrowOffset - half, top), new PointF(left + ArrowOffset, top - arrow.Height), new PointF(left + ArrowOffset + half, top));
			path.LineTo(right - r, top);
			if (r > 0)
				path.AddArc(right - d, top, d, d, 270, 90);

			if (Position == PopoverPosition.Left)
				AddArrow(path, new PointF(right, top + ArrowOffset - half), new PointF(right + arrow.Height, top + ArrowOffset), new PointF(right, top + ArrowOffset + half));
			path.LineTo(right, bottom - r);
			if (r > 0)
				path.AddArc(right - d, bottom - d, d, d, 0, 90);

			if (Position == PopoverPosition.Top)
				AddArrow(path, new PointF(left + ArrowOffset + half, bottom), new PointF(left + ArrowOffset, bottom + arrow.Height), new PointF(left + ArrowOffset - half, bottom));
			path.LineTo(left + r, bottom);
			if (r > 0)
				path.AddArc(left, bottom - d, d, d, 90, 90);

			if (Position == PopoverPosition.Right)
				AddArrow(path, new PointF(left, top + ArrowOffset + half), new PointF(left - arrow.Height, top + ArrowOffset), new PointF(left, top + ArrowOffset - half));
			path.LineTo(left, top + r);
			if (r > 0)
				path.AddArc(left, top, d, d, 180, 90);

			path.CloseFigure();
			return path;
		}

		// flares out from the edge and rounds off at the tip, like the macOS arrow
		static void AddArrow(GraphicsPath path, PointF start, PointF tip, PointF end)
		{
			var along = new PointF((end.X - start.X) / 2, (end.Y - start.Y) / 2);
			path.LineTo(start.X, start.Y);
			path.AddBezier(start, start + along * 0.5f, tip - along * 0.25f, tip);
			path.AddBezier(tip, tip + along * 0.25f, end - along * 0.5f, end);
		}
	}
}
