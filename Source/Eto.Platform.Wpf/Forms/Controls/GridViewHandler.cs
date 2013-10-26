using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using Eto.Forms;
using System.Collections;
using System.Collections.ObjectModel;
using Eto.Platform.Wpf.Forms.Menu;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class GridViewHandler : GridHandler<swc.DataGrid, GridView>, IGridView
	{
		IDataStore store;

		public GridViewHandler ()
		{
		}

		protected override void Initialize ()
		{
			base.Initialize ();
			Control.GridLinesVisibility = swc.DataGridGridLinesVisibility.None;
		}

		protected override object GetItemAtRow (int row)
		{
			if (store == null) return null;
			return store[row];
		}

		public IDataStore DataStore
		{
			get { return store; }
			set
			{
				store = value;
				// must use observable collection for editing
				var source = store as ObservableCollection<IListItem>; 
				if (source != null)
					Control.ItemsSource = source;
				else
					Control.ItemsSource = new ObservableCollection<object> (store.AsEnumerable());
			}
		}

		public bool ShowCellBorders
		{
			set { } // TODO
		}
	}
}
