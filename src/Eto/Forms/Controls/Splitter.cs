namespace Eto.Forms;

/// <summary>
/// Specifies which panel has a fixed size the parent container is resized.
/// </summary>
public enum SplitterFixedPanel
{
	/// <summary>
	/// The first panel will be fixed size, where the second will resize along with the splitter's container.
	/// </summary>
	Panel1,
	/// <summary>
	/// The second panel will be fixed size, where the first will resize along with the splitter's container.
	/// </summary>
	Panel2,
	/// <summary>
	/// Both panels will resize along with the splitter's container.
	/// </summary>
	None
}

/// <summary>
/// Event arguments for the <see cref="Splitter.PositionChanging"/> event.
/// </summary>
public class SplitterPositionChangingEventArgs : CancelEventArgs
{
	/// <summary>
	/// The new position for the splitter
	/// </summary>
	/// <value></value>
	public int NewPosition { get; }

	/// <summary>
	/// Initializes a new instance of the SplitterPositionChangingEventArgs class
	/// </summary>
	/// <param name="newPosition"></param>
	public SplitterPositionChangingEventArgs(int newPosition)
	{
		NewPosition = newPosition;
	}
}

/// <summary>
/// Control to show two panels separated by a splitter
/// </summary>
/// <remarks>
/// Most desktop platforms allow the user to modify the position of the splitter, though some (notibly iOS) do
/// not.
/// The <see cref="Orientation"/> of the splitter determines how the controls are laid out, either horizontally
/// or vertically.
/// </remarks>
[Handler(typeof(Splitter.IHandler))]
public class Splitter : Container
{
	new IHandler Handler { get { return (IHandler)base.Handler; } }

	/// <summary>
	/// Gets an enumeration of controls that are directly contained by this container
	/// </summary>
	/// <value>The contained controls.</value>
	public override IEnumerable<Control> Controls
	{
		get
		{
			if (Panel1 != null)
				yield return Panel1;
			if (Panel2 != null)
				yield return Panel2;
		}
	}

	/// <summary>
	/// Gets a value indicating the <see cref="Splitter"/> is supported in the platform
	/// </summary>
	/// <value><c>true</c> if supported; otherwise, <c>false</c>.</value>
	public static bool IsSupported
	{
		get { return Platform.Instance.Supports<IHandler>(); }
	}

	#region Events

	/// <summary>
	/// Identifier for the <see cref="PositionChanged"/> event
	/// </summary>
	public const string PositionChangedEvent = "Splitter.PositionChanged";

	/// <summary>
	/// Raised when the user moves the splitter.
	/// </summary>
	public event EventHandler<EventArgs> PositionChanged
	{
		add { Properties.AddHandlerEvent(PositionChangedEvent, value); }
		remove { Properties.RemoveEvent(PositionChangedEvent, value); }
	}

	/// <summary>
	/// Raises the <see cref="PositionChanged"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnPositionChanged(EventArgs e)
	{
		Properties.TriggerEvent(PositionChangedEvent, this, e);
	}

	/// <summary>
	/// Identifier for the <see cref="PositionChanging"/> event
	/// </summary>
	public const string PositionChangingEvent = "Splitter.PositionChanging";

	/// <summary>
	/// Raised when the user moves the splitter. Set <see cref="CancelEventArgs.Cancel" /> to true to abort the change.
	/// </summary>
	public event EventHandler<SplitterPositionChangingEventArgs> PositionChanging
	{
		add { Properties.AddHandlerEvent(PositionChangingEvent, value); }
		remove { Properties.RemoveEvent(PositionChangingEvent, value); }
	}

	/// <summary>
	/// Raises the <see cref="PositionChanging"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnPositionChanging(SplitterPositionChangingEventArgs e) => Properties.TriggerEvent(PositionChangingEvent, this, e);

	/// <summary>
	/// Identifier for the <see cref="PositionChangeStarted"/> event
	/// </summary>
	public const string PositionChangeStartedEvent = "Splitter.PositionChangeStarted";

