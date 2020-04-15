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
#if GTK2
			Control = new Gtk.ComboBoxEntry();
#else
			Control = Gtk.ComboBox.NewWithEntry();
			Control.EntryTextColumn = 0;
#endif
			text = Control.Cells[0] as Gtk.CellRendererText;
			entry = (Gtk.Entry)Control.Child;
			entry.IsEditable = true;
			Control.Changed += Connector.HandleChanged;
        }

		protected override void Initialize()
		{
			base.Initialize();
			HandleEvent(ComboBox.TextChangedEvent);
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
				Handler.UpdateSelectedIndexFromText();
			}
		}

		void UpdateSelectedIndexFromText()
		{
			if (collection != null && Widget.ItemTextBinding != null)
			{
				var value = Text;
				// find existing item and set it as selected
				var textBinding = Widget.ItemTextBinding;
				for (int i = 0; i < collection.Count; i++)
				{
					var item = collection.ElementAt(i);
					if (string.Equals(value, textBinding.GetValue(item)))
					{
						base.SelectedIndex = i;
						return;
					}
				}
				base.SelectedIndex = -1;
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
				return font ?? (font = text.FontDesc.ToEto());
			}
			set
			{
				font = value;
				if (font != null)
				{
					var pangoFont = font.ToPango();
					entry.SetFont(pangoFont);
					text.FontDesc = pangoFont;
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
				if (SelectedIndex != value)
				{
					base.SelectedIndex = value;
					if (value == -1)
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
						completion.Model = Control.Model;
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

		public override Color TextColor
		{
			get { return base.TextColor; }
			set
			{
				base.TextColor = value;
				entry.SetTextColor(value);
			}
		}

		public override Color BackgroundColor
		{
			get { return base.BackgroundColor; }
			set
			{
				base.BackgroundColor = value;
				entry.SetBase(value);
			}
		}
	}
}