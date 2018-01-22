using System;
using Eto.Forms;
using Eto.Test.UnitTests.Handlers.Controls;

namespace Eto.Test.UnitTests.Handlers.Layout
{
	public class TestTableLayoutHandler : TestControlHandler, TableLayout.IHandler
	{
		public void CreateControl(int cols, int rows)
		{
		}
		public bool GetColumnScale(int column)
		{
			throw new NotImplementedException();
		}
		public void SetColumnScale(int column, bool scale)
		{
			throw new NotImplementedException();
		}
		public bool GetRowScale(int row)
		{
			throw new NotImplementedException();
		}
		public void SetRowScale(int row, bool scale)
		{
			throw new NotImplementedException();
		}
		public Eto.Drawing.Size Spacing { get; set; }

		public Eto.Drawing.Padding Padding { get; set; }

		public void Add(Control child, int x, int y)
		{
		}
		public void Move(Control child, int x, int y)
		{
			throw new NotImplementedException();
		}
		public void Remove(Control child)
		{
			throw new NotImplementedException();
		}
		public void Update()
		{
			throw new NotImplementedException();
		}
		public Eto.Drawing.Size ClientSize
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
		public bool RecurseToChildren
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}

