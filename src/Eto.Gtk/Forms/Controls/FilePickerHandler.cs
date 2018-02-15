using System;
using System.Collections.Generic;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
	public class FilePickerHandler : GtkControl<Gtk.EventBox, FilePicker, FilePicker.ICallback>, FilePicker.IHandler
	{
		private EventHandler filePathChanged;

		FileAction action;
		Gtk.FileChooserButton filebutton;
		Gtk.HBox savebox;
		Gtk.Entry saveentry;
		Gtk.Button savebutton;

		public FilePickerHandler()
		{
			Control = new Gtk.EventBox();
			action = FileAction.OpenFile;

			filebutton = new Gtk.FileChooserButton("", Gtk.FileChooserAction.Open);
			filebutton.SelectionChanged += Filebutton_SelectionChanged;

			// Save is not a valid option for FileChooserButton, therefore
			// we need to create our own, or use the ThemedFilePickerHandler
			savebox = new Gtk.HBox();
			saveentry = new Gtk.Entry();
			savebox.PackStart(saveentry, true, true, 0);
			savebutton = new Gtk.Button();
			savebutton.Label = "Browse";
			savebox.PackStart(savebutton, false, true, 1);
			saveentry.Changed += Saveentry_Changed;
			savebutton.Clicked += Savebutton_Clicked;

			Control.Child = filebutton;
		}

		private void Saveentry_Changed(object sender, EventArgs e)
		{
			if (action == FileAction.SaveFile)
			{
				filebutton.SetFilename(saveentry.Text);
				filePathChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		private void Filebutton_SelectionChanged(object sender, EventArgs e)
		{
			if (action != FileAction.SaveFile)
			{
				saveentry.Text = filebutton.Filename;
				filePathChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		private void Savebutton_Clicked(object sender, EventArgs e)
		{
			var savedialog = new Gtk.FileChooserDialog(string.Empty, null, Gtk.FileChooserAction.Save);
			savedialog.DoOverwriteConfirmation = true;
			savedialog.AddButton(Gtk.Stock.Cancel, Gtk.ResponseType.Cancel);
			savedialog.AddButton(Gtk.Stock.Save, Gtk.ResponseType.Ok);
			savedialog.DefaultResponse = Gtk.ResponseType.Ok;

			savedialog.Title = filebutton.Title;
			foreach (var filter in filebutton.Filters)
				savedialog.AddFilter(filter);
			savedialog.Filter = filebutton.Filter;

			var result = savedialog.Run();
			savedialog.Hide();

			if (result == (int)Gtk.ResponseType.Ok)
				saveentry.Text = savedialog.Filename;
		}

		public int CurrentFilterIndex
		{
			get
			{
				var filters = filebutton.Filters;
				for (int i = 0; i < filters.Length; i++)
					if (filters[i] == filebutton.Filter)
						return i;

				return -1;
			}

			set
			{
				Gtk.FileFilter[] filters = filebutton.Filters;
				filebutton.Filter = filters[value];
			}
		}

		public FileAction FileAction
		{
			get
			{
				return action;
			}
			set
			{
				if (action == FileAction.SaveFile)
					Control.Remove(savebox);
				else
					Control.Remove(filebutton);

				if (value == FileAction.SaveFile)
					Control.Child = savebox;
				else
				{
					if (value == FileAction.OpenFile)
						filebutton.Action = Gtk.FileChooserAction.Open;
					else
						filebutton.Action = Gtk.FileChooserAction.SelectFolder;

					Control.Child = filebutton;
				}

				action = value;
			}
		}

		public string FilePath
		{
			get { return filebutton.Filename; }
			set { filebutton.SetFilename(value); }
		}

		public string Title
		{
			get { return filebutton.Title; }
			set { filebutton.Title = value; }
		}

		public void ClearFilters()
		{
			for (int i = 0; i < filebutton.Filters.Length; i++)
				filebutton.RemoveFilter(filebutton.Filters[0]);
		}

		public void InsertFilter(int index, FileFilter filter)
		{
			var gtkFilter = new Gtk.FileFilter();
			gtkFilter.Name = filter.Name;
			foreach (var extension in filter.Extensions)
				gtkFilter.AddPattern(extension);

			var filters = new List<Gtk.FileFilter>(filebutton.Filters);
			if (index < filters.Count)
			{
				for (int i = 0; i < filters.Count; i++)
					filebutton.RemoveFilter(filters[i]);
				filters.Insert(index, gtkFilter);
				for (int i = 0; i < filters.Count; i++)
					filebutton.AddFilter(filters[i]);
			}
			else
				filebutton.AddFilter(gtkFilter);
		}

		public void RemoveFilter(int index)
		{
			filebutton.RemoveFilter(filebutton.Filters[index]);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case FilePicker.FilePathChangedEvent:
					filePathChanged += (sender, e) => Callback.OnFilePathChanged(Widget, EventArgs.Empty);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
