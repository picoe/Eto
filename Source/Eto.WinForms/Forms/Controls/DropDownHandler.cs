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

	public class EtoComboBox : swf.ComboBox
	{
		sd.Size? cachedSize;
		public void ResetSize()
		{
			cachedSize = null;
		}

		public sd.Size MinSize { get; set; }

		public override sd.Size GetPreferredSize(sd.Size proposedSize)
		{
			if (cachedSize == null)
			{
				var size = new sd.Size(16, 20);
				var font = Font;
				using (var g = CreateGraphics())
				{
					foreach (object item in Items)
					{
						var text = GetItemText(item);
						var itemSize = swf.TextRenderer.MeasureText(g, text, font);
						size.Width = Math.Max(size.Width, (int) itemSize.Width);
						size.Height = Math.Max(size.Height, (int) itemSize.Height);
					}
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
				string text = Items[e.Index].ToString();

				// Determine the forecolor based on whether or not the item is selected    
				swf.TextRenderer.DrawText(e.Graphics, text, Font, e.Bounds, ForeColor, swf.TextFormatFlags.Left);
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
		CollectionHandler collection;

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

		class Item
		{
			public IIndirectBinding<string> Binding { get; set; }
			public object Value { get; set; }
			public override string ToString()
			{
				return Binding.GetValue(Value);
			}
			public Item(IIndirectBinding<string> binding, object value)
			{
				Binding = binding;
				Value = value;
			}
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public DropDownHandler<TControl, TWidget, TCallback> Handler { get; set; }

			public override void AddRange(IEnumerable<object> items)
			{
				var binding = Handler.Widget.ItemTextBinding;
				Handler.Control.Items.AddRange(items.Select(r => (object)new Item(binding, r)).ToArray());
				Handler.UpdateSizes();
			}

			public override void AddItem(object item)
			{
				var binding = Handler.Widget.ItemTextBinding;
				Handler.Control.Items.Add(new Item(binding, item));
				Handler.UpdateSizes();
			}

			public override void InsertItem(int index, object item)
			{
				var binding = Handler.Widget.ItemTextBinding;
				Handler.Control.Items.Insert(index, new Item(binding, item));
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
			if (Widget.Loaded)
				SetMinimumSize();
			Control.ResetSize();
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

		static readonly Win32.WM[] intrinsicEvents = { Win32.WM.LBUTTONDOWN, Win32.WM.LBUTTONUP, Win32.WM.LBUTTONDBLCLK };
		public override bool ShouldBubbleEvent(swf.Message msg)
		{
			return !intrinsicEvents.Contains((Win32.WM)msg.Msg) && base.ShouldBubbleEvent(msg);
		}
	}
}
