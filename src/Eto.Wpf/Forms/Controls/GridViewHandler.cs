using swc = System.Windows.Controls;
using Eto.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Eto.Drawing;
using System;
using swm = System.Windows.Media;
using System.Collections;
using sw = System.Windows;

namespace Eto.Wpf.Forms.Controls
{
	public class GridViewHandler : GridHandler<GridView, GridView.ICallback>, GridView.IHandler
	{
		IEnumerable<object> store;

		protected override object GetItemAtRow (int row)
		{
			if (row < 0)
				return null;
			var list = store as IList;
			if (list != null)
				return list[row];
			return store?.ElementAt(row);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public object GetCellAt(PointF location, out int column, out int row)
		{
			var hitTestResult = swm.VisualTreeHelper.HitTest(Control, location.ToWpf())?.VisualHit;
			if (hitTestResult == null)
			{
				column = -1;
				row = -1;
				return null;
			}
			var dataGridCell = hitTestResult.GetVisualParent<swc.DataGridCell>();
			column = dataGridCell?.Column != null ? Control.Columns.IndexOf(dataGridCell.Column) : -1;

			var dataGridRow = hitTestResult.GetVisualParent<swc.DataGridRow>();
			if (dataGridRow != null)
			{
				row = dataGridRow.GetIndex();
				return GetItemAtRow(row);
			}
			row = -1;
			return null;
		}


		protected override Eto.Forms.DragEventArgs GetDragEventArgs(System.Windows.DragEventArgs data, object controlObject)
		{
			var location = data.GetPosition(Control).ToEto();
			var cell = Widget.GetCellAt(location);
			var item = cell.Item;
			int index;
			var row = GetDataGridRow(cell.Item);
			GridDragPosition position;
			if (row != null)
			{
				index = row.GetIndex();

				var rowLocation = row.TransformToAncestor(Control).Transform(new sw.Point(0, 0));
				if (location.Y + 4 > rowLocation.Y + row.ActualHeight - row.BorderThickness.Vertical())
				{
					position = GridDragPosition.After;
				}
				else if (location.Y < rowLocation.Y + 4 + row.BorderThickness.Top)
				{
					position = GridDragPosition.Before;
				}
				else
				{
					position = GridDragPosition.Over;
				}
			}
			else
			{
				item = null;
				position = GridDragPosition.Over;
				index = -1;
			}

			controlObject = new GridViewDragInfo(Widget, item, index, position);

			return base.GetDragEventArgs(data, controlObject);
		}

		public GridViewDragInfo GetDragInfo(DragEventArgs args) => args.ControlObject as GridViewDragInfo;

		static readonly object LastDragInfo_Key = new object();
		GridViewDragInfo LastDragInfo
		{
			get { return Widget.Properties.Get<GridViewDragInfo>(LastDragInfo_Key); }
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
				var row = GetDataGridRow(GetItemAtRow(info.Index));
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
						var d = new swm.GeometryDrawing();
						var gg = new swm.GeometryGroup();
						gg.Children.Add(new swm.LineGeometry(new sw.Point(0, 0), new sw.Point(row.ActualWidth, 0)));
						d.Geometry = gg;
						d.Brush = sw.SystemColors.HighlightBrush;
						d.Pen = new swm.Pen(sw.SystemColors.HighlightBrush, 1);
						var b = new swm.DrawingBrush { Drawing = d, TileMode = swm.TileMode.None, Stretch = swm.Stretch.None, AlignmentX = swm.AlignmentX.Left };
						if (info.InsertIndex == row.GetIndex())
						{
							b.AlignmentY = swm.AlignmentY.Top;
							row.BorderThickness = new sw.Thickness(0, 1, 0, 0);
						}
						else
						{
							b.AlignmentY = swm.AlignmentY.Bottom;
							row.BorderThickness = new sw.Thickness(0, 0, 0, 1);
						}

						row.BorderBrush = b;
					}
					return;
				}
			}

			ResetDrag();
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
				info.Index = dragInfo.Index;
				info.Position = dragInfo.Position;
			}
			base.HandleDrop(e, args);
			ResetDrag();
		}

		public IEnumerable<object> DataStore
		{
			get { return store; }
			set
			{
				store = value;
				// must use observable collection for editing and collection update notifications
				if (store is INotifyCollectionChanged)
					Control.ItemsSource = store;
				else
					Control.ItemsSource = store != null ? new ObservableCollection<object>(store) : null;
				EnsureSelection();
			}
		}

		public IEnumerable<object> SelectedItems
		{
			get
			{
				return Control.SelectedItems.OfType<object>();
			}
		}
	}
}
