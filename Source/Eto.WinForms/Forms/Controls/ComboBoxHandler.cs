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
						var itemSize = graphics.MeasureString(text, font);
						size.Width = Math.Max(size.Width, (int)itemSize.Width);
						size.Height = Math.Max(size.Height, (int)itemSize.Height);
					}
					// for drop down glyph and border
					if (DrawMode == swf.DrawMode.OwnerDrawFixed)
						size.Width += 4;
					size.Width += 18;
					size.Height += 4;
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
				}
			}

			protected override void OnDrawItem(swf.DrawItemEventArgs e)
			{
				if (e.State.HasFlag(swf.DrawItemState.ComboBoxEdit))
				{
					// only show the background color for the drop down, not for each item
					e.Graphics.FillRectangle(new sd.SolidBrush(BackColor), e.Bounds);
				}
				else
				{
					e.DrawBackground();
				}

				if (e.Index >= 0)
				{
					string text = Items[e.Index].ToString();

					// Determine the forecolor based on whether or not the item is selected    
					e.Graphics.DrawString(text, Font, new sd.SolidBrush(ForeColor), e.Bounds.X, e.Bounds.Y);
				}

				e.DrawFocusRectangle();
			}
		}

		public ComboBoxHandler()
		{
			Control = new EtoComboBox
			{
				DropDownStyle = swf.ComboBoxStyle.DropDownList,
				AutoSize = true,
				Size = new sd.Size(20, 0)
			};
			Control.SelectedIndexChanged += delegate
			{
				Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
			};
		}

		public override Color BackgroundColor
		{
			get
			{
				return Control.BackColor.ToEto();
			}
			set
			{
				Control.BackColor = value.ToSD();
			}
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
			public ComboBoxHandler Handler { get; set; }

			public override void AddRange(IEnumerable<object> items)
			{
				var binding = Handler.Widget.TextBinding;
				Handler.Control.Items.AddRange(items.Select(r => (object)new Item(binding, r)).ToArray());
				Handler.UpdateSizes();
			}

			public override void AddItem(object item)
			{
				var binding = Handler.Widget.TextBinding;
				Handler.Control.Items.Add(new Item(binding, item));
				Handler.UpdateSizes();
			}

			public override void InsertItem(int index, object item)
			{
				var binding = Handler.Widget.TextBinding;
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
