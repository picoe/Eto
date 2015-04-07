using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Eto.Drawing;
using System.Linq;
using System.Collections;

namespace Eto.Forms
{
	[ContentProperty("Rows")]
	public class TableLayout2 : CustomLayout
	{
		readonly RowCollection collection;

		public override IEnumerable<Control> Controls
		{
			get { return collection.SelectMany(r => r.Where(c => c != null)); }
		}

		List<TableColumn> columns = new List<TableColumn>();

		public IList<TableColumn> Columns
		{
			get { return columns; }
		}

		public IList<TableRow2> Rows
		{
			get { return collection; }
		}

		class RowCollection : Collection<TableRow2>, IList
		{
			public TableLayout2 Layout { get; set; }

			protected override void InsertItem(int index, TableRow2 item)
			{
				if (item == null)
					item = new TableRow2 { Height = new TableLength(1f, TableUnit.Star) };
				base.InsertItem(index, item);
				AddRow(item);
			}

			void AddRow(TableRow2 row)
			{
				if (row == null)
					return;
				row.Layout = Layout;
				foreach (var ctl in row.Where(r => r != null))
					Layout.Handler.Add(ctl);
			}
			void RemoveRow(TableRow2 row)
			{
				if (row == null)
					return;
				foreach (var ctl in row.Where(r => r != null))
					Layout.Handler.Remove(ctl);
			}

			protected override void RemoveItem(int index)
			{
				var item = this[index];
				base.RemoveItem(index);
				RemoveRow(item);
			}

			protected override void SetItem(int index, TableRow2 item)
			{
				if (item == null)
					item = new TableRow2 { ScaleHeight = true };

				var oldItem = this[index];
				base.SetItem(index, item);
				RemoveRow(oldItem);
				AddRow(item);
			}

			protected override void ClearItems()
			{
				Layout.Handler.RemoveAll();
				base.ClearItems();
			}

			int IList.Add(object value)
			{
				// allow adding a control directly from xaml
				var control = value as Control;
				if (control != null)
					Add((TableRow2)control);
				else
					Add((TableRow2)value);
				return Count - 1;
			}

		}

		public TableLayout2()
		{
			collection = new RowCollection { Layout = this };
			HandleEvent(SizeChangedEvent);
		}

		[Obsolete("Use the Rows/Columns properties instead")]
		public TableLayout2(int columns, int rows)
			: this()
		{
			Ensure(columns, rows);
		}

		[Obsolete("Use the Rows/Columns properties instead")]
		public TableLayout2(Size dimensions)
			: this(dimensions.Width, dimensions.Height)
		{
		}

		[Obsolete("Use the Rows/Columns properties instead")]
		public void Add(Control control, int x, int y)
		{
			EnsureRows(y + 1);
			var row = Rows[y];
			while (row.Count < x + 1)
			{
				row.Add(new Panel());
			}
			row[x] = control;
		}

		[Obsolete("Use the Rows/Columns properties instead")]
		public void Add(Control control, int x, int y, bool xscale, bool yscale)
		{
			Add(control, x, y);
			SetRowScale(y, yscale);
			SetColumnScale(x, xscale);
		}

		[Obsolete("Use the Rows/Columns properties instead")]
		public void Add(Control control, Point location)
		{
			Add(control, location.X, location.Y);
		}

		[Obsolete("Use TableRow.Height instead")]
		public void SetRowScale(int row, bool scale = true)
		{
			Rows[row].Height = scale ? TableLength.Star() : TableLength.Auto;
		}

		[Obsolete("Use TableRow.Height instead")]
		public bool GetRowScale(int row)
		{
			return Rows[row].Height.IsStar;
		}

		[Obsolete("Use TableColumn.Width instead")]
		public void SetColumnScale(int column, bool scale = true)
		{
			EnsureColumns();
			Columns[column].Width = scale ? TableLength.Star() : TableLength.Auto;
		}

		[Obsolete("Use TableColumn.Width instead")]
		public bool GetColumnScale(int column)
		{
			return Columns[column].Width.IsStar;
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			UpdateLayout();
		}

		public Size Spacing { get; set; }

		public Padding Padding { get; set; }

		bool updating;

		protected override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			EnsureColumns();
			UpdateLayout();
		}

		void EnsureColumns()
		{
			var columnCount = Rows.Max(r => r.Count);
			while (Columns.Count < columnCount)
			{
				Columns.Add(new TableColumn());
			}
		}

