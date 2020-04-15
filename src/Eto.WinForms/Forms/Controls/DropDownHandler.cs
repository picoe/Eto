using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;

namespace Eto.WinForms.Forms.Controls
{
	public class DropDownHandler : DropDownHandler<EtoComboBox, DropDown, DropDown.ICallback>, DropDown.IHandler
	{

	}
	class EtoComboBoxItem
	{
		DropDown DropDown { get; set; }
		public object Value { get; set; }
		public Image Image => DropDown.ItemImageBinding?.GetValue(Value);

		public override string ToString() => DropDown.ItemTextBinding?.GetValue(Value) ?? string.Empty;

		public EtoComboBoxItem(DropDown dropDown, object value)
		{
			DropDown = dropDown;
			Value = value;
		}
	}

	public class EtoComboBox : swf.ComboBox
	{
		sd.Size? cachedSize;
		public void ResetSize()
		{
			cachedSize = null;
		}

		public sd.Size MinSize { get; set; }

		/*
		protected override void OnMeasureItem(swf.MeasureItemEventArgs e)
		{
			base.OnMeasureItem(e);
			using (var g = CreateGraphics())
			{
				//foreach (object item in Items)
				{
					var font = Font;
					var text = GetItemText(e.Index);
					var itemSize = swf.TextRenderer.MeasureText(g, text, font);
					e.ItemWidth = (int)itemSize.Width;
					e.ItemHeight = (int)itemSize.Height;
				}
			}
		}*/

		public override sd.Size GetPreferredSize(sd.Size proposedSize)
		{
			if (cachedSize == null)
			{
				var size = new sd.Size(16, 20);
				var font = Font;
				foreach (object item in Items)
				{
					var text = GetItemText(item);
					var itemSize = swf.TextRenderer.MeasureText(text, font);
					var image = (item as EtoComboBoxItem)?.Image;
					if (image != null)
						itemSize.Width += 18;
					size.Width = Math.Max(size.Width, (int) itemSize.Width);
					size.Height = Math.Max(size.Height, (int) itemSize.Height);
				}
				// for drop down glyph and border
				if (DrawMode == swf.DrawMode.OwnerDrawFixed)
					size.Width += 4;
				size.Width += 18;
				size.Height += 4;
				size.Width = Math.Max(size.Width, MinSize.Width);
				size.Height = Math.Max(size.Height, MinSize.Height);
				cachedSize = size;
			}
			return cachedSize.Value;
		}

		sd.Color? backColor;
		public new sd.Color BackColor
		{
			get { return backColor ?? base.BackColor; }
			set
			{
				backColor = value;
				DrawMode = swf.DrawMode.OwnerDrawFixed;
				Invalidate();
			}
		}

		public event EventHandler<DropDownFormatEventArgs> FormatItem;


		protected override void OnDrawItem(swf.DrawItemEventArgs e)
		{
			if (e.State.HasFlag(swf.DrawItemState.ComboBoxEdit))
			{
				var bounds = e.Bounds;
				bounds.Inflate(2, 2);
				// only show the background color for the drop down, not for each item
				e.Graphics.FillRectangle(new sd.SolidBrush(BackColor), bounds);
			}
			else
			{
				e.DrawBackground();
			}

			if (e.Index >= 0)
			{
				var item = Items[e.Index];
				var bounds = e.Bounds;
				var etoitem = item as EtoComboBoxItem;
				var image = etoitem?.Image;

				if (image != null)
				{
					e.Graphics.DrawImage(image.ToSD(new Size(16, 16)), bounds.X, bounds.Y, 16, 16);
					bounds.X += 18;
					bounds.Width -= 18;
				}

				string text = item?.ToString();

				var font = Font;
				if (FormatItem != null)
				{
					var args = new DropDownFormatEventArgs(etoitem.Value, e.Index, font.ToEto());
					FormatItem.Invoke(this, args);
					font = args.Font.ToSD();
				}

				// Determine the forecolor based on whether or not the item is selected    
				swf.TextRenderer.DrawText(e.Graphics, text, font, bounds, ForeColor, swf.TextFormatFlags.Left);
			}

			e.DrawFocusRectangle();
		}

