namespace Eto.Forms;

/// <summary>
/// Gesture for multidimensional scrolling, such as with a mouse wheel or two finger trackpad scroll input.
/// </summary>
[Handler(typeof(IHandler))]
public class ScrollGesture : Gesture
{
	static readonly object WheelScrollAmountKey = new object();

	new IHandler Handler => (IHandler)base.Handler;

	/// <summary>
	/// Gets the default logical distance represented by one wheel notch.
	/// </summary>
	public const float DefaultWheelScrollAmount = 48f;

	/// <summary>
	/// Gets or sets the logical distance represented by one wheel notch.
	/// </summary>
	/// <remarks>
	/// This is used to normalize wheel-style input to logical coordinates. Precise scrolling input
	/// that already reports logical deltas, such as macOS trackpad scrolling, is not scaled.
	/// </remarks>
	public float WheelScrollAmount
	{
		get => Properties.Get<float?>(WheelScrollAmountKey) ?? DefaultWheelScrollAmount;
		set => Properties.Set(WheelScrollAmountKey, value);
	}

	/// <summary>
	/// Gets the current scroll delta for the gesture activation, in logical coordinates.
	/// </summary>
	public SizeF Delta => Handler.Delta;

	/// <summary>
	/// Gets the current scroll velocity for the gesture activation, in logical coordinates per second.
	/// </summary>
	public PointF Velocity => Handler.Velocity;

	/// <summary>
	/// Handler interface for <see cref="ScrollGesture"/>.
	/// </summary>
	public new interface IHandler : Gesture.IHandler
	{
		/// <summary>
		/// Gets the current scroll delta for the gesture activation, in logical coordinates.
		/// </summary>
		SizeF Delta { get; }

		/// <summary>
		/// Gets the current scroll velocity for the gesture activation, in logical coordinates per second.
		/// </summary>
		PointF Velocity { get; }
	}
}