	/// <summary>
	/// Raised when the user starts moving the splitter.
	/// </summary>
	public event EventHandler<EventArgs> PositionChangeStarted
	{
		add { Properties.AddHandlerEvent(PositionChangeStartedEvent, value); }
		remove { Properties.RemoveEvent(PositionChangeStartedEvent, value); }
	}

	/// <summary>
	/// Raises the <see cref="PositionChangeStarted"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnPositionChangeStarted(EventArgs e) => Properties.TriggerEvent(PositionChangeStartedEvent, this, e);

	/// <summary>
	/// Identifier for the <see cref="PositionChangeCompleted"/> event
	/// </summary>
	public const string PositionChangeCompletedEvent = "Splitter.PositionChangeCompleted";

	/// <summary>
	/// Raised when the user starts finishes moving the splitter
	/// </summary>
	public event EventHandler<EventArgs> PositionChangeCompleted
	{
		add { Properties.AddHandlerEvent(PositionChangeCompletedEvent, value); }
		remove { Properties.RemoveEvent(PositionChangeCompletedEvent, value); }
	}

	/// <summary>
	/// Raises the <see cref="PositionChangeCompleted"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnPositionChangeCompleted(EventArgs e) => Properties.TriggerEvent(PositionChangeCompletedEvent, this, e);


	#endregion

	static Splitter()
	{
		EventLookup.Register<Splitter>(c => c.OnPositionChanged(null), Splitter.PositionChangedEvent);
		EventLookup.Register<Splitter>(c => c.OnPositionChanging(null), Splitter.PositionChangingEvent);
		EventLookup.Register<Splitter>(c => c.OnPositionChangeStarted(null), Splitter.PositionChangeStartedEvent);
		EventLookup.Register<Splitter>(c => c.OnPositionChangeCompleted(null), Splitter.PositionChangeCompletedEvent);
	}

	/// <summary>
	/// Gets or sets the orientation of the panels in the splitter.
	/// </summary>
	/// <remarks>
	/// This defines the orientation of the panels, with a splitter of the opposite orientation between them.
	/// For example, when set to <see cref="T:Orientation.Horizontal"/>, Panel1 and Panel2 will be horizontal to 
	/// eachother with a vertical splitter/gutter between them.
	/// </remarks>
	/// <value>The orientation of the panels.</value>
	public Orientation Orientation
	{
		get { return Handler.Orientation; }
		set { Handler.Orientation = value; }
	}

	/// <summary>
	/// Gets or sets the panel with fixed size.
	/// </summary>
	/// <remarks>
	/// This specifies which panel will not change its size when the splitter's container is resized.
	/// If <see cref="SplitterFixedPanel.None"/>, both panels will resize.
	/// </remarks>
	/// <value>The fixed panel.</value>
	public SplitterFixedPanel FixedPanel
	{
		get { return Handler.FixedPanel; }
		set { Handler.FixedPanel = value; }
	}

	/// <summary>
	/// Gets or sets the position of the splitter from the left or top, in pixels.
	/// </summary>
	/// <value>The position of the splitter.</value>
	public int Position
	{
		get { return Handler.Position; }
		set { Handler.Position = value; }
	}

	/// <summary>
	/// Gets or sets the relative position of the splitter which is based on <see cref="FixedPanel"/>.
	/// </summary>
	/// <remarks>
	/// Same as <see cref="Position"/> with SplitterFixedPanel.Panel1,
	/// width/height of second panel with SplitterFixedPanel.Panel2
	/// and ratio of width/height of first panel against available size with SplitterFixedPanel.None.
	/// </remarks>
	public double RelativePosition
	{
		get { return Handler.RelativePosition; }
		set { Handler.RelativePosition = value; }
	}

	/// <summary>
	/// Gets or sets size of the splitter/gutter
	/// </summary>
	public int SplitterWidth
	{
		get { return Handler.SplitterWidth; }
		set { Handler.SplitterWidth = value; }
	}

	/// <summary>
	/// Gets or sets the top or left panel of the splitter.
	/// </summary>
	/// <value>The first panel.</value>
	public Control Panel1
	{
		get { return Handler.Panel1; }
		set
		{
			SetParent(value, () => Handler.Panel1 = value, Handler.Panel1);
		}
	}

