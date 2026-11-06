namespace Eto.Forms;

/// <summary>
/// Gesture for trackpad or touchscreen pinch magnification.
/// </summary>
/// <since>2.12</since>
[Handler(typeof(IHandler))]
public class MagnificationGesture : Gesture
{
	new IHandler Handler => (IHandler)base.Handler;

	/// <summary>
	/// Gets the current magnification delta for the gesture activation.
	/// </summary>
	public float Magnification => Handler.Magnification;
	/// <summary>
	/// Handler interface for <see cref="MagnificationGesture"/>.
	/// </summary>
	/// <since>2.12</since>
	public new interface IHandler : Gesture.IHandler
	{
		/// <summary>
		/// Gets the current magnification delta for the gesture activation.
		/// </summary>
		float Magnification { get; }
	}
}
