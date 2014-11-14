using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using Eto.WinForms.Drawing;

namespace Eto.WinForms.Forms.Controls
{
	public class ListBoxHandler : WindowsControl<swf.ListBox, ListBox, ListBox.ICallback>, ListBox.IHandler
	{
		CollectionHandler collection;

		class MyListBox : swf.ListBox
		{
			public MyListBox()
			{
				DrawMode = swf.DrawMode.OwnerDrawFixed;
				SetStyle(swf.ControlStyles.UserPaint | swf.ControlStyles.OptimizedDoubleBuffer | swf.ControlStyles.EnableNotifyMessage | swf.ControlStyles.ResizeRedraw, true);
				ResizeRedraw = false;
				ItemHeight = 18;
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
				using (var foreBrush = new sd.SolidBrush(ForeColor))
				{
					var bounds = e.Bounds;
					var item = (IListItem)Items[e.Index];
					var imageitem = item as IImageListItem;
					if (imageitem != null && imageitem.Image != null)
					{
						var img = imageitem.Image.Handler as IWindowsImageSource;
						if (img != null)
							e.Graphics.DrawImage(img.GetImageWithSize(bounds.Height), bounds.Left, bounds.Top, bounds.Height, bounds.Height);
						bounds.X += bounds.Height + 2;
					}
					var stringSize = e.Graphics.MeasureString(item.Text, e.Font);
					var adjust = Math.Max(0, (bounds.Height - stringSize.Height) / 2);
					e.Graphics.DrawString(item.Text, e.Font, foreBrush, bounds.Left, bounds.Top + adjust);
				}
			}
		}

		public ListBoxHandler()
		{
			Control = new MyListBox();
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
			get { return collection != null ? collection.Collection : null; }
			set
			{
				if (collection != null)
					collection.Unregister();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
			}
		}

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
