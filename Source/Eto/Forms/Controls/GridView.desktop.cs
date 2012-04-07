using System;

namespace Eto.Forms
{
	public partial interface IGridView
	{
		ContextMenu ContextMenu { get; set; }
	}
	
	public partial class GridView
	{
		public ContextMenu ContextMenu
		{
			get { return handler.ContextMenu; }
			set { handler.ContextMenu = value; }
		}

	}
}

