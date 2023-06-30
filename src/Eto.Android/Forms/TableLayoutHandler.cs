using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android.Forms
{
	/// <summary>
	/// Handler for <see cref="TableLayout"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class TableLayoutHandler : AndroidContainer<aw.TableLayout, TableLayout, TableLayout.ICallback>, TableLayout.IHandler
	{
		private const Boolean DebugCellBoundaries = false;

		bool[] columnScale;
		bool[] rowScale;
		private bool isCreated;
		private Size spacing;

		public override av.View ContainerControl { get { return Control; } }

		public TableLayoutHandler()
		{
			Control = new aw.TableLayout(Platform.AppContextThemed);
		}

		public void CreateControl(int cols, int rows)
		{
			columnScale = new bool[cols];
			rowScale = new bool[rows];
			
			
			for (int y = 0; y < rows; y++)
			{
				var row = new aw.TableRow(Platform.AppContextThemed);

				for (int x = 0; x < cols; x++)
					{
					var cell = new aw.FrameLayout(Platform.AppContextThemed);
#if DEBUG
					if(DebugCellBoundaries)
						cell.SetBackgroundColor( (((y % 2) + (x % 2)) %2 == 0) ? ag.Color.DarkTurquoise : ag.Color.LightBlue);
#endif
					row.AddView(cell, new aw.TableRow.LayoutParams(0,0) { Column = x });
					}
				
				Control.AddView(row, new aw.TableLayout.LayoutParams());
			}

			isCreated = true;
		}

		public bool GetColumnScale(int column)
		{
			return columnScale[column];
		}

		public void SetColumnScale(int column, bool scale)
		{			
			columnScale[column] = scale;
			ApplyScaling();
		}

		private void ApplyScaling()
		{
			var HaveScaledColumn2 = false;

			for (var x = 0; x < columnScale.Length; x++)
			{
				var ShouldScaleColumn2 = columnScale[x];
				if (ShouldScaleColumn2) HaveScaledColumn2 = true;

				Control.SetColumnShrinkable(x, true);
				Control.SetColumnStretchable(x, ShouldScaleColumn2);
			}

			if (!HaveScaledColumn2)
				Control.SetColumnStretchable(columnScale.Length - 1, true);

			var HaveScaledRow = false;

			for (var y = 0; y < rowScale.Length; y++)
			{
				var ShouldScaleRow = (!HaveScaledRow && y == rowScale.Length - 1) || rowScale[y];
				if (ShouldScaleRow) HaveScaledRow = true;

				var row = (aw.TableRow)Control.GetChildAt(y);
				row.LayoutParameters = new aw.TableLayout.LayoutParams(av.ViewGroup.LayoutParams.MatchParent, av.ViewGroup.LayoutParams.WrapContent, ShouldScaleRow ? 1 : 0);

				var HaveScaledColumn = false;

				var InSpanFor = 1;
				var ThisSpan = 1;

				for (var x = 0; x < columnScale.Length; x++)
				{
					InSpanFor--;

					var ShouldScaleColumn = (!HaveScaledColumn && x == columnScale.Length - 1) || columnScale[x];
					if (ShouldScaleColumn) HaveScaledColumn = true;

					if (InSpanFor > 0)
						ThisSpan = 0;

					else
					{
						var WidgetRow = Widget.Rows[y];
						InSpanFor = ThisSpan = (WidgetRow.Cells.Any() ? /*WidgetRow.Cells[x].ColumnSpan*/ 1 : 1);
					}

					var cell = row.GetChildAt(x);
					var child = GetChildControl(x, y);

					var hasWidth = child != null && child.Visible && child.Width > 0;
					var glw = (ShouldScaleColumn || ThisSpan > 1) ? av.ViewGroup.LayoutParams.MatchParent : av.ViewGroup.LayoutParams.WrapContent;
					var lw = hasWidth ? Platform.DpToPx(child.Width) : glw;
					var lh = child != null && child.Visible && child.Height > 0 ? Platform.DpToPx(child.Height) : av.ViewGroup.LayoutParams.WrapContent;

					cell.LayoutParameters = new aw.TableRow.LayoutParams(lw, lh, hasWidth ? 0 : (ShouldScaleColumn ? 1 : 0)) { Column = x, Span = ThisSpan };

					var childControl = child?.GetContainerView();
					
					if (childControl != null)
					{
						lw = child.Visible && child.Width > 0 ? av.ViewGroup.LayoutParams.MatchParent : glw;
						lh = av.ViewGroup.LayoutParams.MatchParent;

						childControl.LayoutParameters = new aw.FrameLayout.LayoutParams(lw, lh)
						{
							LeftMargin = spacing.Width,
							RightMargin = spacing.Width,
							TopMargin = spacing.Height,
							BottomMargin = spacing.Height
						};
					}
				}
			}
		}

		private Control GetChildControl(Int32 x, Int32 y)
		{
			if (Widget == null)
				return null;

			if (Widget.Rows.Count <= y)
				return null;

			var Row = Widget.Rows[y];

			if (Row.Cells.Count <= x)
				return null;

			return Row.Cells[x].Control;
		}

		public bool GetRowScale(int row)
		{
			return rowScale[row];
		}

		public void SetRowScale(int row, bool scale)
		{
			rowScale[row] = scale;
			ApplyScaling();
		}

		public Size Spacing
		{
			get
			{
				return spacing;
			}
			set
			{
				spacing = value;
				
				if(isCreated)
					ApplyScaling();
			}
		}

		public Padding Padding
		{
			get { return Control.GetPadding(); }
			set { Control.SetPadding(value); }
		}

		public void Add(Control child, int x, int y)
		{
			var row = (aw.TableRow)Control.GetChildAt(y);
			var cell = (aw.FrameLayout)row.GetChildAt(x);

			cell.RemoveAllViews();
			
			var control = child.GetContainerView();
			if (control != null)
			{
				cell.AddView(control);
				ApplyScaling();
			}
		}

		public void Move(Control child, int x, int y)
		{
			Remove(child);
			Add(child, x, y);
		}

		public void Remove(Control child)
		{
			var control = child.GetContainerView();
			var cell = control.Parent as aw.FrameLayout;
			cell?.RemoveAllViews();
			ApplyScaling();
		}

		public void Update()
		{
			ApplyScaling();
			Control.ForceLayout();
		}
	}
}