		string oldText;
		int oldIndex;

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			if (SelectedIndex == -1 && oldIndex != -1 && oldText != Text)
			{
				OnSelectedIndexChanged(e);
			}
		}

		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			base.OnSelectedIndexChanged(e);
			oldText = Text;
			oldIndex = SelectedIndex;
		}

		bool showBorder = true;
		public bool ShowBorder
		{
			get { return showBorder; }
			set
			{
				SetStyle(swf.ControlStyles.UserPaint, !value);
				showBorder = value;
			}
		}

		protected override void OnPaint(swf.PaintEventArgs e)
		{
			base.OnPaint(e);
		}
	}

	public class DropDownHandler<TControl, TWidget, TCallback> : WindowsControl<TControl, TWidget, TCallback>, DropDown.IHandler
		where TControl: EtoComboBox
		where TWidget: DropDown
		where TCallback: DropDown.ICallback
	{
		IIndirectBinding<string> _itemTextBinding;
		CollectionHandler _collection;

		public bool ShowBorder
		{
			get { return true; }
			set
			{
			}
		}

		public DropDownHandler()
		{
			Control = (TControl)new EtoComboBox
			{
				DropDownStyle = swf.ComboBoxStyle.DropDownList,
				DrawMode = swf.DrawMode.OwnerDrawFixed,
				AutoSize = true,
				Size = new sd.Size(20, 0)
			};
			int lastSelected = -1;
			Control.SelectedIndexChanged += (sender, e) =>
			{
				if (SelectedIndex != lastSelected)
				{
					Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
					lastSelected = SelectedIndex;
				}
			};
		}

		public override Size GetPreferredSize(Size availableSize, bool useCache)
		{
			if (Control.AutoSize)
				return Control.GetPreferredSize(sd.Size.Empty).ToEto();
			return base.GetPreferredSize(availableSize, useCache);
		}

		public int SelectedIndex
		{
			get { return Control.SelectedIndex; }
			set { Control.SelectedIndex = value; }
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public DropDownHandler<TControl, TWidget, TCallback> Handler { get; set; }

			public override void AddRange(IEnumerable<object> items)
			{
				var widget = Handler.Widget;
				Handler.Control.Items.AddRange(items.Select(r => (object)new EtoComboBoxItem(widget, r)).ToArray());
				Handler.UpdateSizes();
			}

			public override void AddItem(object item)
			{
				Handler.Control.Items.Add(new EtoComboBoxItem(Handler.Widget, item));
				Handler.UpdateSizes();
			}

			public override void InsertItem(int index, object item)
			{
				Handler.Control.Items.Insert(index, new EtoComboBoxItem(Handler.Widget, item));
				Handler.UpdateSizes();
			}

			public override void RemoveItem(int index)
			{
				Handler.Control.Items.RemoveAt(index);
				Handler.UpdateSizes();
			}

			public override void RemoveAllItems()
			{
				Handler.Control.Items.Clear();
				Handler.UpdateSizes();
			}
		}

		protected virtual void UpdateSizes()
		{
			Control.ResetSize();
			if (Widget.Loaded)
				SetMinimumSize(true);
		}

		public IEnumerable<object> DataStore
		{
			get { return _collection != null ? _collection.Collection : null; }
			set
			{
				var selected = Widget.SelectedValue;
				_collection?.Unregister();
				_collection = new CollectionHandler { Handler = this };
				_collection.Register(value);
				if (!ReferenceEquals(selected, null))
				{
					var newSelectedIndex = _collection.IndexOf(selected);
					SelectedIndex = newSelectedIndex;
					if (newSelectedIndex == -1)
						Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public IIndirectBinding<string> ItemTextBinding
		{
			get => _itemTextBinding;
			set
			{
				_itemTextBinding = value;
				if (Widget.Loaded)
				{
					Control.Refresh();
					UpdateSizes();
				}
			}

		}
		public IIndirectBinding<string> ItemKeyBinding { get; set; }

		static readonly Win32.WM[] intrinsicEvents = { Win32.WM.LBUTTONDOWN, Win32.WM.LBUTTONUP, Win32.WM.LBUTTONDBLCLK };
		public override bool ShouldBubbleEvent(swf.Message msg)
		{
			return !intrinsicEvents.Contains((Win32.WM)msg.Msg) && base.ShouldBubbleEvent(msg);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case DropDown.DropDownClosedEvent:
					Control.DropDownClosed += (sender, e) => Callback.OnDropDownClosed(Widget, EventArgs.Empty);
					break;
				case DropDown.DropDownOpeningEvent:
					Control.DropDown += (sender, e) => Callback.OnDropDownOpening(Widget, EventArgs.Empty);
					break;
				case DropDown.FormatItemEvent:
					Control.FormatItem += (sender, e) => Callback.OnFormatItem(Widget, e);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
