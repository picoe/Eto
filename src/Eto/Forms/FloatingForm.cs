namespace Eto.Forms
{
	/// <summary>
	/// A specialized modeless Form for floating windows.
	/// </summary>
	/// <remarks>
	/// Some platforms have specialized functionality for floating windows (e.g. macOS' NSPanel) which allows for more interaction
	/// with the parent window.  
	/// Floating forms usually dissapear when going the application is not active, and are Topmost to the application only.
	/// 
	/// Use this for any auxiliary windows that should float above main application windows.
	/// </remarks>
	[Handler(typeof(IHandler))]
	public class FloatingForm : Form
	{
		/// <summary>
		/// Interface handler for the <see cref="FloatingForm"/> control
		/// </summary>
		public new interface IHandler : Form.IHandler
		{
		}
	}
}
