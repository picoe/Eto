using System;
using Eto.Forms;

namespace Eto.iOS.Forms.Controls
{
	public class GridColumnHandler : WidgetHandler<object, GridColumn>, GridColumn.IHandler
	{
		public GridColumnHandler ()
		{
		}

		public string HeaderText {
			get;
			set;
		}

		public bool Resizable {
			get;
			set;
		}

		public bool Sortable {
			get;
			set;
		}

		public bool AutoSize {
			get;
			set;
		}

		public int Width {
			get;
			set;
		}

		public Cell DataCell {
			get;
			set;
		}

		public bool Editable {
			get;
			set;
		}

		public bool Visible {
			get;
			set;
		}
	}
}

