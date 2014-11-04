using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using Eto.Forms;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
	public class ComboBoxHandler : DropDownHandler<Gtk.ComboBox, ComboBox, ComboBox.ICallback>, ComboBox.IHandler
	{
		Gtk.Entry entry;
		Gtk.EntryCompletion completion;

		protected override void Create()
		{
			listStore = new Gtk.ListStore(typeof(string));
#if GTK2
			Control = new Gtk.ComboBoxEntry(listStore, 0);
#else
			Control = Gtk.ComboBox.NewWithModelAndEntry(listStore);
			Control.EntryTextColumn = 0;
#endif
			text = Control.Cells[0] as Gtk.CellRendererText;
			Control.SetAttributes(text, "text", 0);
			entry = (Gtk.Entry)Control.Child;
			entry.IsEditable = true;
			Control.Changed += Connector.HandleChanged;
		}

		protected new ComboBoxConnector Connector { get { return (ComboBoxConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new ComboBoxConnector();
		}

		protected class ComboBoxConnector : DropDownConnector
		{
			public new ComboBoxHandler Handler { get { return (ComboBoxHandler)base.Handler; } }

			public void HandleTextChanged(object sender, EventArgs e)
			{
				Handler.Callback.OnTextChanged(Handler.Widget, EventArgs.Empty);
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ComboBox.TextChangedEvent:
					entry.Changed += Connector.HandleTextChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public override Font Font
		{
			get
			{
				return font ?? (font = new Font(new FontHandler(text.FontDesc)));
			}
			set
			{
				font = value;
				if (font != null)
				{
					var newfont = ((FontHandler)font.Handler).Control;
					entry.ModifyFont(newfont);
					text.FontDesc = newfont;
				}
			}
		}

		public override string Text
		{
			get { return entry.Text; }
			set { entry.Text = value ?? string.Empty; }
		}

		public bool ReadOnly
		{
			get { return !entry.IsEditable; }
			set { entry.IsEditable = !value; }
		}

		public override int SelectedIndex
		{
			get
			{
				return base.SelectedIndex;
			}
			set
			{
				base.SelectedIndex = value;
				if (value == -1)
				{
					Text = null;
				}
			}
		}

		public bool AutoComplete
		{
			get { return completion != null; }
			set
			{
				if (AutoComplete != value)
				{
					if (value)
					{
						completion = new Gtk.EntryCompletion();
						completion.Model = listStore;
						completion.MinimumKeyLength = 1;
						completion.TextColumn = 0;
						entry.Completion = completion;
					}
					else
					{
						completion = null;
						entry.Completion = null;
					}
				}
			}
		}
	}
}