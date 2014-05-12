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
		/// Initializes a new instance of the <see cref="Eto.Forms.SaveFileDialog"/> class.
		/// </summary>
		public SaveFileDialog()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SaveFileDialog"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public SaveFileDialog (Generator generator) : this (generator, typeof(SaveFileDialog.IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SaveFileDialog"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected SaveFileDialog (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}

		/// <summary>
		/// Handler interface for the <see cref="SaveFileDialog"/>
		/// </summary>
		public new interface IHandler : FileDialog.IHandler
		{
		}
	}
}
