using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;

namespace Eto.WinForms
{
	public class ComboBoxHandler : WindowsControl<ComboBoxHandler.EtoComboBox, ComboBox, ComboBox.ICallback>, ComboBox.IHandler
	{
		CollectionHandler collection;

		public class EtoComboBox : swf.ComboBox
		{
			sd.Size? cachedSize;
			public void ResetSize()
			{
				cachedSize = null;
			}
			static readonly sd.Graphics graphics = sd.Graphics.FromHwnd(IntPtr.Zero);

			public override sd.Size GetPreferredSize(sd.Size proposedSize)
			{
				if (cachedSize == null)
				{
					var size = new sd.Size(16, 20);
					var font = Font;

					foreach (object item in Items)
					{
						var text = GetItemText(item);
						var itemSize = graphics.MeasureString(text, font).ToEtoSize();
						// for drop down glyph and border
						size.Width = Math.Max(size.Width, itemSize.Width);
						size.Height = Math.Max(size.Height, itemSize.Height);
					}
					size.Width += 18;
					size.Height += 4;
					cachedSize = size;
				}
				return cachedSize.Value;
			}
		}

		public ComboBoxHandler()
		{
			Control = new EtoComboBox
			{
				DropDownStyle = swf.ComboBoxStyle.DropDownList,
				ValueMember = "Key",
				DisplayMember = "Text",
				AutoSize = true,
				Size = new sd.Size(20, 0)
			};
			Control.SelectedIndexChanged += delegate
			{
				Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
			};
		}

		public override Size GetPreferredSize(Size availableSize)
		{
			if (Control.AutoSize)
				return Control.GetPreferredSize(sd.Size.Empty).ToEto();
			return base.GetPreferredSize(availableSize);
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
			public ComboBoxHandler Handler { get; set; }

			public override int IndexOf(object item)
			{
				return Handler.Control.Items.IndexOf(item);
			}

			public override void AddRange(IEnumerable<object> items)
			{
				var binding = Handler.Widget.TextBinding;
				Handler.Control.Items.AddRange(items.Select(r => new Item(binding, r)).ToArray());
				Handler.UpdateSizes();
			}

			public override void AddItem(object item)
			{
				Handler.Control.Items.Add(item);
				Handler.UpdateSizes();
			}

			public override void InsertItem(int index, object item)
			{
				Handler.Control.Items.Insert(index, item);
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

		protected void UpdateSizes()
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
	}
}
