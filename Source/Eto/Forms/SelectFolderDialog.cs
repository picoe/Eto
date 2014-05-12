using System;

namespace Eto.Forms
{
	/// <summary>
	/// Dialog for the user to select a folder in the filesystem
	/// </summary>
	[Handler(typeof(SelectFolderDialog.IHandler))]
	public class SelectFolderDialog : CommonDialog
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SelectFolderDialog"/> class.
		/// </summary>
		public SelectFolderDialog()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SelectFolderDialog"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public SelectFolderDialog(Generator generator) : this(generator, typeof(SelectFolderDialog.IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SelectFolderDialog"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected SelectFolderDialog(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		/// <summary>
		/// Gets or sets the title of dialog
		/// </summary>
		/// <value>The title.</value>
		public string Title
		{
			get { return Handler.Title; }
			set { Handler.Title = value; }
		}

		/// <summary>
		/// Gets or sets the selected directory.
		/// </summary>
		/// <value>The selected directory.</value>
		public string Directory
		{
			get { return Handler.Directory; }
			set { Handler.Directory = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="SelectFolderDialog"/>
		/// </summary>
		public new interface IHandler : CommonDialog.IHandler
		{
			/// <summary>
			/// Gets or sets the title of dialog
			/// </summary>
			/// <value>The title.</value>
			string Title { get; set; }

			/// <summary>
			/// Gets or sets the selected directory.
			/// </summary>
			/// <value>The selected directory.</value>
			string Directory { get; set; }
		}
	}
}

