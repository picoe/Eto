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

namespace Eto.Android.Forms.Controls
{
	public class GridViewHandler : AndroidControl<aw.GridView, GridView, GridView.ICallback>, GridView.IHandler
	{
		public IEnumerable<int> SelectedRows
		{
			get { yield break; }
			set { }
		}

		public IEnumerable<object> SelectedItems
		{
			get { yield break; }
		}

		public IEnumerable<object> DataStore { get; set; }

		public bool ShowCellBorders { get; set; }

		public bool ShowHeader { get; set; }

		public int RowHeight { get; set; }

		public bool AllowColumnReordering { get; set; }

		public bool AllowMultipleSelection { get; set; }

		public void SelectRow(int row)
		{
		}

		public void UnselectRow(int row)
		{
		}

		public void SelectAll()
		{
		}

		public void UnselectAll()
		{
		}

		public override av.View ContainerControl
		{
			get { return null; }
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
	}
}