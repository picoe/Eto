namespace Eto.Forms;

/// <summary>
/// Side of the anchor that a <see cref="Popover"/> prefers to appear on.
/// </summary>
/// <remarks>
/// The popover moves to the opposite side when there is not enough room on the preferred one.
/// </remarks>
/// <since>2.14</since>
public enum PopoverPosition
{
	/// <summary>
	/// Above the anchor, with the arrow pointing down.
	/// </summary>
	Top,
	/// <summary>
	/// Below the anchor, with the arrow pointing up.
	/// </summary>
	Bottom,
	/// <summary>
	/// Left of the anchor, with the arrow pointing right.
	/// </summary>
	Left,
	/// <summary>
	/// Right of the anchor, with the arrow pointing left.
	/// </summary>
	Right
}

/// <summary>
/// When a <see cref="Popover"/> closes on its own.
/// </summary>
/// <remarks>
/// Pressing Escape closes a popover with any behavior, and <see cref="Popover.Close"/> always closes it.
/// </remarks>
/// <since>2.14</since>
public enum PopoverBehavior
{
	/// <summary>
	/// Closes when the user interacts with anything outside the popover, including other applications.
	/// </summary>
	Transient,
	/// <summary>
	/// Closes when the user interacts with the window the popover is shown from, but not other windows or applications.
	/// </summary>
	Semitransient,
	/// <summary>
	/// Only closes when <see cref="Popover.Close"/> is called or the user presses Escape.
	/// </summary>
	ApplicationDefined
}

/// <summary>
/// Shows content in a bubble with an arrow pointing at a control, such as for quick settings or details.
/// </summary>
/// <remarks>
/// Uses the native popover where the platform has one, otherwise a themed version that looks like the macOS one.
/// <code>
/// var popover = new Popover { Content = new Panel { Padding = 10, Content = "Hello" } };
/// button.Click += (sender, e) => popover.Show(button);
/// </code>
/// </remarks>
/// <since>2.14</since>
[ContentProperty("Content")]
[Handler(typeof(IHandler))]
public class Popover : BindableWidget, IBindableWidgetContainer
{
	new IHandler Handler => (IHandler)base.Handler;

	static readonly object callback = new Callback();

	/// <summary>
	/// Gets an instance of an object used to perform callbacks to the widget from handler implementations.
	/// </summary>
	/// <returns>The callback instance to use for this widget.</returns>
	protected override object GetCallback() => callback;

	/// <summary>
	/// Gets or sets the control shown inside the popover.
	/// </summary>
	/// <remarks>
	/// The popover is sized to the preferred size of the content. The content can be reused each time the popover is shown.
	/// The content inherits the popover's <see cref="BindableWidget.DataContext"/>.
	/// </remarks>
	/// <since>2.14</since>
	public Control Content
	{
		get => Handler.Content;
		set
		{
			var old = Handler.Content;
			if (ReferenceEquals(old, value))
				return;
			if (old != null && ReferenceEquals(((BindableWidget)old).Parent, this))
				((BindableWidget)old).Parent = null;
			if (value != null)
				((BindableWidget)value).Parent = this;
			Handler.Content = value;
		}
	}

	IEnumerable<BindableWidget> IBindableWidgetContainer.Children
	{
		get
		{
			var content = Content;
			if (content != null)
				yield return content;
		}
	}

	/// <summary>
	/// Gets or sets when the popover closes on its own.
	/// </summary>
	/// <since>2.14</since>
	public PopoverBehavior Behavior
	{
		get => Handler.Behavior;
		set => Handler.Behavior = value;
	}

	/// <summary>
	/// Gets a value indicating that the popover is currently shown.
	/// </summary>
	/// <since>2.14</since>
	public bool IsOpen => Handler.IsOpen;

	/// <summary>
	/// Shows the popover pointing at the specified control.
	/// </summary>
	/// <remarks>
	/// Calling this while the popover is open moves it to the new location.
	/// </remarks>
	/// <param name="relativeTo">Control to point at. It must be shown in a window.</param>
	/// <param name="position">Side of the control to prefer.</param>
	/// <since>2.14</since>
	public void Show(Control relativeTo, PopoverPosition position = PopoverPosition.Bottom) => Show(relativeTo, null, position);

