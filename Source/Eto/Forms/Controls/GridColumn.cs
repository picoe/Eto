using System;

namespace Eto.Forms
{
	public interface IGridColumn : IDataColumn
	{

	}
	
	public class GridColumn : DataColumn
	{
		public GridColumn ()
			: this(Generator.Current)
		{
		}
		
		public GridColumn (Generator g)
			: base(g, typeof(IGridColumn), true)
		{
		
		}
	}
}

