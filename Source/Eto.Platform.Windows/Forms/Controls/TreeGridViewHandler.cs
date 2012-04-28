using System;
using System.Linq;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Platform.Windows.CustomControls;
using System.Collections.Specialized;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class TreeGridViewHandler : GridHandler<TreeGridView>, ITreeGridView, ITreeHandler
	{
		public static int INDENT_WIDTH = 16;

		TreeController controller;

		protected override IGridItem GetItemAtRow (int row)
		{
			if (controller == null) return null;
			return controller[row];
		}
		
		public TreeGridViewHandler ()
		{

		}

		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			var col = Widget.Columns.Select(r => r.Handler as GridColumnHandler).FirstOrDefault ();
			if (col != null && col.AutoSize) {
				col.Control.AutoSizeMode = swf.DataGridViewAutoSizeColumnMode.AllCells;
			}
		}

		public override void Initialize ()
		{
			base.Initialize ();

			//Control.AutoSizeColumnsMode = swf.DataGridViewAutoSizeColumnsMode.AllCells;
			Control.BackgroundColor = sd.SystemColors.Window;
			Control.SelectionMode = swf.DataGridViewSelectionMode.FullRowSelect;
			Control.CellBorderStyle = swf.DataGridViewCellBorderStyle.None;
		}

		public ITreeGridStore<ITreeGridItem> DataStore {
			get { return controller != null ? controller.Store : null; }
			set {
				if (controller != null)
					controller.CollectionChanged -= HandleCollectionChanged;

				controller = new TreeController { Handler = this, Store = value };
				controller.InitializeItems ();
				controller.CollectionChanged += HandleCollectionChanged;
				Control.RowCount = controller.Count;
				Control.Refresh ();
			}
		}

		void HandleCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			Control.RowCount = controller.Count;
			if (this.Widget.Loaded)
				Control.Refresh ();
		}

		public ITreeGridItem SelectedItem
		{
			get {
				if (controller == null)
					return null;
				var index = Control.SelectedRows.OfType<swf.DataGridViewRow> ().Select (r => r.Index).FirstOrDefault();
				return controller[index];
			}
			set
			{
			}
		}

		static swf.VisualStyles.VisualStyleRenderer rOpen = new swf.VisualStyles.VisualStyleRenderer (swf.VisualStyles.VisualStyleElement.TreeView.Glyph.Opened);
		static swf.VisualStyles.VisualStyleRenderer rClosed = new swf.VisualStyles.VisualStyleRenderer (swf.VisualStyles.VisualStyleElement.TreeView.Glyph.Closed);


		public override void Paint (GridColumnHandler column, sd.Graphics graphics, sd.Rectangle clipBounds, sd.Rectangle cellBounds, int rowIndex, swf.DataGridViewElementStates cellState, object value, object formattedValue, string errorText, swf.DataGridViewCellStyle cellStyle, swf.DataGridViewAdvancedBorderStyle advancedBorderStyle, ref swf.DataGridViewPaintParts paintParts)
		{
			if (object.ReferenceEquals (column.Widget, this.Widget.Columns[0])) {
				// paint the background
				if (paintParts.HasFlag (swf.DataGridViewPaintParts.Background)) {
					sd.Brush brush;
					if (cellState.HasFlag (swf.DataGridViewElementStates.Selected))
						brush = new sd.SolidBrush (cellStyle.SelectionBackColor);
					else
						brush = new sd.SolidBrush (cellStyle.BackColor);
					graphics.FillRectangle (brush, cellBounds);
					paintParts &= ~swf.DataGridViewPaintParts.Background;
				}

				var node = controller.GetNodeAtRow (rowIndex);
				var glyphRect = cellBounds;
				glyphRect.X += 3 + node.Level * INDENT_WIDTH;
				glyphRect.Width = 14;
				if (true) {
					using (var linePen = new sd.Pen (sd.SystemBrushes.ControlDark, 1.0f)) {
						linePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
						var isFirstSibling = node.IsFirstNode;
						var isLastSibling = node.IsLastNode;
						if (node.Level == 0) {
							// the Root nodes display their lines differently
							if (isFirstSibling && isLastSibling) {
								// only node, both first and last. Just draw horizontal line
								graphics.DrawLine (linePen, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
							}
							else if (isLastSibling) {
								// last sibling doesn't draw the line extended below. Paint horizontal then vertical
								graphics.DrawLine (linePen, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
								graphics.DrawLine (linePen, glyphRect.X + 4, cellBounds.Top, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2);
							}
							else if (isFirstSibling) {
								// first sibling doesn't draw the line extended above. Paint horizontal then vertical
								graphics.DrawLine (linePen, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
								graphics.DrawLine (linePen, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2, glyphRect.X + 4, cellBounds.Bottom);
							}
							else {
								// normal drawing draws extended from top to bottom. Paint horizontal then vertical
								graphics.DrawLine (linePen, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
								graphics.DrawLine (linePen, glyphRect.X + 4, cellBounds.Top, glyphRect.X + 4, cellBounds.Bottom);
							}
						}
						else {
							if (isLastSibling) {
								// last sibling doesn't draw the line extended below. Paint horizontal then vertical
								graphics.DrawLine (linePen, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
								graphics.DrawLine (linePen, glyphRect.X + 4, cellBounds.Top, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2);
							}
							else {
								// normal drawing draws extended from top to bottom. Paint horizontal then vertical
								graphics.DrawLine (linePen, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
								graphics.DrawLine (linePen, glyphRect.X + 4, cellBounds.Top, glyphRect.X + 4, cellBounds.Bottom);
							}

							// paint lines of previous levels to the root
							int horizontalStop = (glyphRect.X + 4) - INDENT_WIDTH;
							var previousNode = node.Parent;

							while (previousNode != null && !previousNode.IsRoot) {
								if (!previousNode.IsLastNode) {
									// paint vertical line
									graphics.DrawLine (linePen, horizontalStop, glyphRect.Top, horizontalStop, glyphRect.Bottom);
								}
								previousNode = previousNode.Parent;
								horizontalStop = horizontalStop - INDENT_WIDTH;
							}
						}

					}
				}

				if (node.Item.Expandable) {
					if (controller.IsExpanded (rowIndex))
						rOpen.DrawBackground (graphics, new sd.Rectangle (glyphRect.X, glyphRect.Y + (glyphRect.Height / 2) - 4, 10, 10));
					else {
						rClosed.DrawBackground (graphics, new sd.Rectangle (glyphRect.X, glyphRect.Y + (glyphRect.Height / 2) - 4, 10, 10));
					}
				}
			}
		}

		public override int GetRowOffset (GridColumnHandler column, int rowIndex)
		{
			if (object.ReferenceEquals(column.Widget, this.Widget.Columns[0]))
				return INDENT_WIDTH + controller.LevelAtRow (rowIndex) * INDENT_WIDTH;
			else
				return 0;
		}

		public override bool CellMouseClick (GridColumnHandler column, swf.MouseEventArgs e, int rowIndex)
		{
			if (rowIndex >= 0 && object.ReferenceEquals (column.Widget, this.Widget.Columns[0])) {
				var offset = INDENT_WIDTH + controller.LevelAtRow (rowIndex) * INDENT_WIDTH;
				if (e.X < offset && e.X >= offset - INDENT_WIDTH) {
					if (controller.IsExpanded (rowIndex))
						controller.CollapseRow (rowIndex);
					else
						controller.ExpandRow (rowIndex);
					return true;
				}
			}
			return false;
		}
	}
}

