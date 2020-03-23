using System;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using swm = System.Windows.Media;
using swi = System.Windows.Input;
using Eto.Forms;
using Eto.CustomControls;
using Eto.Wpf.CustomControls.TreeGridView;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Wpf.Forms.Controls
{
	public class TreeGridViewHandler : GridHandler<TreeGridView, TreeGridView.ICallback>, TreeGridView.IHandler, ITreeHandler
	{
		TreeController controller;
		ITreeGridItem lastSelected;

		protected override object GetItemAtRow(int row)
		{
			return controller[row];
		}

		protected override void Initialize()
		{
			base.Initialize();
			controller = new TreeController { Handler = this };
			Control.Background = sw.SystemColors.WindowBrush;
			Control.PreviewKeyDown += Control_PreviewKeyDown;
		}

		private void Control_PreviewKeyDown(object sender, sw.Input.KeyEventArgs e)
		{
			if (e.Handled || swi.Keyboard.Modifiers != swi.ModifierKeys.None)
				return;

			// handle expanding/collapsing via the keyboard
			if (e.Key == swi.Key.Right)
			{
				var currentCell = Control.CurrentCell;
				if (currentCell.Column != null 
					&& Control.Columns.IndexOf(currentCell.Column) == 0
					&& currentCell.Item is ITreeGridItem item
					&& item.Expandable
					&& !item.Expanded)
				{
					var index = controller.IndexOf(item);
					if (index >= 0)
					{
						controller.ExpandRow(index);

						e.Handled = true;
					}
				}
			}
			else if (e.Key == swi.Key.Left)
			{
				var currentCell = Control.CurrentCell;
				if (currentCell.Column != null
					&& Control.Columns.IndexOf(currentCell.Column) == 0
					&& currentCell.Item is ITreeGridItem item)
				{
					if (!item.Expandable || !item.Expanded)
					{
						// select parent if not the top node
						item = item.Parent;
						if (item != null && !ReferenceEquals(item, DataStore))
						{
							Control.CurrentCell = new swc.DataGridCellInfo(item, currentCell.Column);
							SelectedItem = item;
							e.Handled = true;
						}
					}
					else
					{
						var index = controller.IndexOf(item);
						if (index >= 0)
						{
							controller.CollapseRow(index);

							e.Handled = true;
						}
					}
				}
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TreeGridView.ActivatedEvent:
					Control.PreviewKeyDown += (sender, e) =>
					{
						if (e.Key == sw.Input.Key.Enter)
						{
							if (SelectedItem != null)
							{
								Callback.OnActivated(Widget, new TreeGridViewItemEventArgs(SelectedItem));
								e.Handled = true;
							}
						}
					};
					Control.MouseDoubleClick += (sender, e) =>
					{
						if (SelectedItem != null)
						{
							Callback.OnActivated(Widget, new TreeGridViewItemEventArgs(SelectedItem));
							e.Handled = true;
						}
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
					Control.SelectedCellsChanged += (sender, e) =>
					{
						var item = SelectedItem;
						if (!SkipSelectionChanged && !object.ReferenceEquals(lastSelected, item))
						{
							Callback.OnSelectedItemChanged(Widget, EventArgs.Empty);
							lastSelected = item;
						}
					};
					break;
				case Eto.Forms.Control.DragOverEvent:
					base.AttachEvent(id);
					HandleEvent(Eto.Forms.Control.DragLeaveEvent);
					break;
				case Eto.Forms.Control.DragLeaveEvent:
					base.AttachEvent(id);
					HandleEvent(Eto.Forms.Control.DragOverEvent);
					break;
				case Eto.Forms.Control.DragDropEvent:
					base.AttachEvent(id);
					HandleEvent(Eto.Forms.Control.DragOverEvent);
					HandleEvent(Eto.Forms.Control.DragLeaveEvent);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public ITreeGridStore<ITreeGridItem> DataStore
		{
			get { return controller.Store; }
			set
			{
				controller.InitializeItems(value);
				Control.ItemsSource = controller;
				EnsureSelection();
			}
		}

		public ITreeGridItem SelectedItem
		{
			get { return Control.SelectedItem as ITreeGridItem; }
			set
			{
				if (controller != null && value != null)
				{
					controller.ExpandToItem(value);
					Control.SelectedItem = value;
					Control.ScrollIntoView(value);
				}
				else
					Control.UnselectAll();
			}
		}

		public IEnumerable<object> SelectedItems => Control.SelectedItems.OfType<object>();

		public override sw.FrameworkElement SetupCell(IGridColumnHandler column, sw.FrameworkElement defaultContent)
		{
			if (object.ReferenceEquals(column, Columns.Collection[0].Handler))
				return TreeToggleButton.Create(defaultContent, controller);
			return defaultContent;
		}

		void ITreeHandler.PreResetTree()
		{
			SkipSelectionChanged = true;
			SaveFocus();
		}

		void ITreeHandler.PostResetTree()
		{
			DisableAutoScrollToSelection = true;
			RestoreFocus();
			SkipSelectionChanged = false;
			DisableAutoScrollToSelection = false;
		}

		public void ReloadData()
		{
			controller.ReloadData();
		}

		public void ReloadItem(ITreeGridItem item, bool reloadChildren)
		{
			if (!reloadChildren)
				controller.ReloadItem(item);
			else
				controller.ReloadData();
		}

		public ITreeGridItem GetCellAt(PointF location, out int column)
		{
			var hitTestResult = swm.VisualTreeHelper.HitTest(Control, location.ToWpf())?.VisualHit;
			if (hitTestResult == null)
			{
				column = -1;
				return null;
			}
			var dataGridCell = hitTestResult.GetVisualParent<swc.DataGridCell>();
			column = dataGridCell?.Column != null ? Control.Columns.IndexOf(dataGridCell.Column) : -1;

			var dataGridRow = hitTestResult.GetVisualParent<swc.DataGridRow>();
			if (dataGridRow != null)
			{
				int row = dataGridRow.GetIndex();
				return GetItemAtRow(row) as ITreeGridItem;
			}
			return null;
		}

		public TreeGridViewDragInfo GetDragInfo(DragEventArgs args) => args.ControlObject as TreeGridViewDragInfo;

		protected override DragEventArgs GetDragEventArgs(sw.DragEventArgs data, object controlObject)
		{
			var location = data.GetPosition(Control).ToEto();
			var cell = Widget.GetCellAt(location);
			var item = cell.Item;
			int? childIndex;
			var row = GetDataGridRow(cell.Item);
			GridDragPosition position;
			object parent;
			if (row != null)
			{
				var rowLocation = row.TransformToAncestor(Control).Transform(new sw.Point(0, 0));
				if (location.Y + 4 > rowLocation.Y + row.ActualHeight - row.BorderThickness.Vertical())
				{
					position = GridDragPosition.After;
					var treeGridItem = item as ITreeGridItem;
					if (treeGridItem?.Expanded == true)
					{
						// insert as a child of the parent
						parent = item;
						item = null;
						childIndex = -1;
					}
					else
					{
						parent = treeGridItem?.Parent;
						var node = controller.GetNodeAtRow(row.GetIndex());
						childIndex = node.Index;
					}
				}
				else if (location.Y < rowLocation.Y + 4 + row.BorderThickness.Top)
				{
					position = GridDragPosition.Before;
					parent = (cell.Item as ITreeGridItem)?.Parent;
					var node = controller.GetNodeAtRow(row.GetIndex());
					childIndex = node.Index;
				}
				else
				{
					position = GridDragPosition.Over;
					parent = (cell.Item as ITreeGridItem)?.Parent;
					var node = controller.GetNodeAtRow(row.GetIndex());
					childIndex = node.Index;
				}
			}
			else
			{
				parent = null;
				item = null;
				position = GridDragPosition.Over;
				childIndex = null;
			}

			controlObject = new TreeGridViewDragInfo(Widget, parent, item, childIndex, position);

			return base.GetDragEventArgs(data, controlObject);
		}

		void ResetDrag()
		{
			LastDragInfo = null;
			LastDragRow?.Revert();
			LastDragRow = null;
		}

		protected override void HandleDragEnter(sw.DragEventArgs e, DragEventArgs args)
		{
			ResetDrag();
			base.HandleDragEnter(e, args);
		}

		protected override void HandleDragLeave(sw.DragEventArgs e, DragEventArgs args)
		{
			ResetDrag();
			base.HandleDragLeave(e, args);
		}

		protected override void HandleDrop(sw.DragEventArgs e, DragEventArgs args)
		{
			var dragInfo = LastDragInfo;
			if (dragInfo != null && dragInfo.IsChanged)
			{
				// use modified drag info from DragOver
				var info = GetDragInfo(args);
				info.Item = dragInfo.Item;
				info.Parent = dragInfo.Parent;
				info.Position = dragInfo.Position;
			}
			base.HandleDrop(e, args);
			ResetDrag();
		}

		static readonly object LastDragInfo_Key = new object();
		TreeGridViewDragInfo LastDragInfo
		{
			get { return Widget.Properties.Get<TreeGridViewDragInfo>(LastDragInfo_Key); }
			set { Widget.Properties.Set(LastDragInfo_Key, value); }
		}

		protected override void HandleDragOver(sw.DragEventArgs e, DragEventArgs args)
		{
			var lastRow = LastDragRow;
 			base.HandleDragOver(e, args);
			var info = LastDragInfo = GetDragInfo(args);

			if (args.Effects != DragEffects.None)
			{
				// show drag indicator!
				var row = GetDataGridRow(info.Item ?? info.Parent);
				if (row != null)
				{
					// same position, just return
					if (lastRow != null && lastRow.IsEqual(row, info.InsertIndex))
						return;

					lastRow?.Revert();
					LastDragRow = new GridDragRowState(row, info.InsertIndex);

					if (info.InsertIndex == -1)
					{
						row.Background = sw.SystemColors.HighlightBrush;
						row.Foreground = sw.SystemColors.HighlightTextBrush;
					}
					else
					{
						var node = controller.GetNodeAtRow(row.GetIndex());

						var level = node.Level + 1; // indicator to the right of the expanders to align with text
						var i = info.Parent as ITreeGridItem;
						if (info.Position == GridDragPosition.After && ReferenceEquals(info.Item, null))
							level++;

						level *= 16;
						var d = new swm.GeometryDrawing();
						var gg = new swm.GeometryGroup();
						gg.Children.Add(new swm.EllipseGeometry(new sw.Point(0, 0), 2, 2));
						gg.Children.Add(new swm.LineGeometry(new sw.Point(2, 0), new sw.Point(row.ActualWidth - level - 16, 0)));
						d.Geometry = gg;
						d.Brush = sw.SystemColors.HighlightBrush;
						d.Pen = new swm.Pen(sw.SystemColors.HighlightBrush, 1);
						var b = new swm.DrawingBrush { Drawing = d, TileMode = swm.TileMode.None, Stretch = swm.Stretch.None, AlignmentX = swm.AlignmentX.Left };
						if (info.InsertIndex == node.Index)
						{
							b.AlignmentY = swm.AlignmentY.Top;
							row.BorderThickness = new sw.Thickness(0, 5, 0, 0);
						}
						else
						{
							b.AlignmentY = swm.AlignmentY.Bottom;
							row.BorderThickness = new sw.Thickness(0, 0, 0, 5);
						}

						b.Transform = new swm.TranslateTransform(level, 0);
						row.BorderBrush = b;
					}
					return;
				}
			}

			ResetDrag();
		}

	}
}