	/// <summary>
	/// Gets or sets the minimal size of the first panel.
	/// </summary>
	/// <value>The minimal size of the first panel.</value>
	public int Panel1MinimumSize
	{
		get { return Handler.Panel1MinimumSize; }
		set { Handler.Panel1MinimumSize = value; }
	}

	/// <summary>
	/// Gets or sets the bottom or right panel of the splitter.
	/// </summary>
	/// <value>The second panel.</value>
	public Control Panel2
	{
		get { return Handler.Panel2; }
		set
		{
			SetParent(value, () => Handler.Panel2 = value, Handler.Panel2);
		}
	}

	/// <summary>
	/// Gets or sets the minimal size of the second panel.
	/// </summary>
	/// <value>The minimal size of the second panel.</value>
	public int Panel2MinimumSize
	{
		get { return Handler.Panel2MinimumSize; }
		set { Handler.Panel2MinimumSize = value; }
	}

	/// <summary>
	/// Removes the specified child from the container.
	/// </summary>
	/// <param name="child">Child to remove.</param>
	public override void Remove(Control child)
	{
		if (object.ReferenceEquals(Panel1, child))
		{
			Panel1 = null;
		}
		else if (object.ReferenceEquals(Panel2, child))
		{
			Panel2 = null;
		}
	}

	static readonly object callback = new Callback();
	/// <summary>
	/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
	/// </summary>
	/// <returns>The callback instance to use for this widget</returns>
	protected override object GetCallback() { return callback; }

	/// <summary>
	/// Callback interface for the <see cref="Splitter"/>
	/// </summary>
	public new interface ICallback : Container.ICallback
	{
		/// <summary>
		/// Raises the position changed event.
		/// </summary>
		void OnPositionChanged(Splitter widget, EventArgs e);
		/// <summary>
		/// Raises the position changing event.
		/// </summary>
		void OnPositionChanging(Splitter widget, SplitterPositionChangingEventArgs e);
		/// <summary>
		/// Raises the position change started event.
		/// </summary>
		void OnPositionChangeStarted(Splitter widget, EventArgs e);
		/// <summary>
		/// Raises the position change completed event.
		/// </summary>
		void OnPositionChangeCompleted(Splitter widget, EventArgs e);
	}

	/// <summary>
	/// Callback implementation for handlers of the <see cref="Splitter"/>
	/// </summary>
	protected new class Callback : Container.Callback, ICallback
	{
		/// <summary>
		/// Raises the position changed event.
		/// </summary>
		public void OnPositionChanged(Splitter widget, EventArgs e)
		{
			using (widget.Platform.Context)
				widget.OnPositionChanged(e);
		}
		/// <summary>
		/// Raises the position changing event.
		/// </summary>
		public void OnPositionChanging(Splitter widget, SplitterPositionChangingEventArgs e)
		{
			using (widget.Platform.Context)
				widget.OnPositionChanging(e);
		}
		/// <summary>
		/// Raises the position change started event.
		/// </summary>
		public void OnPositionChangeStarted(Splitter widget, EventArgs e)
		{
			using (widget.Platform.Context)
				widget.OnPositionChangeStarted(e);
		}
		/// <summary>
		/// Raises the position change completed event.
		/// </summary>
		public void OnPositionChangeCompleted(Splitter widget, EventArgs e)
		{
			using (widget.Platform.Context)
				widget.OnPositionChangeCompleted(e);
		}
	}

