using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;
using System.Collections;
using System.Collections.Generic;

namespace Eto.GtkSharp.Forms.Controls
{
	public class DropDownHandler : DropDownHandler<Gtk.ComboBox, DropDown, DropDown.ICallback>
	{
		protected override void Create()
		{
			listStore = new Gtk.ListStore(typeof(string));
			Control = new Gtk.ComboBox(listStore);
			text = new Gtk.CellRendererText();
			Control.PackStart(text, false);
			Control.SetAttributes(text, "text", 0);
			Control.Changed += Connector.HandleChanged;
		}
	}

	public abstract class DropDownHandler<TControl, TWidget, TCallback> : GtkControl<TControl, TWidget, TCallback>, DropDown.IHandler
		where TControl: Gtk.ComboBox
		where TWidget: DropDown
		where TCallback: DropDown.ICallback
	{
		protected Font font;
		CollectionHandler collection;
		protected Gtk.ListStore listStore;
		protected Gtk.CellRendererText text;

		protected override void Initialize()
		{
			Create();
			base.Initialize();
		}

		protected abstract void Create();

		protected new DropDownConnector Connector { get { return (DropDownConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new DropDownConnector();
		}

		protected class DropDownConnector : GtkControlConnector
		{
			int? lastIndex;
			public new DropDownHandler<TControl, TWidget, TCallback> Handler { get { return (DropDownHandler<TControl, TWidget, TCallback>)base.Handler; } }

			public void HandleChanged(object sender, EventArgs e)
			{
				var newIndex = Handler.SelectedIndex;
				if (newIndex != lastIndex)
				{
					Handler.Callback.OnSelectedIndexChanged(Handler.Widget, EventArgs.Empty);
					lastIndex = newIndex;
				}
			}
		}

		public virtual int SelectedIndex
		{
			get { return Control.Active; }
			set { Control.Active = value; }
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
					text.FontDesc = ((FontHandler)font.Handler).Control;
				}
			}
		}

		public class CollectionHandler : EnumerableChangedHandler<object>
		{
			public DropDownHandler<TControl, TWidget, TCallback> Handler { get; set; }

			public override void AddItem(object item)
			{
				var binding = Handler.Widget.ItemTextBinding;
				Handler.listStore.AppendValues(binding.GetValue(item));
				Handler.Control.QueueResize();
			}

			public override void InsertItem(int index, object item)
			{
				var binding = Handler.Widget.ItemTextBinding;
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

		public virtual Color TextColor
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
			get { return Control.Child.GetBackground(); }
			set
			{
				Control.Child.SetBackground(value);
				Control.Child.SetBase(value);
				Control.SetBackground(value);
				Control.SetBase(value);
				if (Widget.Loaded)
					Control.QueueDraw();
			}
		}
	}
}