		public override void UpdateLayout()
		{
			//base.UpdateLayout();
			if (!Loaded || updating)
				return;
			updating = true;
			try
			{
				var controlSize = ClientSize;
				var totalPadding = new SizeF(
					Padding.Horizontal + Spacing.Width * (Columns.Count - 1),
					Padding.Vertical + Spacing.Height * (Rows.Count - 1)
				);
				var required = SizeF.Empty;

				var num = new Size(Math.Max(1, Columns.Count(r => r.Width.IsStar)), Math.Max(1, Rows.Count(r => r.Height.IsStar)));

				var availableChildSize = SizeF.Max(SizeF.Empty, controlSize - totalPadding);

				var weight = SizeF.Empty;
				for (int x = 0; x < Columns.Count; x++)
				{
					var col = Columns[x];
					col.ActualWidth = col.Width.IsAbsolute ? col.Width.Value : 0;
					required.Width += col.ActualWidth;
					if (col.Width.IsStar)
						weight.Width += col.Width.Value;
				}

				for (int y = 0; y < Rows.Count; y++)
				{
					var row = Rows[y];
					row.ActualHeight = row.Height.IsAbsolute ? row.Height.Value : 0;
					required.Height += row.ActualHeight;
					if (row.Height.IsStar)
						weight.Height += row.Height.Value;
					for (int x = 0; x < row.Count; x++)
					{	
						var col = Columns[x];
						var view = row[x];
						if (view != null && view.Visible && (col.Width.IsAuto || row.Height.IsAuto))
						{
							var size = view.GetPreferredSize(availableChildSize);
							if (col.Width.IsAuto && size.Width > col.ActualWidth)
							{
								required.Width += size.Width - col.ActualWidth;
								col.ActualWidth = size.Width;
							}
							if (row.Height.IsAuto && size.Height > row.ActualHeight)
							{
								required.Height += size.Height - row.ActualHeight;
								row.ActualHeight = size.Height;
							}
						}
					}
				}
				var availStarSize = SizeF.Max(SizeF.Empty, availableChildSize - required);
				for (int y = 0; y < Rows.Count; y++)
				{
					var row = Rows[y];
					var ystar = row.Height.IsStar;
					var availCellSize = SizeF.Empty;
					if (ystar)
						availCellSize.Height = availStarSize.Height * row.Height.Value / weight.Height;
					else
						availCellSize.Height = row.ActualHeight;
					for (int x = 0; x < row.Count; x++)
					{	
						var view = row[x];
						var col = Columns[x];
						var xstar = col.Width.IsStar;
						if (xstar)
							availCellSize.Width = availStarSize.Width * col.Width.Value / weight.Width;
						else
							availCellSize.Width = col.ActualWidth;
						if (view != null && view.Visible && (xstar || ystar))
						{
							var size = view.GetPreferredSize(availCellSize);
							if (xstar && size.Width > col.ActualWidth)
							{
								required.Width += size.Width - col.ActualWidth;
								col.ActualWidth = size.Width;
							}
							if ((xstar || ystar) && size.Height > row.ActualHeight)
							{
								required.Height += size.Height - row.ActualHeight;
								row.ActualHeight = size.Height;
							}
						}
					}
				}
				var total = controlSize - totalPadding;
				if (controlSize.Width < required.Width + totalPadding.Width)
				{
					//total.Width = required.Width;
				}
				if (controlSize.Height < required.Height + totalPadding.Height)
				{
					//total.Height = required.Height;
				}

				for (int y = 0; y < Rows.Count; y++)
				{
					var row = Rows[y];
					if (!row.Height.IsStar)
						total.Height -= row.ActualHeight;
				}
				for (int x = 0; x < Columns.Count; x++)
				{
					var col = Columns[x];
					if (!col.Width.IsStar)
						total.Width -= col.ActualWidth;
				}

				for (int x = 0; x < Columns.Count; x++)
				{
					var col = Columns[x];
					if (col.Width.IsStar)
						col.ActualWidth = total.Width * col.Width.Value / weight.Width;
				}

				float starty = Padding.Top;
				for (int y = 0; y < Rows.Count; y++)
				{
					var row = Rows[y];
					if (row.Height.IsStar)
						row.ActualHeight = total.Height * row.Height.Value / weight.Height;
					float startx = Padding.Left;
					for (int x = 0; x < row.Count; x++)
					{
						var view = row[x];
						var col = Columns[x];
						if (view != null && view.Visible)
						{
							var location = new Rectangle((int)startx, (int)starty, (int)col.ActualWidth, (int)row.ActualHeight);
							Handler.Move(view, location);
						}
						startx += col.ActualWidth + Spacing.Width;
					}
					starty += row.ActualHeight + Spacing.Height;
				}
			}
			finally
			{
				updating = false;
			}
		}

