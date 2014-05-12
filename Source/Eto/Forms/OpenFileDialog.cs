using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	/// <summary>
	/// Dialog to select a file (or files) from the file system to open in the application
	/// </summary>
	[Handler(typeof(OpenFileDialog.IHandler))]
	public class OpenFileDialog : FileDialog
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.OpenFileDialog"/> class.
		/// </summary>
		public OpenFileDialog()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.OpenFileDialog"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public OpenFileDialog(Generator generator) : this(generator, typeof(OpenFileDialog.IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.OpenFileDialog"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected OpenFileDialog(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		/// <summary>
		/// Gets or sets a value indicating whether the user can select multiple files, or a single file.
		/// </summary>
		/// <value><c>true</c> if the user can select multiple files; otherwise, <c>false</c>.</value>
		public bool MultiSelect
		{ 
			get { return Handler.MultiSelect; }
			set { Handler.MultiSelect = value; }
		}

		/// <summary>
		/// Gets the full path of the files selected by the user, when <see cref="MultiSelect"/> is true.
		/// </summary>
		/// <remarks>
		/// This will return a single file name if <see cref="MultiSelect"/> is false.
		/// </remarks>
		/// <value>The full path of the files selected, or a single file if <see cref="MultiSelect"/> is false.</value>
		public IEnumerable<string> Filenames { get { return Handler.Filenames; } }

		/// <summary>
		/// Handler interface for the <see cref="OpenFileDialog"/>
		/// </summary>
		public new interface IHandler : FileDialog.IHandler
		{
			/// <summary>
			/// Gets or sets a value indicating whether the user can select multiple files, or a single file.
			/// </summary>
			/// <value><c>true</c> if the user can select multiple files; otherwise, <c>false</c>.</value>
			bool MultiSelect { get; set; }

			/// <summary>
			/// Gets the full path of the files selected by the user, when <see cref="MultiSelect"/> is true.
			/// </summary>
			/// <remarks>
			/// This will return a single file name if <see cref="MultiSelect"/> is false.
			/// </remarks>
			/// <value>The full path of the files selected, or a single file if <see cref="MultiSelect"/> is false.</value>
			IEnumerable<string> Filenames { get; }
		}
	}
}
