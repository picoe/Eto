using System;
using System.Collections.Generic;

namespace Eto.Forms.ThemedControls
{
	/// <summary>
	/// A themed handler for the <see cref="FilePicker"/> control.
	/// </summary>
	public class ThemedFilePickerHandler : ThemedControlHandler<StackLayout, FilePicker, FilePicker.ICallback>, FilePicker.IHandler
	{
		List<FileFilter> filters;
		TextBox entryPath;
		Button buttonBrowse;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.ThemedControls.ThemedFilePickerHandler"/> class.
		/// </summary>
		public ThemedFilePickerHandler()
		{
			filters = new List<FileFilter>();

			Control = new StackLayout();
			Control.Orientation = Orientation.Horizontal;
			Control.Spacing = 4;

			entryPath = new TextBox();
			Control.Items.Add(new StackLayoutItem(entryPath, VerticalAlignment.Center, true));

			buttonBrowse = new Button();
			buttonBrowse.Text = "Browse...";
			buttonBrowse.Width = -1;
			buttonBrowse.Click += ButtonBrowse_Click;
			Control.Items.Add(new StackLayoutItem(buttonBrowse, VerticalAlignment.Center, false));
		}

		/// <summary>
		/// Gets or sets <see cref="FileAction"/> that is used when the user is selecting the file.
		/// </summary>
		/// <value>The file action.</value>
		public FileAction FileAction { get; set; }

		/// <summary>
		/// Gets or sets the full path of the file that is selected.
		/// </summary>
		/// <value>The path of the file.</value>
		public string FilePath
		{
			get { return entryPath.Text; }
			set { entryPath.Text = value; }
		}

		/// <summary>
		/// Gets or sets the index of the current filter in the <see cref="FilePicker.Filters"/> collection
		/// </summary>
		/// <value>The index of the current filter.</value>
		public int CurrentFilterIndex { get; set; }

		/// <summary>
		/// Gets or sets the title of the dialog that the control will show.
		/// </summary>
		/// <value>The title of the dialog.</value>
		public string Title { get; set; }

		/// <summary>
		/// Clears all filters
		/// </summary>
		public void ClearFilters()
		{
			filters.Clear();
		}

		/// <summary>
		/// Inserts a filter at the specified index
		/// </summary>
		/// <param name="index">Index to insert the filter</param>
		/// <param name="filter">Filter to insert</param>
		public void InsertFilter(int index, FileFilter filter)
		{
			filters.Insert(index, filter);
		}

		/// <summary>
		/// Removes a filter at the specified index
		/// </summary>
		/// <param name="index">Index of the filter to remove</param>
		public void RemoveFilter(int index)
		{
			filters.RemoveAt(index);
		}

		private void ButtonBrowse_Click(object sender, EventArgs e)
		{
			if (FileAction != FileAction.SelectFolder)
			{
				FileDialog dialog;
				if (FileAction == FileAction.OpenFile)
					dialog = new OpenFileDialog();
				else
					dialog = new SaveFileDialog();
				dialog.Title = Title;

				if (!string.IsNullOrEmpty(FilePath))
					dialog.FileName = FilePath;

				if (filters.Count > 0)
				{
					foreach (var filter in filters)
						dialog.Filters.Add(filter);
					dialog.CurrentFilterIndex = CurrentFilterIndex;
				}

				if (dialog.ShowDialog(Widget) == DialogResult.Ok)
					FilePath = dialog.FileName;
			}
			else
			{
				var dialog = new SelectFolderDialog();
				dialog.Title = Title;

				if (!string.IsNullOrEmpty(FilePath))
					dialog.Directory = FilePath;

				if (dialog.ShowDialog(Widget) == DialogResult.Ok)
					FilePath = dialog.Directory;
			}
		}

		/// <summary>
		/// Attaches control events.
		/// </summary>
		/// <param name="id">ID of the event to attach</param>
		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case FilePicker.FilePathChangedEvent:
					entryPath.TextChanged += (sender, e) => Callback.OnFilePathChanged(Widget, EventArgs.Empty);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
