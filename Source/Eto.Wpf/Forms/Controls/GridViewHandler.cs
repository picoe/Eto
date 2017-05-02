using swc = System.Windows.Controls;
using Eto.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Eto.Drawing;
using System;
using swm = System.Windows.Media;

namespace Eto.Wpf.Forms.Controls
{
	public class GridViewHandler : GridHandler<GridView, GridView.ICallback>, GridView.IHandler
	{
		IEnumerable<object> store;

		protected override object GetItemAtRow (int row)
		{
			return store != null ? store.ElementAt(row) : null;
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
