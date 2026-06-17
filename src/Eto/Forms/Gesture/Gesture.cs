namespace Eto.Forms;

/// <summary>
/// Event arguments for determining if two gestures can be recognized simultaneously.
/// </summary>
public class GestureRecognitionEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a new instance of the <see cref="GestureRecognitionEventArgs"/> class.
	/// </summary>
	/// <param name="otherGesture">The other gesture being recognized.</param>
	/// <param name="allow">Initial value indicating whether simultaneous recognition is allowed.</param>
	public GestureRecognitionEventArgs(Gesture otherGesture, bool allow)
	{
		OtherGesture = otherGesture;
		Allow = allow;
	}

	/// <summary>
	/// Gets the other gesture being recognized.
	/// </summary>
	public Gesture OtherGesture { get; }

	/// <summary>
	/// Gets or sets a value indicating whether simultaneous recognition is allowed.
	/// </summary>
	public bool Allow { get; set; }
}

/// <summary>
/// Base class for platform gestures that can be attached to a <see cref="Control"/>.
/// </summary>
[Handler(typeof(IHandler))]
public abstract class Gesture : Widget
{
	static readonly object AllowedGestureTypesKey = new object();
	static readonly object AllowedGesturesKey = new object();
	static readonly object CanRecognizeSimultaneouslyKey = new object();

	new IHandler Handler => (IHandler)base.Handler;

	Control _control;
	
	/// <summary>
	/// Gets or sets a value indicating whether this gesture can be recognized.
	/// </summary>
	public bool Enabled
	{
		get => Handler.Enabled;
		set => Handler.Enabled = value;
	}
	
	/// <summary>
	/// Gets the control that this gesture is attached to, or null if not attached to a control.
	/// </summary>
	public Control Control
	{
		get => _control;
		internal set => _control = value;
	}

	/// <summary>
	/// Occurs when determining whether this gesture can recognize simultaneously with another gesture.
	/// </summary>
	/// <remarks>
	/// Simultaneous recognition requires both gestures to allow the pairing.
	/// </remarks>
	public event EventHandler<GestureRecognitionEventArgs> CanRecognizeSimultaneously
	{
		add { Properties.AddEvent(CanRecognizeSimultaneouslyKey, value); }
		remove { Properties.RemoveEvent(CanRecognizeSimultaneouslyKey, value); }
	}

	/// <summary>
	/// Allows this gesture to recognize simultaneously with gestures of the specified type.
	/// </summary>
	/// <typeparam name="TGesture">Type of gesture to allow.</typeparam>
	public void AllowSimultaneousWith<TGesture>()
		where TGesture : Gesture
	{
		AllowSimultaneousWith(typeof(TGesture));
	}

	/// <summary>
	/// Allows this gesture to recognize simultaneously with gestures of the specified type.
	/// </summary>
	/// <param name="gestureType">Type of gesture to allow.</param>
	public void AllowSimultaneousWith(Type gestureType)
	{
		if (gestureType == null)
			throw new ArgumentNullException(nameof(gestureType));
		if (!typeof(Gesture).IsAssignableFrom(gestureType))
			throw new ArgumentException($"Type must derive from {nameof(Gesture)}.", nameof(gestureType));

		Properties.Create<HashSet<Type>>(AllowedGestureTypesKey).Add(gestureType);
	}

	/// <summary>
	/// Allows this gesture and the specified gesture to recognize simultaneously.
	/// </summary>
	/// <param name="gesture">Gesture to allow.</param>
	/// <remarks>
	/// This adds the pairing to both gestures.
	/// </remarks>
	public void AllowSimultaneousWith(Gesture gesture)
	{
		if (gesture == null)
			throw new ArgumentNullException(nameof(gesture));

		Properties.Create<HashSet<Gesture>>(AllowedGesturesKey).Add(gesture);
		gesture.Properties.Create<HashSet<Gesture>>(AllowedGesturesKey).Add(this);
	}

	/// <summary>
	/// Removes a previously allowed simultaneous gesture type.
	/// </summary>
	/// <typeparam name="TGesture">Type of gesture to remove.</typeparam>
	public void DisallowSimultaneousWith<TGesture>()
		where TGesture : Gesture
	{
		DisallowSimultaneousWith(typeof(TGesture));
	}