		public override SizeF GetPreferredSize(SizeF availableSize)
		{
			EnsureColumns();
			var totalPadding = new SizeF(
				Padding.Horizontal + Spacing.Width * (Columns.Count - 1),
				Padding.Vertical + Spacing.Height * (Rows.Count - 1)
			);
			var required = SizeF.Empty;
			var availableChildSize = SizeF.Max(SizeF.Empty, availableSize - totalPadding);
			var num = new Size(Math.Max(1, Columns.Count(r => r.Width.IsStar)), Math.Max(1, Rows.Count(r => r.Height.IsStar)));

			var weight = SizeF.Empty;
			for (int x = 0; x < Columns.Count; x++)
			{
				var col = Columns[x];
				col.MeasureWidth = col.Width.IsAbsolute ? col.Width.Value : 0;
				required.Width += col.MeasureWidth;
				if (col.Width.IsStar)
					weight.Width += col.Width.Value;
			}

			for (int y = 0; y < Rows.Count; y++)
			{
				var row = Rows[y];
				row.MeasureHeight = row.Height.IsAbsolute ? row.Height.Value : 0;
				required.Height += row.MeasureHeight;
				if (row.Height.IsStar)
					weight.Height += row.Height.Value;
				for (int x = 0; x < row.Count; x++)
				{	
					var col = Columns[x];
					var view = row[x];
					if (view != null && view.Visible && (!col.Width.IsStar || !row.Height.IsStar))
					{
						var size = view.GetPreferredSize(availableChildSize);
						if (col.Width.IsAuto && size.Width > col.MeasureWidth)
						{
							required.Width += size.Width - col.MeasureWidth;
							col.MeasureWidth = size.Width;
						}
						if (row.Height.IsAuto && size.Height > row.MeasureHeight)
						{
							required.Height += size.Height - row.MeasureHeight;
							row.MeasureHeight = size.Height;
						}
					}
				}
			}
			var availStarSize = SizeF.Max(SizeF.Empty, availableChildSize - required);
			for (int y = 0; y < Rows.Count; y++)
			{
				var row = Rows[y];
				var yscale = row.Height.IsStar;
				var availCellSize = SizeF.Empty;
				if (yscale)
					availCellSize.Height = availStarSize.Height * row.Height.Value / weight.Height;
				else
					availCellSize.Height = row.MeasureHeight;
				for (int x = 0; x < row.Count; x++)
				{	
					var col = Columns[x];
					var xscale = col.Width.IsStar;
					if (xscale)
						availCellSize.Width = availStarSize.Width * col.Width.Value / weight.Width;
					else
						availCellSize.Width = col.MeasureWidth;

					var view = row[x];
					if (view != null && view.Visible && (xscale || yscale))
					{
						var size = view.GetPreferredSize(availCellSize);
						if (xscale && size.Width > col.MeasureWidth)
						{
							required.Width += size.Width - col.MeasureWidth;
							col.MeasureWidth = size.Width;
						}
						if ((xscale || yscale) && size.Height > row.MeasureHeight)
						{
							required.Height += size.Height - row.MeasureHeight;
							row.MeasureHeight = size.Height;
						}
					}
				}
			}
			var maxWidth = Columns.Where(r => r.Width.IsStar).Select(r => r.MeasureWidth * weight.Width / r.Width.Value).DefaultIfEmpty().Max();
			for (int x = 0; x < Columns.Count; x++)
			{
				var col = Columns[x];
				if (!col.Width.IsStar)
					continue;
				var width = maxWidth * col.Width.Value / weight.Width;
				if (width > col.MeasureWidth)
				{
					required.Width += width - col.MeasureWidth;
					col.MeasureWidth = width;
				}
				
			}
			var maxHeight = Rows.Where(r => r.Height.IsStar).Select(r => r.MeasureHeight * weight.Height / r.Height.Value).DefaultIfEmpty().Max();
			for (int y = 0; y < Rows.Count; y++)
			{
				var row = Rows[y];
				if (!row.Height.IsStar)
					continue;
				var height = maxHeight * row.Height.Value / weight.Height;
				if (height > row.MeasureHeight)
				{
					required.Height += height - row.MeasureHeight;
					row.MeasureHeight = height;
				}

			}
			return required + totalPadding;
		}

		public override void Remove(Control child)
		{
			collection.Remove(child);
		}

		void UpdateSize(object sender, EventArgs e)
		{
			UpdateLayout();
		}

		internal void EnsureColumns(int columnCount)
		{
			while (Columns.Count < columnCount)
			{
				Columns.Add(new TableColumn());
			}
		}

		void EnsureRows(int rowCount)
		{
			while (Rows.Count < rowCount)
			{
				Rows.Add(new TableRow2());
			}
		}

		internal void Ensure(int columnCount, int rowCount)
		{
			EnsureColumns(columnCount);
			EnsureRows(rowCount);
		}
	}
}

