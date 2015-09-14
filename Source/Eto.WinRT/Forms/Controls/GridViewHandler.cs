#if TODO_XAML
using swc = Windows.UI.Xaml.Controls;
using Eto.Forms;
using System.Collections.ObjectModel;

namespace Eto.WinRT.Forms.Controls
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
				// must use observable collection for editing and collection update notifications
				var source = store as ObservableCollection<object>;
				if (source != null)
					Control.ItemsSource = source;
				else
					Control.ItemsSource = new ObservableCollection<object>(store.AsEnumerable());
			}
		}
	}
}
#endif