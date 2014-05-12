using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	/// <summary>
	/// Interface for a filter for a <see cref="FileDialog"/>
	/// </summary>
	public interface IFileDialogFilter
	{
		/// <summary>
		/// Gets the name of the filter
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the extensions to filter the file list
		/// </summary>
		/// <remarks>
		/// Each extension should include the period.  e.g. ".jpeg", ".png", etc.
		/// </remarks>
		string[] Extensions { get; }
	}

	/// <summary>
	/// Filter definition for a <see cref="FileDialog"/>
	/// </summary>
	/// <remarks>
	/// Each filter defines an option for the user to limit the selection of files in the dialog.
	/// </remarks>
	public class FileDialogFilter : IFileDialogFilter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.FileDialogFilter"/> class.
		/// </summary>
		public FileDialogFilter()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.FileDialogFilter"/> class.
		/// </summary>
		/// <param name="name">Name of the filter to display to the user</param>
		/// <param name="extensions">Extensions of the files to filter by, including the dot for each</param>
		public FileDialogFilter(string name, params string[] extensions)
		{
			this.Name = name;
			this.Extensions = extensions;
		}

		/// <summary>
		/// Gets or sets the name of the filter.
		/// </summary>
		/// <value>The name of the filter.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the extensions to filter the file list
		/// </summary>
		/// <remarks>Each extension should include the period. e.g. ".jpeg", ".png", etc.</remarks>
		/// <value>The extensions.</value>
		public string[] Extensions { get; set; }

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Eto.Forms.FileDialogFilter"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Eto.Forms.FileDialogFilter"/>.</returns>
		public override string ToString()
		{
			return Name;
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="Eto.Forms.FileDialogFilter"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			int hash;
			hash = Name != null ? Name.GetHashCode() : string.Empty.GetHashCode();
			if (Extensions != null)
				hash ^= Extensions.GetHashCode();
			return hash;
		}
	}

	/// <summary>
	/// Base file dialog class
	/// </summary>
	public abstract class FileDialog : CommonDialog
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.FileDialog"/> class.
		/// </summary>
		protected FileDialog()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.FileDialog"/> class.
		/// </summary>
		/// <param name="g">The green component.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected FileDialog(Generator g, Type type, bool initialize = true) : base(g, type, initialize)
		{
		}

		/// <summary>
		/// Gets or sets the full name and path of the file that is selected
		/// </summary>
		/// <value>The name of the file.</value>
		public string FileName
		{
			get { return Handler.FileName; }
			set { Handler.FileName = value; }
		}

		/// <summary>
		/// Gets or sets the available filters for the user.
		/// </summary>
		/// <value>The filters the user can select.</value>
		public IEnumerable<IFileDialogFilter> Filters
		{
			get { return Handler.Filters; }
			set { Handler.Filters = value; }
		}

		/// <summary>
		/// Gets or sets the index of the current filter in the <see cref="Filters"/> enumeration
		/// </summary>
		/// <value>The index of the current filter.</value>
		public int CurrentFilterIndex
		{
			get { return Handler.CurrentFilterIndex; }
			set { Handler.CurrentFilterIndex = value; }
		}

		/// <summary>
		/// Gets or sets the currently selected filter from <see cref="Filters"/>
		/// </summary>
		/// <remarks>
		/// This should always match an entry in the <see cref="Filters"/> enumeration.
		/// </remarks>
		/// <value>The current filter.</value>
		public IFileDialogFilter CurrentFilter
		{
			get { return Handler.CurrentFilter; }
			set { Handler.CurrentFilter = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.FileDialog"/> checks if the file exists 
		/// before the user can close the dialog.
		/// </summary>
		/// <value><c>true</c> to check if the file exists; otherwise, <c>false</c>.</value>
		public bool CheckFileExists
		{
			get { return Handler.CheckFileExists; }
			set { Handler.CheckFileExists = value; }
		}

		/// <summary>
		/// Gets or sets the title of the dialog.
		/// </summary>
		/// <value>The title.</value>
		public string Title
		{
			get { return Handler.Title; }
			set { Handler.Title = value; }
		}

		/// <summary>
		/// Gets or sets the directory the file dialog will show files.
		/// </summary>
		/// <value>The directory.</value>
		public Uri Directory
		{
			get { return Handler.Directory; }
			set { Handler.Directory = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="FileDialog"/> based widgets
		/// </summary>
		public new interface IHandler : CommonDialog.IHandler
		{
			/// <summary>
			/// Gets or sets the full name and path of the file that is selected
			/// </summary>
			/// <value>The name of the file.</value>
			string FileName { get; set; }

			/// <summary>
			/// Gets or sets the available filters for the user.
			/// </summary>
			/// <value>The filters the user can select.</value>
			IEnumerable<IFileDialogFilter> Filters { get; set; }

			/// <summary>
			/// Gets or sets the index of the current filter in the <see cref="Filters"/> enumeration
			/// </summary>
			/// <value>The index of the current filter.</value>
			int CurrentFilterIndex { get; set; }

			/// <summary>
			/// Gets or sets the currently selected filter from <see cref="Filters"/>
			/// </summary>
			/// <remarks>
			/// This should always match an entry in the <see cref="Filters"/> enumeration.
			/// </remarks>
			/// <value>The current filter.</value>
			IFileDialogFilter CurrentFilter { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.FileDialog"/> checks if the file exists 
			/// before the user can close the dialog.
			/// </summary>
			/// <value><c>true</c> to check if the file exists; otherwise, <c>false</c>.</value>
			bool CheckFileExists { get; set; }

			/// <summary>
			/// Gets or sets the title of the dialog.
			/// </summary>
			/// <value>The title.</value>
			string Title { get; set; }

			/// <summary>
			/// Gets or sets the directory the file dialog will show files.
			/// </summary>
			/// <value>The directory.</value>
			Uri Directory { get; set; }
		}
	}
}