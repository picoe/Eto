using System;
using System.Linq;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.CustomControls;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace Eto.WinForms.Forms.Controls
{
	public class TreeGridViewHandler : GridHandler<TreeGridView, TreeGridView.ICallback>, TreeGridView.IHandler, ITreeHandler
	{
		public static int INDENT_WIDTH = 16;
		ITreeGridItem lastSelected;

		swf.VisualStyles.VisualStyleRenderer openRenderer;
		swf.VisualStyles.VisualStyleRenderer closedRenderer;

		public bool ClassicStyle { get; set; }

		public bool ClassicGridLines { get; set; }

		TreeController controller;

		protected override object GetItemAtRow(int row)
		{
			if (row >= controller.Count)
				return null;
			return controller[row];
		}

		public TreeGridViewHandler()
		{
			controller = new TreeController { Handler = this };
			controller.CollectionChanged += (sender, e) => UpdateCollection();
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			var col = Widget.Columns.Select(r => r.Handler as GridColumnHandler).FirstOrDefault();
			if (col != null && col.AutoSize)
			{
				col.Control.AutoSizeMode = swf.DataGridViewAutoSizeColumnMode.AllCells;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();

			//Control.AutoSizeColumnsMode = swf.DataGridViewAutoSizeColumnsMode.AllCells;
			Control.BackgroundColor = sd.SystemColors.Window;
			Control.SelectionMode = swf.DataGridViewSelectionMode.FullRowSelect;
			Control.CellBorderStyle = swf.DataGridViewCellBorderStyle.None;
		}

		public override void AttachEvent(string handler)
		{
			switch (handler)
			{
				case TreeGridView.ActivatedEvent:
					Control.KeyDown += (sender, e) =>
					{
						if (!e.Handled && SelectedItem != null && e.KeyData == swf.Keys.Enter)
						{
							Callback.OnActivated(Widget, new TreeGridViewItemEventArgs(SelectedItem));
							e.Handled = true;
						}
					};
					Control.CellDoubleClick += (sender, e) =>
					{
						if (SelectedItem != null)
							Callback.OnActivated(Widget, new TreeGridViewItemEventArgs(SelectedItem));
					};
					break;
				case TreeGridView.ExpandingEvent:
					controller.Expanding += (sender, e) => Callback.OnExpanding(Widget, e);
					break;
				case TreeGridView.ExpandedEvent:
					controller.Expanded += (sender, e) => Callback.OnExpanded(Widget, e);
					break;
				case TreeGridView.CollapsingEvent:
					controller.Collapsing += (sender, e) => Callback.OnCollapsing(Widget, e);
					break;
				case TreeGridView.CollapsedEvent:
					controller.Collapsed += (sender, e) => Callback.OnCollapsed(Widget, e);
					break;
				case TreeGridView.SelectedItemChangedEvent:
					Control.SelectionChanged += (sender, e) =>
					{
						var item = this.SelectedItem;
						if (!object.ReferenceEquals(lastSelected, item))
						{
							Callback.OnSelectedItemChanged(Widget, EventArgs.Empty);
							lastSelected = item;
						}
					};
					break;
				default:
					base.AttachEvent(handler);
					break;
			}
		}

		public ITreeGridStore<ITreeGridItem> DataStore
		{
			get { return controller.Store; }
			set
			{
				controller.InitializeItems(value);
			}
		}

		public override void Invalidate(bool invalidateChildren)
		{
			base.Invalidate(invalidateChildren);
			if (this.Widget.Loaded)
			{
				Control.Refresh();
				AutoSizeColumns();
			}
		}

		void UpdateCollection()
		{
			Control.RowCount = controller.Count;
			if (Widget.Loaded)
			{
				Control.Refresh();
				AutoSizeColumns();
			}
		}

		public ITreeGridItem SelectedItem
		{
			get
			{
				if (Control.SelectedRows.Count == 0)
					return null;
				var index = Control.SelectedRows.OfType<swf.DataGridViewRow>().Select(r => r.Index).FirstOrDefault();
				return controller[index];
			}
			set
			{
				if (controller != null && value != null)
				{
					controller.ExpandToItem(value);
					var index = controller.IndexOf(value);
					if (index >= 0)
						Control.Rows[index].Selected = true;
				}
				else
					Control.ClearSelection();
			}
		}

		public IEnumerable<object> SelectedItems
		{
			get
			{
				foreach (swf.DataGridViewRow row in Control.SelectedRows)
				{
					yield return GetItemAtRow(row.Index);
				}
			}
		}

		void EnsureGlyphRenderers()
		{
			if (openRenderer == null || closedRenderer == null)
			{
				if (ClassicStyle)
				{
					openRenderer = new swf.VisualStyles.VisualStyleRenderer(swf.VisualStyles.VisualStyleElement.TreeView.Glyph.Opened);
					closedRenderer = new swf.VisualStyles.VisualStyleRenderer(swf.VisualStyles.VisualStyleElement.TreeView.Glyph.Closed);
				}
				else
				{
					openRenderer = new swf.VisualStyles.VisualStyleRenderer("Explorer::TreeView", 2, 2);
					closedRenderer = new swf.VisualStyles.VisualStyleRenderer("Explorer::TreeView", 2, 1);
				}
			}
		}

		public override void Paint(GridColumnHandler column, sd.Graphics graphics, sd.Rectangle clipBounds, sd.Rectangle cellBounds, int rowIndex, swf.DataGridViewElementStates cellState, object value, object formattedValue, string errorText, swf.DataGridViewCellStyle cellStyle, swf.DataGridViewAdvancedBorderStyle advancedBorderStyle, ref swf.DataGridViewPaintParts paintParts)
		{
			if (object.ReferenceEquals(column.Widget, this.Widget.Columns[0]))
			{
				// paint the background
				if (paintParts.HasFlag(swf.DataGridViewPaintParts.Background))
				{
					sd.Brush brush;
					if (cellState.HasFlag(swf.DataGridViewElementStates.Selected))
						brush = new sd.SolidBrush(cellStyle.SelectionBackColor);
					else
						brush = new sd.SolidBrush(cellStyle.BackColor);
					graphics.FillRectangle(brush, cellBounds);
					paintParts &= ~swf.DataGridViewPaintParts.Background;
				}

				var node = controller.GetNodeAtRow(rowIndex);
				var treeRect = cellBounds;
				treeRect.X += node.Level * INDENT_WIDTH;
				treeRect.Width = 16;

				if (ClassicStyle && ClassicGridLines)
				{
					// Draw grid lines - for classic style
					using (var linePen = new sd.Pen(sd.SystemBrushes.ControlDark, 1.0f))
					{
						var lineRect = treeRect;
						lineRect.X += 7;
						lineRect.Width = 10;
						linePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
						var isFirstSibling = node.IsFirstNode;
						var isLastSibling = node.IsLastNode;
						if (node.Level == 0)
						{
							// the Root nodes display their lines differently
							if (isFirstSibling && isLastSibling)
							{
								// only node, both first and last. Just draw horizontal line
								graphics.DrawLine(linePen, lineRect.X, cellBounds.Top + cellBounds.Height / 2, lineRect.Right, cellBounds.Top + cellBounds.Height / 2);
							}
							else if (isLastSibling)
							{
								// last sibling doesn't draw the line extended below. Paint horizontal then vertical
								graphics.DrawLine(linePen, lineRect.X, cellBounds.Top + cellBounds.Height / 2, lineRect.Right, cellBounds.Top + cellBounds.Height / 2);
								graphics.DrawLine(linePen, lineRect.X, cellBounds.Top, lineRect.X, cellBounds.Top + cellBounds.Height / 2);
							}
							else if (isFirstSibling)
							{
								// first sibling doesn't draw the line extended above. Paint horizontal then vertical
								graphics.DrawLine(linePen, lineRect.X, cellBounds.Top + cellBounds.Height / 2, lineRect.Right, cellBounds.Top + cellBounds.Height / 2);
								graphics.DrawLine(linePen, lineRect.X, cellBounds.Top + cellBounds.Height / 2, lineRect.X, cellBounds.Bottom);
							}
							else
							{
								// normal drawing draws extended from top to bottom. Paint horizontal then vertical
								graphics.DrawLine(linePen, lineRect.X, cellBounds.Top + cellBounds.Height / 2, lineRect.Right, cellBounds.Top + cellBounds.Height / 2);
								graphics.DrawLine(linePen, lineRect.X, cellBounds.Top, lineRect.X, cellBounds.Bottom);
							}
						}
						else
						{
							if (isLastSibling)
							{
								// last sibling doesn't draw the line extended below. Paint horizontal then vertical
								graphics.DrawLine(linePen, lineRect.X, cellBounds.Top + cellBounds.Height / 2, lineRect.Right, cellBounds.Top + cellBounds.Height / 2);
								graphics.DrawLine(linePen, lineRect.X, cellBounds.Top, lineRect.X, cellBounds.Top + cellBounds.Height / 2);
							}
							else
							{
								// normal drawing draws extended from top to bottom. Paint horizontal then vertical
								graphics.DrawLine(linePen, lineRect.X, cellBounds.Top + cellBounds.Height / 2, lineRect.Right, cellBounds.Top + cellBounds.Height / 2);
								graphics.DrawLine(linePen, lineRect.X, cellBounds.Top, lineRect.X, cellBounds.Bottom);
							}

							// paint lines of previous levels to the root
							int horizontalStop = lineRect.X - INDENT_WIDTH;
							var previousNode = node.Parent;

							while (previousNode != null)
							{
								if (!previousNode.IsLastNode)
								{
									// paint vertical line
									graphics.DrawLine(linePen, horizontalStop, lineRect.Top, horizontalStop, lineRect.Bottom);
								}
								previousNode = previousNode.Parent;
								horizontalStop = horizontalStop - INDENT_WIDTH;
							}
						}

					}
				}

				if (node.Item.Expandable)
				{
					// draw open/close glyphs
					if (swf.Application.RenderWithVisualStyles)
					{
						EnsureGlyphRenderers();
						if (controller.IsExpanded(rowIndex))
							openRenderer.DrawBackground(graphics, new sd.Rectangle(treeRect.X, treeRect.Y + (treeRect.Height / 2) - 8, 16, 16));
						else
						{
							closedRenderer.DrawBackground(graphics, new sd.Rectangle(treeRect.X, treeRect.Y + (treeRect.Height / 2) - 8, 16, 16));
						}
					}
					else
					{
						// todo: draw +/- manually
						var glyphRect = treeRect;
						glyphRect.Width = glyphRect.Height = 8;
						glyphRect.X += 3;
						glyphRect.Y += (treeRect.Height - glyphRect.Height) / 2;
						graphics.FillRectangle(sd.SystemBrushes.Window, glyphRect);
						graphics.DrawRectangle(sd.SystemPens.ControlDark, glyphRect);
						glyphRect.Inflate(-2, -2);
						if (!controller.IsExpanded(rowIndex))
						{
							var midx = glyphRect.X + glyphRect.Width / 2;
							graphics.DrawLine(sd.SystemPens.ControlDarkDark, midx, glyphRect.Top, midx, glyphRect.Bottom);
						}

						var midy = glyphRect.Y + glyphRect.Height / 2;
						graphics.DrawLine(sd.SystemPens.ControlDarkDark, glyphRect.Left, midy, glyphRect.Right, midy);
					}
				}
			}
		}

		public override int GetRowOffset(GridColumnHandler column, int rowIndex)
		{
			if (object.ReferenceEquals(column.Widget, this.Widget.Columns[0]))
				return INDENT_WIDTH + controller.LevelAtRow(rowIndex) * INDENT_WIDTH;
			else
				return 0;
		}

		public override bool CellMouseClick(GridColumnHandler column, swf.MouseEventArgs e, int rowIndex)
		{
			if (rowIndex >= 0 && object.ReferenceEquals(column.Widget, this.Widget.Columns[0]))
			{
				var offset = INDENT_WIDTH + controller.LevelAtRow(rowIndex) * INDENT_WIDTH;
				if (e.X < offset && e.X >= offset - INDENT_WIDTH)
				{
					if (controller.IsExpanded(rowIndex))
						controller.CollapseRow(rowIndex);
					else
						controller.ExpandRow(rowIndex);

					return true;
				}
			}
			return false;
		}

		void AutoSizeColumns()
		{
			foreach (var colHandler in Widget.Columns.Where (r => r.AutoSize).Select (r => r.Handler).OfType<GridColumnHandler> ())
			{
				if (colHandler.AutoSize)
				{
					// expand this column to fit content width
					var width = colHandler.Control.GetPreferredWidth(swf.DataGridViewAutoSizeColumnMode.AllCells, false);
					if (width > colHandler.Control.Width)
						colHandler.Control.Width = width;
				}
			}
		}

		void ITreeHandler.PreResetTree()
		{
		}

		void ITreeHandler.PostResetTree()
		{
		}

		public void ReloadData()
		{
			var selectedItems = SelectedItems.OfType<ITreeGridItem>().ToList();
			SupressSelectionChanged++;
			controller.ReloadData();
			Control.ClearSelection();
			bool selectionChanged = false;
			foreach (var selectedItem in selectedItems)
			{
				var row = controller.IndexOf(selectedItem);
				if (row >= 0)
					Control.Rows[row].Selected = true;
				else
					selectionChanged = true;
			}
			if (selectionChanged)
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
			SupressSelectionChanged--;
		}

		public void ReloadItem(ITreeGridItem item)
		{
			ReloadData();
		}

		public ITreeGridItem GetCellAt(PointF location, out int column)
		{
			var result = Control.HitTest((int)location.X, (int)location.Y);
			column = result.ColumnIndex;
			if (result.RowIndex == -1)
				return null;
			return GetItemAtRow(result.RowIndex) as ITreeGridItem;
		}

		public TreeGridViewDragInfo GetDragInfo(DragEventArgs args) => args.ControlObject as TreeGridViewDragInfo;
	}
}

