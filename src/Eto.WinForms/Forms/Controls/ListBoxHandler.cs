using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using Eto.WinForms.Drawing;
using Eto.Drawing;

namespace Eto.WinForms.Forms.Controls
{
	public class ListBoxHandler : WindowsControl<swf.ListBox, ListBox, ListBox.ICallback>, ListBox.IHandler
	{
		IIndirectBinding<string> _itemTextBinding;
		CollectionHandler _collection;

		public static int ItemPadding = 2;

		class EtoListBox : swf.ListBox
		{
			private readonly ListBoxHandler handler;

			public EtoListBox(ListBoxHandler handler)
			{
				this.handler = handler;
				DrawMode = swf.DrawMode.OwnerDrawVariable;
				SetStyle(swf.ControlStyles.UserPaint | swf.ControlStyles.OptimizedDoubleBuffer | swf.ControlStyles.EnableNotifyMessage | swf.ControlStyles.ResizeRedraw, true);
				ResizeRedraw = false;
			}

			public override sd.Font Font
			{
				get { return base.Font; }
				set
				{
					base.Font = value;
					ItemHeight = value.Height;
				}
			}
			
			protected override void OnMeasureItem(swf.MeasureItemEventArgs e)
			{
				base.OnMeasureItem(e);
				e.ItemHeight = (int)Math.Ceiling(Font.GetHeight(e.Graphics)) + ItemPadding * 2;
				var item = Items[e.Index];
				var image = handler.Widget.ItemImageBinding?.GetValue(item);
				if (image != null)
				{
					e.ItemHeight = Math.Max(e.ItemHeight, image.Height + ItemPadding * 2);
					e.ItemWidth += image.Width + ItemPadding * 2;
				}
			}

			protected override void OnPaint(swf.PaintEventArgs e)
			{
				using (var backBrush = new sd.SolidBrush(BackColor))
				{
					e.Graphics.FillRectangle(backBrush, e.ClipRectangle);
				}
				for (int i = 0; i < Items.Count; ++i)
				{
					var itemRect = GetItemRectangle(i);
					if (e.ClipRectangle.IntersectsWith(itemRect))
					{
						var state = swf.DrawItemState.Default;
						if ((SelectionMode == swf.SelectionMode.One && SelectedIndex == i)
						|| (SelectionMode == swf.SelectionMode.MultiSimple && SelectedIndices.Contains(i))
						|| (SelectionMode == swf.SelectionMode.MultiExtended && SelectedIndices.Contains(i)))
						{
							state = swf.DrawItemState.Selected;
						}
						OnDrawItem(new swf.DrawItemEventArgs(e.Graphics, Font, itemRect, i, state, ForeColor, BackColor));
					}
				}
			}

			protected override void OnDrawItem(swf.DrawItemEventArgs e)
			{
				e.DrawBackground();
				e.DrawFocusRectangle();

				if (e.Index == -1)
					return;

				var bounds = e.Bounds;
				var item = Items[e.Index];
				var text = handler.Widget.ItemTextBinding?.GetValue(item) ?? string.Empty;
				var image = handler.Widget.ItemImageBinding?.GetValue(item).ToSD(new Size(bounds.Width, bounds.Height - ItemPadding * 2));
				if (image != null)
				{
					// just in case, make image fit in our bounds
					var imageHeight = bounds.Height - ItemPadding * 2;
					var imageWidth = imageHeight * image.Width / image.Height;
					e.Graphics.DrawImage(image, bounds.X + ItemPadding, bounds.Y + ItemPadding, imageWidth, imageHeight);
					bounds.X += imageWidth + ItemPadding * 2;
				}
				var stringSize = swf.TextRenderer.MeasureText(e.Graphics, text, e.Font);
				bounds.Y += Math.Max(0, (bounds.Height - stringSize.Height) / 2);
				var foreColor = e.State.HasFlag(swf.DrawItemState.Selected) ? sd.SystemColors.HighlightText : ForeColor;
				swf.TextRenderer.DrawText(e.Graphics, text, e.Font, bounds, foreColor, swf.TextFormatFlags.Left);
			}
		}

		public ListBoxHandler()
		{
			Control = new EtoListBox(this);
			Control.SelectedIndexChanged += control_SelectedIndexChanged;
			Control.IntegralHeight = false;
			Control.DoubleClick += control_DoubleClick;
			Control.KeyDown += control_KeyDown;
		}

		public int SelectedIndex
		{
			get { return Control.SelectedIndex; }
			set { Control.SelectedIndex = value; }
		}

		void control_SelectedIndexChanged(object sender, EventArgs e)
		{
			Callback.OnSelectedIndexChanged(Widget, e);
		}

		void control_DoubleClick(object sender, EventArgs e)
		{
			Callback.OnActivated(Widget, EventArgs.Empty);
		}

		void control_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == swf.Keys.Return)
			{
				Callback.OnActivated(Widget, EventArgs.Empty);
				e.Handled = true;
			}
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public ListBoxHandler Handler { get; set; }

			public override int IndexOf(object item)
			{
				return Handler.Control.Items.IndexOf(item);
			}

			public override void AddRange(IEnumerable<object> items)
			{
				Handler.Control.Items.AddRange(items.ToArray());
			}

			public override void AddItem(object item)
			{
				Handler.Control.Items.Add(item);
			}

			public override void InsertItem(int index, object item)
			{
				Handler.Control.Items.Insert(index, item);
			}

			public override void RemoveItem(int index)
			{
				Handler.Control.Items.RemoveAt(index);
			}

			public override void RemoveAllItems()
			{
				Handler.Control.Items.Clear();
			}
		}

		public IEnumerable<object> DataStore
		{
			get { return _collection != null ? _collection.Collection : null; }
			set
			{
				if (_collection != null)
					_collection.Unregister();
				_collection = new CollectionHandler { Handler = this };
				_collection.Register(value);
			}
		}

		public IIndirectBinding<string> ItemTextBinding
		{
			get => _itemTextBinding;
			set
			{
				_itemTextBinding = value;
				Control.Invalidate();
			}
		}

		public IIndirectBinding<string> ItemKeyBinding { get; set; }

		static readonly Win32.WM[] intrinsicEvents = {
														 Win32.WM.LBUTTONDOWN, Win32.WM.LBUTTONUP, Win32.WM.LBUTTONDBLCLK,
														 Win32.WM.RBUTTONDOWN, Win32.WM.RBUTTONUP, Win32.WM.RBUTTONDBLCLK
													 };
		public override bool ShouldBubbleEvent(swf.Message msg)
		{
			return !intrinsicEvents.Contains((Win32.WM)msg.Msg) && base.ShouldBubbleEvent(msg);
		}
	}
}
