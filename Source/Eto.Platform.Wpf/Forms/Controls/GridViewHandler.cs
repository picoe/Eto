using swc = System.Windows.Controls;
using Eto.Forms;
using System.Collections.ObjectModel;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class GridViewHandler : GridHandler<swc.DataGrid, GridView>, IGridView
	{
		IDataStore store;

		protected override void Initialize ()
		{
			base.Initialize ();
			Control.GridLinesVisibility = swc.DataGridGridLinesVisibility.None;
		}

		protected override object GetItemAtRow (int row)
		{
			return store == null ? null : store[row];
		}

		public IDataStore DataStore
		{
			get { return store; }
			set
			{
				store = value;
				// must use observable collection for editing
				var source = store as ObservableCollection<IListItem>; 
 				Control.ItemsSource = source ?? new ObservableCollection<object>(store.AsEnumerable());
			}
		}

		public bool ShowCellBorders
		{
			set { } // TODO
		}
	}
}
