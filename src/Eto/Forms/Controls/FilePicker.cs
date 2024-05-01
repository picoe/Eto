namespace Eto.Forms;

/// <summary>
/// A control that allows the user to interact with files and folders.
/// It can be used to save files, select files or select directories.
/// </summary>
/// <example>
/// <code>
/// var myFilePicker = new FilePicker
/// {
///		FileAction = FileAction.OpenFile
/// }
///
/// var myFolderPicker = new FilePicker
/// {
///		FileAction = FileAction.SelectFolder
/// }
/// </code>
/// </example>
[Handler(typeof(FilePicker.IHandler))]
public class FilePicker : Control
{
	FilterCollection filters;

	new IHandler Handler { get { return (IHandler)base.Handler; } }

	static readonly object callback = new Callback();

	/// <inheritdoc/>
	protected override object GetCallback() { return callback; }

	/// <summary>
	/// Gets or sets the index of the current filter in the <see cref="Filters"/> collection.
	/// </summary>
	/// <seealso cref="Filters"/>
	/// <seealso cref="CurrentFilter"/>
	/// <value>The index of the current filter, or <c>-1</c> if none is selected.</value>
	public int CurrentFilterIndex
	{
		get { return Handler.CurrentFilterIndex; }
		set { Handler.CurrentFilterIndex = value; }
	}

	/// <summary>
	/// Gets or sets the currently selected filter from the <see cref="Filters"/> collection. Also updates <see cref="CurrentFilterIndex"/> accordingly.
	/// </summary>
	/// <remarks>
	/// This can return <see langword="null"/> if either the <see cref="Filters"/> collection is <see langword="null"/>,
	/// or if the current filter is not in the <see cref="Filters"/> collection.
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
	/// <example>
	/// This is an example that would let the user select either PNG images, or all files.
	/// <code>
	/// myFilePicker.Filters.Add(new FileFilter("PNG Images", "png"));
	/// myFilePicker.Filters.Add(new FileFilter("All Files", "*"));
	/// </code>
	/// </example>
	/// <seealso cref="CurrentFilterIndex"/>
	/// <seealso cref="CurrentFilter"/>
	/// <value>The filters that the user can select.</value>
	public Collection<FileFilter> Filters
	{
		get { return filters ??= new FilterCollection { Picker = this }; }
	}

	/// <summary>
	/// Gets or sets the <see cref="FileAction"/>, which indicates how the file picker should behave.
	/// </summary>
	/// <value>The file action.</value>
	public FileAction FileAction
	{
		get { return Handler.FileAction; }
		set { Handler.FileAction = value; }
	}

	/// <summary>
	/// Gets or sets the full path of the file that was selected,
	/// or should be selected by default when opening the picker.
	/// </summary>
	/// <value>The path of the file.</value>
	public string FilePath
	{
		get { return Handler.FilePath; }
		set { Handler.FilePath = value; }
	}

	/// <summary>
	/// Gets or sets the title of the dialog that the file picker will show.
	/// </summary>
	/// <value>The title of the dialog.</value>
	public string Title
	{
		get { return Handler.Title; }
		set { Handler.Title = value; }
	}

	/// <summary>
	/// Handler interface for the <see cref="FilePicker"/> control.
	/// </summary>
	public new interface IHandler : Control.IHandler
	{
		/// <inheritdoc cref="FileAction"/>
		FileAction FileAction { get; set; }

		/// <inheritdoc cref="FilePath"/>
		string FilePath { get; set; }

		/// <inheritdoc cref="CurrentFilterIndex"/>
		int CurrentFilterIndex { get; set; }

		/// <inheritdoc cref="Title"/>
		string Title { get; set; }

		/// <inheritdoc cref="FilterCollection.ClearItems"/>
		void ClearFilters();
		
		/// <inheritdoc cref="FilterCollection.InsertItem"/>
		void InsertFilter(int index, FileFilter filter);

		/// <inheritdoc cref="FilterCollection.RemoveItem"/>
		void RemoveFilter(int index);
	}

	class FilterCollection : Collection<FileFilter>
	{
		public FilePicker Picker { get; set; }

		/// <summary>
		/// Inserts a filter at the specified index.
		/// </summary>
		/// <param name="index">Index to insert the filter.</param>
		/// <param name="item">Filter to insert.</param>
		protected override void InsertItem(int index, FileFilter item)
		{
			base.InsertItem(index, item);
			Picker.Handler.InsertFilter(index, item);
		}

		/// <summary>
		/// Removes a filter at the specified index.
		/// </summary>
		/// <param name="index">Index of the filter to remove.</param>
		protected override void RemoveItem(int index)
		{
			base.RemoveItem(index);
			Picker.Handler.RemoveFilter(index);
		}

		/// <summary>
		/// Sets a filter at a specified index to be the provided one.
		/// </summary>
		/// <param name="index">The index whose filter should be changed.</param>
		/// <param name="item">The filter which should be set.</param>
		protected override void SetItem(int index, FileFilter item)
		{
			Picker.Handler.RemoveFilter(index);
			base.SetItem(index, item);
			Picker.Handler.InsertFilter(index, item);
		}

		/// <summary>
		/// Clears all filters.
		/// </summary>
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
	/// Raises the <see cref="FilePathChanged"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnFilePathChanged(EventArgs e)
	{
		Properties.TriggerEvent(FilePathChangedEvent, this, e);
	}

	// Events

	/// <summary>
	/// Callback interface for <see cref="FilePicker"/>.
	/// </summary>
	public new interface ICallback : Control.ICallback
	{
		/// <inheritdoc cref="FilePicker.OnFilePathChanged"/>
		void OnFilePathChanged(FilePicker widget, EventArgs e);
	}

	/// <summary>
	/// Callback implementation for handlers of <see cref="FilePicker"/>.
	/// </summary>
	protected new class Callback : Control.Callback, ICallback
	{
		/// <inheritdoc cref="FilePicker.OnFilePathChanged"/>
		public void OnFilePathChanged(FilePicker widget, EventArgs e)
		{
			using (widget.Platform.Context)
				widget.OnFilePathChanged(e);
		}
	}
}