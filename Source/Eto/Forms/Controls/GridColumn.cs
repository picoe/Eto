using System;

namespace Eto.Forms
{
	public interface IGridColumn : IInstanceWidget
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
	
	public class GridColumn : InstanceWidget
	{
		IGridColumn handler;
		
		public GridColumn ()
			: this(Generator.Current)
		{
		}
		
		public GridColumn (Generator g)
			: base(g, typeof(IGridColumn), true)
		{
			handler = (IGridColumn)Handler;
		}
		
		public string HeaderText {
			get { return handler.HeaderText; }
			set { handler.HeaderText = value; }
		}
		
		public bool Resizable {
			get { return handler.Resizable; }
			set { handler.Resizable = value; }
		}
		
		public bool AutoSize {
			get { return handler.AutoSize; }
			set { handler.AutoSize = value; }
		}
		
		public bool Sortable {
			get { return handler.Sortable; }
			set { handler.Sortable = value; }
		}
		
		public int Width {
			get { return handler.Width; }
			set { handler.Width = value; }
		}
		
		public Cell DataCell {
			get { return handler.DataCell; }
			set { handler.DataCell = value; }
		}
		
		public bool Editable {
			get { return handler.Editable; }
			set { handler.Editable = true; }
		}
		
		public bool Visible {
			get { return handler.Visible; }
			set { handler.Visible = value; }
		}
	}
}

