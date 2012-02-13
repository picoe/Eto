using System;
using Eto.Collections;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	public interface IGridStore : IDataStore
	{
		int Count { get; }

		IGridItem GetItem (int index);
	}
	
	public class GridItemCollection : Collection<IGridItem>, IGridStore
	{
		IGridItem IGridStore.GetItem (int index)
		{
			return this [index];
		}
	}
	
	public interface IGridView : IControl
	{
		bool ShowHeader { get; set; }
		
		bool AllowColumnReordering { get; set; }

		void InsertColumn (int index, GridColumn column);
		
		void RemoveColumn (int index, GridColumn column);
		
		void ClearColumns ();
		
		IGridStore DataStore { get; set; }
	}
	
	public class ColumnCollection : Collection<GridColumn>
	{
		internal IGridView Handler { get; set; }
		
		protected override void InsertItem (int index, GridColumn item)
		{
			base.InsertItem (index, item);
			Handler.InsertColumn (index, item);
		}
		
		protected override void RemoveItem (int index)
		{
			var item = this [index];
			base.RemoveItem (index);
			Handler.RemoveColumn (index, item);
		}
		
		protected override void SetItem (int index, GridColumn item)
		{
			var oldItem = this [index];
			base.SetItem (index, item);
			Handler.RemoveColumn (index, oldItem);
			Handler.InsertColumn (index, item);
		}
		
		protected override void ClearItems ()
		{
			base.ClearItems ();
			Handler.ClearColumns ();
		}
	}
	
	public class GridView : Control
	{
		IGridView handler;
		
		public ColumnCollection Columns { get; private set; }
		
		public GridView ()
			: this(Generator.Current)
		{
		}
		
		public GridView (Generator g)
			: base(g, typeof(IGridView), true)
		{
			handler = (IGridView)Handler;
			Columns = new ColumnCollection{ Handler = handler };
		}
		
		public bool ShowHeader {
			get { return handler.ShowHeader; }
			set { handler.ShowHeader = value; }
		}
		
		public bool AllowColumnReordering {
			get { return handler.AllowColumnReordering; }
			set { handler.AllowColumnReordering = value; }
		}
		
		public IGridStore DataStore {
			get { return handler.DataStore; }
			set { handler.DataStore = value; }
		}
	}
}

