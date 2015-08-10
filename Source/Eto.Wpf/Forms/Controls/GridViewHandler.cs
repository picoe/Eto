using swc = System.Windows.Controls;
using Eto.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Eto.Wpf.Forms.Controls
{
	public class GridViewHandler : GridHandler<EtoDataGrid, GridView, GridView.ICallback>, GridView.IHandler
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
