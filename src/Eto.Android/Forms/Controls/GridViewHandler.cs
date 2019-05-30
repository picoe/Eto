using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;
using Eto.Android.Forms.Cells;

namespace Eto.Android.Forms.Controls
{
	public class GridViewHandler : AndroidControl<aw.ListView, GridView, GridView.ICallback>, GridView.IHandler
	{
		ColumnsChangedHandler columns;
		DataStoreHandler collection;
		Adapter adapter;

		class ColumnsChangedHandler : EnumerableChangedHandler<GridColumn>
		{
			public GridViewHandler Handler { get; set; }

			public override void AddRange(IEnumerable<GridColumn> items)
			{
				Handler.adapter.NotifyDataSetChanged();
			}

			public override void RemoveRange(int index, int count)
			{
				Handler.adapter.NotifyDataSetChanged();
			}

			public override void AddItem(GridColumn item)
			{
				Handler.adapter.NotifyDataSetChanged();
			}

			public override void InsertItem(int index, GridColumn item)
			{
				Handler.adapter.NotifyDataSetChanged();
			}

			public override void RemoveItem(int index)
			{
				Handler.adapter.NotifyDataSetChanged();
			}

			public override void RemoveAllItems()
			{
				Handler.adapter.NotifyDataSetChanged();
			}
		}

		class DataStoreHandler : EnumerableChangedHandler<object>
		{
			public GridViewHandler Handler { get; set; }

			public override void AddRange(IEnumerable<object> items)
			{
				Handler.adapter.NotifyDataSetChanged();
			}

			public override void InsertRange(int index, IEnumerable<object> items)
			{
				Handler.adapter.NotifyDataSetChanged();
			}

			public override void AddItem(object item)
			{
				Handler.adapter.NotifyDataSetChanged();
			}

			public override void InsertItem(int index, object item)
			{
				Handler.adapter.NotifyDataSetChanged();
			}

			public override void RemoveItem(int index)
			{
				Handler.adapter.NotifyDataSetChanged();
			}

			public override void RemoveAllItems()
			{
				Handler.adapter.NotifyDataSetChanged();
			}
		}

		class Adapter : aw.BaseAdapter
		{
			public GridViewHandler Handler { get; set; }

			public override Java.Lang.Object GetItem(int position)
			{
				return null;
			}

			public override long GetItemId(int position)
			{
				return position;
			}

			public override int GetItemViewType(int position)
			{
				return position % ViewTypeCount;
			}

			public override int ViewTypeCount
			{
				get { return Math.Max(Handler.columns.Count, 1); }
			}

			public override av.View GetView(int position, av.View convertView, av.ViewGroup parent)
			{
				var colCount = Handler.columns.Count;
				var row = position;
				var view = convertView as aw.LinearLayout;
				var item = Handler.collection.ElementAt(row);
				if (view == null || view.ChildCount != Handler.columns.Count)
				{
					view = new aw.LinearLayout(aa.Application.Context);

					for (int i = 0; i < Handler.columns.Count; i++)
					{
						var column = Handler.columns.ElementAt(i);
						var cell = column.DataCell;
						av.View colView = null;
						if (cell != null)
						{
							var cellHandler = cell.Handler as ICellHandler;
							if (cellHandler != null)
							{
								colView = cellHandler.CreateView(null, item);
							}
						}
						view.AddView(colView ?? new av.View(aa.Application.Context), new aw.LinearLayout.LayoutParams(av.ViewGroup.LayoutParams.MatchParent, av.ViewGroup.LayoutParams.MatchParent, 1f));
					}
				}
				else
				{
					for (int i = 0; i < Handler.columns.Count; i++)
					{
						var column = Handler.columns.ElementAt(i);
						var cell = column.DataCell;
						if (cell != null)
						{
							var cellHandler = cell.Handler as ICellHandler;
							if (cellHandler != null)
							{
								var colView = view.GetChildAt(i);
								cellHandler.CreateView(colView, item);
							}
						}
					}
				}
				return view;
			}

			public override int Count
			{
				get { return Handler.collection.Count; }
			}
		}

		public GridViewHandler()
		{
			columns = new ColumnsChangedHandler { Handler = this };
			collection = new DataStoreHandler { Handler = this };
			adapter = new Adapter { Handler = this };
			Control = new aw.ListView(aa.Application.Context)
			{
				Adapter = adapter,
				ChoiceMode = aw.ChoiceMode.Single
			};
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case GridView.SelectionChangedEvent:
					Control.ItemClick += (sender, e) => 
						Callback.OnSelectionChanged(Widget, EventArgs.Empty);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			columns.Register(Widget.Columns);
		}

		public IEnumerable<object> DataStore
		{
			get { return collection.Collection; }
			set { collection.Register(value); }
		}

		public bool ShowHeader { get; set; }

		public int RowHeight { get; set; }

		public bool AllowColumnReordering { get; set; }

		public bool AllowMultipleSelection
		{
			get { return Control.ChoiceMode == aw.ChoiceMode.Multiple; }
			set { Control.ChoiceMode = value ? aw.ChoiceMode.Multiple : aw.ChoiceMode.Single; }
		}

		public IEnumerable<object> SelectedItems
		{ 
			get
			{ 
				foreach (var row in SelectedRows)
					yield return collection.ElementAt(row);
			}
		}

		public IEnumerable<int> SelectedRows
		{
			get
			{
				var c = Control.CheckedItemPositions;
				for (int i = 0; i < c.Size(); i++)
					yield return c.KeyAt(i);
			}
			set
			{
				var c = Control.CheckedItemPositions;
				c.Clear();
				foreach (var row in value)
					c.Append(row, true);

			}
		}

		public void SelectRow(int row)
		{
			Control.SetItemChecked(row, true);
		}

		public void UnselectRow(int row)
		{
			Control.SetItemChecked(row, false);
		}

		public void SelectAll()
		{
			for (int i = 0; i < collection.Count; i++)
				SelectRow(i);
		}

		public void UnselectAll()
		{
			Control.ClearChoices();
		}

		public override av.View ContainerControl
		{
			get { return Control; }
		}

		public ContextMenu ContextMenu
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public void BeginEdit(int row, int column)
		{
			throw new NotImplementedException();
		}

		public void ScrollToRow(int row)
		{
			throw new NotImplementedException();
		}

		public GridLines GridLines
		{
			get;
			set;
		}

		public BorderType Border
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public void ReloadData(IEnumerable<int> rows)
		{
			throw new NotImplementedException();
		}
	}
}