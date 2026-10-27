namespace Eto.Forms;

/// <summary>
/// Gesture for pan translation, typically activated by dragging with the mouse or a single finger on a touch screen.
/// </summary>
/// <remarks>
/// Scroll-wheel and trackpad scroll input is exposed through <see cref="ScrollGesture"/>.
/// </remarks>
[Handler(typeof(IHandler))]
public class PanGesture : Gesture
{
	new IHandler Handler => (IHandler)base.Handler;

	/// <summary>
	/// Gets the current translation delta for the gesture activation.
	/// </summary>
	public PointF Translation => Handler.Translation;

	/// <summary>
	/// Gets the current velocity for the gesture activation.
	/// </summary>
	public PointF Velocity => Handler.Velocity;

	/// <summary>
	/// Gets or sets the mouse buttons that can activate the pan gesture.
	/// </summary>
	/// <remarks>
	/// The default value is <see cref="MouseButtons.Primary"/>. All specified buttons must be down
	/// for the pan gesture to activate.
	/// </remarks>
	public MouseButtons Buttons
	{
		get => Handler.Buttons;
		set
		{
			if (value == MouseButtons.None)
				throw new ArgumentException($"{nameof(Buttons)} cannot be {nameof(MouseButtons.None)}.", nameof(value));
			Handler.Buttons = value;
		}
	}

	/// <summary>
	/// Handler interface for <see cref="PanGesture"/>.
	/// </summary>
	public new interface IHandler : Gesture.IHandler
	{
		/// <summary>
		/// Gets the current translation delta for the gesture activation.
		/// </summary>
		PointF Translation { get; }

		/// <summary>
		/// Gets the current velocity for the gesture activation.
		/// </summary>
		PointF Velocity { get; }

		/// <summary>
		/// Gets or sets the mouse buttons that can activate the pan gesture.
		/// </summary>
		MouseButtons Buttons { get; set; }
	}
}