	/// <summary>
	/// Removes a previously allowed simultaneous gesture type.
	/// </summary>
	/// <param name="gestureType">Type of gesture to remove.</param>
	public void DisallowSimultaneousWith(Type gestureType)
	{
		if (gestureType == null)
			throw new ArgumentNullException(nameof(gestureType));
		Properties.Get<HashSet<Type>>(AllowedGestureTypesKey)?.Remove(gestureType);
	}

	/// <summary>
	/// Removes a previously allowed simultaneous gesture pairing.
	/// </summary>
	/// <param name="gesture">Gesture pairing to remove.</param>
	/// <remarks>
	/// This removes the pairing from both gestures.
	/// </remarks>
	public void DisallowSimultaneousWith(Gesture gesture)
	{
		if (gesture == null)
			throw new ArgumentNullException(nameof(gesture));

		Properties.Get<HashSet<Gesture>>(AllowedGesturesKey)?.Remove(gesture);
		gesture.Properties.Get<HashSet<Gesture>>(AllowedGesturesKey)?.Remove(this);
	}

	/// <summary>
	/// Determines whether this gesture allows simultaneous recognition with the specified gesture.
	/// </summary>
	/// <param name="otherGesture">Other gesture being recognized.</param>
	/// <returns>True if this gesture allows simultaneous recognition with <paramref name="otherGesture"/>.</returns>
	public bool AllowsSimultaneousRecognition(Gesture otherGesture)
	{
		if (otherGesture == null)
			return false;

		var allow = Properties.Get<HashSet<Gesture>>(AllowedGesturesKey)?.Contains(otherGesture) == true
			|| Properties.Get<HashSet<Type>>(AllowedGestureTypesKey)?.Any(type => type.IsInstanceOfType(otherGesture)) == true;

		var args = new GestureRecognitionEventArgs(otherGesture, allow);
		Properties.TriggerEvent(CanRecognizeSimultaneouslyKey, this, args);
		allow = args.Allow;

		return allow;
	}

	/// <summary>
	/// Determines whether this gesture and another gesture mutually allow simultaneous recognition.
	/// </summary>
	/// <param name="otherGesture">Other gesture being recognized.</param>
	/// <returns>True if both gestures allow simultaneous recognition with each other.</returns>
	public bool CanRecognizeSimultaneouslyWith(Gesture otherGesture)
	{
		return otherGesture != null
			&& AllowsSimultaneousRecognition(otherGesture)
			&& otherGesture.AllowsSimultaneousRecognition(this);
	}
	
	private readonly object ActivatedKey = new object();

	/// <summary>
	/// Occurs when the gesture has been recognized by the platform.
	/// </summary>
	public event EventHandler<EventArgs> Activated
	{
		add { Properties.AddEvent(ActivatedKey, value); }
		remove { Properties.RemoveEvent(ActivatedKey, value); }
	}

	/// <summary>
	/// Raises the <see cref="Activated"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnActivated(EventArgs e)
	{
		Properties.TriggerEvent(ActivatedKey, this, e);
	}

	/// <summary>
	/// Handler interface for <see cref="Gesture"/>.
	/// </summary>
	public new interface IHandler : Widget.IHandler
	{
		/// <summary>
		/// Gets or sets a value indicating whether this gesture can be recognized.
		/// </summary>
		bool Enabled { get; set; }
	}

	/// <inheritdoc />
	protected override object GetCallback() => Callback.Instance;

	/// <summary>
	/// Callback interface for <see cref="Gesture"/>.
	/// </summary>
	public new interface ICallback : Widget.ICallback
	{
		/// <summary>
		/// Raises the <see cref="Activated"/> event.
		/// </summary>
		/// <param name="widget">Gesture widget.</param>
		/// <param name="e">Event arguments.</param>
		void OnActivated(Gesture widget, EventArgs e);
	}

	class Callback : ICallback
	{
		public static readonly Callback Instance = new Callback();

		public void OnActivated(Gesture widget, EventArgs e)
		{
			using (widget.Platform.Context)
				widget.OnActivated(e);
		}
	}
}