	/// <summary>
	/// Handler interface for the <see cref="Splitter"/>
	/// </summary>
	public new interface IHandler : Container.IHandler
	{
		/// <summary>
		/// Gets or sets the orientation of the panels in the splitter.
		/// </summary>
		/// <value>The orientation of the panels.</value>
		Orientation Orientation { get; set; }

		/// <summary>
		/// Gets or sets the panel with fixed size.
		/// </summary>
		/// <remarks>
		/// This specifies which panel will not change its size when the splitter's container is resized.
		/// If <see cref="SplitterFixedPanel.None"/>, both panels will resize.
		/// </remarks>
		/// <value>The fixed panel.</value>
		SplitterFixedPanel FixedPanel { get; set; }

		/// <summary>
		/// Gets or sets the position of the splitter from the left or top, in pixels.
		/// </summary>
		/// <value>The position of the splitter.</value>
		int Position { get; set; }

		/// <summary>
		/// Gets or sets the relative position of the splitter which is based on <see cref="FixedPanel"/>.
		/// </summary>
		/// <remarks>
		/// Same as <see cref="Position"/> with SplitterFixedPanel.Panel1,
		/// width/height of second panel with SplitterFixedPanel.Panel2
		/// and ratio of width/height of first panel against available size with SplitterFixedPanel.None.
		/// </remarks>
		double RelativePosition { get; set; }

		/// <summary>
		/// Gets or sets size of the splitter/gutter
		/// </summary>
		int SplitterWidth { get; set; }

		/// <summary>
		/// Gets or sets the top or left panel of the splitter.
		/// </summary>
		/// <value>The first panel.</value>
		Control Panel1 { get; set; }

		/// <summary>
		/// Gets or sets the bottom or right panel of the splitter.
		/// </summary>
		/// <value>The second panel.</value>
		Control Panel2 { get; set; }

		/// <summary>
		/// Gets or sets the minimal size of the first panel.
		/// </summary>
		/// <value>The minimal size of the first panel.</value>
		int Panel1MinimumSize { get; set; }

		/// <summary>
		/// Gets or sets the minimal size of the second panel.
		/// </summary>
		/// <value>The minimal size of the second panel.</value>
		int Panel2MinimumSize { get; set; }
	}
}

/// <summary>
/// Orientation of a <see cref="Splitter"/> control.
/// </summary>
[Obsolete("Since 2.1: Use Orientation instead")]
public struct SplitterOrientation
{
	readonly Orientation orientation;

	SplitterOrientation(Orientation orientation)
	{
		this.orientation = orientation;
	}

	/// <summary>
	/// Controls are in horizontal orientation, with a vertical divider between them.
	/// </summary>
	public static SplitterOrientation Horizontal { get { return Orientation.Horizontal; } }

	/// <summary>
	/// Controls are in vertical orientation, with a horizontal divider betwen them.
	/// </summary>
	public static SplitterOrientation Vertical { get { return Orientation.Vertical; } }

	/// <summary>Converts to an Orientation</summary>
	public static implicit operator Orientation(SplitterOrientation orientation)
	{
		return orientation.orientation;
	}

	/// <summary>Converts an Orientation to a SplitterOrientation</summary>
	public static implicit operator SplitterOrientation(Orientation orientation)
	{
		return new SplitterOrientation(orientation);
	}

	/// <summary>Compares for equality</summary>
	/// <param name="orientation1">Orientation1.</param>
	/// <param name="orientation2">Orientation2.</param>
	public static bool operator ==(Orientation orientation1, SplitterOrientation orientation2)
	{
		return orientation1 == orientation2.orientation;
	}

	/// <summary>Compares for inequality</summary>
	/// <param name="orientation1">Orientation1.</param>
	/// <param name="orientation2">Orientation2.</param>
	public static bool operator !=(Orientation orientation1, SplitterOrientation orientation2)
	{
		return orientation1 != orientation2.orientation;
	}

	/// <summary>
	/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Eto.Forms.SplitterOrientation"/>.
	/// </summary>
	/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="Eto.Forms.SplitterOrientation"/>.</param>
	/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
	/// <see cref="Eto.Forms.SplitterOrientation"/>; otherwise, <c>false</c>.</returns>
	public override bool Equals(object obj)
	{
		return (obj is SplitterOrientation && (this == (SplitterOrientation)obj))
		       || (obj is Orientation && (this == (Orientation)obj));
	}

	/// <summary>
	/// Serves as a hash function for a <see cref="Eto.Forms.SplitterOrientation"/> object.
	/// </summary>
	/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
	public override int GetHashCode()
	{
		return orientation.GetHashCode();
	}
}