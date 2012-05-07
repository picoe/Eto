﻿using System;
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
	public class GridHandler : GridHandler<swc.DataGrid, GridView>, IGridView
	{
		IGridStore store;

		public GridHandler ()
		{
		}

		protected override IGridItem GetItemAtRow (int row)
		{
			if (store == null) return null;
			return store[row];
		}

		public IGridStore DataStore
		{
			get { return store; }
			set
			{
				store = value;
				// must use observable collection for editing
				if (store is ObservableCollection<IGridItem>)
					Control.ItemsSource = store as ObservableCollection<IGridItem>;
				else
					Control.ItemsSource = new ObservableCollection<IGridItem> (store.AsEnumerable());
			}
		}
	}
}
