using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Eto.Forms
{
	/// <summary>
	/// Filter definition for a <see cref="FileDialog"/>
	/// </summary>
	/// <remarks>
	/// Each filter defines an option for the user to limit the selection of files in the dialog.
	/// </remarks>
	public class FileDialogFilter
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

		/// <summary>
		/// Converts a string representation of a filter in the form of "[name]|[ext];[ext];[ext]" to a FileDialogFilter.
		/// </summary>
		/// <param name="filter">String representation of the file dialog filter</param>
		/// <returns>A new file dialog filter with the name and extensions specified in the <paramref name="filter"/> argument</returns>
		public static implicit operator FileDialogFilter(string filter)
		{
			var parts = filter.Split('|');
			if (parts.Length != 2)
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Filter must be in the form of '<name>|<ext>;<ext>;<ext>;"), "filter");

			return new FileDialogFilter
			{
				Name = parts[0],
				Extensions = parts[1].Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries)
			};
		}
	}

	/// <summary>
	/// Base file dialog class
	/// </summary>
	public abstract class FileDialog : CommonDialog
	{
		FilterCollection filters;

		new IHandler Handler { get { return (IHandler)base.Handler; } }

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
		public Collection<FileDialogFilter> Filters
		{
			get { return filters ?? (filters = new FilterCollection { Dialog = this }); }
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
		public FileDialogFilter CurrentFilter
		{
			get
			{
				var index = CurrentFilterIndex;
				if (index == -1 || filters == null || index >= filters.Count) return null;
				return Filters[index];
			}
			set
			{
				CurrentFilterIndex = Filters.IndexOf(value);
			}
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
		/// <remarks>
		/// You can use <see cref="EtoEnvironment.GetFolderPath(EtoSpecialFolder)"/> to set the initial value of the directory,
		/// though the user should be able to change the folder and keep it
		/// </remarks>
		/// <value>The directory.</value>
		public Uri Directory
		{
			get { return Handler.Directory; }
			set { Handler.Directory = value; }
		}

		class FilterCollection : Collection<FileDialogFilter>
		{
			public FileDialog Dialog { get; set; }

			protected override void InsertItem(int index, FileDialogFilter item)
			{
				base.InsertItem(index, item);
				Dialog.Handler.InsertFilter(index, item);
			}

			protected override void RemoveItem(int index)
			{
				base.RemoveItem(index);
				Dialog.Handler.RemoveFilter(index);
			}

			protected override void SetItem(int index, FileDialogFilter item)
			{
				Dialog.Handler.RemoveFilter(index);
				base.SetItem(index, item);
				Dialog.Handler.InsertFilter(index, item);
			}

			protected override void ClearItems()
			{
				base.ClearItems();
				Dialog.Handler.ClearFilters();
			}
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
			/// Gets or sets the index of the current filter in the <see cref="Filters"/> enumeration
			/// </summary>
			/// <value>The index of the current filter.</value>
			int CurrentFilterIndex { get; set; }

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

			/// <summary>
			/// Inserts a filter at the specified index
			/// </summary>
			/// <param name="index">Index to insert the filter</param>
			/// <param name="filter">Filter to insert</param>
			void InsertFilter(int index, FileDialogFilter filter);

			/// <summary>
			/// Removes a filter at the specified index
			/// </summary>
			/// <param name="index">Index of the filter to remove</param>
			void RemoveFilter(int index);

			/// <summary>
			/// Clears all filters
			/// </summary>
			void ClearFilters();
		}
	}
}