	/// <summary>
	/// Shows the popover pointing at an area of the specified control.
	/// </summary>
	/// <remarks>
	/// Calling this while the popover is open moves it to the new location.
	/// </remarks>
	/// <param name="relativeTo">Control to point at. It must be shown in a window.</param>
	/// <param name="rectangle">Area of <paramref name="relativeTo"/> to point at, in its coordinates, or <c>null</c> for the whole control.</param>
	/// <param name="position">Side of the area to prefer.</param>
	/// <since>2.14</since>
	public void Show(Control relativeTo, RectangleF? rectangle, PopoverPosition position = PopoverPosition.Bottom)
	{
		if (relativeTo == null)
			throw new ArgumentNullException(nameof(relativeTo));
		if (!relativeTo.Loaded)
			throw new InvalidOperationException("The control to show the popover from must be shown in a window");
		Handler.Show(relativeTo, rectangle, position);
	}

	/// <summary>
	/// Closes the popover if it is open.
	/// </summary>
	/// <since>2.14</since>
	public void Close() => Handler.Close();

	/// <summary>
	/// Event identifier for handlers when attaching the <see cref="Shown"/> event.
	/// </summary>
	/// <since>2.14</since>
	public const string ShownEvent = "Popover.Shown";

	/// <summary>
	/// Raised after the popover is shown.
	/// </summary>
	/// <since>2.14</since>
	public event EventHandler<EventArgs> Shown
	{
		add => Properties.AddHandlerEvent(ShownEvent, value);
		remove => Properties.RemoveEvent(ShownEvent, value);
	}

	/// <summary>
	/// Raises the <see cref="Shown"/> event.
	/// </summary>
	/// <param name="e">Event arguments</param>
	/// <since>2.14</since>
	protected virtual void OnShown(EventArgs e) => Properties.TriggerEvent(ShownEvent, this, e);

	/// <summary>
	/// Event identifier for handlers when attaching the <see cref="Closed"/> event.
	/// </summary>
	/// <since>2.14</since>
	public const string ClosedEvent = "Popover.Closed";

	/// <summary>
	/// Raised after the popover is closed, whether by the user or by calling <see cref="Close"/>.
	/// </summary>
	/// <since>2.14</since>
	public event EventHandler<EventArgs> Closed
	{
		add => Properties.AddHandlerEvent(ClosedEvent, value);
		remove => Properties.RemoveEvent(ClosedEvent, value);
	}

	/// <summary>
	/// Raises the <see cref="Closed"/> event.
	/// </summary>
	/// <param name="e">Event arguments</param>
	/// <since>2.14</since>
	protected virtual void OnClosed(EventArgs e) => Properties.TriggerEvent(ClosedEvent, this, e);

	/// <summary>
	/// Callback interface for the <see cref="Popover"/>
	/// </summary>
	/// <since>2.14</since>
	public new interface ICallback : Widget.ICallback
	{
		/// <summary>
		/// Raises the shown event.
		/// </summary>
		/// <since>2.14</since>
		void OnShown(Popover widget, EventArgs e);

		/// <summary>
		/// Raises the closed event.
		/// </summary>
		/// <since>2.14</since>
		void OnClosed(Popover widget, EventArgs e);
	}

	/// <summary>
	/// Callback implementation for handlers of the <see cref="Popover"/>
	/// </summary>
	protected class Callback : ICallback
	{
		/// <summary>
		/// Raises the shown event.
		/// </summary>
		public void OnShown(Popover widget, EventArgs e)
		{
			using (widget.Platform.Context)
				widget.OnShown(e);
		}

		/// <summary>
		/// Raises the closed event.
		/// </summary>
		public void OnClosed(Popover widget, EventArgs e)
		{
			using (widget.Platform.Context)
				widget.OnClosed(e);
		}
	}

	/// <summary>
	/// Handler interface for the <see cref="Popover"/>
	/// </summary>
	/// <since>2.14</since>
	public new interface IHandler : Widget.IHandler
	{
		/// <summary>
		/// Gets or sets the control shown inside the popover.
		/// </summary>
		/// <since>2.14</since>
		Control Content { get; set; }

		/// <summary>
		/// Gets or sets when the popover closes on its own.
		/// </summary>
		/// <since>2.14</since>
		PopoverBehavior Behavior { get; set; }

		/// <summary>
		/// Gets a value indicating that the popover is currently shown.
		/// </summary>
		/// <since>2.14</since>
		bool IsOpen { get; }

		/// <summary>
		/// Shows the popover pointing at an area of the specified control, or moves it there if it is already open.
		/// </summary>
		/// <param name="relativeTo">Control to point at, already shown in a window.</param>
		/// <param name="rectangle">Area of <paramref name="relativeTo"/> to point at, or <c>null</c> for the whole control.</param>
		/// <param name="position">Side of the area to prefer.</param>
		/// <since>2.14</since>
		void Show(Control relativeTo, RectangleF? rectangle, PopoverPosition position);

		/// <summary>
		/// Closes the popover if it is open.
		/// </summary>
		/// <since>2.14</since>
		void Close();
	}
}
