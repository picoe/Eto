using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Test.UnitTests.Handlers.Controls
{
	public class TestGridColumnHandler : GridColumn.IHandler
	{
		public string HeaderText { get; set; }

		public bool Resizable { get; set; }

		public bool Sortable { get; set; }

		public bool AutoSize { get; set; }

		public int Width { get; set; }

		public Cell DataCell { get; set; }

		public bool Editable { get; set; }

		public bool Visible { get; set; }

		public string ID { get; set; }

		public Widget Widget { get; set; }

		public void Initialize()
		{
		}

		public void HandleEvent(string id, bool defaultEvent = false)
		{
		}
	}
}
