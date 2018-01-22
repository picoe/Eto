using System;

namespace Eto.Forms
{
	/// <summary>
	/// Dialog for the user to select a file to save
	/// </summary>
	[Handler(typeof(SaveFileDialog.IHandler))]
	public class SaveFileDialog : FileDialog
	{
		/// <summary>
		/// Handler interface for the <see cref="SaveFileDialog"/>
		/// </summary>
		public new interface IHandler : FileDialog.IHandler
		{
		}
	}
}
