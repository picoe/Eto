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

namespace Eto.Platform.Android.Forms.Controls
{
	public class GridViewHandler : AndroidControl<aw.TableLayout, GridView>, IGridView
	{
		public IDataStore DataStore
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

		public bool ShowCellBorders
		{
			set { throw new NotImplementedException(); }
		}

		public bool ShowHeader
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

		public int RowHeight
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

		public bool AllowColumnReordering
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

		public bool AllowMultipleSelection
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

		public IEnumerable<int> SelectedRows
		{
			get { throw new NotImplementedException(); }
		}

		public void SelectRow(int row)
		{
			throw new NotImplementedException();
		}

		public void UnselectRow(int row)
		{
			throw new NotImplementedException();
		}

		public void SelectAll()
		{
			throw new NotImplementedException();
		}

		public void UnselectAll()
		{
			throw new NotImplementedException();
		}

		public override av.View ContainerControl
		{
			get { throw new NotImplementedException(); }
		}
	}
}