namespace Eto.Forms;

/// <summary>
/// Gesture for trackpad or touchscreen two-finger rotation.
/// </summary>
[Handler(typeof(IHandler))]
public class RotationGesture : Gesture
{
	new IHandler Handler => (IHandler)base.Handler;

	/// <summary>
	/// Gets the current rotation delta, in degrees, for the gesture activation.
	/// </summary>
	/// <remarks>
	/// Positive values indicate clockwise rotation, negative values counter-clockwise.
	/// The value is the change since the last activation, not the absolute rotation since the gesture began.
	/// </remarks>
	public float Rotation => Handler.Rotation;

	/// <summary>
	/// Handler interface for <see cref="RotationGesture"/>.
	/// </summary>
	public new interface IHandler : Gesture.IHandler
	{
		/// <summary>
		/// Gets the current rotation delta, in degrees, for the gesture activation.
		/// </summary>
		float Rotation { get; }
	}
}
