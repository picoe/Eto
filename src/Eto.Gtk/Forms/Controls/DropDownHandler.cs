using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;
using System.Collections;
using System.Collections.Generic;
using Gtk;

namespace Eto.GtkSharp.Forms.Controls
{
	public class DropDownHandler : DropDownHandler<Gtk.ComboBox, DropDown, DropDown.ICallback>
	{
		protected override void Create()
		{
			listStore = new Gtk.ListStore(typeof(string), typeof(Gdk.Pixbuf));
			Control = new Gtk.ComboBox(listStore);
			var imageCell = new Gtk.CellRendererPixbuf();
			Control.PackStart(imageCell, false);
			Control.SetAttributes(imageCell, "pixbuf", 1);
			text = new Gtk.CellRendererText();
			Control.PackStart(text, true);
			Control.SetAttributes(text, "text", 0);
			Control.Changed += Connector.HandleChanged;
		}
	}

	public abstract class DropDownHandler<TControl, TWidget, TCallback> : GtkControl<TControl, TWidget, TCallback>, DropDown.IHandler
		where TControl : Gtk.ComboBox
		where TWidget : DropDown
		where TCallback : DropDown.ICallback
	{
		IIndirectBinding<string> _itemTextBinding;
		protected Font font;
		protected CollectionHandler collection;
		protected Gtk.ListStore listStore;
		protected Gtk.CellRendererText text;
		protected Gtk.EventBox container;

		protected override void Initialize()
		{
			Create();
			container = new Gtk.EventBox();
			container.Child = Control;
			base.Initialize();
		}

		static readonly object SuppressIndexChanged_Key = new object();

		int SuppressIndexChanged
		{
			get => Widget.Properties.Get<int>(SuppressIndexChanged_Key);
			set => Widget.Properties.Set(SuppressIndexChanged_Key, value);
		}

		protected abstract void Create();

		protected new DropDownConnector Connector { get { return (DropDownConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new DropDownConnector();
		}

		protected class DropDownConnector : GtkControlConnector
		{
			protected int lastIndex = -1;
			public new DropDownHandler<TControl, TWidget, TCallback> Handler { get { return (DropDownHandler<TControl, TWidget, TCallback>)base.Handler; } }

			public virtual void HandleChanged(object sender, EventArgs e)
			{
				if (Handler.SuppressIndexChanged > 0)
					return;
				var newIndex = Handler.SelectedIndex;
				if (newIndex != lastIndex)
				{
					Handler.Callback.OnSelectedIndexChanged(Handler.Widget, EventArgs.Empty);
					lastIndex = newIndex;
				}
			}

#if GTK2
			internal void HandlePopupShownChanged(object o, GLib.NotifyArgs args)
			{
				if (Handler.Control.PopupShown)
					Handler.Callback.OnDropDownOpening(Handler.Widget, EventArgs.Empty);
				else
					Handler.Callback.OnDropDownClosed(Handler.Widget, EventArgs.Empty);
			}
#elif GTK3
			[GLib.ConnectBefore]
			public virtual void HandlePoppedUp(object sender, EventArgs e)
			{
				Handler.Callback.OnDropDownOpening(Handler.Widget, EventArgs.Empty);
			}

			public virtual void HandlePoppedDown(object o, PoppedDownArgs args)
			{
				Handler.Callback.OnDropDownClosed(Handler.Widget, EventArgs.Empty);
			}
#endif
		}

		public override Size Size
		{
			get { return base.Size; }
			set
			{
				if (value.Width == -1)
					text.Ellipsize = Pango.EllipsizeMode.None;
				else
					text.Ellipsize = Pango.EllipsizeMode.End;

				base.Size = value;
			}
		}

		public virtual int SelectedIndex
		{
			get { return Control.Active; }
			set { Control.Active = value; }
		}

		public override Gtk.Widget ContainerControl
		{
			get { return container; }
		}

		public override Gtk.Widget EventControl
		{
			get { return container; }
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
				Handler.listStore.AppendValues(GetValues(item));
				Handler.Control.QueueResize();
			}

			object[] GetValues(object dataItem)
			{
				return new object[] {
					Handler.Widget.ItemTextBinding?.GetValue(dataItem) ?? string.Empty,
					Handler.Widget.ItemImageBinding?.GetValue(dataItem).ToGdk()
				};
			}

			public override void InsertItem(int index, object item)
			{
				Handler.listStore.InsertWithValues(index, GetValues(item));
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
				SuppressIndexChanged++;
				var selected = Widget.SelectedValue;
				collection?.Unregister();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
				if (!ReferenceEquals(selected, null))
				{
					SelectedIndex = collection.IndexOf(selected);
					Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
				}
				SuppressIndexChanged--;
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

		public IIndirectBinding<string> ItemTextBinding
		{
			get => _itemTextBinding;
			set
			{
				_itemTextBinding = value;
				if (Widget.Loaded)
					Control.QueueDraw();
			}
		}
		public IIndirectBinding<string> ItemKeyBinding { get; set; }

		public override void AttachEvent(string id)
		{
			switch (id)
			{
#if GTK2
				case DropDown.DropDownOpeningEvent:
					Control.AddNotification("popup-shown", Connector.HandlePopupShownChanged);
					break;
				case DropDown.DropDownClosedEvent:
					HandleEvent(DropDown.DropDownOpeningEvent);
					break;
#elif GTK3
				case DropDown.DropDownOpeningEvent:
					Control.PoppedUp += Connector.HandlePoppedUp;
					break;
				case DropDown.DropDownClosedEvent:
					Control.PoppedDown += Connector.HandlePoppedDown;
					break;
#endif
				case Eto.Forms.Control.ShownEvent:
					Control.Mapped += Connector.MappedEvent;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}

