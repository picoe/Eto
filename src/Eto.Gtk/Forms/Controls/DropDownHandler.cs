using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;
using System.Collections;
using System.Collections.Generic;
using GLib;
using Gtk;

namespace Eto.GtkSharp.Forms.Controls
{
	public class DropDownHandler : DropDownHandler<Gtk.ComboBox, DropDown, DropDown.ICallback>
	{
		protected override void Create()
		{
			Control = new Gtk.ComboBox();
			var imageCell = new Gtk.CellRendererPixbuf();
			Control.PackStart(imageCell, false);
			Control.AddAttribute(imageCell, "pixbuf", 1);
			text = new Gtk.CellRendererText();

			Control.PackStart(text, true);
			Control.Changed += Connector.HandleChanged;
		}
	}

	public abstract class DropDownHandler<TControl, TWidget, TCallback> : GtkControl<TControl, TWidget, TCallback>, DropDown.IHandler, IGtkListModelHandler<object>
		where TControl : Gtk.ComboBox
		where TWidget : DropDown
		where TCallback : DropDown.ICallback
	{
		IIndirectBinding<string> _itemTextBinding;
		protected GtkListModel<object> model;
		protected Eto.Drawing.Font font;
		protected CollectionHandler collection;
		protected Gtk.CellRendererText text;
		protected Gtk.EventBox container;

		protected override void Initialize()
		{
			Create();
			UpdateModel();
			container = new Gtk.EventBox();
			container.Child = Control;
			SetAttributes(false);
			base.Initialize();
		}

		static readonly object SuppressIndexChanged_Key = new object();

		int SuppressIndexChanged
		{
			get => Widget.Properties.Get<int>(SuppressIndexChanged_Key);
			set => Widget.Properties.Set(SuppressIndexChanged_Key, value);
		}

		protected abstract void Create();

		protected new DropDownConnector Connector => (DropDownConnector)base.Connector;

		protected override WeakConnector CreateConnector() => new DropDownConnector();

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

			public virtual void HandlePoppedDown(object o, Gtk.PoppedDownArgs args)
			{
				Handler.Callback.OnDropDownClosed(Handler.Widget, EventArgs.Empty);
			}
#endif
		}

		public override Eto.Drawing.Size Size
		{
			get { return base.Size; }
			set
			{
				if (value.Width == -1)
				{
					text.Ellipsize = Pango.EllipsizeMode.None;
					text.Width = -1;
				}
				else
				{
					text.Ellipsize = Pango.EllipsizeMode.End;
					text.Width = 1;
				}

				base.Size = value;
			}
		}

		public virtual int SelectedIndex
		{
			get { return Control.Active; }
			set { Control.Active = value; }
		}

		public override Gtk.Widget ContainerControl => container;

		public override Gtk.Widget EventControl => container;

		public override Eto.Drawing.Font Font
		{
			get => font ?? (font = text.FontDesc.ToEto());
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

			public override void AddRange(IEnumerable<object> items)
			{
				Handler.UpdateModel();
				Handler.Control.QueueResize();
			}

			public override void AddItem(object item)
			{
				var iter = Handler.model.GetIterAtRow(Handler.Count);
				var path = Handler.model.GetPathAtRow(Handler.Count);
				Handler.Control.Model.EmitRowInserted(path, iter);
				Handler.Control.QueueResize();
			}

			public override void InsertItem(int index, object item)
			{
				var iter = Handler.model.GetIterAtRow(index);
				var path = Handler.model.GetPathAtRow(index);
				Handler.Control.Model.EmitRowInserted(path, iter);
				Handler.Control.QueueResize();
			}

			public override void RemoveItem(int index)
			{
				var path = Handler.model.GetPathAtRow(index);
				Handler.Control.Model.EmitRowDeleted(path);
				Handler.Control.QueueResize();
			}

			public override void RemoveAllItems()
			{
				Handler.UpdateModel();
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

		public virtual Eto.Drawing.Color TextColor
		{
			get { return text.ForegroundGdk.ToEto(); }
			set
			{
				text.ForegroundGdk = value.ToGdk();
				if (Widget.Loaded)
					Control.QueueDraw();
			}
		}

		public override Eto.Drawing.Color BackgroundColor
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
		public int Count => collection?.Count ?? 0;
		public int NumberOfColumns => 3;

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
				case DropDown.FormatItemEvent:
					SetAttributes(true);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected virtual void SetAttributes(bool useFormatting)
		{
			Control.ClearAttributes(text);
			Control.AddAttribute(text, "text", 0);
			if (useFormatting)
			{
				Control.AddAttribute(text, "font-desc", 2);
			}
		}

		protected void UpdateModel()
		{
			model = new GtkListModel<object> { Handler = this };
			Control.Model = new Gtk.TreeModelAdapter(model);
		}

		public object GetItem(int row) => collection?.ElementAt(row);

		Bitmap DefaultImage => new Bitmap(1, 1, PixelFormat.Format32bppRgba);

		public Value GetColumnValue(object item, int column, int row, TreeIter iter)
		{
			if (column == 0)
			{
				// text
				var val = ItemTextBinding?.GetValue(item);
				return new Value(val ?? string.Empty);
			}
			else if (column == 1)
			{
				// image
				var val = Widget.ItemImageBinding?.GetValue(item);
				// Value.Empty causes warnings.. hrm..
				return val != null ? new Value(val.ToGdk()) : new Value((Gdk.Pixbuf)null);
			}
			else if (column == 2)
			{
				// font-desc
				var currentFont = FontControl.GetFont();
				var args = new DropDownFormatEventArgs(item, row, currentFont.ToEto());
				Callback.OnFormatItem(Widget, args);

				if (args.Font != null)
				{
					var val = args.Font.ToPango();
					if (val != null)
						return new Value(val);
				}
				return new Value(currentFont);
			}

			return Value.Empty;
		}

		public int GetRowOfItem(object item) => collection?.IndexOf(item) ?? -1;
	}
}

