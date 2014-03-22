#if !NO_UNITTESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.UnitTests
{
	/// <summary>
	/// A mock GridViewHandler implementation.
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	/// </summary>
	class TestGridViewHandler : IGridView
	{
		CollectionHandler collection;
		GridView GridView { get { return Widget as GridView; } }

		// Boilerplate
		public Eto.Generator Generator { get; set; }
		public ContextMenu ContextMenu { get; set; }
		public Cursor Cursor { get; set; }
		public string ID { get; set; }
		public bool ShowCellBorders { get; set; }
		public bool ShowHeader { get; set; }
		public int RowHeight { get; set; }
		public bool AllowColumnReordering { get; set; }
		public bool AllowMultipleSelection { get; set; }
		public string ToolTip { get; set; }
		public object ControlObject { get; set; }
		public Color BackgroundColor { get; set; }
		public Size Size { get; set; }
		public bool Enabled { get; set; }
		public Widget Widget { get; set; }
		public bool HasFocus { get; set; }
		public bool Visible { get; set; }
		public Point Location { get; set; }
		public IEnumerable<string> SupportedPlatformCommands { get; set; }

		/// <summary>
		/// Simulates the UI control's row count.
		/// </summary>
		int RowCount { get; set; }

		public IDataStore DataStore
		{
			get { return collection != null ? collection.Collection : null; }
			set
			{
				if (collection != null)
					collection.Unregister();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
			}
		}

		HashSet<int> selectedRows = new HashSet<int>();
		/// <summary>
		/// Indexes of rows selected in the UI.
		/// </summary>
		public IEnumerable<int> SelectedRows
		{
			get { return selectedRows; }			
		}

		public void SelectRow(int row)
		{
			selectedRows.Add(row);
		}

		public void UnselectRow(int row)
		{
			if (selectedRows.Contains(row))
				selectedRows.Remove(row);
		}

		public void SelectAll()
		{
			selectedRows.Clear();
			for (var i = 0; i < RowCount; ++i)
				selectedRows.Add(i);
		}

		public void UnselectAll()
		{
			selectedRows.Clear();
		}

		public void Invalidate()
		{
		}

		public void Invalidate(Rectangle rect)
		{
		}

		public void SuspendLayout()
		{
		}

		public void ResumeLayout()
		{
		}

		public void Focus()
		{
		}

		public void OnPreLoad(EventArgs e)
		{
		}

		public void OnLoad(EventArgs e)
		{
		}

		public void OnLoadComplete(EventArgs e)
		{
		}

		public void OnUnLoad(EventArgs e)
		{
		}

		public void SetParent(Container parent)
		{
		}

		public void MapPlatformCommand(string systemCommand, Command command)
		{
		}

		public PointF PointFromScreen(PointF point)
		{
			throw new NotImplementedException();
		}

		public PointF PointToScreen(PointF point)
		{
			throw new NotImplementedException();
		}

		public void HandleEvent(string id, bool defaultEvent = false)
		{
		}

		public void Initialize()
		{
		}

		void SetRowCount()
		{
			RowCount = collection.Collection != null ? collection.Collection.Count : 0;
		}

		void IncrementRowCountBy(int increment)
		{
			RowCount += increment;
		}

		class CollectionHandler : DataStoreChangedHandler<object, IDataStore>
		{
			public TestGridViewHandler Handler { get; set; }

			public override void AddRange(IEnumerable<object> items)
			{
				Handler.SetRowCount();
			}

			public override void AddItem(object item)
			{
				Handler.IncrementRowCountBy(1);
			}

			public override void InsertItem(int index, object item)
			{
				Handler.IncrementRowCountBy(1);
			}

			public override void InsertRange(int index, IEnumerable<object> items)
			{
				Handler.SetRowCount();
			}

			public override void RemoveItem(int index)
			{
				Handler.IncrementRowCountBy(-1);
			}

			public override void RemoveRange(int index, int count)
			{
				Handler.SetRowCount();
			}

			public override void RemoveRange(IEnumerable<object> items)
			{
				Handler.SetRowCount();
			}

			public override void RemoveAllItems()
			{
				Handler.SetRowCount();
			}
		}
	}
}
#endif