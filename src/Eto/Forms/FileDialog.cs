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
	[Obsolete("Since 2.4: Use FileFilter instead")]
	public class FileDialogFilter : FileFilter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FileFilter"/> class.
		/// </summary>
		public FileDialogFilter() : base()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileFilter"/> class.
		/// </summary>
		/// <param name="name">Name of the filter to display to the user</param>
		/// <param name="extensions">Extensions of the files to filter by, including the dot for each</param>
		public FileDialogFilter(string name, params string[] extensions) : base(name, extensions)
		{
			
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
		/// Gets the collection of available file filters the user can select from.
		/// </summary>
		/// <remarks>
		/// Add entries to this collection to set the filters the user can select when the file dialog is shown.
		/// 
		/// Some platforms may either disable (OS X) or hide (GTK/WinForms/WPF) files that do not match the currently selected filter.
		/// </remarks>
		/// <seealso cref="CurrentFilterIndex"/>
		/// <seealso cref="CurrentFilter"/>
		/// <value>The filters the user can select.</value>
		public Collection<FileFilter> Filters
		{
			get { return filters ?? (filters = new FilterCollection { Dialog = this }); }
		}

		/// <summary>
		/// Gets or sets the index of the current filter in the <see cref="Filters"/> collection
		/// </summary>
		/// <seealso cref="Filters"/>
		/// <seealso cref="CurrentFilter"/>
		/// <value>The index of the current filter, or -1 if none is selected.</value>
		public int CurrentFilterIndex
		{
			get { return Handler.CurrentFilterIndex; }
			set { Handler.CurrentFilterIndex = value; }
		}

		/// <summary>
		/// Gets or sets the currently selected filter from the <see cref="Filters"/> collection.
		/// </summary>
		/// <remarks>
		/// This should always match an instance of a filter in the <see cref="Filters"/> collection, otherwise
		/// the current filter will be set to null.
		/// </remarks>
		/// <seealso cref="Filters"/>
		/// <value>The current filter.</value>
		public FileFilter CurrentFilter
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

		class FilterCollection : Collection<FileFilter>
		{
			public FileDialog Dialog { get; set; }

			protected override void InsertItem(int index, FileFilter item)
			{
				base.InsertItem(index, item);
				Dialog.Handler.InsertFilter(index, item);
			}

			protected override void RemoveItem(int index)
			{
				base.RemoveItem(index);
				Dialog.Handler.RemoveFilter(index);
			}

			protected override void SetItem(int index, FileFilter item)
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
			/// Gets or sets the index of the current filter in the <see cref="Filters"/> collection
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
			void InsertFilter(int index, FileFilter filter);

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