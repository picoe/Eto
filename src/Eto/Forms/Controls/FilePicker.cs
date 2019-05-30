using System;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	/// <summary>
	/// Control for picking a file or folder.
	/// </summary>
	[Handler(typeof(FilePicker.IHandler))]
	public class FilePicker : Control
	{
		FilterCollection filters;

		new IHandler Handler { get { return (IHandler)base.Handler; } }

		static readonly object callback = new Callback();

		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations.
		/// </summary>
		/// <returns>The callback instance to use for this widget.</returns>
		protected override object GetCallback() { return callback; }

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
		/// Gets the collection of available file filters the user can select from.
		/// </summary>
		/// <remarks>
		/// Add entries to this collection to set the filters the user can select when the file dialog is shown.
		/// 
		/// Some platforms may either disable (OS X) or hide (GTK/WinForms/WPF) files that do not match the currently selected filter.
		/// </remarks>
		/// <seealso cref="CurrentFilterIndex"/>
		/// <seealso cref="CurrentFilter"/>
		/// <value>The filters that the user can select.</value>
		public Collection<FileFilter> Filters
		{
			get { return filters ?? (filters = new FilterCollection { Picker = this }); }
		}

		/// <summary>
		/// Gets or sets <see cref="FileAction"/> that is used when the user is selecting the file.
		/// </summary>
		/// <value>The file action.</value>
		public FileAction FileAction
		{
			get { return Handler.FileAction; }
			set { Handler.FileAction = value; }
		}

		/// <summary>
		/// Gets or sets the full path of the file that is selected.
		/// </summary>
		/// <value>The path of the file.</value>
		public string FilePath
		{
			get { return Handler.FilePath; }
			set { Handler.FilePath = value; }
		}

		/// <summary>
		/// Gets or sets the title of the dialog that the control will show.
		/// </summary>
		/// <value>The title of the dialog.</value>
		public string Title
		{
			get { return Handler.Title; }
			set { Handler.Title = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="FilePicker"/> control
		/// </summary>
		public new interface IHandler : Control.IHandler
		{
			/// <summary>
			/// Gets or sets <see cref="FileAction"/> that is used when the user is selecting the file.
			/// </summary>
			/// <value>The file action.</value>
			FileAction FileAction { get; set; }

			/// <summary>
			/// Gets or sets the full path of the file that is selected
			/// </summary>
			/// <value>The path of the file.</value>
			string FilePath { get; set; }

			/// <summary>
			/// Gets or sets the index of the current filter in the <see cref="Filters"/> collection
			/// </summary>
			/// <value>The index of the current filter.</value>
			int CurrentFilterIndex { get; set; }

			/// <summary>
			/// Gets or sets the title of the dialog that the control will show.
			/// </summary>
			/// <value>The title of the dialog.</value>
			string Title { get; set; }

			/// <summary>
			/// Clears all filters
			/// </summary>
			void ClearFilters();

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
		}

		class FilterCollection : Collection<FileFilter>
		{
			public FilePicker Picker { get; set; }

			protected override void InsertItem(int index, FileFilter item)
			{
				base.InsertItem(index, item);
				Picker.Handler.InsertFilter(index, item);
			}

			protected override void RemoveItem(int index)
			{
				base.RemoveItem(index);
				Picker.Handler.RemoveFilter(index);
			}

			protected override void SetItem(int index, FileFilter item)
			{
				Picker.Handler.RemoveFilter(index);
				base.SetItem(index, item);
				Picker.Handler.InsertFilter(index, item);
			}

			protected override void ClearItems()
			{
				base.ClearItems();
				Picker.Handler.ClearFilters();
			}
		}

		// FilePath Event

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="FilePathChanged"/> event.
		/// </summary>
		public const string FilePathChangedEvent = "FilePicker.FilePathChanged";

		/// <summary>
		/// Event to handle when the user selects a new file.
		/// </summary>
		public event EventHandler<EventArgs> FilePathChanged
		{
			add { Properties.AddHandlerEvent(FilePathChangedEvent, value); }
			remove { Properties.RemoveEvent(FilePathChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="FilePathChanged"/> event
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnFilePathChanged(EventArgs e)
		{
			Properties.TriggerEvent(FilePathChangedEvent, this, e);
		}

		// Events

		/// <summary>
		/// Callback interface for <see cref="FilePicker"/>
		/// </summary>
		public new interface ICallback : Control.ICallback
		{
			/// <summary>
			/// Raises file path changed event.
			/// </summary>
			void OnFilePathChanged(FilePicker widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of <see cref="FilePicker"/>
		/// </summary>
		protected new class Callback : Control.Callback, ICallback
		{
			/// <summary>
			/// Raises file path changed event.
			/// </summary>
			public void OnFilePathChanged(FilePicker widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnFilePathChanged(e);
			}
		}
	}
}
