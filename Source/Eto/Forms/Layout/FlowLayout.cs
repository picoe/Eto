using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Eto.Drawing;

namespace Eto.Forms
{
	[ContentProperty("Contents")]
	public class FlowLayout : CustomLayout
	{
		readonly FlowCollection collection;

		public override IEnumerable<Control> Controls
		{
			get { return collection; }
		}

		public IList<Control> Contents
		{
			get { return collection; }
		}

		class FlowCollection : Collection<Control>
		{
			public FlowLayout Layout { get; set; }

			protected override void InsertItem(int index, Control item)
			{
				base.InsertItem(index, item);
				Layout.Handler.Add(item);
				item.SizeChanged += Layout.UpdateSize;
			}

			protected override void RemoveItem(int index)
			{
				var item = this[index];
				item.SizeChanged -= Layout.UpdateSize;
				base.RemoveItem(index);
				Layout.Handler.Remove(item);
			}

			protected override void SetItem(int index, Control item)
			{
				var oldItem = this[index];
				oldItem.SizeChanged -= Layout.UpdateSize;
				base.SetItem(index, item);
				Layout.Handler.Remove(oldItem);
				Layout.Handler.Add(item);
				item.SizeChanged += Layout.UpdateSize;
			}

			protected override void ClearItems()
			{
				Layout.Handler.RemoveAll();
				base.ClearItems();
			}
		}

		public FlowLayout()
		{
			collection = new FlowCollection { Layout = this };
			HandleEvent(SizeChangedEvent);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			UpdateLayout();
			base.OnSizeChanged(e);
		}

		bool updating;

		public override void UpdateLayout()
		{
			base.UpdateLayout();
			if (updating)
				return;
			updating = true;
			try
			{
				var size = this.Size;
				var location = PointF.Empty;
				var maxSize = SizeF.Empty;
				var oldMargin = 0;
				var alignControls = new List<Tuple<Control, RectangleF>>();
				for (int i = 0; i < collection.Count; i++)
				{
					var item = collection[i];
					var margin = item.Margin;
					var prefSize = item.GetPreferredSize(size);
					location.X += margin.Left;
					if (location.X + Math.Max(margin.Left, oldMargin) + prefSize.Width + margin.Right > size.Width)
					{
						AlignRow(maxSize, alignControls);
						location.X = 0;
						location.Y += maxSize.Height;
						oldMargin = 0;
						maxSize = Size.Empty;
					}
					maxSize = SizeF.Max(maxSize, prefSize + margin.Size);

					var controlLoc = location;
					controlLoc.X += Math.Max(margin.Left, oldMargin);
					controlLoc.Y += margin.Top;
					oldMargin = margin.Right;
					var controlRect = new RectangleF(controlLoc, prefSize);
					if (item.VerticalAlignment != Forms.VerticalAlignment.Top)
						alignControls.Add(Tuple.Create(item, controlRect));
					else
						Handler.Move(item, Rectangle.Round(controlRect));
					location.X = controlLoc.X + prefSize.Width;

				}

				AlignRow(maxSize, alignControls);

			}
			finally
			{
				updating = false;
			}
		}

		void AlignRow(Drawing.SizeF maxSize, List<Tuple<Control, RectangleF>> adjustControls)
		{
			foreach (var c in adjustControls)
			{
				var rect = c.Item2;
				switch (c.Item1.VerticalAlignment)
				{
					case Forms.VerticalAlignment.Center:
						rect.Y += (maxSize.Height - rect.Height) / 2;
						break;
					case Forms.VerticalAlignment.Bottom:
						rect.Y += maxSize.Height - rect.Height;
						break;
					case Forms.VerticalAlignment.Stretch:
						rect.Height = maxSize.Height;
						break;
				}
				Handler.Move(c.Item1, Rectangle.Round(rect));
			}
			adjustControls.Clear();
		}

		public override SizeF GetPreferredSize(SizeF availableSize)
		{
			var size = availableSize;
			var location = PointF.Empty;
			var maxSize = SizeF.Empty;
			var oldMargin = 0;
			float maxX = 0;
			for (int i = 0; i < collection.Count; i++)
			{
				var item = collection[i];
				var margin = item.Margin;
				var prefSize = item.GetPreferredSize(size);
				location.X += margin.Left;
				if (location.X + Math.Max(margin.Left, oldMargin) + prefSize.Width + margin.Right > size.Width)
				{
					location.X = 0;
					location.Y += maxSize.Height;
					oldMargin = 0;
					maxSize = Size.Empty;
				}
				maxSize = SizeF.Max(maxSize, prefSize + margin.Size);

				var controlLoc = location;
				controlLoc.X += Math.Max(margin.Left, oldMargin);
				controlLoc.Y += margin.Top;
				oldMargin = margin.Right;
				location.X = controlLoc.X + prefSize.Width;

				maxX = Math.Max(maxX, location.X);
			}
			return new SizeF(maxX, location.Y + maxSize.Height);
		}

		public override void Remove(Control child)
		{
			collection.Remove(child);
		}

		void UpdateSize(object sender, EventArgs e)
		{
			if (Loaded)
				UpdateLayout();
		}
	}
}

