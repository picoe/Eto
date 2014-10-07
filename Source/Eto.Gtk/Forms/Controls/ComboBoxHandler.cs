using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;
using System.Collections;
using System.Collections.Generic;

namespace Eto.GtkSharp.Forms.Controls
{
	public class ComboBoxHandler : GtkControl<Gtk.ComboBox, ComboBox, ComboBox.ICallback>, ComboBox.IHandler
	{
		Font font;
		CollectionHandler collection;
		readonly Gtk.ListStore listStore;
		readonly Gtk.CellRendererText text;

		public ComboBoxHandler()
		{
			listStore = new Gtk.ListStore(typeof(string));
			Control = new Gtk.ComboBox(listStore);
			text = new Gtk.CellRendererText();
			Control.PackStart(text, false);
			Control.AddAttribute(text, "text", 0);
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Changed += Connector.HandleChanged;
		}

		protected new ComboBoxConnector Connector { get { return (ComboBoxConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new ComboBoxConnector();
		}

		protected class ComboBoxConnector : GtkControlConnector
		{
			public new ComboBoxHandler Handler { get { return (ComboBoxHandler)base.Handler; } }

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
				if (font == null)
					font = new Font(new FontHandler(text.FontDesc));
				return font;
			}
			set
			{
				font = value;
				text.FontDesc = font != null ? ((FontHandler)font.Handler).Control : null;
			}
		}

		public class CollectionHandler : EnumerableChangedHandler<object>
		{
			public ComboBoxHandler Handler { get; set; }

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
				collection = new CollectionHandler{ Handler = this };
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

