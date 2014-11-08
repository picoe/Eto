using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using Eto.Forms;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
	public class ComboBoxHandler : DropDownHandler, ComboBox.IHandler
	{
		Gtk.Entry entry;
		bool editable;

		public override void Create()
		{
			editable = true;
			listStore = new Gtk.ListStore(typeof(string));
#if GTK2
			Control = new Gtk.ComboBoxEntry(listStore, 0);
			text = Control.Cells[0] as Gtk.CellRendererText;
#else
			Control = Gtk.ComboBox.NewWithModelAndEntry(listStore);
			Control.EntryTextColumn = 0;
#endif
			text = Control.Cells[0] as Gtk.CellRendererText;
			Control.SetAttributes(text, "text", 0);
			entry = Control.Child as Gtk.Entry;
			entry.IsEditable = editable;
			Control.Changed += Connector.HandleChanged;
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
					if (editable)
					{
						entry.ModifyFont(newfont);
					}
					text.FontDesc = newfont;
				}
			}
		}

		public override string Text
		{
			get
			{
				return editable ? entry.Text : "";
			}
			set
			{
				if (editable && value != null)
				{
					entry.Text = value;
				}
			}
		}

		public bool IsEditable
		{
			get { return editable; }
			set
			{
				editable = value;
				entry.IsEditable = editable;
			}
		}
	}
}