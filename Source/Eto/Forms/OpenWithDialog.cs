using System;

namespace Eto.Forms
{
	/// <summary>
	/// Dialog for a user to pick the default application for the given file.
	/// </summary>
	/// <remarks>
	/// The OpenWithDialog on some platforms may run asynchronously, and return immediately after
	/// the <see cref="CommonDialog.ShowDialog(Control)"/> call. On some platforms, like Windows,
	/// it might not even look like a standard dialog.
	/// </remarks>
	[Handler(typeof(OpenWithDialog.IHandler))]
    public class OpenWithDialog : CommonDialog
    {
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.OpenWithDialog"/> class.
		/// </summary>
		/// <param name="filepath">File path of the file that should be opened.</param>
        public OpenWithDialog(string filepath)
        {
			Handler.FilePath = filepath;
        }

		/// <summary>
		/// Handler interface for the <see cref="OpenWithDialog"/>.
		/// </summary>
		public new interface IHandler : CommonDialog.IHandler
		{
			/// <summary>
			/// Gets or sets the file path of the file that should be opened.
			/// </summary>
			/// <value>The file path of the file that should be opened.</value>
			string FilePath { get; set; }
		}
    }
}
