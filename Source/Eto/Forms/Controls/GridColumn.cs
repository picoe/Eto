using System;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	public class GridColumnCollection : ObservableCollection<GridColumn>
	{
	}

	[Handler(typeof(GridColumn.IHandler))]
	public class GridColumn : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		public GridColumn()
		{
		}

		[Obsolete("Use default constructor instead")]
		public GridColumn (Generator generator)
			: this (generator, typeof(IHandler), true)
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected GridColumn (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}
		
		public string HeaderText {
			get { return Handler.HeaderText; }
			set { Handler.HeaderText = value; }
		}
		
		public bool Resizable {
			get { return Handler.Resizable; }
			set { Handler.Resizable = value; }
		}
		
		public bool AutoSize {
			get { return Handler.AutoSize; }
			set { Handler.AutoSize = value; }
		}
		
		public bool Sortable {
			get { return Handler.Sortable; }
			set { Handler.Sortable = value; }
		}
		
		public int Width {
			get { return Handler.Width; }
			set { Handler.Width = value; }
		}
		
		public Cell DataCell {
			get { return Handler.DataCell; }
			set { Handler.DataCell = value; }
		}
		
		public bool Editable {
			get { return Handler.Editable; }
			set { Handler.Editable = value; }
		}
		
		public bool Visible {
			get { return Handler.Visible; }
			set { Handler.Visible = value; }
		}

		public interface IHandler : Widget.IHandler
		{
			string HeaderText { get; set; }

			bool Resizable { get; set; }

			bool Sortable { get; set; }

			bool AutoSize { get; set; }

			int Width { get; set; }

			Cell DataCell { get; set; }

			bool Editable { get; set; }

			bool Visible { get; set; }
		}
	}
}

