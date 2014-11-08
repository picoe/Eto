using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;
using System.Collections;
using System.Collections.Generic;

namespace Eto.GtkSharp.Forms.Controls
{
	public class DropDownHandler : GtkControl<Gtk.ComboBox, DropDown, DropDown.ICallback>, DropDown.IHandler
	{
		public Font font;
		public CollectionHandler collection;
		public Gtk.ListStore listStore;
		public Gtk.CellRendererText text;

		public virtual void Create()
		{
			listStore = new Gtk.ListStore(typeof(string));
			Control = new Gtk.ComboBox(listStore);
			text = new Gtk.CellRendererText();
			Control.PackStart(text, false);
			Control.SetAttributes(text, "text", 0);
			Control.Changed += Connector.HandleChanged;
		}

		protected new ComboBoxConnector Connector { get { return (ComboBoxConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new ComboBoxConnector();
		}

		protected class ComboBoxConnector : GtkControlConnector
		{
			public new DropDownHandler Handler { get { return (DropDownHandler)base.Handler; } }

			public void HandleChanged(object sender, EventArgs e)
			{
				Handler.Callback.OnSelectedIndexChanged(Handler.Widget, EventArgs.Empty);
			}
		}

		public int SelectedIndex
		{
			get { return Control.Active; }
			set { Control.Active = value; }
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
					text.FontDesc = ((FontHandler) font.Handler).Control;
				}
			}
		}

		public class CollectionHandler : EnumerableChangedHandler<object>
		{
			public DropDownHandler Handler { get; set; }

			public override void AddItem(object item)
			{
				var binding = Handler.Widget.TextBinding;
				Handler.listStore.AppendValues(binding.GetValue(item));
				Handler.Control.QueueResize();
			}

			public override void InsertItem(int index, object item)
			{
				var binding = Handler.Widget.TextBinding;
				Handler.listStore.InsertWithValues(index, binding.GetValue(item));
				Handler.Control.QueueResize();
			}

			public override void RemoveItem(int index)
			{
				Gtk.TreeIter iter;
				if (Handler.listStore.IterNthChild(out iter, index))
					Handler.listStore.Remove(ref iter);
				Handler.Control.QueueResize();
			}

			public override void RemoveAllItems()
			{
				Handler.listStore.Clear();
				Handler.Control.QueueResize();
			}
		}

		public IEnumerable<object> DataStore
		{
			get { return collection != null ? collection.Collection : null; }
			set
			{
				if (collection != null)
					collection.Unregister();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
			}
		}

		public Color TextColor
		{
			get { return text.ForegroundGdk.ToEto(); }
			set
			{
				text.ForegroundGdk = value.ToGdk();
				if (Widget.Loaded)
					Control.QueueDraw();
			}
		}

		public override Color BackgroundColor
		{
			get { return Control.Child.Style.Base(Gtk.StateType.Normal).ToEto(); }
			set
			{
				Control.Child.ModifyBg(Gtk.StateType.Normal, value.ToGdk());
				Control.Child.ModifyBase(Gtk.StateType.Normal, value.ToGdk());
				Control.Child.ModifyFg(Gtk.StateType.Normal, value.ToGdk());
				Control.ModifyBg(Gtk.StateType.Normal, value.ToGdk());
				Control.ModifyBase(Gtk.StateType.Normal, value.ToGdk());
				Control.ModifyFg(Gtk.StateType.Normal, value.ToGdk());
				if (Widget.Loaded)
					Control.QueueDraw();
			}
		}
	}
}

